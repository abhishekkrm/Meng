/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following
   disclaimer in the documentation and/or other materials provided
   with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.

*/

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_e_.Repository_
{
/*
    public class Service : MarshalByRefObject, IRepositoryService, QS.TMS.Inspection.IInspectable
    {
        private static System.Runtime.Remoting.Channels.Tcp.TcpChannel myTcpChannel;

        public static IRepositoryService GetService(string serverAddress)
        {
            lock (typeof(Service))
            {
                if (myTcpChannel == null)
                {
                    myTcpChannel = new System.Runtime.Remoting.Channels.Tcp.TcpChannel();
                    System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(myTcpChannel, true);
                }
            }

            return (QS.TMS.Repository.IRepositoryService) Activator.GetObject(typeof(QS.TMS.Repository.IRepositoryService),
                "tcp://" + serverAddress + "/Repository.rem");

/-*
            bool found = false;
            foreach (System.Runtime.Remoting.Channels.IChannel channel in
                System.Runtime.Remoting.Channels.ChannelServices.RegisteredChannels)
            {
                found = channel is System.Runtime.Remoting.Channels.Tcp.TcpChannel;
                if (found)
                    break;
            }

            if (!found)
                
*-/
        }


        public Service(string root, QS.Fx.Logging.ILogger logger) : this(new QS.TMS.Repository.Repository(root), logger)
        {
        }

        public Service(QS.TMS.Repository.IRepository repository, QS.Fx.Logging.ILogger logger)
        {
            this.logger = logger;
            this.repository = repository;
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.TMS.Repository.IRepository repository;

        #region IRepositoryService Members

        QS.CMS.Base3.RegularSO IRepositoryService.Get(string link)
        {            
            return new QS.CMS.Base3.RegularSO(((QS.TMS.Repository.ScalarAttribute) repository.AttributeOf(link)).Object);
        }

        string IRepositoryService.Add(string[] path, QS.CMS.Base3.RegularSO dataObject)
        {
            try
            {
                return ((QS.TMS.Repository.IAttribute)QS.TMS.Repository.Repository.Save(repository, path, dataObject.EncapsulatedObject)).Ref;
            }
            catch (Exception exc)
            {
                logger.Log(this, exc.ToString());
                throw;
            }
        }

        #endregion

        #region IInspectable Members

        QS.TMS.Inspection.IAttributeCollection QS.TMS.Inspection.IInspectable.Attributes
        {
            get { return repository; }
        }

        #endregion
    }
*/ 
}
