/*

Copyright (c) 2004-2009 Deepak Nataraj. All rights reserved.

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
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using GOTransport.Frontend;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Remoting.Channels;
using System.Collections;
using System.Runtime.Serialization.Formatters;
using GOTransport.GOBaseLibrary;
using System.IO;
using GOBaseLibrary.Common;
using GOBaseLibrary.Debugging;
using GOBaseLibrary.Interfaces;

namespace GOTransport.Examples
{
    /// <summary>
    /// expose GOConnection as a remoting object
    /// </summary>
    [QS.Fx.Reflection.ComponentClass("100`1", "GOInitializer")]
    class GOInitializer : QS.Fx.Object.Classes.IObject
    {
        TextWriter tx = new StreamWriter("c:\\gossip_log.txt",true);

        #region constructors

        /// <summary>
        /// expose the GOConnection as a remoting object
        /// </summary>
        /// <param name="_mycontext">context for the liveobject</param>
        /// <param name="_goConnectionRemotingUrl">URL used for the exposed remoting object</param>
        /// <param name="_goConnectionRemotingPort">port used for the exposed remoting object</param>
        /// <param name="_outPorts">ports specific for each instance of the platfom intended to run on local machine</param>
        /// <param name="_gossipIntervals">interval (in milliseconds) between gossip intervals for the above specified instances</param>
        /// <param name="_rumorTimeouts">timeout of rumors (cleanup) for the above specified instances</param>
        /// <param name="_maxT">max timesteps to check (input for content selection algorithm)</param>
        /// <param name="_numberOfRecepients">number of recepients in each gossip round (input for content selection algorithm)</param>
        /// <param name="_debugLevel">debug level used for logging - one of MUTE, TERSE, VERBOSE, VERYVERBOSE</param>
        public GOInitializer(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("GOConnection_URL", QS.Fx.Reflection.ParameterClass.Value)]
            string _goConnectionRemotingUrl,
            [QS.Fx.Reflection.Parameter("GOConnection_PORT", QS.Fx.Reflection.ParameterClass.Value)]
            string _goConnectionRemotingPort,
            [QS.Fx.Reflection.Parameter("OUT_PORTS", QS.Fx.Reflection.ParameterClass.Value)]
            string _outPorts,
            [QS.Fx.Reflection.Parameter("GOSSIP_INTERVALS", QS.Fx.Reflection.ParameterClass.Value)]
            string _gossipIntervals,
            [QS.Fx.Reflection.Parameter("RUMOR_TIMEOUTS", QS.Fx.Reflection.ParameterClass.Value)]
            string _rumorTimeouts,
            [QS.Fx.Reflection.Parameter("MAX_T", QS.Fx.Reflection.ParameterClass.Value)]
            string _maxT,
            [QS.Fx.Reflection.Parameter("NUMBER_OF_RECEPIENTS", QS.Fx.Reflection.ParameterClass.Value)]
            string _numberOfRecepients,
            [QS.Fx.Reflection.Parameter("DEBUG_LEVEL", QS.Fx.Reflection.ParameterClass.Value)]
            string _debugLevel)
        {
            try
            {
                Initialize(_debugLevel);

                tx.WriteLine("starting the service");
                Debug.WriteLineIf(Utils.debugSwitch.Terse,
                                    "\r\n" + DateTime.Now
                                    + "[Thread: " + Thread.CurrentThread.ManagedThreadId + "]"
                                    + "Starting the service");

                BinaryClientFormatterSinkProvider clientProvider = null;
                BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                //Properties for the IPC channel
                IDictionary properties = new Hashtable();
                properties["port"] = _goConnectionRemotingPort;
                properties["typeFilterLevel"] = TypeFilterLevel.Full;
                properties["portName"] = "GossipChannel";
                properties["authorizedGroup"] = "Everyone";

                tx.WriteLine("\r\n" + DateTime.Now
                                    + "[Thread: " + Thread.CurrentThread.ManagedThreadId + "]"
                                    + "GOConnection starting with parameters"
                                    + "port: " + properties["port"]
                                    + ", typeFilterLevel: " + TypeFilterLevel.Full
                                    + ", portName: " + "GossipChannel");

                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    "\r\n" + DateTime.Now
                                    + "[Thread: " + Thread.CurrentThread.ManagedThreadId + "]"
                                    + "GOConnection starting with parameters"
                                    + "port: " + properties["port"]
                                    + ", typeFilterLevel: " + TypeFilterLevel.Full
                                    + ", portName: " + "GossipChannel");

                // create the channel
                IpcChannel chan = new IpcChannel(properties, clientProvider, serverProvider);

                // register the channel
                ChannelServices.RegisterChannel(chan);

                RemotingConfiguration.RegisterWellKnownServiceType(typeof(IGOConnection),
                                                                "IGOConnection",
                                                                WellKnownObjectMode.Singleton);
                // expose the service
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(GOConnection),
                                                                    "GOConnection",
                                                                    WellKnownObjectMode.Singleton);


                // create the first (and only) instance of the GOConnection.
                // subsequent calls to Activator.GetObject should return this
                // same instance every time it is called
                Type typeOfRemoteObject = typeof(GOConnection);
                String urlOfRemoteObject = _goConnectionRemotingUrl;
                GOConnection goConnection = (GOConnection)Activator.GetObject(typeOfRemoteObject,
                                urlOfRemoteObject);

                Dictionary<string, string> loConfig = new Dictionary<string, string>();
                loConfig["MAX_T"] = _maxT;
                loConfig["NUMBER_OF_RECEPIENTS"] = _numberOfRecepients;
                loConfig["OUT_PORTS"] = _outPorts;
                loConfig["GOSSIP_INTERVALS"] = _gossipIntervals;
                loConfig["RUMOR_TIMEOUTS"] = _rumorTimeouts;

                // activate the first (and only) instance of the GOConnection
                goConnection.Touch(loConfig);

                tx.WriteLine("GOConnection started!");
                tx.Flush();
                Debug.WriteLineIf(Utils.debugSwitch.Terse,
                                    "\r\n" + DateTime.Now
                                    + "[Thread: " + Thread.CurrentThread.ManagedThreadId + "]"
                                    + "GOConnection started");
                tx.Close();
                

                // an infinite lock, to keep the exposed object active
                new AutoResetEvent(false).WaitOne();
            }
            catch (Exception e)
            {
                
                tx.WriteLine("\r\n" + DateTime.Now + " exception when starting GOConnection: " + e + e.StackTrace);

                tx.Flush();
                tx.Close();
                Debug.WriteLine("\r\n" + DateTime.Now
                                + "[Thread: " + Thread.CurrentThread.ManagedThreadId + "]"
                                + "An exception occurred while exposing the GOConnection: " + e);
            }
        }

#endregion

        #region private methods

        private static void Initialize(String _debugLevel)
        {
            if (_debugLevel.Equals("MUTE", StringComparison.CurrentCultureIgnoreCase))
            {
                Utils.InitDebugging(DebugSwitchLevel.Mute);
            }
            else if (_debugLevel.Equals("TERSE", StringComparison.CurrentCultureIgnoreCase))
            {
                Utils.InitDebugging(DebugSwitchLevel.Terse);
            }
            else if (_debugLevel.Equals("VERBOSE", StringComparison.CurrentCultureIgnoreCase))
            {
                Utils.InitDebugging(DebugSwitchLevel.Verbose);
            }
            else if (_debugLevel.Equals("VERYVERBOSE", StringComparison.CurrentCultureIgnoreCase))
            {
                Utils.InitDebugging(DebugSwitchLevel.VeryVerbose);
            }else{
                Utils.InitDebugging(DebugSwitchLevel.Verbose);
            }
        }

        #endregion
    }
}
