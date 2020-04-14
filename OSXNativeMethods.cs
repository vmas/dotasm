using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DotAsm
{
	static class OSXNativeMethods
	{
		[DllImport("libdl", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint _dyld_image_count();

		[DllImport("libdl", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr _dyld_get_image_name(uint index);
	}
}
