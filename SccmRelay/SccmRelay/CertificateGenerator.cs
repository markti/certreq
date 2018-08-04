using SccmRelay.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SccmRelay
{
    public class CertificateGenerator : ICertificateGenerator
    {
        public byte[] GetCertificate(string hostname)
        {
            byte[] allBytes = null;

            var templateName = ConfigurationManager.AppSettings["TemplateName"];
            var caServerName = ConfigurationManager.AppSettings["CertificateAuthority"];

            // C:\CERTREQ
            var rootPath = ConfigurationManager.AppSettings["CertificatePath"];
            var rootDirectory = new DirectoryInfo(rootPath);
            // C:\CERTREQ\Request.Certificate.ps1
            var powershellFilePath = rootDirectory.FullName + "\\Request-Certificate.ps1";
            var powershellFile = new FileInfo(powershellFilePath);

            try
            {
                // C:\CERTREQ\{GUID}
                var newDirectory = rootDirectory.CreateSubdirectory(Guid.NewGuid().ToString());
                
                Console.WriteLine("Creating new sub-directory: " + newDirectory.FullName);

                var newPowerShellExe = newDirectory.FullName + "\\powershell.exe";
                var newPowerShellFilename = newDirectory.FullName + "\\Request-Certificate.ps1";

                // C:\CERTREQ\{GUID}\Request.Certificate.ps1
                var newPowershellFile = powershellFile.CopyTo(newPowerShellFilename);

                Console.WriteLine("Copied PowerShell File: " + newPowershellFile.FullName);
                
                StringBuilder commandBuilder = new StringBuilder();
                commandBuilder.Append("-ExecutionPolicy bypass");
                commandBuilder.Append(" ");
                commandBuilder.Append("-File ");
                commandBuilder.Append("\"");
                commandBuilder.Append(newPowershellFile.FullName);
                commandBuilder.Append("\"");
                commandBuilder.Append(" ");
                commandBuilder.Append("-CN ");
                commandBuilder.Append(hostname);
                commandBuilder.Append(" ");
                commandBuilder.Append("-TemplateName ");
                commandBuilder.Append(templateName);
                commandBuilder.Append(" ");
                commandBuilder.Append("-CAName ");
                commandBuilder.Append(caServerName);
                commandBuilder.Append(" ");
                commandBuilder.Append("-Export");

                var powershellPath = "";

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = newPowerShellExe;
                startInfo.Arguments = commandBuilder.ToString();
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                Console.WriteLine("PowerShell Arguments:" + startInfo.Arguments);
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();

                Console.WriteLine(output);

                string errors = process.StandardError.ReadToEnd();

                Console.WriteLine(errors);

                // C:\CERTREQ\{GUID}\{hostname}.pfx
                var certificatePath = newDirectory.FullName + "\\" + hostname + ".pfx";
                //var certificatePath = "./" + hostname + ".pfx";

                Console.WriteLine("Converting Certificate File to Bytes:" + certificatePath);
                
                Console.WriteLine("About to read certificate here: " + certificatePath);
                allBytes = File.ReadAllBytes(certificatePath); 

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return allBytes;
        }
    }
}