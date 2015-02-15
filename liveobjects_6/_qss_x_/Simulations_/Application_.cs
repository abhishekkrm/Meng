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
using System.Net;

namespace QS._qss_x_.Simulations_
{
    [QS._qss_x_.Platform_.ApplicationAttribute("Application_")]
    public sealed class Application_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Platform_.IApplication
    {
        public Application_()
        {
        }

        void IDisposable.Dispose()
        {
            if ((_object != null) && (_object is IDisposable))
                ((IDisposable)_object).Dispose();

            _object = null;
            _objectname = null;
            _objectclassname = null;
            _objectref = null;
            _objectxml = null;
            _platform = null;
        }

        void QS._qss_x_.Platform_.IApplication.Start(QS.Fx.Platform.IPlatform _platform, QS._qss_x_.Platform_.IApplicationContext _context)
        {
            this._platform = _platform;
/*
            this._type =  _context.Arguments["type"];
            _platform.Logger.Log("Acting as a " + this._type + ".");
            this._objectxml = _context.Arguments[this._type];
*/
            this._objectxml = _context.Arguments["object"];
            _platform.Logger.Log("Object:\n\n" + _objectxml.ToString() + "\n\n");


            QS.Fx.Object.IContext _mycontext = 
                new QS._qss_x_.Object_.Context_(
                    _platform, 
                    QS._qss_x_.Object_.Context_.ErrorHandling_.Halt, 
                    QS.Fx.Object.Runtime.SynchronizationOption, 
                    QS.Fx.Object.Runtime.SynchronizationOption);

            using (QS._qss_x_.Component_.Classes_.Loader _loader = new QS._qss_x_.Component_.Classes_.Loader(
                _mycontext,
                QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILibrary>>.Create(
                    QS._qss_x_.Reflection_.Library.LocalLibrary.GetComponentClass(QS.Fx.Reflection.ComponentClasses.Library))))
            {
                QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject> _interface;
                using (QS._qss_x_.Component_.Classes_.Service<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>.Connect(
                    _mycontext, _loader, out _interface))
                {
                    this._objectref = _interface.Load(_objectxml);
                }
            }
            if (_objectref != null)
            {
                QS.Fx.Attributes.IAttribute _nameattribute;
                if (_objectref.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute))
                    this._objectname = "Object \"" + _nameattribute.Value + "\"";
                else
                    this._objectname = "Unnamed Object";

                QS.Fx.Attributes.IAttribute _classnameattribute;
                if (_objectref.ObjectClass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _classnameattribute))
                    this._objectclassname = "class \"" + _classnameattribute.Value + "\"";
                else
                    this._objectclassname = "unnamed class";

                this._object = _objectref.Dereference(_mycontext);

                if (this._object is QS._qss_x_.Platform_.IApplication)
                    ((QS._qss_x_.Platform_.IApplication)this._object).Start(this._platform, null);
                else
                    throw new Exception("The object does not implement IApplication.");

                _platform.Logger.Log("Running " + _objectname + " of " + _objectclassname + ".");
            }
            else
                _platform.Logger.Log("Cannot run, the object is null.");

/*
            nic = platform.Network.Interfaces[0];
            platform.Logger.Log("Using network interface adapter at: " + nic.InterfaceAddress.ToString());
            listener = nic.Listen(
                new QS.Fx.Network.NetworkAddress(MulticastAddress), new QS.Fx.Network.ReceiveCallback(_ReceiveCallback), null, null);
            sender = nic.GetSender(new QS.Fx.Network.NetworkAddress(MulticastAddress));
            alarm = platform.AlarmClock.Schedule(1, new QS.Fx.Clock.AlarmCallback(_SendCallback), null);
*/
        }

        void QS._qss_x_.Platform_.IApplication.Stop()
        {
/*
            platform.Scheduler.Execute(new AsyncCallback(_TerminateCallback), alarm);
            alarm = null;
            listener.Dispose();
            listener = null;
            sender = null;
            nic = null;
*/

            if (this._object is QS._qss_x_.Platform_.IApplication)
                ((QS._qss_x_.Platform_.IApplication) this._object).Stop();
        }

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Platform.IPlatform _platform;
/*
        [QS.Fx.Base.Inspectable]
        private string _type;
*/
        [QS.Fx.Base.Inspectable]
        private string _objectxml;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
        [QS.Fx.Base.Inspectable]
        private string _objectname;
        [QS.Fx.Base.Inspectable]
        private string _objectclassname;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IObject _object;

/*
        private const string MulticastAddress = "224.66.66.66:12345";

        private QS.Fx.Network.INetworkInterface nic;
        private QS.Fx.Network.IListener listener;
        private QS.Fx.Network.ISender sender;
        private QS.Fx.Clock.IAlarm alarm;
        private int messageno;

        private void _TerminateCallback(IAsyncResult result)
        {
            QS.Fx.Clock.IAlarm alarm = (QS.Fx.Clock.IAlarm) result.AsyncState;
            if (!alarm.Cancelled)
                alarm.Cancel();
        }

        private void _SendCallback(QS.Fx.Clock.IAlarm alarm)
        {
            string message = (++messageno).ToString();
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(message);
            IList<QS.Fx.Base.Block> totransmit = new List<QS.Fx.Base.Block>();
            totransmit.Add(new QS.Fx.Base.Block(bytes));
            platform.Logger.Log("Sending \"" + message + "\".");
            sender.Send(new QS.Fx.Network.Data(totransmit), new QS.Fx.Base.ContextCallback(_TransmitCallback), message);
            if (!alarm.Cancelled)
                alarm.Reschedule();
        }

        private void _TransmitCallback(object context)
        {
            string message = (string)context;
            platform.Logger.Log("Transmitted \"" + message + "\".");
        }

        private void _ReceiveCallback(IPAddress ipaddress, int port, QS.Fx.Base.Block data, object context)
        {
            string message = ASCIIEncoding.ASCII.GetString(data.buffer, (int)data.offset, (int)data.size);
            platform.Logger.Log("Received \"" + message + "\" from " + ipaddress.ToString() + ".");
        }
*/ 
    }
}
