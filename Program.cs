using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Threading;

namespace DotAsm
{
	class Program
	{
		static int Main(string[] args)
		{
			string runtime = null;
			string corclrPath, nativePath;

			bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

			try
			{
				runtime = GetRuntimeID();
				nativePath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "..", "..", "..", "runtimes", runtime, "native");
				corclrPath = Path.GetDirectoryName(GetLibraryPath());
				if (string.IsNullOrEmpty(corclrPath))
					throw new DllNotFoundException("The coreclr library not found!");
			}
			catch (PlatformNotSupportedException)
			{
				Console.WriteLine("Unsupported runtime{0}.", runtime != null ? ": " + runtime : string.Empty);
				return -1;
			}
			catch (DllNotFoundException dllex)
			{
				Console.WriteLine(dllex.Message);
				return -1;
			}
			
			if (!args.Contains("/NOLOGO", new IgnoreCaseStringComparer()))
			{
				WriteLogo(corclrPath, runtime);
			}

			string arg0 = args.FirstOrDefault()?.ToLowerInvariant();
			if (arg0 != "ildasm" && arg0 != "ilasm")
			{
				Console.WriteLine("Usage: dotnet asm ilasm|ildasm [args...]");
				return -1;
			}

			string envPath = Environment.GetEnvironmentVariable("PATH");
			var envPathData = new List<string>(3);
			envPathData.Add(nativePath);
			if (corclrPath != null)
				envPathData.Add(corclrPath);
			envPathData.Add(envPath);
			envPath =  string.Join(";", envPathData);
			Environment.SetEnvironmentVariable("PATH", envPath);

			string executablePath = Path.Combine(nativePath, arg0 + (isWindows ? ".exe" : null));
			//Console.WriteLine(executablePath);
			ProcessStartInfo psi = new ProcessStartInfo(executablePath);
			psi.Arguments = string.Join(" ", EscapeArgs(args.Skip(1)));
			psi.UseShellExecute = false;
			psi.WorkingDirectory = nativePath;
			psi.Environment["PATH"] = envPath;
			psi.EnvironmentVariables["PATH"] = envPath;

			int exitcode = -1;

			Process process = null;
			try
			{
				process = Process.Start(psi);
				process.WaitForExit();
				Thread.Sleep(1000);
				exitcode = process.ExitCode;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				process?.Dispose();
			}
			return exitcode;
		}

		private static void WriteLogo(string corclrPath, string runtime)
		{
			Console.WriteLine("DotAsm Tool {0} (.NET Core {1} {2})", typeof(Program).Assembly.GetName().Version, corclrPath?.TrimEnd('/').Split(new[] { '/', '\\' }).LastOrDefault(), runtime);
		}

		private static IEnumerable<string> EscapeArgs(IEnumerable<string> args)
		{
			foreach (string arg in args)
			{
				if (arg.Length == 0 || arg.Any(char.IsWhiteSpace) || arg.Contains('"'))
					yield return "\"" + arg.Replace("\"", "\\\"") + "\"";
				else
					yield return arg;
			}
		}

		private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			Console.Write(e.Data);
		}

		private static bool IsLinux
		{
			get
			{
				PlatformID platform = Environment.OSVersion.Platform;
				int p = (int)Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}

		private static string GetRuntimeID()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return "win-" + GetArch();
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return "linux-" + GetArch();
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return "osx-" + GetArch();
			throw new PlatformNotSupportedException();

		}

		private static string GetArch()
		{
			if (IntPtr.Size == 4)
				return "x86";
			if (IntPtr.Size == 8)
				return "x64";
			throw new PlatformNotSupportedException();
		}

		private static string GetLibraryPath()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return GetLibraryPathWindows();
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return GetLibraryPathLinux();
			throw new PlatformNotSupportedException();
		}

		private static string GetLibraryPathWindows()
		{
			IntPtr coreclrModule = WindowsNativeMethods.GetModuleHandle("coreclr.dll");
			if (coreclrModule == IntPtr.Zero)
				return null;
			var sb = new StringBuilder(2048);
			sb.Length = WindowsNativeMethods.GetModuleFileName(coreclrModule, sb, sb.Capacity);
			return sb.ToString();
		}

		private static string GetLibraryPathLinux()
		{
			// Linux: https://stackoverflow.com/questions/1681060/library-path-when-dynamically-loaded

			foreach (string line in File.ReadAllLines("/proc/self/maps"))
			{
				if (line.Contains("libcoreclr.so"))
				{
					return line.Substring(line.IndexOf('/'));
				}
			}
			return null;
		}

	}


}
