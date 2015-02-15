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
using System.ComponentModel;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace QS.GUI.Components
{
    public partial class AddressCollector : UserControl, IDisposable
    {
        public AddressCollector()
        {
            InitializeComponent();
        }

        public void Start(QS.Fx.Network.NetworkAddress multicastAddress, TimeSpan pollingInterval)            
        {
            this.Start(new QS._qss_c_.Platform_.PhysicalPlatform(), multicastAddress, pollingInterval);
        }

        public void Start(
            QS._qss_c_.Platform_.IPlatform platform, QS.Fx.Network.NetworkAddress multicastAddress, TimeSpan pollingInterval)
        {
            this.pollingInterval = pollingInterval;
            multicastCollector = new QS._qss_c_.Connections_.MulticastCollector(platform, multicastAddress);
            myThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.CheckingCallback));
            myThread.Start();                
        }

        public event EventHandler Selected;

        private TimeSpan pollingInterval;
        private QS._qss_c_.Connections_.MulticastCollector multicastCollector;
        private System.Threading.Thread myThread;
        private bool completed;
        private System.Threading.ManualResetEvent completedEvent = new System.Threading.ManualResetEvent(false);
        private ICollection<QS.Fx.Network.NetworkAddress> collectedAddresses = 
            new System.Collections.ObjectModel.Collection<QS.Fx.Network.NetworkAddress>();

        private void CheckingCallback()
        {
            while (!completed)
            {
                lock (this)
                {
                    foreach (QS.Fx.Network.NetworkAddress address in multicastCollector.CollectedObjects)
                    {
                        if (!collectedAddresses.Contains(address))
                        {
                            collectedAddresses.Add(address);
                            listBox1.BeginInvoke(
                                new AsyncCallback(delegate(IAsyncResult result) { listBox1.Items.Add(address); }), new object[] { null });
                        }
                    }
                }

                completedEvent.WaitOne(pollingInterval, false);
            }
        }

        public QS.Fx.Network.NetworkAddress SelectedAddress
        {
            get { return listBox1.SelectedItem as QS.Fx.Network.NetworkAddress; }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            try
            {
                completed = true;
                completedEvent.Set();

                if (myThread != null)
                    myThread.Join();

                if (multicastCollector != null)
                    ((IDisposable)multicastCollector).Dispose();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        private void listBox1_DoubleClick_1(object sender, EventArgs e)
        {
            if (this.SelectedAddress != null && Selected != null)
                Selected(this, null);
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.SelectedAddress != null && Selected != null)
                Selected(this, null);
        }
    }
}
