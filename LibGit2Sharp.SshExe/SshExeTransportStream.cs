using System;
using LibGit2Sharp;
using System.IO;
using System.Diagnostics;

namespace LibGit2Sharp.SshExe
{
    public class SshExeTransportStream : SmartSubtransportStream
    {
        Process process;

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
            // this probably needs more escaping so we pass single quotes
            // to the upload-pack/receive-pack process itself
            string host, user, path, port;
            splitHostPath(url, out host, out user, out path, out port);
            var args = port == null ?
                String.Format("'{3}@{0}' '{1} '\\''{2}'\\'''", host, procName, path, user) :
                String.Format("-p {4} '{3}@{0}' '{1}' '{2}'", host, procName, path, user, port);
            Console.WriteLine("args {0}", args);

            process = new Process();
            process.StartInfo.FileName = SshExeTransport.ExePath;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.ErrorDataReceived += (sender, e) => Console.WriteLine("error: {0}", e.Data);
        }

        void AssertAlive()
        {
            if (!started)
            {
                process.Start();
                started = true;
            }

            if (process.HasExited)
            {
                byte[] buf = new byte[8*1024];
                int read = process.StandardError.BaseStream.Read(buf, 0, buf.Length);
                Console.WriteLine("pues {0}", System.Text.Encoding.UTF8.GetString(buf, 0, read));
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
            stream.Write(buf, 0, read);

            readTotal = read;

            // If we get EOF, let's make sure the process didn't just die on us
            if (read == 0)
            {
                AssertAlive();
            }

            return 0;
        }
    }
}

