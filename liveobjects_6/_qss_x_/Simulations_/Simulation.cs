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

#define DEBUG_LogGenerously

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;

namespace QS._qss_x_.Simulations_
{    
    public sealed class Simulation : QS.Fx.Inspection.Inspectable, ISimulation, IDisposable, QS.Fx.Base.IParametrized
        //, QS.TMS.Management.IManagedComponent
    {
        public Simulation(QS.Fx.Clock.IClock physicalClock, QS._qss_c_.Base1_.Subnet subnet, 
            int nnodes,
            QS._qss_x_.Simulations_.ITask_[] tasks,
/*
            int nclients, int nservers, 
*/            
            double mttb, double mttf, double mttr,
            double bandwidthmegabytespersecond, double timebetweenframesmicroseconds, double lossratepercent,
            Type applicationType, IDictionary<string, string> arguments, double maximumcpuusage, double maxstepspersecond)
        {
            parameters.RegisterLocal(this);

            this.physicalClock = physicalClock;
            this.subnet = subnet;
/*
            int nnodes = nclients + nservers + 1;
*/
            this.nnodes = nnodes;
            this.applicationType = applicationType;
            this.arguments = (arguments != null) ? arguments : new Dictionary<string, string>();
            this.mttf = mttf;
            this.mttr = mttr;
            this.mttb = mttb;

            if (!typeof(Platform_.IApplication).IsAssignableFrom(applicationType))
                throw new Exception("Application type must implement QS.Fx.Platform.IApplication.");

            applicationConstructorInfo = applicationType.GetConstructor(Type.EmptyTypes);
            if (applicationConstructorInfo == null)
                throw new Exception("Application type must have a default no-argument constructor.");

            applicationConstructor = new QS._qss_c_.Base3_.Constructor<QS._qss_x_.Platform_.IApplication>(this.ApplicationConstructorCallback);

            mainlogger = new QS._qss_c_.Base3_.Logger(null,  true, null);
            
            mainlogger.Log(this, "Initializing the simulation with " + nnodes.ToString() + " nodes on subnet " + subnet.ToString() + ".");
 
            simulatedClock = new QS._qss_c_.Simulations_1_.SimulatedClock(mainlogger); // new QS._qss_c_.Collections_4_.BinaryTree());
            // logger = new QS.CMS.Base.Logger(simulatedClock, true);
            eventLogger = new QS._qss_c_.Logging_1_.EventLogger(simulatedClock, true);

            mainlogger.Clock = simulatedClock;

            network = new QS._qss_x_.Network_.VirtualNetwork(subnet, simulatedClock, simulatedClock,
                bandwidthmegabytespersecond * 1024.0 * 1024.0, timebetweenframesmicroseconds * 0.000001, lossratepercent * 0.01);

            this.nnodes = tasks.Length; // Added to allow there to be auxiliary support nodes to the original nnodes we want
            nodenames = new string[this.nnodes];
            for (int ind = 0; ind < this.nnodes; ind++)
                nodenames[ind] = tasks[ind]._Name;

/*
            nodenames = new string[nclients + nservers + 1];
            nodenames[0] = "bootstrap";
            for (int ind = 0; ind < nservers; ind++)
                nodenames[ind + 1] = "server_" + (ind + 1).ToString();
            for (int ind = 0; ind < nclients; ind++)
                nodenames[ind + 1 + nservers] = "client_" + (ind + 1).ToString();
*/

            applicationContext = new QS._qss_x_.Platform_.ApplicationContext(nodenames, arguments);

            node_logs = new Dictionary<string, QS._core_c_.Base.IOutputReader>();
            _node_logs_inspectable = 
                new QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS._core_c_.Base.IOutputReader>(
                    "Node Logs", node_logs,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS._core_c_.Base.IOutputReader>.ConversionCallback(
                        delegate(string s) { return s; }));

            nodes = new Dictionary<string, Node>(this.nnodes);
            for (int ind = 0; ind < this.nnodes; ind++)
            {
                string _nodename = nodenames[ind];
/*
                bool _isstable = (ind <= nservers);
*/
                IDictionary<string, string> nodeargs = new Dictionary<string, string>(arguments);
/*
                if (ind == 0)
                    nodeargs.Add("type", "bootstrap");
                else if (ind <= nservers)
                    nodeargs.Add("type", "service");
                else
                    nodeargs.Add("type", "client");
*/

                string _objectxml = tasks[ind]._ObjectXml;
                if (_objectxml == null)
                {
                    StringBuilder _stringbuilder = new StringBuilder();
                    using (StringWriter _writer = new StringWriter(_stringbuilder))
                    {
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object = tasks[ind]._Object;
                        (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Serialize(_writer, 
                            (new QS.Fx.Reflection.Xml.Root((_object != null) ? _object.Serialize : null)));
                    }
                    _objectxml = _stringbuilder.ToString();
                }
                nodeargs.Add("object", _objectxml);

                QS._qss_x_.Platform_.ApplicationContext nodecontext = new QS._qss_x_.Platform_.ApplicationContext(nodenames, nodeargs);
                Node _node = 
                    new Node(
                        _nodename,
                        tasks[ind]._MTTB,
                        tasks[ind]._MTTF,
                        tasks[ind]._MTTR,
                        applicationConstructor,
                        nodecontext,
                        physicalClock, 
                        simulatedClock, 
                        simulatedClock, 
                        network, 
                        eventLogger);
                nodes.Add(_nodename, _node);                
                node_logs.Add(_nodename, ((INode) _node).Log);
            }
            _nodes_inspectable =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<string, Node>("Nodes", nodes,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<string, Node>.ConversionCallback(delegate(string s) { return s; }));
            
            foreach (Node node in nodes.Values)
            {
                ((INode) node).Start();
            }

            // ......................

            cpuusage = maximumcpuusage;
            stepsasec = maxstepspersecond;

            simulationThread = new Thread(new ThreadStart(this.SimulationMainLoop));
            release = new ManualResetEvent(false);
            // finishedEvent = new ManualResetEvent(false);
            simulationThread.Start();
        }

        
        private IDictionary<string, QS._core_c_.Base.IOutputReader> node_logs;
        [QS.Fx.Base.Inspectable("Node Logs")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS._core_c_.Base.IOutputReader> _node_logs_inspectable;

        private static Random random = new Random();

        private Thread simulationThread;
        private ManualResetEvent release; // , finishedEvent;
        private bool running, exiting, finished;
        private int nprocessed, nprocessed_thissec;
        private double mttb, mttf, mttr, lastcheck, timeused, cpuusage, stepsasec;
//        private event EventHandler onChange;

        private QS.Fx.Clock.IClock physicalClock;
        private ConstructorInfo applicationConstructorInfo;
        private QS._qss_c_.Base3_.Constructor<Platform_.IApplication> applicationConstructor;

        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base1_.Subnet subnet;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Base.Parameter(Platform_.EnvironmentInfo.Parameters.NumberOfNodes, QS.Fx.Base.ParameterAccess.Readable)]
        private int nnodes;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Base.Parameter(Platform_.EnvironmentInfo.Parameters.NodeNames, QS.Fx.Base.ParameterAccess.Readable)]
        private string[] nodenames;
        [QS.Fx.Base.Inspectable]
        private Type applicationType;
        [QS.Fx.Base.Inspectable]
        private IDictionary<string, string> arguments;
        [QS.Fx.Base.Inspectable]
        private Platform_.ApplicationContext applicationContext;

        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base3_.Logger mainlogger; // , logger;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Simulations_1_.SimulatedClock simulatedClock;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Logging_1_.EventLogger eventLogger;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Network_.VirtualNetwork network;
        
        private IDictionary<string, Node> nodes;
        [QS.Fx.Base.Inspectable("Nodes")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<string, Node> _nodes_inspectable;


        #region IParametrized Members

        [QS.Fx.Printing.NonPrintable]
        private QS._core_x_.Base.Parameters parameters = new QS._core_x_.Base.Parameters();

        [QS.Fx.Printing.NonPrintable]
        QS.Fx.Base.IParameters QS.Fx.Base.IParametrized.Parameters
        {
            get { return parameters; }
        }

        #endregion

        #region ApplicationConstructorCallback

        private Platform_.IApplication ApplicationConstructorCallback()
        {
            return (Platform_.IApplication) applicationConstructorInfo.Invoke(new object[] { });
        }

        #endregion

        #region SimulationMainLoop

        private void SimulationMainLoop()
        {
#if DEBUG_LogGenerously
            mainlogger.Log(this, "SimulationMainLoop : Start");                
#endif

            while (release.WaitOne() && !exiting)
            {
                running = true;

                try
                {
                    lock (this)
                    {
                        double t1 = physicalClock.Time;

                        bool _advanced;

                        try
                        {
                            _advanced = this.Advance();
                        }
                        catch (Exception exc)
                        {
                            _advanced = false;
                            System.Windows.Forms.MessageBox.Show("Exception throw during the simulation.\n\n" + exc.StackTrace, "Exception",
                                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        }

                        if (_advanced)
                        {
                            double t2 = physicalClock.Time;
                            double dt = t2 - t1;

                            timeused += dt;
                            nprocessed_thissec++;

                            double totaltime = t2 - lastcheck;

                            double sleep0 = (nprocessed_thissec >= stepsasec) ? (1 - totaltime) : 0;
                            double sleep1 = timeused / cpuusage;
                            double sleep2 = (sleep1 > totaltime) ? (sleep1 - totaltime) : 0;
                            double sleep3 = Math.Min(Math.Max(0, Math.Max(sleep0, sleep2)), 1);

                            if (totaltime > 1)
                            {
                                lastcheck = t2;
                                timeused = 0;
                                nprocessed_thissec = 0;
                            }

                            int milliseconds = (int)Math.Round(sleep3 * 1000);
                            if (milliseconds > 0)
                            {
                                Monitor.Exit(this);
                                try
                                {
                                    Thread.Sleep(milliseconds);
                                }
                                finally
                                {
                                    Monitor.Enter(this);
                                }
                            }
                        }
                        else
                        {
                            finished = true;
                            // finishedEvent.Set();
                            release.Reset();
                        }
                    }
                }
                catch (Exception exc)
                {
#if DEBUG_LogGenerously
                    mainlogger.Log(this, "SimulationMainLoop : " + exc.ToString());
#endif
                }

                running = false;
            }

#if DEBUG_LogGenerously
            mainlogger.Log(this, "SimulationMainLoop : Stop");
#endif
        }

        #endregion

        #region Advance

        private bool Advance()
        {
            if (simulatedClock.QueueSize > 0)
            {
                try
                {
                    simulatedClock.advance();
                }
                catch (Exception exc)
                {
                    mainlogger.Log(this, "Cannot advance.\n" + exc.ToString());
                    return false;
                }

                nprocessed++;

//                if (onChange != null)
//                    onChange(this, null);

                return true;
            }
            else
                return false;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            ((ISimulation)this).Shutdown();
        }

        #endregion

        #region ISimulation Members

        void ISimulation.Start()
        {
            release.Set();
        }

        bool ISimulation.Step()
        {
            lock (this)
            {
                return Advance();
            }
        }

        void ISimulation.Stop()
        {
            release.Reset();
        }

        int ISimulation.NumberOfEventsHandled
        {
            get { return nprocessed; }
        }

        int ISimulation.NumberOfEventsPending
        {
            get { return simulatedClock.QueueSize; }
        }

        bool ISimulation.Running
        {
            get { return running; }
        }

        bool ISimulation.Finished
        {
            get { return finished; }
        }

//        event EventHandler ISimulation.OnChange
//        {
//            add { onChange += value; }
//            remove { onChange -= value; }
//        }

        double ISimulation.SimulationTime
        {
            get { return simulatedClock.Time; }
        }

        void ISimulation.Shutdown()
        {
            exiting = true;
            release.Set();

            try
            {
                if (!simulationThread.Join(3000))
                    simulationThread.Abort();
            }
            catch (Exception exc)
            {
                mainlogger.Log(this, "Cannot dispose.\n" + exc.ToString());
            }
        }

        #endregion

/*
        #region IManagedComponent Members

        string QS.TMS.Management.IManagedComponent.Name
        {
            get { return "Simulation"; }
        }

        QS.TMS.Management.IManagedComponent[] QS.TMS.Management.IManagedComponent.Subcomponents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        QS.CMS.Base.IOutputReader QS.TMS.Management.IManagedComponent.Log
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        object QS.TMS.Management.IManagedComponent.Component
        {
            get { return this; }
        }

        #endregion
*/
    }
}
