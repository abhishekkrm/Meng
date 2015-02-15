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

#define DEBUG_Uploader

using System;
using System.Threading;
using System.Net;

namespace QS._qss_e_.Deployment_
{
	/// <summary>
	/// Summary description for Uploader.
	/// </summary>
	public class ServiceUploader : IUploader
	{
		public static byte[] File2Bytes(string localPath)
		{
			byte[] fileAsBytes = null;
			using (System.IO.FileStream fileStream = new System.IO.FileStream(localPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				fileAsBytes = new byte[fileStream.Length];
				if (fileStream.Read(fileAsBytes, 0, (int) fileStream.Length) != fileStream.Length)
					throw new Exception("could not read the whole file");
			}
			return fileAsBytes;
		}

		public ServiceUploader(QS._qss_d_.Service_2_.IClient serviceClient, QS.Fx.Logging.ILogger logger, TimeSpan timeoutOnRequests)
		{
			this.serviceClient = serviceClient;
			this.requests = new QS._qss_c_.Collections_1_.BiLinkableCollection();
			this.logger = logger;
			this.timeoutOnRequests = timeoutOnRequests;
		}

		private QS._qss_d_.Service_2_.IClient serviceClient;
		private QS._qss_c_.Collections_1_.IBiLinkableCollection requests;
		private QS.Fx.Logging.ILogger logger;
		private TimeSpan timeoutOnRequests;

		private class Request : QS._qss_c_.Collections_1_.GenericBiLinkable, IDisposable
		{
			public Request(string localPath, IPAddress destination, string remotePath, ServiceUploader encapsulatingUploader)
				: this(localPath, null, destination, remotePath, encapsulatingUploader)
			{
			}

			public Request(byte[] fileAsBytes, IPAddress destination, string remotePath, ServiceUploader encapsulatingUploader)
				: this(null, fileAsBytes, destination, remotePath, encapsulatingUploader)
			{
			}

			private Request(string localPath, byte[] fileAsBytes, IPAddress destination, string remotePath, ServiceUploader encapsulatingUploader)
			{
				this.localPath = localPath;
				this.fileAsBytes = fileAsBytes;
				this.remotePath = remotePath;
				this.destination = destination;
				this.encapsulatingUploader = encapsulatingUploader;

				this.thread = new Thread(new ThreadStart(this.mainloop));
				thread.Start();
			}

			private string localPath, remotePath;
			private byte[] fileAsBytes;
			private IPAddress destination;
			private ServiceUploader encapsulatingUploader;
			private Thread thread;

			private void mainloop()
			{
				try
				{
#if DEBUG_Uploader
					encapsulatingUploader.logger.Log(null, "connecting to " + destination.ToString() + " to transmit " + localPath);
#endif

					using (QS._qss_d_.Service_2_.IServiceRef serviceRef = encapsulatingUploader.serviceClient.connectTo(
						new QS.Fx.Network.NetworkAddress(destination, (int) QS._qss_d_.Base_.Win32Config.DefaultMainTCPServicePortNo), 
						encapsulatingUploader.logger, encapsulatingUploader.timeoutOnRequests))
					{
#if DEBUG_Uploader
						encapsulatingUploader.logger.Log(null, "initiating upload of " + localPath + " to " + destination.ToString());
#endif
						if (localPath != null)
							serviceRef.upload(localPath, remotePath);
						else
							serviceRef.upload(fileAsBytes, remotePath);
					}

#if DEBUG_Uploader
					encapsulatingUploader.logger.Log(null, "transmission of " + localPath + " to " + destination.ToString() + " succeeded");
#endif
				}
				catch (Exception exc)
				{
					encapsulatingUploader.logger.Log(this, this.ToString() + ", mainloop : " + exc.ToString());
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				if (!thread.Join(TimeSpan.FromSeconds(300)))
				{
					try
					{
						thread.Abort();
					}
					catch (Exception exc)
					{
						encapsulatingUploader.logger.Log(this, "Dispose : " + exc.ToString());
					}
				}
			}

			#endregion

			public override string ToString()
			{
				return "Uploader.Request(" + localPath + " -> " + destination.ToString() + ":" + remotePath + ")";
			}
		}

		#region IUploader Members

		public void schedule(string localPath, IPAddress destination, string remotePath)
		{
			lock (requests)
			{
				requests.insertAtTail(new Request(localPath, destination, remotePath, this));
			}
		}

		public void schedule(byte[] fileAsBytes, IPAddress destination, string remotePath)
		{
			lock (requests)
			{
				requests.insertAtTail(new Request(fileAsBytes, destination, remotePath, this));
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			lock (this)
			{
				while (requests.Count > 0)
				{
					Request request = (Request) requests.elementAtHead();
					requests.remove(request);

					request.Dispose();
				}
			}
		}

		#endregion
	}
}
