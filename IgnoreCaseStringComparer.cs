using System;
using System.Collections.Generic;
using System.Text;

namespace DotAsm
{
	sealed class IgnoreCaseStringComparer : IEqualityComparer<string>
	{
		public bool Equals(string x, string y)
		{
			if (x == null)
				return y == null;
			return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
		}

		public int GetHashCode(string s)
		{
			if (s == null)
				return 0;
			return s.GetHashCode();
		}
	}
}
