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

		Process process;
		readonly ProcessStartInfo startInfo;

		bool started;

        void splitHostPath(string url, out string host, out string user, out string path, out string port)
		{
            try
            {
			    var parsedUrl = new Uri(url);
			    host = parsedUrl.Host;
                user = parsedUrl.UserInfo;
                port = parsedUrl.IsDefaultPort ? null : parsedUrl.Port.ToString();
                path = parsedUrl.LocalPath.Substring(1);
            }
            catch (UriFormatException)
            {
                throw new NotImplementedException();
            }
		}

        public SshExeTransportStream(SshExeTransport parent, string url, string procName)
            : base(parent)
		{
			this.parent = parent;
			this.url = url;

			// this probably needs more escaping so we pass single quotes
			// to the upload-pack/receive-pack process itself
            string host, user, path, port;
            splitHostPath(url, out host, out user, out path, out port);
            var args = port == null ?
                String.Format("'{1}@{0}' '{2}' '{3}'", host, user, procName, path) :
                String.Format("-p {0} {1}@{0}'{2}' '{3}' '{4}'", port, user, host, procName, path);
            Console.WriteLine("args {0}", args);

            startInfo = new ProcessStartInfo()
            {
                FileName = SshExeTransport.ExePath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                //RedirectStandardError = true,
            };
		}

		void AssertAlive()
		{
            if (process == null)
			{
                Console.WriteLine("starting ssh");
				process = Process.Start(startInfo);
                Console.WriteLine("started ssh");
                process.ErrorDataReceived += (sender, e) => Console.WriteLine("error: {0}", e.Data);
			}

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

