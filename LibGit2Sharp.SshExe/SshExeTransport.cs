using System;
using LibGit2Sharp;

namespace LibGit2Sharp.SshExe
{
	public class SsshExeTransport : SmartSubtransport
	{
		public string ExePath { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LibGit2Sharp.SshExe.SsshExeTransport"/> class.
		/// </summary>
		/// <param name="exePath">Path to the ssh executable</param>
		public SsshExeTransport (string exePath)
		{
			ExePath = exePath;
		}

		protected override SmartSubtransportStream Action(string url, GitSmartSubtransportAction action)
		{
			throw new NotImplementedException();
		}
	}
}
