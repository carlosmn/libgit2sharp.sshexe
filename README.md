# LibGit2Sharp.SshExe

This project aims to fill the ssh gap in LibGit2Sharp by providing a managed transport which executes the ssh binary to provide the connection and move the bytes.

This works around having to ship a libgit2 which needs libssh2 or depending on a managed SSH implementation, where we've found no projects which fit the bill. It also avoids having to implement all of the configuration-dependent implementation which we already get with the OpenSSH binary.

It does mean that you need to provide your own `ssh` or `ssh.exe` binary, but you had to do so anyway.

## How to use

Once the code is written, you'd be able to do something like

```csharp
using LibGit2Sharp;
using LibGit2Sharp.SshExe;

// Configure the transport, specifying which ssh binary you want to use
SshExeTransport.SshExe = @"C:\something\bin\ssh.exe";

// Register the transport with libgit2(sharp)
var registration = GlobalSettings.RegisterSmartSubtransport<SshExeTransport>("ssh");

// if you want to remove this transport
GlobalSettings.UnregisterSmartSubtransport(registration);
```

## Maybes and whishlist

We will need some way to tell ssh to ask us for the registration. This will likely be through its askpass envrionment viriable, or by pretending to be an agent. If we go the agent route, we might want to put ourselves between ssh and the agent, so we can control what the agent does and can present the authentication callbacks to libgit2.
