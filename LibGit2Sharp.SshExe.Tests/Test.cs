using Xunit;
using System;
using LibGit2Sharp;
using LibGit2Sharp.SshExe;

namespace LibGit2Sharp.SshExe.Tests
{
    public class Test
    {
        [Fact]
        public void TestCase()
        {
            SshExeTransport.ExePath = "/usr/bin/ssh";
            var registration = GlobalSettings.RegisterSmartSubtransport<SshExeTransport>("ssh");
            Repository.Clone("ssh://git@github.com/libgit2/TestGitRepository", "/tmp/foo");

            GlobalSettings.UnregisterSmartSubtransport(registration);
        }
    }
}

