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
using System.Threading;
using System.Net;

using QS._qss_e_.Parameters_.Specifications;

namespace QS._qss_e_.Experiments_
{
/*
    [TMS.Experiments.DefaultExperiment]
    public class Experiment_266 : Experiment_200
    {
        #region experimentWork

        protected override void experimentWork(QS.CMS.Components.IAttributeSet results)
        {
            logger.Log(this, "Waiting 2s");
            sleeper.sleep(2);

            logger.Log(this, "Changing membership");
            for (int ind = 0; ind < this.NumberOfApplications; ind++)
            {
                this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("Subscribe"), new object[] { });
            }

            logger.Log(this, "Waiting for the system to stabilize");
            sleeper.sleep(Convert.ToDouble((string)arguments["stabilize"]));

            Coordinator.invoke(typeof(Application).GetMethod("Send"), new object[] { });

            logger.Log(this, "Waiting 3s");
            sleeper.sleep(3);

            Coordinator.invoke(typeof(Application).GetMethod("Cleanup"), new object[] { });

            logger.Log(this, "Cooling down");
            sleeper.sleep(Convert.ToDouble((string)arguments["cooldown"]));

            logger.Log(this, "Completed.");
        }

        #endregion

        #region Class Application

        protected new class Application : Experiment_200.Application, QS.TMS.Runtime.IControlledApp
        {
            // private const uint myloid = (uint)ReservedObjectID.User_Min + 10;

            #region Constructor

            public Application(CMS.Platform.IPlatform platform, QS.CMS.Components.AttributeSet args) : base(platform, args)
            {
                framework = new QS.CMS.Framework.FrameworkOnCore2(
                    new QS.CMS.QS._core_c_.Base3.InstanceID(localAddress, incarnation), coordinatorAddress, platform.Logger, platform.EventLogger);

                // .....................................................

                ((QS.TMS.Runtime.IControlledApp)framework).Start();
                logger.Log(this, "Ready");
            }

            #endregion

            private QS.CMS.Framework.FrameworkOnCore2 framework;
            private IFoo replicatedFoo;

            #region Replicated Objects

            private interface IFoo
            {
                void foo(string s);
            }

            private class Foo : IFoo
            {
                public Foo(QS.Fx.Logging.ILogger logger)
                {
                    this.logger = logger;
                }

                private QS.Fx.Logging.ILogger logger;

                #region IFoo Members

                void IFoo.foo(string s)
                {
                    logger.Log(this, "_________Called IFoo.foo(\"" + s + "\")");
                }

                #endregion
            }

            #endregion

            #region Subscribe

            public void Subscribe()
            {
                framework.Core.ScheduleCall(new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        logger.Log(this, "__Subscribe");
                        framework.GroupServices.BeginOpen<IFoo>("GlobalFoo", new Foo(logger), 
                            new AsyncCallback(this.SubscribeCallback), null);
                    }), null);
            }

            private void SubscribeCallback(IAsyncResult asyncResult)
            {
                logger.Log(this, "__SubscribeCallback_Begin");
                replicatedFoo = framework.GroupServices.EndOpen<IFoo>(asyncResult);
                logger.Log(this, "__SubscribeCallback_End");
            }

            #endregion

            #region Send

            public void Send()
            {
                framework.Core.ScheduleCall(new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        if (replicatedFoo != null)
                            replicatedFoo.foo("This is a very nice message from (" + localAddress.ToString() + ").");
                        else
                            logger.Log(this, "Cannot call replicatedFoo, the object is null.");
                    }), null);
            }

            #endregion

            #region Cleanup

            public void Cleanup()
            {
                framework.Core.ScheduleCall(new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        replicatedFoo = null;
                        GC.Collect();
                    }), null);
            }

            #endregion

            #region Terminating and Disposing

            public override void TerminateApplication(bool smoothly)
            {
                platform.ReleaseResources();
            }

            public override void Dispose()
            {
            }

            #endregion

            #region IControlledApp Members

            bool QS.TMS.Runtime.IControlledApp.Running
            {
                get { return ((QS.TMS.Runtime.IControlledApp) framework).Running; }
            }

            void QS.TMS.Runtime.IControlledApp.Start()
            {
                ((QS.TMS.Runtime.IControlledApp)framework).Start();
            }

            void QS.TMS.Runtime.IControlledApp.Stop()
            {
                ((QS.TMS.Runtime.IControlledApp)framework).Stop();
            }

            #endregion
        }

        #endregion

        #region Other Garbage

        public Experiment_266()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
*/ 
}
