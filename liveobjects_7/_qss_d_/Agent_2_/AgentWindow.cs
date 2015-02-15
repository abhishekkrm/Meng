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
using System.Net;

namespace QS._qss_d_.Agent_2_
{
    public partial class AgentWindow : Form
    {
        public AgentWindow(string[] args)
        {
            InitializeComponent();

            if (args.Length != 1)
                throw new Exception("Bad arguments.");

            IPAddress[] ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());

            IDictionary<QS._qss_c_.Base1_.Subnet, int> subnets = new Dictionary<QS._qss_c_.Base1_.Subnet, int>();
            foreach (string address_spec in args[0].Split(','))
            {
                int separator = address_spec.IndexOf(":");
                QS._qss_c_.Base1_.Subnet subnet = new QS._qss_c_.Base1_.Subnet(address_spec.Substring(0, separator));
                int port = Convert.ToInt32(address_spec.Substring(separator + 1));
                subnets.Add(subnet, port);
            }

            List<QS.Fx.Network.NetworkAddress> networkAddresses = new List<QS.Fx.Network.NetworkAddress>();
            foreach (IPAddress ipAddress in ipAddresses)
            {
                foreach (QS._qss_c_.Base1_.Subnet subnet in subnets.Keys)
                {
                    if (subnet.contains(ipAddress))
                        networkAddresses.Add(new QS.Fx.Network.NetworkAddress(ipAddress, subnets[subnet]));
                }
            }

            this.Text = "QuickSilver Agent : " + 
                QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS.Fx.Network.NetworkAddress>(networkAddresses, ", ");

            // this.agent = new Agent(networkAddresses, 
            //      new ConnectCallback(this.ConnectCallback), new DisconnectCallback(this.DisconnectCallback), new Service());

            this.agent = new Agent2(networkAddresses,
                new ConnectCallback(this.ConnectCallback), new DisconnectCallback(this.DisconnectCallback), new ServiceObject());
        }

        private IAgent agent;
        private IDictionary<ISessionController, SessionItem> sessionItems = new Dictionary<ISessionController, SessionItem>();

        private void ConnectCallback(ISessionController session)
        {
            SessionWindow sessionWindow = new SessionWindow(session);
            SessionItem sessionItem = new SessionItem(session.Address.ToString(), sessionWindow);
            sessionItems.Add(session, sessionItem);

            listView1.Items.Add(sessionItem);
        }

        private void DisconnectCallback(ISessionController session)
        {
            SessionItem sessionItem;
            if (sessionItems.TryGetValue(session, out sessionItem))
            {
                SessionWindow sessionWindow = sessionItem.sessionWindow;
                sessionItems.Remove(session);

                if (listView1.Items.Contains(sessionItem))
                    listView1.Items.Remove(sessionItem);

                if (splitContainer1.Panel2.Controls.Contains(sessionWindow))
                    splitContainer1.Panel2.Controls.Remove(sessionWindow);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();

            if (listView1.SelectedItems.Count > 0)
            {
                SessionItem sessionItem = listView1.SelectedItems[0] as SessionItem;
                if (sessionItem != null)
                {
                    SessionWindow sessionWindow = sessionItem.sessionWindow;
                    splitContainer1.Panel2.Controls.Add(sessionWindow);
                    sessionWindow.Dock = DockStyle.Fill;
                }
            }
        }

        private class SessionItem : ListViewItem
        {
            public SessionItem(string name, SessionWindow sessionWindow) : base(name)
            {
                this.sessionWindow = sessionWindow;
            }

            public SessionWindow sessionWindow;
        }

        private void AgentWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((IDisposable)agent).Dispose();
        }
    }
}
