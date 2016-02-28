using System;
using LibGit2Sharp;
using System.IO;
using System.Diagnostics;

namespace LibGit2Sharp.SshExe
{
	public class SshExeTransportStream : SmartSubtransportStream
	{
		readonly SshExeTransport parent;
		readonly string url;

		readonly Process process;

		void splitHostPath(string url, out string host, out string path)
		{
			var parsedUrl = new Uri(url);
			throw new NotImplementedException();
		}

		public SshExeTransportStream (SshExeTransport parent, string url)
		{
			this.parent = parent;
			this.url = url;

			// this probably needs more escaping so we pass single quotes
			// to the upload-pack/receive-pack process itself
			var args = String.Format("'{0}' '{1}' '{2}'");

			process = new Process();
			process.StartInfo.FileName = SshExeTransport.ExePath;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.Arguments = args;
			process.Start();
		}

		void AssertAlive()
		{
			if (process.HasExited)
			{
				throw new Exception("ssh process terminated unexpectedly");
			}
		}

		public override int Write(Stream stream, long length)
		{
			AssertAlive();

			while (length > 0)
			{
				int toCopy = length > int.MaxValue ? int.MaxValue : (int)length;
				stream.CopyTo(process.StandardInput.BaseStream, toCopy);
				length -= toCopy;
			}

			return 0;
		}

		public override int Read(Stream stream, long length, out long readTotal)
		{
			AssertAlive();

			byte[] buf = new byte[Math.Min(length, 8*1024)];
			int read = process.StandardOutput.BaseStream.Read(buf, 0, buf.Length);
			stream.Write(buf, 0, buf.Length);

			readTotal = read;
			return 0;
		}
	}
}

