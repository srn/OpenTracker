using System;

namespace OpenTracker.Core.Common
{
	public static class Calculation
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string FormatBytes(float bytes)
		{
			var suffix = new[] { "Bytes", "KiB", "MiB", "GiB", "TiB" };
			int i;
			double dblSByte = 0;
			for (i = 0; (int)(bytes / 1024) > 0; i++, bytes /= 1024)
				dblSByte = bytes / 1024.0;
			return String.Format("{0:0.00} {1}", dblSByte, suffix[i]).Replace(",", ".");
		}
	}
}
