using Microsoft.ServiceBus;
using SccmRelay.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SccmRelayClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostName = Dns.GetHostName();

            if(args.Length < 1)
            {
            }
            else
            {
                hostName = args[0];
            }

            var serviceNamespace = ConfigurationManager.AppSettings["ServiceNamespace"];
            var serviceKey = ConfigurationManager.AppSettings["ServiceKey"];
            var serviceKeyName = ConfigurationManager.AppSettings["ServiceKeyName"];
            var servicePath = ConfigurationManager.AppSettings["ServicePath"];

            Console.WriteLine("HOSTNAME: " + hostName);

            var cf = new ChannelFactory<ICertificateGeneratorChannel>(
               new NetTcpRelayBinding(),
                new EndpointAddress(ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, servicePath)));

            cf.Endpoint.Behaviors.Add(new TransportClientEndpointBehavior
            { TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(serviceKeyName, serviceKey) });

            using (var ch = cf.CreateChannel())
            {
                var data = ch.GetCertificate(hostName);
                File.WriteAllBytes("client.pfx", data);
            }
        }
    }
}