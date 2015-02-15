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
using System.ServiceModel;

namespace QS._qss_x_.Service_Old_
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class Service : QS.Fx.Inspection.Inspectable, IServiceControl, IService, IDisposable, QS.Fx.Inspection.IAttribute
    {
        #region Constructor

        public Service(QS._core_c_.Base.IReadableLogger logger, QS.Fx.Platform.IPlatform platform, Configuration configuration)
        {
            this.logger = logger;
            this.platform = platform;
            this.configuration = configuration;

            ns = new QS._qss_x_.Namespace_.Namespace(platform);

            bbnode = new QS._qss_x_.Backbone_.Node.Node(configuration.Name, QS.Fx.Base.ID.NewID(), 
                platform, new QS._qss_c_.Base1_.Subnet(configuration.Subnet), configuration.Port);

            bbcontroller = new QS._qss_x_.Backbone_.Controller.Controller(bbnode, configuration.Controller);
            controllernsref = new QS._qss_x_.Backbone_.Namespace.ControllerRef(bbcontroller, ns);

            bbnode.Controller = bbcontroller;

            if (configuration.Connections != null)
            {
                foreach (Configuration.Connection connection in configuration.Connections)
                    ((QS._qss_x_.Backbone_.Node.INode)bbnode).Connect(connection.Name, new QS._qss_x_.Base1_.Address(connection.Address));
            }
        }

        #endregion

        #region Fields

        private QS._core_c_.Base.IReadableLogger logger;
        private Backbone_.Namespace.ControllerRef controllernsref;

        [QS.Fx.Base.Inspectable] private QS.Fx.Platform.IPlatform platform;
        [QS.Fx.Base.Inspectable] private Configuration configuration;
        [QS.Fx.Base.Inspectable] private QS._qss_x_.Namespace_.Namespace ns;
        [QS.Fx.Base.Inspectable] private QS._qss_x_.Backbone_.Controller.Controller bbcontroller;
        [QS.Fx.Base.Inspectable] private QS._qss_x_.Backbone_.Node.Node bbnode;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            ((IDisposable) ns).Dispose();
            ((IDisposable) bbnode).Dispose();
        }

        #endregion

        #region QS.TMS.Inspection.IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return "QS.Fx.Service.Service(\"" + configuration.Name + ":" + configuration.Port + "\")"; }
        }

        #endregion

        #region IServiceControl Members

        QS._qss_x_.Namespace_.INamespace IServiceControl.Namespace
        {
            get { return ns; }
        }

        #endregion

        #region IService Members

        int IService.NumberOfLogMessages
        {
            get { return (int) logger.NumberOfMessages; }
        }

        string[] IService.DownloadLogMessages(int from, int to)
        {
            return logger.rangeAsArray((uint) (from - 1), (uint) (to - from + 1));
        }

        ulong IService.NamespaceRoot
        {
            get
            {
                return ((Namespace_.INamespaceControl) ns).Root.Identifier;
            }
        }

        void IService.NamespaceGetObjectInfo(ulong identifier, out ObjectInfo info)
        {
            Namespace_.IObject obj;
            if (((Namespace_.INamespaceControl)ns).Lookup(identifier, out obj))
            {
                info = new ObjectInfo();

                info.Category = (uint) ((int) obj.Category);
                info.Name = obj.Name;
                info.IsContainer = obj.IsFolder;
                if (info.IsContainer)
                {
                    Namespace_.IFolder folder = (Namespace_.IFolder)obj;
                    info.HasObjects = folder.HasObjects;
                }
                else
                    info.HasObjects = false;
            }
            else
                throw new Exception("No node with id = " + identifier.ToString("x") + " exists in the namespace.");
        }

        void IService.NamespaceGetSubObjects(ulong nodeid, out ulong[] subfolderids)
        {
            Namespace_.IObject obj;
            if (((Namespace_.INamespaceControl)ns).Lookup(nodeid, out obj))
            {
                if (obj.IsFolder)
                {
                    Namespace_.IFolder folder = (Namespace_.IFolder)obj;
                    List<ulong> ids = new List<ulong>();
                    foreach (Namespace_.IObject _obj in folder.Objects)
                        ids.Add(_obj.Identifier);
                    subfolderids = ids.ToArray();
                }
                else
                    subfolderids = new ulong[0];
            }
            else
                throw new Exception("No node with id = " + nodeid.ToString("x") + " exists in the namespace.");
        }

        void IService.NamespaceGetActionInfos(ulong identifier, out ActionInfo[] actioninfos)
        {
            Namespace_.IObject obj;
            if (((Namespace_.INamespaceControl)ns).Lookup(identifier, out obj))
            {
                List<ActionInfo> _actioninfos = new List<ActionInfo>();
                foreach (Namespace_.IAction action in obj.Actions)
                {
                    ActionInfo info = new ActionInfo();
                    info.Identifier = action.Identifier;
                    info.Name = action.Name;
                    info.Context = action.Context;
                    _actioninfos.Add(info);
                }
                actioninfos = _actioninfos.ToArray();
            }
            else
                throw new Exception("No node with id = " + identifier.ToString("x") + " exists in the namespace.");
        }

        void IService.NamespaceInvokeAction(ulong nodeid, ulong actionid, ulong actioncontext)
        {
            Namespace_.IObject obj;
            if (((Namespace_.INamespaceControl)ns).Lookup(nodeid, out obj))
            {
                obj.Invoke(actionid, actioncontext);
            }
            else
                throw new Exception("No node with id = " + nodeid.ToString("x") + " exists in the namespace.");
        }

        #endregion
    }
}
