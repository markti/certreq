using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace SccmRelayWebClient
{

    /// <summary>
    /// this client is for machines that do not have powershell 3.0
    /// powershell 3.0 introduced the Invoke-Webrequest command
    /// powershell 3.0 requires full version of .net framework 4.0
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("error");
            }
            var hostName = args[0];
            var machineName = Environment.MachineName;

            var postUri = new Uri(hostName + "/api/Certificate");

            var request = (HttpWebRequest)WebRequest.Create(postUri);

            var postData = "{ 'hostName': '" + machineName + "' }";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream);

            var fileStream = File.OpenWrite("client.pfx");

            CopyStream(responseStream, fileStream);

            fileStream.Flush();
            fileStream.Close();

            Console.ReadLine();
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}