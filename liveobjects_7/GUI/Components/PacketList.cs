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
    public partial class PacketList : UserControl
    {
        public PacketList()
        {
            InitializeComponent();
        }

        private IDictionary<uint, QS._qss_c_.Packets_.IPacket> packets =
            new Dictionary<uint, QS._qss_c_.Packets_.IPacket>();
        private EventHandler handler;

        #region Adding and removing

        public void BeginUpdate()
        {
            listView1.BeginUpdate();
        }

        public void EndUpdate()
        {
            listView1.EndUpdate();
        }

        public void Add(uint packetno, QS._qss_c_.Packets_.IPacket packet)
        {
            lock (this)
            {
                if (packets.ContainsKey(packetno))
                    throw new Exception("Packet with number " + packetno.ToString() + " has already been added.");

                packets.Add(packetno, packet);
                listView1.Items.Add(new OurItem(packetno, packet));
            }
        }

        #endregion

        #region Accessors

        public event EventHandler OnSelected
        {
            add { handler += value; }
            remove { handler -= value; }
        }

        public bool Selected
        {
            get { return listView1.SelectedItems.Count == 1; }
        }

        public bool GetSelected(out uint packetno, out QS._qss_c_.Packets_.IPacket packet)
        {
            lock (this)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    OurItem item = listView1.SelectedItems[0] as OurItem;
                    if (item != null)
                    {
                        packetno = item.PacketNo;
                        packet = item.Packet;
                        return true;
                    }
                }

                packetno = 0;
                packet = null;
                return false;
            }
        }

/*
        public uint PacketNo
        {
            get 
            { 
                lock (this)
                {
                    if (listView1.SelectedItems.Count == 1)
                        return ((OurItem)listView1.SelectedItems[0]).PacketNo;
                    else
                        throw new Exception("Nothing selected.");
                }
            }
        }

        public QS.CMS.Packets.DeserializedPacket Packet
        {
            get
            {
                lock (this)
                {
                    if (listView1.SelectedItems.Count == 1)
                        return ((OurItem)listView1.SelectedItems[0]).Packet;
                    else
                        throw new Exception("Nothing selected.");
                }
            }
        }
*/

        #endregion

        #region Class OurItem

        private class OurItem : ListViewItem
        {
            private static string ChannelName(uint channel)
            {
                return Enum.IsDefined(typeof(ReservedObjectID), channel) ? Enum.GetName(typeof(ReservedObjectID), channel) : channel.ToString(); 
            }

            private static string MessageString(QS.Fx.Serialization.ISerializable message)
            {
                if (message == null)
                    return "null";
                else
                {
                    try
                    {
                        return QS.Fx.Printing.Printable.ToString(message);
                    }
                    catch (Exception)
                    {
                        return "?";
                    }
                }
            }

            public OurItem(uint packetno, QS._qss_c_.Packets_.IPacket packet)
                : base(new string[] { packetno.ToString(), packet.Time.ToString("000000.000000"), packet.Sender.ToString(), 
                    packet.Destination.ToString(), packet.Stream.ToString(), packet.SequenceNo.ToString(), ChannelName(packet.Channel),
                    MessageString(packet.Object) })
            {
                this.packetno = packetno;
                this.packet = packet;
            }

            private uint packetno;
            private QS._qss_c_.Packets_.IPacket packet;

            public uint PacketNo
            {
                get { return packetno; }
            }

            public QS._qss_c_.Packets_.IPacket Packet
            {
                get { return packet; }
            }
        }

        #endregion

        #region Clicking

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EventHandler handler = this.handler;
            if (handler != null)
                handler(this, null);
        }

        #endregion
    }
}
