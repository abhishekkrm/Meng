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
using System.Threading;

namespace QS._qss_e_.Components_
{
    public class ManualController : QS.Fx.Inspection.Inspectable, IExperimentController
    {
        public ManualController(QS._qss_c_.Base1_.Subnet localSubnet)
            : this(QS._qss_c_.Devices_2_.Network.AnyAddressOn(localSubnet))
        {
        }

        public ManualController(IPAddress localAddress) : this(localAddress, localAddress)
        {
        }

        public ManualController(IPAddress localAddress, IPAddress anotherAddress)
        {
            this.localAddress = localAddress;
            administrativeLogger = new QS._core_c_.Base.Logger(QS._core_c_.Base2.PreciseClock.Clock, true, null, false, "");
            node1 = new QS._qss_e_.Runtime_.ManuallyAttachedNode(
                new QS._core_c_.Base.Logger(QS._core_c_.Base2.PreciseClock.Clock, true), administrativeLogger, localAddress, localAddress,
                TimeSpan.FromSeconds(10), 60001, 60002);
            arguments1 = administrativeLogger.CurrentContents;
            administrativeLogger.Clear();
            node2 = new QS._qss_e_.Runtime_.ManuallyAttachedNode(
                new QS._core_c_.Base.Logger(QS._core_c_.Base2.PreciseClock.Clock, true), administrativeLogger, localAddress, anotherAddress,
                TimeSpan.FromSeconds(10), 60003, 60004);
            arguments2 = administrativeLogger.CurrentContents;
            administrativeLogger.Clear();
            environment = new QS._qss_e_.Environments_.CombinedEnvironment(
                new QS._qss_e_.Runtime_.IEnvironment[] { node1, node2 }, administrativeLogger);
        }

        [QS.Fx.Base.Inspectable("Environment", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_e_.Environments_.CombinedEnvironment environment;
        private IPAddress localAddress;
        [QS.Fx.Base.Inspectable("Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._core_c_.Base.Logger administrativeLogger;
        [QS.Fx.Base.Inspectable("Arguments1", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private string arguments1;
        [QS.Fx.Base.Inspectable("Arguments2", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private string arguments2;
        [QS.Fx.Base.Inspectable("Node1", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_e_.Runtime_.ManuallyAttachedNode node1;
        [QS.Fx.Base.Inspectable("Node2", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_e_.Runtime_.ManuallyAttachedNode node2;
        private Type experimentClass;
        private QS._core_c_.Components.AttributeSet experimentArgs;
        [QS.Fx.Base.Inspectable("Results", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._core_c_.Components.AttributeSet experimentResults;
        [QS.Fx.Base.Inspectable("Experiment", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_e_.Experiments_.IExperiment experiment;

        #region RunExperiment

        private void RunExperiment(object o)
        {
            administrativeLogger.Log(this, "Initializing experiment.");

            experiment = (QS._qss_e_.Experiments_.IExperiment) 
                experimentClass.GetConstructor(System.Type.EmptyTypes).Invoke(new object[] { });

            administrativeLogger.Log(this, "Running experiment.");

            experiment.run(environment, administrativeLogger, experimentArgs, experimentResults);

            administrativeLogger.Log(this, "Experiment complete.");
        }

        #endregion

        #region IExperimentController Members

        void IExperimentController.Run(Type experimentClass, QS._core_c_.Components.AttributeSet experimentArgs)
        {
            this.experimentArgs = experimentArgs;
            this.experimentClass = experimentClass;
            experimentResults = new QS._core_c_.Components.AttributeSet();

            ThreadPool.QueueUserWorkItem(new WaitCallback(this.RunExperiment), null);
        }

        void IExperimentController.Shutdown()
        {
        }

        Type IExperimentController.Class
        {
            get { return experimentClass; }
        }

        QS._core_c_.Components.AttributeSet IExperimentController.Arguments
        {
            get { return experimentArgs; }
        }

        QS._core_c_.Components.AttributeSet IExperimentController.Results
        {
            get { return experimentResults; }
        }

        #endregion
    }
}
