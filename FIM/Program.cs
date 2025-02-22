﻿using Manager;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FIM
{
    public class Program
    {
        public static string pa = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())), "..\\Files\\"));
        public static string fimConfig = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())), "FimConfig.txt"));
        static void Main(string[] args)
        {
            Console.ReadLine();
            Thread thread = new Thread(() => CheckingFunction());
            thread.Start();
            Console.ReadLine();

        }

        public static void CheckingFunction()
        {
            string srvCertCN = "ips"; //IPS

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertificateManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:8888/IIPSService"),new X509CertificateEndpointIdentity(srvCert));
            
            try
            {
                int msSleep = 7000;

                using (FimIPS proxy = new FimIPS(binding, address))
                {
                    while (true)
                    {
                        Console.WriteLine("Validating files...");
                        List<string> filenames = File.ReadAllText(fimConfig).Split('\n').Select(x => x.Replace("\r", string.Empty)).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();

                        FimService service = new FimService();

                        filenames.ForEach(currentFilename =>
                        {
                            //Alarm al = new Alarm(DateTime.Now, pa, AuditEventTypes.Information, currentFilename);
                            //proxy.LogInformation(al);

                            Alarm alarm = service.VerifySignature(currentFilename);
							
                            if (alarm != null)
                            {
                                //Console.WriteLine($"{alarm.Filename}\t{alarm.Risk}");
                                switch (alarm.Risk)
                                {
                                    case AuditEventTypes.Critical:
                                        proxy.LogCritical(alarm);
                                        break;

                                    case AuditEventTypes.Information:
                                        proxy.LogInformation(alarm);
                                        break;

                                    case AuditEventTypes.Warning:
                                        proxy.LogWarning(alarm);
                                        break;
                                }
                            }
                        });

                        Thread.Sleep(msSleep);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FIM error: {0}\n", ex.Message);
            }
        }


    }
}
