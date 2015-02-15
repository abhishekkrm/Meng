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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace QS._qss_d_.Agent_
{
	public class Agent : MarshalByRefObject, IAgent, IDisposable
	{
		public Agent()
		{
		}

		#region IAgent Members

        string IAgent.Execute(string executablePath, string arguments, TimeSpan operationTimeout)
        {
            QS._core_c_.Base.Logger logger = new QS._core_c_.Base.Logger(null, true);
            try
            {
                ManualResetEvent completed = new ManualResetEvent(false);
                Base_.ProcessController processController = new QS._qss_d_.Base_.ProcessController(executablePath, arguments, operationTimeout,
                    new QS._qss_d_.Base_.ProcessExitedCallback(delegate(QS._qss_d_.Base_.ProcessRef processRef) { completed.Set(); }));

                try
                {
                    if (!completed.WaitOne(operationTimeout, false))
                        throw new Exception("Operation timed out.");
                }
                finally
                {
                    try
                    {
                        logger.Log(this, "__________OperationLog:\n" + processController.Log);
                        logger.Log(this, "__________Output:\n" + processController.Out);
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        if (processController.CurrentStatus != QS._qss_d_.Base_.ProcessController.Status.TERMINATED)
                            processController.shutdown();
                    }
                    catch (Exception)
                    {
                    }
                }

                logger.Log(this, "Operation completed succesfully.");
            }
            catch (Exception exc)
            {
                logger.Log(this, "Execution failed: Uncatched exception " + exc.ToString());
            }

            return logger.CurrentContents;
        }

		void IAgent.RestartService(string serviceName, bool shouldStop, bool shouldStart, TimeSpan operationTimeout)
		{
            using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController(serviceName))
			{
				if (shouldStop && serviceController.Status != ServiceControllerStatus.Stopped)
				{
					serviceController.Stop();

					serviceController.WaitForStatus(ServiceControllerStatus.Stopped, operationTimeout);

					if (serviceController.Status != ServiceControllerStatus.Stopped)
						throw new Exception("Could not stop the service.");
				}

				if (shouldStart && serviceController.Status != ServiceControllerStatus.Running)
				{
					serviceController.Start();

					serviceController.WaitForStatus(ServiceControllerStatus.Running, operationTimeout);

					if (serviceController.Status != ServiceControllerStatus.Running)
						throw new Exception("Could not start the service.");
				}
			}
		}

		byte[] IAgent.Download(string path)
		{
            return Helpers_.File.Load(path);
		}

		byte[] IAgent.DownloadCompressed(string path)
		{
			return Helpers_.Compressor.Compress(((IAgent) this).Download(path));
		}

		void IAgent.Upload(string path, byte[] bytes)
		{
            Helpers_.File.Save(path, bytes);
		}

		void IAgent.UploadCompressed(string path, byte[] bytes)
		{
			((IAgent)this).Upload(path, Helpers_.Compressor.Uncompress(bytes));
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{			
		}

		#endregion
	}
}
