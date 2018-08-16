using System;
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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace SccmRelayWeb.Controllers
{
    public class CertificateController : ApiController
    {
        public class LogCertificateReqRequest
        {
            public string HostName { get; set; }
            public string ClientSecret { get; set; }
        }
        public class LogCertificateAckRequest
        {
            public string HostName { get; set; }
            public string ClientSecret { get; set; }
        }
        public class LogErrorRequest
        {
            public string HostName { get; set; }
            public string ClientSecret { get; set; }
        }

        public class CertificateRequest
        {
            public string HostName { get; set; }
            public bool GenerateRandom { get; set; }
            public string ClientSecret { get; set; }
        }

        // GET api/values/5
        [SwaggerOperation("GetByHostname")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]CertificateRequest request)
        {
            HttpResponseMessage result = null;
            

            if (request == null)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return result;
            }

            var actualSecretKey = ConfigurationManager.AppSettings["ClientSecretKey"];

            if(string.IsNullOrEmpty(request.ClientSecret) || !request.ClientSecret.Equals(actualSecretKey))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return result;
            }

            var hostName = "client-endpoint";

            if (!string.IsNullOrEmpty(request.HostName))
            {
                hostName = request.HostName;
            }

            if(request.GenerateRandom)
            {
                hostName = "client-" + Guid.NewGuid().ToString().Replace("-", "");
            }

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



        // GET api/values/5
        [SwaggerOperation("LogError")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpPost]
        [Route("/api/Log/Error")]
        public HttpResponseMessage LogError([FromBody]LogErrorRequest request)
        {
            HttpResponseMessage result = null;


            if (request == null)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return result;
            }

            var actualSecretKey = ConfigurationManager.AppSettings["ClientSecretKey"];

            if (string.IsNullOrEmpty(request.ClientSecret) || !request.ClientSecret.Equals(actualSecretKey))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return result;
            }

            CloudStorageAccount storageAccount = new CloudStorageAccount(
        new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
            "<name>", "<account-key>"), true);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            return result;
        }
        // GET api/values/5
        [SwaggerOperation("LogError")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpPost]
        [Route("/api/Log/Req")]
        public HttpResponseMessage LogCertReq([FromBody]LogErrorRequest request)
        {
            HttpResponseMessage result = null;


            if (request == null)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return result;
            }

            var actualSecretKey = ConfigurationManager.AppSettings["ClientSecretKey"];

            if (string.IsNullOrEmpty(request.ClientSecret) || !request.ClientSecret.Equals(actualSecretKey))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return result;
            }

            CloudStorageAccount storageAccount = new CloudStorageAccount(
        new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
            "<name>", "<account-key>"), true);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            return result;
        }
        // GET api/values/5
        [SwaggerOperation("LogError")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpPost]
        [Route("/api/Log/Ack")]
        public HttpResponseMessage LogCertAck([FromBody]LogErrorRequest request)
        {
            HttpResponseMessage result = null;


            if (request == null)
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return result;
            }

            var actualSecretKey = ConfigurationManager.AppSettings["ClientSecretKey"];

            if (string.IsNullOrEmpty(request.ClientSecret) || !request.ClientSecret.Equals(actualSecretKey))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return result;
            }

            CloudStorageAccount storageAccount = new CloudStorageAccount(
        new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
            "<name>", "<account-key>"), true);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            return result;
        }
    }
}