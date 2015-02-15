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
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace QS._qss_d_.Service_1_
{
	[Serializable]
	[XmlType("ServiceConfiguration")]
	public class Configuration : System.Runtime.Serialization.ISerializable
	{
		public Configuration()
		{
			initialization();
		}

		private void initialization()
		{
			applications = new QS._core_c_.Collections.Hashtable(10);				
		}

		[NonSerialized]
		private QS._core_c_.Collections.Hashtable applications;

		public Application lookupApp(string name)
		{
			return (Application) applications[name];
		}

		public void createApp(Application application)
		{
			lock (this)
			{
				QS._core_c_.Collections.IDictionaryEntry dic_en = 
					applications.lookupOrCreate(application.identifyingName);
				if (dic_en.Value != null)
					throw new Exception("application with this name already exists");

				dic_en.Value = application;
			}
		}

		public void removeApp(string name)
		{
			lock (this)
			{
				if (((Application) applications[name]).CanBeRemoved)
					applications.remove(name);
				else 
					throw new Exception("cannot remove");
			}
		}

		public Configuration(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
		{
			initialization();
			Applications = (Application[]) info.GetValue("Applications", typeof(Application[]));
		}

		#region System.Runtime.Serialization.ISerializable Members

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Applications", Applications);
		}

		#endregion

		public Application[] Applications
		{
			get
			{
				object[] theValues = applications.Values;
				Application[] apps = new Application[theValues.Length];
				for (int ind = 0; ind < theValues.Length; ind++)
					apps[ind] = (Application) theValues[ind]; 
				return apps;
			}

			set
			{
				if (value != null)
				{
					foreach (Application app in value)
						applications[app.identifyingName] = app;
				}
			}
		}

		[Serializable]
		public class Application
		{
			public bool CanBeRemoved
			{
				get
				{
					foreach (Instance instance in this.Instances)
					{
						if (instance.status != Instance.Status.TERMINATED)
							return false;
					}
					return true;
				}
			}

			public Application()
			{
				this.instances = new QS._core_c_.Collections.Hashtable(5);
			}

			public Application(string identifyingName, string descriptiveName, string executableName, string startParameters)
			{
				this.identifyingName = identifyingName;
				this.descriptiveName = descriptiveName;
				this.executableName = executableName;
				this.startParameters = startParameters;
				this.instances = new QS._core_c_.Collections.Hashtable(5);
			}

			public override string ToString()
			{
				return descriptiveName;
			}

			[XmlIgnore]
			public Instance[] Instances
			{
				set
				{
					if (value != null)
					{
						instances = new QS._core_c_.Collections.Hashtable((uint) value.Length);
						foreach (Instance process in value)
							instances[process.InstanceRef] = process;
					}
				}

				get
				{
					object[] inst = instances.Values;
					Instance[] results = new Instance[inst.Length];
					for (int ind = 0; ind < inst.Length; ind++)
						results[ind] = (Instance) inst[ind];
					return results;
				}
			}

			public string identifyingName, descriptiveName, executableName, startParameters;

			[NonSerialized]
			private QS._core_c_.Collections.IDictionary instances;

			public Instance launchAnInstance()
			{
				Instance instance = new Instance(this, System.TimeSpan.FromSeconds(1));
				lock (instances)
				{
					instances[instance.InstanceRef] = instance;
				}
				return instance;
			}

			public void shutdownInstance(Instance.Ref instanceRef)
			{
				Instance instance = null;
				lock (instances)
				{
					instance = (Instance) instances[instanceRef];
				}
				instance.shutdown();
			}

			public Instance lookupAnInstance(Instance.Ref instanceRef)
			{
				lock (instances)
				{
					return (Instance) instances[instanceRef];
				}
			}

			public void removeInstance(Instance.Ref instanceRef)
			{
				lock (instances)
				{
					Instance instance = (Instance) instances[instanceRef];
					if (instance.status == Instance.Status.TERMINATED)
						instances.remove(instanceRef);
					else
						throw new Exception("cannot remove instance because it is running");
				}
			}

			[Serializable]
			public class Instance
			{
				[Serializable]
					public class Ref
				{
					public Ref(Instance process)
					{
						processID = process.processID;
						launchTime = process.launchingTime;
					}

					public int processID;
					public DateTime launchTime;

					public override string ToString()
					{
						return launchTime.ToString() + " : " + processID.ToString();
					}

					public override int GetHashCode()
					{
						return processID.GetHashCode() ^ launchTime.GetHashCode();
					}

					public override bool Equals(object obj)
					{
						return obj != null && (obj is QS._qss_d_.Service_1_.Configuration.Application.Instance.Ref) &&
							((QS._qss_d_.Service_1_.Configuration.Application.Instance.Ref) obj).launchTime == launchTime &&
							((QS._qss_d_.Service_1_.Configuration.Application.Instance.Ref) obj).processID == processID;
					}
				}

				public Ref InstanceRef
				{
					get
					{
						return new Ref(this);
					}
				}

				public Instance()
				{
				}

				public Instance(Configuration.Application application, TimeSpan outputPollingInterval)
				{
					this.applicationName = application.identifyingName;					
					this.startupParameters = application.startParameters;
					this.executableName = application.executableName;

					launchingTime = DateTime.Now;
					
					logger = new QS._core_c_.Base.Logger(null, true);

					thisProcess = new Process();

					thisProcess.StartInfo.FileName = executableName; 
					thisProcess.StartInfo.Arguments = startupParameters; 
					thisProcess.StartInfo.UseShellExecute = false;
					thisProcess.StartInfo.CreateNoWindow = true;
					thisProcess.StartInfo.RedirectStandardOutput = true;
					thisProcess.StartInfo.RedirectStandardError = true;

					thisProcess.EnableRaisingEvents = true;
					thisProcess.Exited += new EventHandler(thisProcess_Exited);
					
					status = Status.RUNNING;

					thisProcess.Start();
					processID = thisProcess.Id;

					stdOutReader = new QS._qss_d_.Base_.OutputReader(thisProcess.StandardOutput, outputPollingInterval);
					stdErrReader = new QS._qss_d_.Base_.OutputReader(thisProcess.StandardError, outputPollingInterval);

					logger.Log(this, "new process created");
				}

				public void shutdown()
				{
					if (thisProcess != null)
						thisProcess.Kill();
				}

				private void thisProcess_Exited(object sender, EventArgs e)
				{
					status = Status.TERMINATED;

					if (stdOutReader != null)
						stdOutReader.shutdown();

					if (stdErrReader != null)
						stdErrReader.shutdown();
				}

				public override string ToString()
				{
					return this.InstanceRef.ToString();
				}

				[XmlIgnore]
				public string StdOut
				{
					get
					{
						return stdOutReader.CurrentOutput;
					}
				}

				[XmlIgnore]
				public string StdErr
				{
					get
					{
						return stdErrReader.CurrentOutput;
					}
				}

				[XmlIgnore]
				public string LogOut
				{
					get
					{
						return logger.CurrentContents;
					}
				}

				public int processID;
				public string applicationName, executableName, startupParameters;
				public Status status;
				public DateTime launchingTime;

				[NonSerialized] private QS._qss_d_.Base_.OutputReader stdOutReader, stdErrReader;
				[NonSerialized] private Process thisProcess;
				[NonSerialized] private QS._core_c_.Base.Logger logger;

				public enum Status
				{
					RUNNING, TERMINATED
				}
			}
		}
	}
}
