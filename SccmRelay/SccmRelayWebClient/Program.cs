﻿using System;
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
        private const string ClientSecret = "FL3YnTgg5Qb9aRgXeeKx8NAUGnVdcv2G6cfcSvsQ8BmJ24Xj5M4tUcDgb6hmwBLG";

        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("error");
            }
            var hostName = args[0];

            GetCertificate(hostName);
        }

        public static void GetCertificate(string hostName)
        {
            try
            {
                LogCertificateRequested(hostName);

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;

                var postUri = new Uri(hostName + "/api/Certificate");

                var request = (HttpWebRequest)WebRequest.Create(postUri);

                var postData = "{ 'hostName': '" + hostName + "', 'clientSecret': '"  + ClientSecret + "' }";
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

                if (File.Exists("client.pfx"))
                {
                    File.Delete("client.pfx");
                }
                var fileStream = File.OpenWrite("client.pfx");

                CopyStream(responseStream, fileStream);

                fileStream.Flush();
                fileStream.Close();

                LogCertificateReceived(hostName);

            }
            catch (Exception ex)
            {
                LogEcxeption(hostName, ex);
            }
        }
        public static void LogCertificateRequested(string hostName)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;

            var postUri = new Uri(hostName + "/api/Log/Req");

            var request = (HttpWebRequest)WebRequest.Create(postUri);

            var postData = "{ 'hostName': '" + hostName + "', 'clientSecret': '" + ClientSecret + "' }";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
        }

        public static void LogCertificateReceived(string hostName)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;

            var postUri = new Uri(hostName + "/api/Log/Ack");

            var request = (HttpWebRequest)WebRequest.Create(postUri);

            var postData = "{ 'hostName': '" + hostName + "', 'clientSecret': '" + ClientSecret + "' }";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
        }

        public static void LogEcxeption(string hostName, Exception ex)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
            var errorMessage = ex.ToString();

            var postUri = new Uri(hostName + "/api/Log/Error");

            var request = (HttpWebRequest)WebRequest.Create(postUri);

            var postData = "{ 'hostName': '" + hostName + "', 'error': '" + errorMessage + "', 'clientSecret': '" + errorMessage + "' }";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
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