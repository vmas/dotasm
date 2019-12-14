using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DotAsm
{
	internal static class WindowsNativeMethods
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int GetModuleFileName(IntPtr hModule, [Out] StringBuilder lpFilename, int size);
	}
}
