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

            try
            {

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"powershell.exe";
                startInfo.Arguments = @"-ExecutionPolicy bypass -File ""C:\CERTREQ\Request-Certificate.ps1"" -CN " + hostname + " -TemplateName " + templateName + " -CAName " + caServerName + " -Export";
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                Console.WriteLine("About to execute Process: " + startInfo.Arguments);
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();

                Console.WriteLine(output);

                string errors = process.StandardError.ReadToEnd();

                Console.WriteLine(errors);


                var certFilename = "./" + hostname + ".pfx";

                Console.WriteLine("About to read certificate here: " + certFilename);
                allBytes = File.ReadAllBytes(certFilename);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return allBytes;
        }
    }
}