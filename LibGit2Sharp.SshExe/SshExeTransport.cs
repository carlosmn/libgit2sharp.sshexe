using System;
using LibGit2Sharp;

namespace LibGit2Sharp.SshExe
{
	public class SshExeTransport : SmartSubtransport
	{
		public static string ExePath { get; private set; }

		public SshExeTransport ()
		{
		}

		protected override SmartSubtransportStream Action(string url, GitSmartSubtransportAction action)
		{
			switch (action)
			{
			// Both of these mean we're starting a new connection
			case GitSmartSubtransportAction.UploadPackList:
                    return new SshExeTransportStream(this, url, "git-upload-pack");
			case GitSmartSubtransportAction.ReceivePackList:
				return new SshExeTransportStream(this, url, "git-receive-pack");
			case GitSmartSubtransportAction.UploadPack:
			case GitSmartSubtransportAction.ReceivePack:
				// FIXME: do we get these actions when we're a
				// stateful transport?
				throw new InvalidOperationException("Shouldn't get these actions");
			}

			throw new InvalidOperationException("Invalid action");
		}
	}
}
