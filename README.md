# cse210-ww-student-template
This is the starter code for students in the worldwide content version of CSE 210.

This repository contains the starter code for many different projects. They are arranged folders by week. There is also an an empty sandbox project that can be used to play around with any concept you would like.

The Mindfulness program saves completed activities and typed responses in
`week05/Mindfulness/activity-history.jsonl` when run from the project. A published
copy outside the project saves the file next to the executable instead. Select
"Review activity history" from its menu to view the entries. The file is local,
unencrypted, and may contain personal responses; Git ignores it. Elapsed time covers
the timed activity itself, including time spent waiting for input, but not preparation
or the ending message.

The sandbox Mindfulness copy also has a Windows desktop interface. From the
repository root, run `dotnet run --project sandbox\Sandbox\week05\Mindfulness\Desktop\Mindfulness.Desktop.csproj`,
or select "Sandbox: Mindfulness Desktop" in VS Code's Run and Debug menu. The
console version remains available, and both sandbox interfaces share the history
file in the sandbox Mindfulness project directory.

## Mindfulness web app

The sandbox copy also includes an HTTPS web app with personal accounts, the
three activities, and per-account history. Its history is separate from the
console and desktop files.

1. Obtain a PFX certificate with a private key, valid for the hostname or IP
   address devices will use, from a certificate authority trusted by each
   device. The standard .NET localhost development certificate is not suitable
   for other devices. Do not ignore browser certificate warnings.
2. In a PowerShell terminal at the repository root, set the certificate path
   and password for that terminal session, then start the app:

   ```powershell
   $env:MINDFULNESS_CERT_PATH = 'C:\path\to\your-certificate.pfx'
   $env:MINDFULNESS_CERT_PASSWORD = Read-Host 'Certificate password'
   dotnet run --project '.\sandbox\Sandbox\week05\Mindfulness\Web\Mindfulness.Web.csproj'
   ```

3. Visit `https://<certificate-hostname>:5443` on the server or another device
   on the private network. The server must remain running. If needed, allow
   inbound TCP port 5443 in the host firewall only for the trusted network.
   The app listens on all network interfaces by default; set
   `MINDFULNESS_BIND_ADDRESS` to a specific local IP to restrict the listener,
   or `127.0.0.1` for the local computer only. Set `MINDFULNESS_HTTPS_PORT`
   to change the port. No HTTP listener is opened.

Anyone who can reach the app can register; there is no email or password
recovery. Passwords are hashed, but responses are stored unencrypted in
`sandbox\Sandbox\week05\Mindfulness\Web\App_Data\mindfulness.db` on the server.
Restrict access to that directory, protect the certificate and its password,
and keep private backups of the database. Git ignores the data directory.
The database schema is created on first run; the app does not automatically
migrate existing databases when the schema changes.
