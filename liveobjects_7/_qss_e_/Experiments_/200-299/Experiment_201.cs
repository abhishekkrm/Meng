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

namespace QS._qss_e_.Experiments_
{
	[QS._qss_e_.Base_1_.Arguments("-nnodes:2 -time:300 -mttf:30000 -downtime:5")]
	public class Experiment_201 : Experiment_200
	{
		public Experiment_201()
		{
		}

		protected override Type ApplicationClass
		{
			get { return typeof(Application); }
		}

        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
		{
			sleeper.sleep(100);
			foreach (Runtime_.IApplicationRef application in Applications)
				application.invoke(typeof(Application).GetMethod("Send"), new object[] { });
			sleeper.sleep(100);
		}

		protected new class Application : Experiment_200.Application
		{
			private const int myloid = 1000;

			public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args) : base(platform, args)
			{
				framework = new QS._qss_c_.Framework_1_.SimpleFramework(incarnation, platform, localAddress, coordinatorAddress);
				framework.Demultiplexer.register(myloid, new QS._qss_c_.Base3_.ReceiveCallback(ReceiveCallback));
			}

			private QS._qss_c_.Framework_1_.SimpleFramework framework;

			#region Class MyMessage

			private class MyMessage : QS._qss_c_.Base3_.AsynchronousRequest1<QS._core_c_.Base3.Message>
			{
				public MyMessage(QS.Fx.Logging.IConsole console, string s) 
					: base(new QS._core_c_.Base3.Message(myloid, new QS._core_c_.Base2.StringWrapper(s)))
				{
					this.s = s;
					this.console = console;
				}

				private string s;
				private QS.Fx.Logging.IConsole console;

				public override void Completed()
				{
					console.Log("Completed: \"" + s + "\".");					
				}

				public override void Failed(Exception exception)
				{
					console.Log("Failed: \"" + s + "\"." + exception.ToString());
				}
			}

			#endregion

			public void Send()
			{
				try
				{
					QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> senderCollection = 
						new QS._qss_c_.Senders5.Sender.Collection(
						new QS._qss_c_.Senders5.SendingSink.Collection(framework.RootSender.SenderCollection,
						typeof(QS._qss_c_.FlowControl4.NoController)));

/*
					QS.CMS.Senders5.RequestQueue messageQueue = new QS.CMS.Senders5.RequestQueue();
					messageQueue.Enqueue(new MyMessage(logger, "Kazik-1"));
					messageQueue.Enqueue(new MyMessage(logger, "Kazik-2"));
					messageQueue.Enqueue(new MyMessage(logger, "Kazik-3"));

					QS.CMS.Senders5.ISimpleSink sendingSink = new QS.CMS.Senders5.SendingSink(
						framework.RootSender.SenderCollection[coordinatorAddress], new QS.CMS.FlowControl4.NoController());

					QS.CMS.Senders5.BufferingSink bufferingSink =
						new QS.CMS.Senders5.BufferingSink(sendingSink, QS.CMS.Buffering3.AccumulatingController.ControllerClass);
					sendingSink.Source = bufferingSink;

					((QS.CMS.Senders5.ISimpleSink)bufferingSink).Source = messageQueue;
					((QS.CMS.Senders5.ISimpleSink) bufferingSink).Signal();
*/

					senderCollection[coordinatorAddress].send(myloid, new QS._core_c_.Base2.StringWrapper("Misiu-1"));
					senderCollection[coordinatorAddress].send(myloid, new QS._core_c_.Base2.StringWrapper("Misiu-2"));
				}
				catch (Exception exc)
				{
					logger.Log(this, "__Send: " + exc.ToString());
				}
			}

			private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sourceIID,
				QS.Fx.Serialization.ISerializable receivedObject)
			{
				logger.Log(this, QS._core_c_.Helpers.ToString.ReceivedObject(sourceIID, receivedObject));
				return null;
			}
		}
	}
}
