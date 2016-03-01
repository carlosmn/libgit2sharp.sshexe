using System;
using LibGit2Sharp;

namespace LibGit2Sharp.SshExe
{
    public class SshExeTransport : SmartSubtransport
    {
        public static string ExePath { get; set; }
        SshExeTransportStream stream;

        public SshExeTransport ()
        {
        }

        protected override SmartSubtransportStream Action(string url, GitSmartSubtransportAction action)
        {
            switch (action)
            {
                // Both of these mean we're starting a new connection
                case GitSmartSubtransportAction.UploadPackList:
                    stream = new SshExeTransportStream(this, url, "git-upload-pack");
                    break;
                case GitSmartSubtransportAction.ReceivePackList:
                    stream = new SshExeTransportStream(this, url, "git-receive-pack");
                    break;
                case GitSmartSubtransportAction.UploadPack:
                case GitSmartSubtransportAction.ReceivePack:
                    break;
                default:
                    throw new InvalidOperationException("Invalid action for subtransport");    
            }

            return stream;
        }
    }
}
