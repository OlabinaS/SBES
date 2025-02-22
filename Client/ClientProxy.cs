﻿using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientProxy : ChannelFactory<IFileManagerServiceAddChange>, IFileManagerServiceAddChange, IDisposable
    {
        private IFileManagerServiceAddChange factory;

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void AddFile(string name, byte[] signature, string text)
        {
            try
            {
                factory.AddFile(name, signature, text);
                Console.WriteLine("File added.");
            }
            catch (FaultException<FileExceptions> e)
            {
                Console.WriteLine($"{e.Detail.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ChangeFile(string name, byte[] signature, string text)
        {
            try
            {
                factory.ChangeFile(name, signature, text);
                Console.WriteLine("File changed");
            }
            catch (FaultException<FileExceptions> e)
            {
                Console.WriteLine($"{e.Detail.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Dispose()
		{
            if(factory != null)
			{
                factory = null;
			}
            this.Close();
		}

    }
}
