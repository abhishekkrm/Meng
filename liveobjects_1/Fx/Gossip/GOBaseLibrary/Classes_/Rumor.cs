/*

Copyright (c) 2004-2009 Deepak Nataraj. All rights reserved.

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
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using GOBaseLibrary.Interfaces;

namespace GOBaseLibrary.Common
{
    public enum GossipType { DIGEST, FULL_MESSAGE };

    /// <summary>
    /// represents a concrete IGossip object
    /// </summary>
    [Serializable]
    [QS.Fx.Reflection.ValueClass("6`1", "Rumor")]
    public class Rumor : IGossip
    {
        private String id;
        private GossipType type;
        private Group sourceGroup;
        private Group destinationGroup;
        private Node sourceNode;
        private Node destinationNode;
        private String payLoad;
        private string version;
        private int hopCount;
        private bool mark;
        private double receivedTimestamp;                 // set to timestamp when the rumor is received
        private int size;
        private double utility;
        private long age;
        private long timeReceived;

        public Rumor() { size = this.GetSize(); }

        public Rumor(String uniqueID, String payload)
        {
            type = GossipType.FULL_MESSAGE;
            id = uniqueID;
            payLoad = payload;
            mark = false;
            receivedTimestamp = -1;
            size = this.GetSize();
        }

        [XmlElement]
        public String Id { get { return id; } set { id = value; } }

        [XmlElement]
        public GossipType Type { get { return type; } set { type = value; } }
        
        [XmlElement]
        public Group SourceGroup { get { return sourceGroup; } set { sourceGroup = value; } }

        [XmlElement]
        public Group DestinationGroup { get { return destinationGroup; } set { destinationGroup = value; } }

        [XmlElement]
        public Node SourceNode { get { return sourceNode; } set { sourceNode = value; } }

        [XmlElement]
        public Node DestinationNode { get { return destinationNode; } set { destinationNode = value; } }

        [XmlElement]
        public String PayLoad { get { return payLoad; } set { payLoad = value; } }     // Content that has to be rumored

        [XmlElement]
        public String Version { get { return version; } set { version = value; } }

        [XmlElement]
        public int HopCount { get { return hopCount; } set { hopCount = value; } }     // time to live
        
        [XmlElement]
        public bool Mark { get { return mark; } set { mark = value; } }     // mark for deletion etc.,.

        [XmlElement]
        public int Size { get { return this.GetSize(); } }

        [XmlElement]
        public double Utility { get { return utility; } set { utility = value; } }

        public double ReceivedTimestamp { get { return this.receivedTimestamp; } }
        
        [XmlElement]
        public long Age { get { return this.age; } set { this.age = value; } }

        [XmlElement]
        public long ReceivedTS { get { return this.timeReceived; } }

        public int GetSize()
        {
            MemoryStream outStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(outStream);

            /*
             * <Int32> number of rumors in this message
             * <String> rumor id
             * <Int32> hopcount
             * <String> source group
             * <String> destination group
             * <String> source node
             * <String> destination node
             * <String> payload
             * */

                writer.Write((int)this.type);

                if (this.id != null)
                    writer.Write(this.Id);

                writer.Write(this.HopCount);

                if (this.payLoad != null)
                    writer.Write(this.PayLoad);

                if (this.sourceGroup != null)
                    writer.Write(this.SourceGroup.Id);

                if (this.destinationGroup != null)
                    writer.Write(this.DestinationGroup.Id);

                if (this.sourceNode != null)
                    writer.Write(this.SourceNode.Id);

                if (this.destinationNode != null)
                    writer.Write(this.DestinationNode.Id);

            return (int) outStream.Length;
        }

        public void StampTheTime()
        {
            double now = Utils.ConvertToUnixTimestamp(DateTime.Now);
            this.receivedTimestamp = now;
            this.timeReceived = DateTime.Now.Ticks;
        }

        public double GetTimestamp()
        {
            Debug.WriteLineIf(Utils.debugSwitch.Verbose,"Rumor::GetTimestamp: returning " + this.receivedTimestamp + "[" + DateTime.Now + "]");
            return this.receivedTimestamp;
        }

        /// <summary>
        /// check if the timestamp on the rumor has expired or not
        /// </summary>
        /// <param name="interval">timestamp expiry in milliseconds</param>
        /// <returns>true if timestamp has expired, false otherwise</returns>
        public bool IsTimerExpired(double interval)
        {
            double now = Utils.ConvertToUnixTimestamp(DateTime.Now);
            bool retValue = now > this.receivedTimestamp + interval;

            Debug.WriteLineIf(Utils.debugSwitch.Verbose,"Rumor::IsExpired: rumor[" + this.id + "], interval is " + interval 
                            + ", returning " + retValue + "["
                            + Utils.ConvertFromUnixTimestamp(this.receivedTimestamp) + "]");
            return retValue;
        }

        /// <summary>
        /// set a timer which fires up later to pass 'this' to the callback specified
        /// </summary>
        /// <param name="_timerDelegate">callback function to call when the timer expires</param>
        /// <param name="interval">amount of time elapsed in milliseconds, before which the callback is invoked</param>
        public void StartTTLTicks(TimerCallback _timerDelegate, long interval)
        {
            if (_timerDelegate == null)
            {
                throw new Exception("timercallback is undefined");
            }

            new Timer(_timerDelegate, this, interval, Timeout.Infinite);
        }

        // deep copy in separate memory space
        public object Clone()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;
            object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }
}
