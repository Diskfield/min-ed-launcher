[<RequireQualifiedAccess>]
module MinEdLauncher.Cobra
open System
open System.Diagnostics
open System.Globalization
open System.IO
open System.Reflection
open System.Resources
open System.Text
open FsToolkit.ErrorHandling
open MinEdLauncher
open Types

type ProductsDir = Local of string | PermissionsIgnored of string | NoWriteAccess of fallback: string * deniedPath: string
let getDefaultProductsDir fallbackPath hasWriteAccess dirExists (forceLocal:ForceLocal) launcherDir =
    let productsPath = "Products"
    let localPath = Path.Combine(launcherDir, productsPath)
    if forceLocal then PermissionsIgnored localPath
    elif (dirExists localPath && hasWriteAccess localPath) then Local localPath
    elif not (dirExists localPath) && hasWriteAccess launcherDir then Local localPath
    else (Path.Combine(fallbackPath, productsPath), localPath) |> NoWriteAccess

let getProductDir defaultProductsDir fileExists (readAllLines: string -> string[]) dirExists directoryName =
    let rdrPath = Path.Combine(defaultProductsDir, directoryName) + ".rdr"
    
    if fileExists rdrPath then
        readAllLines rdrPath
        |> Array.map (fun p -> p.Trim())
        |> Array.tryFind dirExists
        |> Option.defaultValue defaultProductsDir
    else
        defaultProductsDir
    
let getVersion cbLauncherDir =
    let cobraPath = Path.Combine(cbLauncherDir, "CBViewModel.dll")
    
    if not (File.Exists cobraPath) then
        Error $"Unable to find CBViewModel.dll in directory %s{cbLauncherDir}"
    else
        let cobraVersion =
            let version = FileVersionInfo.GetVersionInfo(cobraPath)
            if String.IsNullOrEmpty(version.FileVersion) then version.ProductVersion else version.FileVersion
        
        Ok cobraVersion

let decrypt text =
    text |> Convert.FromBase64String |> Encoding.Unicode.GetString |> Ok
    
let encrypt (text: string) =
    text |> Encoding.Unicode.GetBytes |> Convert.ToBase64String |> Ok
    
type CredResult = Found of string * string * string option | NotFound of string | UnexpectedFormat of string | Failure of string
let readCredentials path = task {
    if File.Exists(path) then
        let! lines = FileIO.readAllLines(path)
        return match lines with
               | Ok lines ->
                    if lines.Length = 2 then
                        (lines.[0], lines.[1], None) |> CredResult.Found
                    else if lines.Length = 3 then
                        match lines.[2] |> decrypt with
                        | Ok token -> (lines.[0], lines.[1], Some token) |> CredResult.Found
                        | Error e -> CredResult.Failure e
                    else
                       CredResult.UnexpectedFormat path
               | Error msg -> CredResult.Failure msg
    else
        return CredResult.NotFound path }

let setUserOnly path =
        let file = Mono.Unix.UnixFileInfo(path)
        try
            file.FileAccessPermissions <- Mono.Unix.FileAccessPermissions.UserRead ||| Mono.Unix.FileAccessPermissions.UserWrite 
            Ok ()
        with e ->
            File.Delete(path)
            Error $"Unable to set credential file permissions - {e}"
let saveCredentials path credentials machineToken =
    let nl = Environment.NewLine
    match machineToken with
    | Some token ->
        token
        |> encrypt
        |> Result.bindTask (fun encryptedToken ->
            $"{credentials.Username}{nl}{credentials.Password}{nl}{encryptedToken}" |> FileIO.writeAllText path)
    | None -> $"{credentials.Username}{nl}{credentials.Password}" |> FileIO.writeAllText path
    |> TaskResult.bind (fun () -> setUserOnly path |> Task.fromResult)
    
let discardToken path =
    FileIO.readAllLines path
    |> TaskResult.bind (fun lines ->
        String.Join(Environment.NewLine, lines |> Seq.take 2)
        |> FileIO.writeAllText path)
    |> TaskResult.bind (fun () -> setUserOnly path |> Task.fromResult)

let getGameLang cbLauncherDir langCode =
    let asm = Assembly.LoadFrom(Path.Combine(cbLauncherDir, $"LocalResources.dll"))
    let resManager = ResourceManager("LocalResources.Properties.Resources", asm)
    try
        langCode
        |> Option.bind (fun c ->
            if Directory.Exists(Path.Combine(cbLauncherDir, c))
               || c.Equals("en", StringComparison.OrdinalIgnoreCase)
            then c.ToLowerInvariant() |> Some
            else None)
        |> Option.map (fun c ->
            let culture = CultureInfo.CreateSpecificCulture(c)
            resManager.GetString("GameLanguage", culture))
        |> Option.orElseWith (fun () ->
            let culture = CultureInfo.GetCultureInfo(Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName)
            resManager.GetString("GameLanguage", culture) |> Some)
    with e -> None