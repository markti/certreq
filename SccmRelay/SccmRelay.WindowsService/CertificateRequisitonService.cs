﻿using Microsoft.ServiceBus;
using SccmRelay;
using SccmRelay.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SccmRelay.WindowsService
{
    public partial class CertificateRequisitonService : ServiceBase
    {
        public CertificateRequisitonService()
        {
            InitializeComponent();
        }

        ServiceHost sh;

        protected override void OnStart(string[] args)
        {
            //sh = new ServiceHost(typeof(CertificateGenerator));
            //sh = new ServiceHost(null);

            var localAddress = ConfigurationManager.AppSettings["LocalAddress"];
            var serviceNamespace = ConfigurationManager.AppSettings["ServiceNamespace"];
            var serviceKey = ConfigurationManager.AppSettings["ServiceKey"];
            var serviceKeyName = ConfigurationManager.AppSettings["ServiceKeyName"];
            var servicePath = ConfigurationManager.AppSettings["ServicePath"];

            sh.AddServiceEndpoint(typeof(ICertificateGenerator), new NetTcpBinding(), localAddress);

            sh.AddServiceEndpoint(
               typeof(ICertificateGenerator), new NetTcpRelayBinding(),
               ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, servicePath))
                .Behaviors.Add(new TransportClientEndpointBehavior
                {
                    TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(serviceKeyName, serviceKey)
                });

            sh.Open();
        }

        protected override void OnStop()
        {
            if (sh != null)
            {
                sh.Close();
                sh = null;
            }
        }
    }
}