﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
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
using System.Net.Http.Headers;

namespace SccmRelayWeb.Controllers
{
    public class CertificateController : ApiController
    {
        public class CertificateRequest
        {
            public string HostName { get; set; }
        }

        // GET api/values/5
        [SwaggerOperation("GetByHostname")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]CertificateRequest request)
        {
            HttpResponseMessage result = null;

            if(request == null || string.IsNullOrEmpty(request.HostName))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return result;
            }

            var hostName = request.HostName;

            byte[] data = null;

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
                data = ch.GetCertificate(hostName);
            }
            result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(new MemoryStream(data));
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "client.pfx";
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }
    }
}