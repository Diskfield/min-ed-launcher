module MinEdLauncher.Interop
#nowarn "9"

open System.Diagnostics
open System
open System.IO
open System.Runtime.InteropServices

open Microsoft.FSharp.NativeInterop

[<DllImport("libc", SetLastError = true)>]
extern int private kill(int pid, int signal)

[<DllImport("libc", SetLastError = true)>]
extern int private strerror_r(int errnum, char *buf, UInt64 buflen);

let private getErrorMessage errno =    
    let buffer = NativePtr.stackalloc<char> 1024;
    let result = strerror_r(errno, buffer, 1024UL);

    if result = 0 then
        Marshal.PtrToStringAnsi(buffer |> NativePtr.toNativeInt)
    else
        $"errno %i{errno}"

[<Literal>]
let private SIGTERM = 15
[<Literal>]
let private ESRCH = 3  
let private terminate (p: Process) =
    let code = kill(p.Id, SIGTERM)
    let errno = Marshal.GetLastWin32Error()
    if code = -1 && errno <> ESRCH then // ESRCH = process does not exist, assume it exited
        getErrorMessage errno |> Error
    else
        Ok code
        
let ansiColorSupported() = not (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("TERM")))

let freeDiskSpace path =
    try
        let drive = DriveInfo(path)
        Some drive.AvailableFreeSpace
    with _ ->
        None

let termProcess (timeout: TimeSpan) (p: Process) =
    if p.HasExited then
        Ok ()
    else
        terminate p
        |> Result.bind(fun _ ->
            if p.WaitForExit(timeout) then
                Ok ()
            else
                Error $"Process took longer than %f{timeout.TotalSeconds} seconds to shutdown"
            )
        |> Result.mapError (fun msg ->
            p.Kill(true)
            $"Unable to gracefully stop %s{p.ProcessName}. Killed process instead. %s{msg}"
            )