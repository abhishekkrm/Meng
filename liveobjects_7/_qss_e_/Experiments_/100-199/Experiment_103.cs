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

﻿// #define DEBUG_Experiment103

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Threading;

namespace QS._qss_e_.Experiments_
{
    [QS._qss_e_.Base_1_.Arguments(
		"-nnodes:0 -ngroups:1 -nmessages:1000 -windowsize:1 -timeout:0.05 -routing:log -ack_channel:root -flushing_interval:0.005 -senderclass:100 -buffersize:10000 -fc:window -fwd:off")]
		// "-nnodes:20 -ngroups:1 -nmessages:10000 -windowsize:1 -timeout:0.05 -routing:btree,2 -ack_channel:root -senderclass:202 -buffersize:5000 -fc:window -fwd:on")]
		// "-nnodes:20 -ngroups:1 -nmessages:25000 -windowsize:1 -timeout:0.05 -routing:ring -ack_channel:lazy -flushing_interval:0.01 -senderclass:100 -buffersize:5000 -fc:window -fwd:off")]
		// "-nnodes:99 -ngroups:20 -nmessages:1000 -windowsize:20 -timeout:0.05 -routing:prefix,5 -ack_channel:lazy -flushing_interval:0.005 -senderclass:202 -buffersize:5000")]
		// "-nnodes:99 -ngroups:20 -nmessages:25000 -windowsize:20 -timeout:0.05 -routing:prefix,5 -ack_channel:lazy -flushing_interval:0.005 -senderclass:200 -buffersize:25000")]
		// "-nnodes:99 -ngroups:1 -nmessages:10000 -windowsize:10 -timeout:10 -routing:prefix,2 -ack_channel:root -senderclass:202 -buffersize:25000")]
		// "-nnodes:99 -ngroups:1 -nmessages:50000 -windowsize:20 -timeout:0.05 -routing:prefix,5 -ack_channel:lazy -flushing_interval:0.005 -senderclass:200 -buffersize:25000")]
		// "-nnodes:99 -ngroups:1 -nmessages:25000 -windowsize:20 -timeout:0.05 -routing:prefix,5 -ack_channel:lazy -flushing_interval:0.005 -senderclass:3")]
		// "-nnodes:99 -ngroups:1 -nmessages:1000 -windowsize:20 -timeout:0.05 -routing:prefix,5 -ack_channel:lazy -flushing_interval:0.005 -senderclass:3")]
		// "-nnodes:99 -ngroups:1 -nmessages:5000 -windowsize:1 -timeout:0.05 -routing:prefix,5 -ack_channel:lazy -flushing_interval:0.005")]
		// "-nnodes:99 -ngroups:1 -nmessages:1000 -windowsize:1 -timeout:0.05 -routing:prefix,5 -ack_channel:threshold -buffering_threshold:10/0.1")]
	public class Experiment_103 : Experiment_100
    {
		private const int defaultOutgoingBufferSize = 20000;

		public Experiment_103()
        {
        }

        private static System.Type testClass = typeof(QS._qss_e_.Experiments_.Experiment_103.TestApp);
        protected override QS._core_c_.Components.AttributeSet initializeWith(out System.Type testClass)
        {
            testClass = Experiment_103.testClass;
            QS._core_c_.Components.AttributeSet attributes = new QS._core_c_.Components.AttributeSet(2);
			attributes["senderclass"] = (string) experimentArgs["senderclass"];
			attributes["windowsize"] = (string) experimentArgs["windowsize"];
            attributes["timeout"] = (string)experimentArgs["timeout"];
            attributes["routing"] = (string)experimentArgs["routing"];
            attributes["ack_channel"] = (string)experimentArgs["ack_channel"];
            if (experimentArgs.contains("flushing_interval"))
                attributes["flushing_interval"] = (string)experimentArgs["flushing_interval"];
			if (experimentArgs.contains("buffering_threshold"))
				attributes["buffering_threshold"] = (string)experimentArgs["buffering_threshold"];
			attributes["buffersize"] = (string)experimentArgs["buffersize"];
			attributes["fc"] = (string)experimentArgs["fc"];
			attributes["fwd"] = (string)experimentArgs["fwd"];
			return attributes;
		}

        protected override void experimentWork()
        {
			int delay_multiplier = (this.environment is QS._qss_e_.Environments_.SimulatedEnvironment) ? 10 : 1;

			logger.Log(this, "Waiting until system stabilizes before requesting a membership change.");

			Sleeper.sleep(10 * delay_multiplier);

			logger.Log(this, "Requesting a membership change now...");

            for (int ind = 0; ind < apps.Length; ind++)
                apps[ind].invoke(testClass.GetMethod("change_membership"), new object[] { Convert.ToInt32((string) experimentArgs["ngroups"]) });

            logger.Log(this, "Waiting until membership all change notifications have been propagated");

			Sleeper.sleep(80 * delay_multiplier);

			int nmessages = Convert.ToInt32((string)experimentArgs["nmessages"]);
            if (nmessages > 0)
            {
                logger.Log(this, "Starting to multicast messages...");

                Coordinator.invoke(testClass.GetMethod("start_multicasting"), new object[] { nmessages });
            }
            else
            {
                this.completed();
            }
        }

		public void percentageCompleted(QS._core_c_.Components.AttributeSet args)
		{
			logger.Log(null, "Completed: " + ((double) (args["percentage"])).ToString("00.00") + "%");
		}

		public new class TestApp : Experiment_100.TestApp
        {
			// to make it easier to look it up
			[QS.Fx.Base.Inspectable]
			public QS.Fx.Inspection.IAttribute Aggregation4Agent_Attributes
			{
				get { return ((QS.Fx.Inspection.IInspectable)framework.Aggregation4Agent).Attributes; }
			}

			public void change_membership(int ngroups)
            {
                platform.Logger.Log(this, "Changing membership...");

                groupIDs = new QS._qss_c_.Base3_.GroupID[ngroups];
                for (int ind = 0; ind < ngroups; ind++)
                    groupIDs[ind] = new QS._qss_c_.Base3_.GroupID((uint) (1000 + ind));

                framework.MembershipAgent.ChangeMembership(
                    new List<QS._qss_c_.Base3_.GroupID>(groupIDs), new List<QS._qss_c_.Base3_.GroupID>());
            }

            QS._qss_c_.Base3_.AsynchronousOperationCallback asynchronousCallback;
            QS.Fx.Serialization.ISerializable messageToSend;

            public void start_multicasting(int nmessages)
            {
                for (int ind = 0; ind < groupIDs.Length; ind++)
                {
                    platform.Logger.Log("Group " + groupIDs[ind].ToString() + " has " +  
                        framework.MembershipController[groupIDs[ind]].CurrentView.Members.Count.ToString() + " members.");
                }

                this.nmessages = nmessages;

                asynchronousCallback = new QS._qss_c_.Base3_.AsynchronousOperationCallback(this.completionCallback);
                messageToSend = new QS._core_c_.Base2.StringWrapper("A nice little message.");

                startingTimes = new double[nmessages];
                completionTimes = new double[nmessages];

                nmessages_sent = nmessages_received = 0;

				int tosend_now = nmessages < 5000 ? nmessages : 5000;
				while (send_message() < tosend_now)
					;

				platform.AlarmClock.Schedule(5, new QS.Fx.Clock.AlarmCallback(multicastRateCallback), null);
			}

			private int nmessages_received_snapshot = int.MinValue;
			private void multicastRateCallback(QS.Fx.Clock.IAlarm alarmRef)
			{
				if (!isCompleted)
				{
					int nmessages_received_now = nmessages_received;
					bool trouble = nmessages_received_now == nmessages_received_snapshot;
					nmessages_received_snapshot = nmessages_received_now;
					alarmRef.Reschedule();

					if (trouble)
						platform.Logger.Log(this, "Warning: Not multicasting!");
					else
					{
						applicationController.upcall(
							"percentageCompleted", new QS._core_c_.Components.AttributeSet(
								"percentage", 100 * ((double) nmessages_received_now) / ((double) nmessages)));
					}
				}
			}

			private int send_message()
            {
                int message_ind = Interlocked.Increment(ref nmessages_sent) - 1;

				if (message_ind < nmessages)
                {
                    int group_ind = message_ind % groupIDs.Length;
                    startingTimes[message_ind] = platform.Clock.Time;

					QS._qss_c_.Multicasting3.IGroupSender mysender = simpleSender[groupIDs[group_ind]];

					// int current_windowSize = (int)(Math.Ceiling((message_ind * windowSize) / ((double) nmessages)));
					// mysender.WindowSize = current_windowSize;

					mysender.BeginSend(999, messageToSend, asynchronousCallback, message_ind);
                }

                return message_ind;
            }

            private int nmessages;
			[QS.Fx.Base.Inspectable]
			private int nmessages_sent;
			[QS.Fx.Base.Inspectable]
			private int nmessages_received;
            private double[] startingTimes, completionTimes;

			[QS.Fx.Base.Inspectable]
			public int PendingCompletion
			{
				get { return nmessages_sent - nmessages_received; }
			}

			private void completionCallback(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {
#if DEBUG_Experiment103
                platform.Logger.Log(this, "__CompletionCallback_Enter : " + asynchronousOperation.AsyncState.ToString());
#endif
				try
				{
					int message_ind = (int)asynchronousOperation.AsyncState;
					completionTimes[message_ind] = platform.Clock.Time;

					if (Interlocked.Increment(ref nmessages_received) < nmessages)
					{
						if (nmessages_sent < nmessages)
							send_message();
					}
					else
					{
						this.completed();
					}
				}
#if DEBUG_Experiment103
				catch (Exception exc)
				{
					platform.Logger.Log(this, "__CompletionCallback_Failed : " + asynchronousOperation.AsyncState.ToString() + "\n" +
						exc.ToString() + "\n" + exc.StackTrace);
				}
#endif
				finally
				{
#if DEBUG_Experiment103
					platform.Logger.Log(this, "__CompletionCallback_Leave : " + asynchronousOperation.AsyncState.ToString());
#endif
				}
			}

			[QS.Fx.Base.Inspectable]
			public QS._core_c_.Components.AttributeSet ResultAttributes
			{
				get
				{
					QS._core_c_.Components.AttributeSet result = new QS._core_c_.Components.AttributeSet();
					generate_results(result);
					return result;
				}
			}

			protected override void generate_results(QS._core_c_.Components.AttributeSet resultAttributes)
            {
                if (completionTimes != null && startingTimes != null)
                {
                    resultAttributes["app_completion_times"] = new QS._core_e_.Data.DataSeries(completionTimes);
					double[] roundtrip_times = new double[nmessages];
					for (int ind = 0; ind < nmessages; ind++)
						roundtrip_times[ind] = completionTimes[ind] - startingTimes[ind];
					resultAttributes["roundtrip_times"] = new QS._core_e_.Data.DataSeries(roundtrip_times);

					// resultAttributes["received_packetSizes"] = framework.RootSender.ReceivedPacketSizes;
					// resultAttributes["unwrapper_msgcounts"] = framework.Unwrapper.MessageCountStatistics;

					resultAttributes.Add(Base_1_.StatisticsCollector.AsAttribute("_SimpleSender", simpleSender));

					// resultAttributes.AddRange(((TMS.Base.IStatisticsCollector) framework.RegionSender).Statistics);

					resultAttributes.Add(Base_1_.StatisticsCollector.AsAttribute("_Framework", framework));
				}
            }

			[QS.Fx.Base.Inspectable("Framework", QS.Fx.Base.AttributeAccess.ReadOnly)]
			QS._qss_c_.Framework_1_.SimpleFramework framework;

			[QS.Fx.Base.Inspectable("SimpleSender", QS.Fx.Base.AttributeAccess.ReadOnly)]
			QS._qss_c_.Multicasting3.ISimpleSender simpleSender;

			QS._qss_c_.Aggregation1_.IAggregationClass aggregationClass;

            QS._qss_c_.Base3_.GroupID[] groupIDs;
            int windowSize;

            public TestApp(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args) : base(platform, args)
            {
                platform.Logger.Log(this, "Arguments: " + args.ToString());

                windowSize = Convert.ToInt32((string)args["windowsize"]);
                double retransmissionTimeout = Convert.ToDouble((string)args["timeout"]);

                framework = new QS._qss_c_.Framework_1_.SimpleFramework(platform, this.localAddress, this.coordinatorAddress);

                framework.Demultiplexer.register(999, new QS._qss_c_.Base3_.ReceiveCallback(this.receiveCallback));

                framework.LazySender.FlushingInterval = TimeSpan.FromSeconds(
                    args.contains("flushing_interval") ? Convert.ToDouble((string)args["flushing_interval"]) : 0.05);

				if (args.contains("buffering_threshold"))
				{
					string threshold_string = (string) args["buffering_threshold"];
					int slash_position = threshold_string.IndexOf('/');
					double threshold_value = Convert.ToDouble(threshold_string.Substring(0, slash_position));
					double reference_interval = Convert.ToDouble(threshold_string.Substring(slash_position + 1));
					platform.Logger.Log(this, "Setting threshold to " + threshold_value.ToString() + " / " + reference_interval.ToString());
					framework.ThresholdSender.SetThreshold(reference_interval, threshold_value);
				}

				QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender> acknowledgementSenderClass;
                string ackchannel_name = (string) args["ack_channel"];
                if (ackchannel_name.Equals("root"))
                    acknowledgementSenderClass = (QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>)framework.RootSender;
                else if (ackchannel_name.Equals("lazy"))
                {
                    acknowledgementSenderClass = (QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>) framework.LazySender;
                }
				else if (ackchannel_name.Equals("threshold"))
				{
					acknowledgementSenderClass = (QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>)framework.ThresholdSender;
				}
				else
					throw new ArgumentException("Acknowledgement channel type unknown.");

                QS._qss_c_.Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> ackISC =
                    new QS._qss_c_.Senders6.InstanceSender(
                        platform.Logger, framework.FailureDetector, acknowledgementSenderClass.SenderCollection, null);

                framework.AggregationAgent.UnderlyingSenderCollection = ackISC; //  cknowledgementSenderClass.SenderCollection;

				((QS._qss_c_.Senders3.ReliableSender1) framework.ReliableSender2).UnderlyingSenderCollection = 
					acknowledgementSenderClass.SenderCollection;

				QS._qss_c_.Routing_1_.IRoutingAlgorithm routingAlgorithm;
                string algorithm_name = (string)args["routing"];
                if (algorithm_name.Equals("ring"))
                    routingAlgorithm = QS._qss_c_.Routing_1_.RingRouting.Algorithm;
                else if (algorithm_name.Equals("no"))
                    routingAlgorithm = QS._qss_c_.Routing_1_.NoRouting.Algorithm;
                else if (algorithm_name.StartsWith("prefix"))
                {
                    uint routing_base = Convert.ToUInt32(algorithm_name.Substring(algorithm_name.IndexOf(",") + 1));
                    routingAlgorithm = QS._qss_c_.Routing_1_.PrefixRouting.Algorithm(routing_base);
                }
                else if (algorithm_name.StartsWith("log"))
                {
                    routingAlgorithm = new QS._qss_c_.Routing_1_.LogRouting(); // no base needs to be specified
                }
				else if (algorithm_name.StartsWith("btree"))
				{
					uint routing_base = Convert.ToUInt32(algorithm_name.Substring(algorithm_name.IndexOf(",") + 1));
					routingAlgorithm = QS._qss_c_.Routing_1_.BalancedTreeRouting.Algorithm(routing_base);
				}
				else
					throw new ArgumentException("Unknown routing algorithm");

                aggregationClass = new QS._qss_c_.Multicasting3.AggregationClass(platform.Logger, framework.MembershipController, routingAlgorithm);  
                framework.AggregationAgent.registerClass(aggregationClass);

				framework.AggregationRouter.RoutingAlgorithm = routingAlgorithm;
				((QS._qss_c_.Aggregation4_.IAgent) framework.Aggregation4Agent).RoutingAlgorithm = routingAlgorithm;

				int sender_type = Convert.ToInt32(args["senderclass"] as string);
				switch (sender_type)
				{
					case 100:
					{
						simpleSender = new QS._qss_c_.Multicasting3.SimpleSender1(framework.Platform.Logger, framework.MembershipController,
							framework.Demultiplexer, framework.RootSender.SenderCollection, framework.AggregationAgent, platform.Clock, platform.AlarmClock,
							retransmissionTimeout, defaultOutgoingBufferSize, windowSize);
					}
					break;

					case 200:
					case 201:
					case 202:
					{
						int buffersize = 5000;
						if (args.contains("buffersize"))
							buffersize = Convert.ToInt32(args["buffersize"]);

						QS._qss_c_.Aggregation1_.IAggregationAgent aggregationAgent = null;
						switch (sender_type)
						{
							case 200:
								aggregationAgent = framework.AggregationAgent;
								break;

							case 201:
								aggregationAgent = framework.Aggregation3Agent;
								break;

							case 202:
								aggregationAgent = framework.Aggregation4Agent;
								break;
						}

						QS._qss_c_.Multicasting3.SimpleSender2.RegionController.FlowControlScheme fcscheme;
						if (args.contains("fc"))
						{
							if (args["fc"].Equals("window"))
								fcscheme = QS._qss_c_.Multicasting3.SimpleSender2.RegionController.FlowControlScheme.WINDOW;
							else if (args["fc"].Equals("rate"))
								fcscheme = QS._qss_c_.Multicasting3.SimpleSender2.RegionController.FlowControlScheme.RATE;
							else
								throw new ArgumentException();
						}
						else
							fcscheme = QS._qss_c_.Multicasting3.SimpleSender2.RegionController.FlowControlScheme.WINDOW;

						simpleSender = new QS._qss_c_.Multicasting3.SimpleSender2(framework.Platform.Logger,
							framework.MembershipController, aggregationAgent, framework.Platform.Clock, 
							framework.Platform.AlarmClock, framework.Demultiplexer, framework.RootSender.SenderCollection,
                            framework.UnreliableInstanceSenderCollection, retransmissionTimeout, buffersize, buffersize, windowSize, fcscheme);
					}
					break;

					case 300:
					{
						framework.RegionSender.InitialWindowSize = windowSize;
						simpleSender = new QS._qss_c_.Multicasting3.SimpleSender3(
							framework.Platform.Logger, framework.MembershipController, framework.RegionSender, 
							framework.AggregationAgent, // framework.Aggregation3Agent, 
							framework.Platform.Clock, framework.Platform.AlarmClock, framework.Demultiplexer);
					}
					break;

					default:
						throw new Exception("Sender class not specified.");
				}

//				controllerCollectionFactory = new CMS.Components.TimedCollection<CMS.Aggregation3.Controller>.Factory(
//					platform.AlarmClock, TimeSpan.FromSeconds(retransmissionTimeout / 2));
//				framework.Aggregation3Agent.ControllerCollectionConstructor = controllerCollectionFactory.Constructor;

				framework.Aggregation4Controller1.ForwardingAllowed = args["fwd"].Equals("on");
			}

			// private CMS.Components.TimedCollection<CMS.Aggregation3.Controller>.Factory controllerCollectionFactory;

			private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
            {
#if DEBUG_Experiment103
                framework.Platform.logger.Log(this, "ReceiveCallback : " + QS.CMS.Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif

                return null;
            }

            public override void Dispose()
            {
                framework.Dispose();
            }
       }
	}
}
