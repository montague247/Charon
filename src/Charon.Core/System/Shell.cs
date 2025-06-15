using System.Diagnostics;
using Serilog;

namespace Charon.System
{
    public static class Shell
    {
        public const string AptGetCommand = "apt-get";

        private static HashSet<string>? _installedTools;

        public static int Execute(string fileName, List<string> arguments, bool verbose = false, bool shellExecute = false)
        {
            return Execute(fileName, Environment.CurrentDirectory, arguments, verbose, shellExecute);
        }

        public static int Execute(string fileName, string workingDirectory, List<string> arguments, bool verbose = false, bool shellExecute = false)
        {
            if (verbose)
                Log.Information("Execute ({Type}): {FileName} {Arguments}", shellExecute ? "shell" : "direct", fileName, string.Join(' ', arguments));
            else
                Log.Debug("Execute ({Type}): {FileName} {Arguments}", shellExecute ? "shell" : "direct", fileName, string.Join(' ', arguments));

            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = !shellExecute,
                RedirectStandardError = !shellExecute,
                UseShellExecute = shellExecute,
                CreateNoWindow = !shellExecute
            };

            foreach (var argument in arguments)
            {
                psi.ArgumentList.Add(argument);
            }

            var process = new Process { StartInfo = psi };

            process.Start();

            if (!shellExecute)
            {
                // Start parallel reading of Standard-Output and Standard-Error
                var outputTask = Task.Run(() => ReadStreamAsync(process.StandardOutput, false));
                var errorTask = Task.Run(() => ReadStreamAsync(process.StandardError, true));

                Task.WhenAll(outputTask, errorTask);
            }

            var timeout = TimeSpan.FromMinutes(60);

            if (!process.WaitForExit(timeout))
            {
                process.Close();
                throw new InvalidOperationException($"{fileName} timeout after {timeout}");
            }

            return process.ExitCode;
        }

        public static int BashExecute(string command, string fileName, List<string> arguments, bool verbose = false)
        {
            return BashExecute(command, fileName, Environment.CurrentDirectory, arguments, verbose);
        }

        public static int BashExecute(string command, string fileName, string workingDirectory, List<string> arguments, bool verbose = false)
        {
            var bashArguments = new List<string> { command, fileName };
            bashArguments.AddRange(arguments);

            // Arguments = string.Format("-c \"sudo {0} {1} {2}\"", "/path/to/script", "arg1", arg2)
            return Execute("/bin/bash", workingDirectory, ["-c", string.Join(' ', bashArguments)], verbose);
        }

        public static int BashExecute(List<string> bashArguments, bool verbose = false)
        {
            return Execute(Environment.CurrentDirectory, bashArguments, verbose);
        }

        public static int BashExecute(string workingDirectory, List<string> bashArguments, bool verbose = false)
        {
            return Execute("/bin/bash", workingDirectory, ["-c", string.Join(' ', bashArguments)], verbose);
        }

        public static int SudoExecute(string fileName, List<string> arguments, IShellOptions shellOptions, bool verbose = false)
        {
            return SudoExecute(fileName, Environment.CurrentDirectory, arguments, shellOptions, verbose);
        }

        public static int SudoExecute(string fileName, string workingDirectory, List<string> arguments, IShellOptions shellOptions, bool verbose = false)
        {
            if (shellOptions.SudoAlternative)
            {
                var sudoArguments = new List<string> { fileName };
                sudoArguments.AddRange(arguments);

                return Execute("sudo", workingDirectory, sudoArguments, verbose);
            }

            return BashExecute("sudo", fileName, workingDirectory, arguments, verbose);
        }

        public static async Task<string?> GetOutput(string fileName, List<string> arguments, bool standardOutput = true, bool verbose = false)
        {
            if (verbose)
                Log.Information("GetOutput: {FileName} {Arguments}", fileName, string.Join(' ', arguments));
            else
                Log.Debug("GetOutput: {FileName} {Arguments}", fileName, string.Join(' ', arguments));

            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                RedirectStandardOutput = standardOutput,
                RedirectStandardError = !standardOutput,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            foreach (var argument in arguments)
            {
                psi.ArgumentList.Add(argument);
            }

            using var process = new Process { StartInfo = psi };
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();

            process.WaitForExit();

            return output;
        }

        public static async Task<string?> GetBashOutput(List<string> bashArguments, bool standardOutput = true, bool verbose = false)
        {
            return await GetOutput("/bin/bash", ["-c", string.Join(' ', bashArguments)], standardOutput, verbose);
        }

        public static bool IsCommandAvailable(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Command name cannot be null or empty.", nameof(name));

            return Execute("which", [name]) == 0;
        }

        public static void CheckInstall(IShellOptions shellOptions, params string[] toolNames)
        {
            if (shellOptions.NoInstall)
                return;

            foreach (var toolName in toolNames)
            {
                CheckInstall(toolName, shellOptions);
            }
        }

        public static void CheckInstall(string toolName, IShellOptions shellOptions)
        {
            if (shellOptions.NoInstall)
                return;

            _installedTools ??= [];

            if (!_installedTools.Add(toolName))
                return;

            if (IsCommandAvailable(toolName))
                return;

            Log.Information("{Name} is not installed. Try to install it.", toolName);

            SudoExecute(AptGetCommand, ["install", toolName, "-y"], shellOptions);
        }

        public static bool IsDebianBased(bool force = false)
        {
            // Check if the system is running on a Debian-based distribution
            var osReleaseFile = "/etc/os-release";

            if (!File.Exists(osReleaseFile))
            {
                if (force)
                    Log.Error("The file {OsReleaseFile} does not exist. Cannot determine if the system is Debian-based.", osReleaseFile);

                return false;
            }

            var osReleaseContent = File.ReadAllText(osReleaseFile);

            if (osReleaseContent.Contains("ID=debian", StringComparison.OrdinalIgnoreCase) ||
                osReleaseContent.Contains("ID=ubuntu", StringComparison.OrdinalIgnoreCase) ||
                osReleaseContent.Contains("ID=linuxmint", StringComparison.OrdinalIgnoreCase) ||
                osReleaseContent.Contains("ID=kali", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (!force)
                return false;

            Log.Error("The system is not Debian-based. Cannot execute this operation.");

            throw new NotImplementedException();
        }

        public static bool HasSudoPrivileges()
        {
            // Check if the user has sudo privileges
            var exitCode = Execute("sudo", ["-n", "true"], shellExecute: true);

            return exitCode == 0;
        }

        public static void EnsureNodeJS(IShellOptions shellOptions)
        {
            if (IsCommandAvailable("node") && IsCommandAvailable("npm"))
            {
                Log.Information("Node.js and npm are already installed.");
                return;
            }

            if (Execute("curl", ["-sL", "https://deb.nodesource.com/setup_20.x", "-o", "/tmp/nodesource_setup.sh"]) != 0)
            {
                Log.Error("Failed to download Node.js setup script.");
                return;
            }

            SudoExecute("bash", ["/tmp/nodesource_setup.sh"], shellOptions);
            CheckInstall("nodejs", shellOptions);

            Log.Information("Node.js and npm have been installed.");
        }

        private static void ReadStreamAsync(StreamReader stream, bool error)
        {
            string? line;

            while ((line = stream.ReadLine()) != null)
            {
                if (error)
                    Log.Error(line);
                else
                    Log.Information(line);
            }
        }
    }
}
