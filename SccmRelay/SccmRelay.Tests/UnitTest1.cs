using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SccmRelay.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"powershell.exe";
            startInfo.Arguments = @"-ExecutionPolicy bypass -File C:\CERTREQ\Request-Certificate.ps1";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            
            Debug.WriteLine(output);

            string errors = process.StandardError.ReadToEnd();

            Debug.WriteLine(errors);

            Assert.IsTrue(string.IsNullOrEmpty(errors));
        }
    }
}
