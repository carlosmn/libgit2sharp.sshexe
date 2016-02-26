using System;
using LibGit2Sharp;
using System.IO;

namespace LibGit2Sharp.SshExe
{
	public class SshExeTransportStream : SmartSubtransportStream
	{
		readonly SshExeTransport parent;
		readonly string url;

		public SshExeTransportStream (SshExeTransport parent, string url)
		{
			this.parent = parent;
			this.url = url;
		}

		public override int Write(Stream stream, long length)
		{
			throw new NotImplementedException();
		}

		public override int Read(Stream stream, long length, out long readTotal)
		{
			throw new NotImplementedException();
		}
	}
}

