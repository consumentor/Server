using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using NBehave.Spec.NUnit;

namespace ShopGunSpecBase
{
    public abstract class WebServiceSpec<TGateway> : SpecBase<TGateway>
    {
        protected abstract void Setup_webService();
        protected abstract TGateway Given_this_gateway();
        protected abstract void When_this_service_is_called();

        private readonly List<Process> _webServiceProcess = new List<Process>();
        private readonly List<ServiceUnderTest> _servicesToStart = new List<ServiceUnderTest>();

        protected override void Because()
        {
            Wait_for_services_to_start();
            When_this_service_is_called();
        }

        protected override TGateway Given_these_conditions()
        {
            return Given_this_gateway();
        }

        protected override void Before_all_specs()
        {
            Setup_webService();
            StartWebServices();
            base.Before_all_specs();
        }

        protected virtual void Wait_for_services_to_start()
        {
        }

        private void StartWebServices()
        {
            foreach (ServiceUnderTest webServiceUnderTest in _servicesToStart)
            {
                var process = new Process();
                process.StartInfo = new ProcessStartInfo(webServiceUnderTest.Executable);
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(webServiceUnderTest.Executable);
                process.StartInfo.Arguments = webServiceUnderTest.Arguments;
                process.Start();
                _webServiceProcess.Add(process);
            }
            Thread.Sleep(100);
        }

        protected override void After_all_specs()
        {
            RestoreConnectionStrings();
            KillProcesses();
        }

        private void KillProcesses()
        {
            foreach (Process process in _webServiceProcess)
            {
                process.Kill();
            }
        }

        private void RestoreConnectionStrings()
        {
            foreach (var serviceUnderTest in _servicesToStart)
            {
                serviceUnderTest.RestoreConnectionStringsConfigFiles();
            }
        }

        protected void AddServiceToStart(ServiceUnderTest serviceUnderTest)
        {
            _servicesToStart.Add(serviceUnderTest);
        }

        protected void WaitForServiceToStart(string uri)
        {
            bool httpOk = false;
            int tries = 0;
            while ((!httpOk) && (tries < 20))
            {
                try
                {
                    tries++;
                    var webRequest = WebRequest.Create(uri);

                    using (var webResponse = webRequest.GetResponse())
                    {
                        httpOk = true;
                    }

                }
                catch (Exception)
                {
                    httpOk = false;
                    Thread.Sleep(100);
                }
            }
            if (httpOk == false)
                Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Host for uri {0} not up yet?", uri));
        }

        protected void DoWebServiceCallWithRetries(Action action)
        {
            DoWebServiceCallWithRetries(action, 3);
        }

        protected void DoWebServiceCallWithRetries(Action action, int retries)
        {
            int i = 0;
            while (i < retries)
            {
                try
                {
                    action.Invoke();
                    i = retries;
                }
                catch (Exception)
                {
                    if (i >= retries)
                        throw;
                    Thread.Sleep(100);
                    i++;
                }
            }
        }
    }
}
