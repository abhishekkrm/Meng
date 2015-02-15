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

#define DEBUG_WindowMode
// #define AllowMultipleCPUs

using System;
using System.Diagnostics;

namespace QS._qss_d_.Base_
{
	/// <summary>
	/// Summary description for ProcessController.
	/// </summary>
	public class ProcessController
	{
		public enum Status
		{
			RUNNING, TERMINATED
		}

        public ProcessController(int pid, TimeSpan outputPollingInterval, ProcessExitedCallback processExitedCallback)
        {
            string started_name = "process (pid = " + pid.ToString() + ")";
            try
            {
                this.processExitedCallback = processExitedCallback;

                System.DateTime launchTime = DateTime.Now;
                logger = new QS._core_c_.Base.Logger(QS._core_c_.Base2.PreciseClock.Clock, true);

                process = Process.GetProcessById(pid);

                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(this.processExitedUnexpectedly);

                status = Status.RUNNING;

                logger.Log(this, "Attaching to " + started_name + ".");

                try
                {
                    process.ProcessorAffinity = new IntPtr(1);
                }
                catch (Exception exc)
                {
                    logger.Log(this, "Could not set affinity of an existing process.\n" + exc.ToString());
                }

                processRef = new ProcessRef(pid, launchTime);
            }
            catch (Exception exc)
            {
                throw new Exception("Cannot attach to  " + started_name + ".", exc);
            }
        }

/*
        public ProcessController(string executablePath, string parameterString, TimeSpan outputPollingInterval, 
            ProcessExitedCallback processExitedCallback) 
            : this(executablePath, parameterString, outputPollingInterval, processExitedCallback, false)
        {
        }
*/

		public ProcessController(string executablePath, string parameterString, TimeSpan outputPollingInterval, 
            ProcessExitedCallback processExitedCallback)
		{
            string started_name = "process \"" + executablePath + "\" with parameters \"" + parameterString + "\"";
			try
			{
				this.processExitedCallback = processExitedCallback;

				System.DateTime launchTime = DateTime.Now;
				logger = new QS._core_c_.Base.Logger(QS._core_c_.Base2.PreciseClock.Clock, true);

				process = new Process();

				process.StartInfo.FileName = executablePath;
				process.StartInfo.Arguments = parameterString;

#if DEBUG_WindowMode
                process.StartInfo.UseShellExecute = true;

                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
#else
                process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
#endif

                process.EnableRaisingEvents = true;
				process.Exited += new EventHandler(this.processExitedUnexpectedly);

				status = Status.RUNNING;

                logger.Log(this, "Starting " + started_name + ".");

                try
                {
                    process.ProcessorAffinity = new IntPtr(1);
                }
                catch (Exception)
                {
                    logger.Log(this, "Could not set process affinity before starting.");
                }

                process.Start();

                process.ProcessorAffinity = new IntPtr(1);
/*
#if !AllowMultipleCPUs
                if (System.Environment.ProcessorCount > 1)
                {
                    if (System.Environment.OSVersion.Platform == PlatformID.Win32NT
                       && System.Environment.OSVersion.Version.Major == 5
                       && System.Environment.OSVersion.Version.Minor == 2)
                    {
                        if (QS.CMS.Native.Win32.SetProcessAffinityMask(process.Handle, 1) == 0)
                        {
                            logger.Log(this, "Could not set process affinity mask, operation failed.");
                        }
                        else
                        {
                            ulong processMask, systemMask;
                            if (QS.CMS.Native.Win32.GetProcessAffinityMask(
                                System.Diagnostics.Process.GetCurrentProcess().Handle,
                                out processMask, out systemMask) == 0)
                            {
                                logger.Log(this, "Could not read process affinity mask to verify it has been set.");
                            }
                            else
                            {
                                if (processMask != 1)
                                {
                                    logger.Log(this, "Process affinity mask has not been set.");
                                }
                                else
                                {
                                    // everything is fine
                                    logger.Log(this, "Process affinity mask set successfully.");
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Log(this, "Could not set process affinity mask, not a Windows Server 2003.");
                    }
                }
#endif
*/

                processRef = new ProcessRef(process.Id, launchTime);

#if DEBUG_WindowMode
#else
				stdOutReader = new OutputReader(process.StandardOutput, outputPollingInterval);
				stdErrReader = new OutputReader(process.StandardError, outputPollingInterval);
#endif
			}
			catch (Exception exc)
			{
				throw new Exception("Cannot start " + started_name + ".", exc);
			}
		}

		private ProcessRef processRef;
		private QS._core_c_.Base.IReadableLogger logger;
		private Process process;
		private OutputReader stdOutReader = null, stdErrReader = null;
		private Status status;
		private ProcessExitedCallback processExitedCallback;

		public void shutdown()
		{
			lock (this)
			{
				if (process != null)
					process.Kill();
			}
		}

		private void processExitedUnexpectedly(object sender, EventArgs e)
		{
			try
			{
				lock (this)
				{
					status = Status.TERMINATED;

					if (stdOutReader != null)
						stdOutReader.shutdown();

					if (stdErrReader != null)
						stdErrReader.shutdown();
				}

				processExitedCallback(this.processRef);
			}
			catch (Exception exc)
			{
				logger.Log(this, exc.ToString());
			}
		}

		public ProcessRef Ref
		{
			get
			{
				return processRef;
			}
		}

		public Status CurrentStatus
		{
			get
			{
				return status;
			}
		}

		public string Out
		{
			get
			{
				return stdOutReader.CurrentOutput;
			}
		}

		public string Err
		{
			get
			{
				return stdErrReader.CurrentOutput;
			}
		}

		public string Log
		{
			get
			{
				return logger.CurrentContents;
			}
		}
	}

	public delegate void ProcessExitedCallback(ProcessRef processRef);

	[Serializable]
	public class ProcessRef
	{
		public ProcessRef(int processID, System.DateTime launchTime)
		{
			this.processID = processID;
			this.launchTime = launchTime;
		}

		public override bool Equals(object obj)
		{
			return (obj != null) && (obj is ProcessRef) && 
				(((ProcessRef) obj).processID == this.processID) && (((ProcessRef) obj).launchTime == this.launchTime);
		}

		public override int GetHashCode()
		{
			return processID.GetHashCode() ^ launchTime.GetHashCode();
		}

		public override string ToString()
		{
			return launchTime.ToString() + ":" + processID.ToString();
		}

		public int PID
		{
			get
			{
				return processID;
			}
		}

		public System.DateTime LaunchTime
		{
			get
			{
				return launchTime;
			}
		}

		private int processID;
		private System.DateTime launchTime;
	}
}
