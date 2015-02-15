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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Diagnostics;

namespace QS._qss_e_.Experiments_
{
    [QS._qss_e_.Base_1_.Arguments("-nnodes:20")]
    public abstract class Experiment_100 : IExperiment, QS.Fx.Inspection.IInspectable
    {
        public Experiment_100()
        {
        }

        protected Components_.Sleeper Sleeper;

        public void run(Runtime_.IEnvironment environment, QS.Fx.Logging.ILogger logger,
            QS._core_c_.Components.IAttributeSet args, QS._core_c_.Components.IAttributeSet result_attributes)
        {
            try
            {
                this.logger = logger;
                this.environment = environment;
                this.experimentArgs = args;

                Sleeper = new QS._qss_e_.Components_.Sleeper(environment.AlarmClock);

                System.Type testClass;
                QS._core_c_.Components.AttributeSet attributes = this.initializeWith(out testClass);            
                this.launchApplications(testClass,
                    (args.contains("nnodes") ? Convert.ToUInt32(args["nnodes"]) : uint.MaxValue), attributes);

                Coordinator.Controller = new Base_1_.ObjectController(this, logger);

                logger.Log(null, "initializing experiment");

                this.experimentWork();

                logger.Log(null, "experiment initialized");

                // QS.CMS.Components.AttributeSet resultAttributes = (QS.CMS.Components.AttributeSet)
                //    apps[0].invoke(testClass.GetMethod("collect_results"), new object[] { });
                resultsArrived.WaitOne();

				Debug.Assert(result_attributes != null);
				foreach (QS._core_c_.Collections.IDictionaryEntry dic_en in resultAttributes.Attributes)
				{
					Debug.Assert(dic_en.Key != null);
					result_attributes[(string)dic_en.Key] = dic_en.Value;
				}

				logger.Log(this, "\nExperiment Completed.\n");
            }
            catch (Exception exc)
            {
				logger.Log(this, "\nExperiment Failed:\n\n" + exc.ToString() + "\n" + exc.StackTrace + "\n");
				// throw new Exception("Could not run experiment.", exc);
            }
        }

        private System.Threading.ManualResetEvent resultsArrived = new System.Threading.ManualResetEvent(false);
        private QS._core_c_.Components.AttributeSet resultAttributes;
        public void upload_results(QS._core_c_.Components.AttributeSet arguments)
        {
            resultAttributes = arguments;
            resultsArrived.Set();
        }

        protected abstract QS._core_c_.Components.AttributeSet initializeWith(out System.Type testClass);
//      {
//          testClass = typeof(TestApp);
//          return QS.CMS.Components.AttributeSet.None;
//      }

        protected abstract void experimentWork();

		protected void completed()
        {
			// Coordinator.invoke(typeof(TestApp).GetMethod("completed"), new object[] {});

            upload_results(QS._core_c_.Components.AttributeSet.None);
        }

        private void launchApplications(System.Type testClass, uint maxnodes, QS._core_c_.Components.AttributeSet appargs)
        {
            numberOfNodes = (uint) environment.Nodes.Length;
            if (numberOfNodes > maxnodes && maxnodes > 0)
                numberOfNodes = maxnodes;

            logger.Log("Starting experiment on " + numberOfNodes.ToString() + " nodes.\nArguments:\n" + appargs.ToString());

            addresses = new System.Net.IPAddress[numberOfNodes];
            apps = new Runtime_.IApplicationRef[numberOfNodes];
            int appno = 0;
            System.Net.IPAddress lastAddress = System.Net.IPAddress.Any;
            for (uint ind = 0; ind < numberOfNodes; ind++)
            {
                QS._core_c_.Components.AttributeSet args = new QS._core_c_.Components.AttributeSet(appargs);

                // logger.Log(this, "after copying " + appargs.ToString() + " became " + args.ToString());

                System.Net.IPAddress thisAddress = environment.Nodes[ind].NICs[0];
                addresses[ind] = thisAddress;

                if (thisAddress == lastAddress)
                    appno++;
                else
                    appno = 0;
                lastAddress = thisAddress;

                args["_base"] = thisAddress.ToString();
                args["_appno"] = appno.ToString();
                if (ind != 0)
                    args["_coordinator"] = addresses[0].ToString();
                apps[ind] = environment.Nodes[ind].launch(testClass.FullName, args);

				QS.Fx.Inspection.AttributeCollection attributeCollection = 
                    new QS.Fx.Inspection.AttributeCollection(ind.ToString("0000") + ":" + addresses[ind].ToString());
				attributeCollection.Add(apps[ind].Attributes);
				applicationsAttributeCollection.Add(attributeCollection);
			}

            logger.Log(null, "Applications launched.");
		}

		private QS.Fx.Inspection.AttributeCollection applicationsAttributeCollection = new QS.Fx.Inspection.AttributeCollection("Controlled Applications");

		protected Runtime_.IEnvironment environment;
        protected QS.Fx.Logging.ILogger logger;
        protected uint numberOfNodes;
        protected System.Net.IPAddress[] addresses;
        protected QS._core_c_.Components.IAttributeSet experimentArgs;
        protected Runtime_.IApplicationRef[] apps = null;

        protected Runtime_.IApplicationRef Coordinator
        {
            get
            {
                return apps[0];
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (apps != null)
            {
                for (uint ind = 0; ind < apps.Length; ind++)
                {
                    try
                    {
                        if (apps[ind] != null)
                            apps[ind].Dispose();
                    }
                    catch (Exception exc)
                    {
                        logger.Log(this, exc.ToString());
                    }
                }
            }
        }

        #endregion

        public abstract class TestApp : Base_1_.ControlledApplication // System.IDisposable
        {
            public const uint port_number = 12022;

            public TestApp(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
            {
                try
                {
                    this.platform = platform;
                    QS._core_c_.Base2.Serializer.CommonSerializer.registerClasses(platform.Logger);

                    System.Net.IPAddress localIPAddress;
                    if (args.contains("_base"))
                        localIPAddress = System.Net.IPAddress.Parse((string)args["_base"]);
                    else if (args.contains("_subnet"))
                        localIPAddress = QS._qss_c_.Devices_2_.Network.AnyAddressOn(
                            new QS._qss_c_.Base1_.Subnet((string)args["_subnet"]), platform);
                    else
                        localIPAddress = platform.NICs[0];
                    localAddress = new QS.Fx.Network.NetworkAddress(localIPAddress, ((int) port_number) + Convert.ToInt32((string) args["_appno"]));

                    platform.Logger.Log(null, "Base Address Chosen : " + localAddress.ToString());

                    if (!(isCoordinator = !args.contains("_coordinator")))
                    {
                        coordinatorAddress = new QS.Fx.Network.NetworkAddress(
                            System.Net.IPAddress.Parse((string)args["_coordinator"]), (int)TestApp.port_number);
                        platform.Logger.Log(null, "Coordinator Address : " + coordinatorAddress.ToString());
                    }
                    else
                    {
                        coordinatorAddress = localAddress;

                        platform.Logger.Log(null, "Acting as Coordinator.");
                        // experimentComplete = new System.Threading.ManualResetEvent(false);
                    }
                }
                catch (Exception exc)
                {
                    platform.Logger.Log(this, "TestApp_Constructor : " + exc.ToString());
                }
            }

            protected QS._qss_c_.Platform_.IPlatform platform;
            protected QS.Fx.Network.NetworkAddress localAddress, coordinatorAddress;
            protected bool isCoordinator;

			[QS.Fx.Base.Inspectable]
            public QS._qss_c_.Platform_.IPlatform Platform
			{
				get { return platform; }
			}

//			[Inspection.Inspectable]
//			public bool IsCoordinator
//			{
//				get { return isCoordinator; }
//			}

			// protected bool experimentComplete = false;
            // protected System.Threading.ManualResetEvent experimentComplete = null;

            protected abstract void generate_results(QS._core_c_.Components.AttributeSet resultAttributes);

			protected bool isCompleted = false;
			public void completed()
			{
                // experimentComplete.Set();
				isCompleted = true;

				platform.Logger.Log("GenerateResults_Enter");                

                QS._core_c_.Components.AttributeSet resultAttributes = new QS._core_c_.Components.AttributeSet(20);
                generate_results(resultAttributes);

                platform.Logger.Log("GenerateResults_Leave");

                try
                {
                    applicationController.upcall("upload_results", resultAttributes);
                }
                catch (Exception exc)
                {
                    platform.Logger.Log(this, "__completed() : Cannot upload results.\n" + exc.ToString());
                }
            }

            // public QS.CMS.Components.AttributeSet collect_results()
            // {
            //    if (!isCoordinator)
            //        throw new Exception("Not a coordinator!");
            //
            //
            //    platform.Logger.writeLine("CollectResults_Enter");
            //
            //
            //     experimentComplete.WaitOne();
            //
            //    platform.Logger.writeLine("CollectResults_Leave");
            //
            //    QS.CMS.Components.AttributeSet resultAttributes = new QS.CMS.Components.AttributeSet(20);
            //    generate_results(resultAttributes);
            //
            //    return resultAttributes;
            // }

            #region IDisposable Members

            public override void Dispose()
            {
            }

            #endregion
        }

		#region IInspectable Members

		QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
		{
			get { return applicationsAttributeCollection; }
		}

		#endregion
	}
}
