/*

Copyright (c) 2010 Bo Peng. All rights reserved.

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

#define VERBOSE

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QS.Fx.Base;

namespace Quilt.PubsubApp
{
    [QS.Fx.Reflection.ComponentClass("C131AB0C81C54f078B6BFF0CF642C6EF", "PubSubClient", "subscribe with user interest")]
    public sealed class PubSubClient : QS._qss_x_.Properties_.Component_.Base_, QS.Fx.Interface.Classes.IPubSubClient
    {
        #region Constructor

        public PubSubClient(QS.Fx.Object.IContext _context,
            [QS.Fx.Reflection.Parameter("nodeID", QS.Fx.Reflection.ParameterClass.Value)]
            string _nodeID,
            [QS.Fx.Reflection.Parameter("isPublisher", QS.Fx.Reflection.ParameterClass.Value)]
            bool _isPublisher,
            [QS.Fx.Reflection.Parameter("loop", QS.Fx.Reflection.ParameterClass.Value)]
            bool _loop,
            //[QS.Fx.Reflection.Parameter("InterestFile", QS.Fx.Reflection.ParameterClass.Value)]
            //String _interestFile,
            //[QS.Fx.Reflection.Parameter("StreamFile", QS.Fx.Reflection.ParameterClass.Value)]
            //String _streamFile,
            [QS.Fx.Reflection.Parameter("GradientOverlay", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IPubSub> _pubsub
            )
            : base(_context, true)
        {
            this.myID = _nodeID;
            //this.loop = true;
            this.loop = _loop;
            this.isPublisher = _isPublisher;

            if (_nodeID == null || _nodeID == "")
            {
                this.myID = System.Net.Dns.GetHostName();
                StreamReader map = new StreamReader("c:/map.txt");
                string line;
                Dictionary<string, string> map_dict = new Dictionary<string, string>();
                while (null != (line = map.ReadLine()))
                {
                    char[] set = { ' ' };
                    string[] elems = line.Split(set, StringSplitOptions.RemoveEmptyEntries);
                    if (!map_dict.ContainsKey(elems[3]))
                    {
                        map_dict.Add(elems[3], elems[0]);
                    }
                }
                map.Close();

                try
                {
                    this.myID = map_dict[this.myID];
                }
                catch (Exception exc)
                {
                    throw new Exception("Quilt.PubsubApp.PubSubClient.Constructor " + exc);
                }
            }
            this.myInterest = new Dictionary<string, Interest>();
            this.myState = new Dictionary<string, StateUnion>();
            this.streamAlarms = new Dictionary<string, QS.Fx.Clock.IAlarm>();
            this.streamMsg = new Dictionary<string, PubSubData>();

            this.getInterest();
            //this.pubDel = new PubDelegate(this.PublishData);

            this.mycontext = _context;
            this.pubsubEndpoint = this.mycontext.DualInterface<QS.Fx.Interface.Classes.IPubSubOps, QS.Fx.Interface.Classes.IPubSubClient>(this);
            this.pubsubEndpoint.OnConnected += new QS.Fx.Base.Callback(delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.Connect))); });
            this.pubsubEndpoint.OnDisconnect += new QS.Fx.Base.Callback(delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.Disconnect))); });
            this.pubsub = _pubsub.Dereference(this.mycontext);
            this.pubsubConn = this.pubsubEndpoint.Connect(this.pubsub.PubSub);
        }

        #endregion

        #region Fields

        private string myID;
        public StreamWriter logFile;
        //private String myName;
        private bool loop;
        private bool isPublisher;
        private StreamReader interestFile;
        private StreamReader streamFile;
        private Dictionary<string, QS.Fx.Clock.IAlarm> streamAlarms;
        private Dictionary<string, PubSubData> streamMsg;

        public struct Interest
        {
            public string streamID;
            public int dur;
            public int bitRate;
            public int msgSize;
            public double utility;

            public Interest(string _strID, int _dur, int _bitRate, int _msgSize, double _resol)
            {
                streamID = _strID;
                dur = _dur;
                bitRate = _bitRate;
                msgSize = _msgSize;
                utility = _resol;
            }

            public void setResol(double _resol)
            {
                utility = _resol;
            }
        }
        public enum StateUnion
        {
            UNSUBSCRIBED = 0,
            SUBSCRIBED = 1,
            PUBLISHING = 2
        }
        private Dictionary<string, StateUnion> myState;
        private Dictionary<string, Interest> myInterest;
        private QS.Fx.Object.IContext mycontext;
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IPubSubOps, QS.Fx.Interface.Classes.IPubSubClient> pubsubEndpoint;
        private QS.Fx.Endpoint.IConnection pubsubConn;
        private QS.Fx.Object.Classes.IPubSub pubsub;

        private int alarmIntervalMS = 20;
        //public delegate void PubDelegate(string streamID);
        //private PubDelegate pubDel;

        #endregion

        #region Utilities

        private void getInterest()
        {
            string line = null;
            string[] toks;
            this.streamFile = new StreamReader("C:/stream.txt");
            //read stream file
            while ((line = this.streamFile.ReadLine()) != null)
            {
                //parse each line to get stream info
                toks = line.Split('\t');
                //file format:
                // streamID, streamType, duration, bitRate(kbps), msgSize(byte), serverID(cs*)
                if (this.isPublisher)
                {
                    if (!this.myInterest.ContainsKey(toks[0]))
                    {
                        if ("cs" + toks[5] == this.myID)
                        {
                            this.myInterest.Add(toks[0], new Interest(toks[0], Convert.ToInt32(toks[2]), Convert.ToInt32(toks[3]), Convert.ToInt32(toks[4]), 100));
                        }
                    }
                    int sent_msg_sz = Convert.ToInt32(toks[4]);
                    // make msg
                    StringBuilder x = new StringBuilder();
                    x.Append('a', sent_msg_sz/2 - 25);
                    this._logger.Log("pub data size: " + x.Length);
                    this.streamMsg[toks[0]] = new PubSubData(new Name(x.ToString()), 100);
                }
                else
                {
                    if (!this.myInterest.ContainsKey(toks[0]))
                    {
                        this.myInterest.Add(toks[0], new Interest(toks[0], Convert.ToInt32(toks[2]), Convert.ToInt32(toks[3]), Convert.ToInt32(toks[4]), 0));
                    }
                }
                if (!this.myState.ContainsKey(toks[0]))
                {
                    this.myState.Add(toks[0], StateUnion.UNSUBSCRIBED);
                }
            }
            this.streamFile.Close();

            //if this is not the source, get Interests
            if (!this.isPublisher)
            {
                //read interest file
                double resolution;
                Interest oldresol;
                this.interestFile = new StreamReader("C:/interest.txt");
                while ((line = this.interestFile.ReadLine()) != null)
                {
                    //parse each line to get interest of this node
                    toks = line.Split('\t');
                    //file format:
                    // userID, streamID, resolution, serverID
                    if (toks[3] == this.myID)
                    {
                        resolution = Convert.ToDouble(toks[2]);
                        if (this.myInterest.TryGetValue(toks[1], out oldresol))
                        {
                            if (resolution > oldresol.utility)
                            {
                                this.myInterest.Remove(toks[1]);
                                oldresol.utility = resolution;
                                if (!this.myInterest.ContainsKey(toks[1]))
                                {
                                    this.myInterest.Add(toks[1], oldresol);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("invalid stream # read");
                        }
                    }
                }
                this.interestFile.Close();
            }
        }

        #endregion

        #region IPubSubClient Members

        void QS.Fx.Interface.Classes.IPubSubClient.Ready()
        {
            //start to subscribe when connected
            foreach (string i in this.myInterest.Keys)
            {
                if (this.myInterest[i].utility > 0)
                {
                    this._logger.Log("client calls subscribe: " + i);
                    this.pubsubEndpoint.Interface.Subscribe(Convert.ToString(i), this.myInterest[i].utility, this.isPublisher);
                    //System.Threading.Thread.Sleep(100);
                }
            }
        }

        void QS.Fx.Interface.Classes.IPubSubClient.Subscribed(string group_id, bool is_publisher)
        {
            //for publisher, start publishing 
            if (is_publisher)
            {
                //wait for 5s before starting publishing
                if (!this.streamAlarms.ContainsKey(group_id))
                {
                    this.myState[group_id] = StateUnion.PUBLISHING;
                    this._logger.Log("subscribed");
                    //System.Threading.Thread.Sleep(5000);
                    QS.Fx.Clock.IAlarm stream_alarm = this._platform.AlarmClock.Schedule
                    (
                        alarmIntervalMS * 0.001, // sec level
                        new QS.Fx.Clock.AlarmCallback
                        (
                            delegate(QS.Fx.Clock.IAlarm _alarm)
                            {
                                this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string>(this.PublishData, group_id));
                            }
                        ),
                        null
                     );
                    this.streamAlarms.Add(group_id, stream_alarm);
                }
                //this.pubDel.BeginInvoke(group_id, new AsyncCallback(PubCallback), null);
            }
            else // for subscriber, log subscribed
            {
                this.myState[group_id] = StateUnion.SUBSCRIBED;
                this._logger.Log("subscribed");
                // only for debugging
                //this.pubDel.BeginInvoke(group_id, new AsyncCallback(PubCallback), null);
            }
        }

        void QS.Fx.Interface.Classes.IPubSubClient.Data(string group_id, QS.Fx.Serialization.ISerializable _data)
        {
            if (this.isPublisher)
            {
                throw new Exception("I'm publisher, should not RECEIVE data");
            }
            else
            {
                if (_data.SerializableInfo.ClassID != (ushort)QS.ClassID.PubSubData)
                {
                    throw new Exception("The data is broken");
                }
                //this._logger.Log(Convert.ToString(this.myID) + " :receiving message of length " + Convert.ToString(((PubSubData)_data)._data.String.Length) + " and rate " + Convert.ToString(((PubSubData)_data)._rate));
                //this.logFile = new StreamWriter("C:/" + this.myID + "_gradient_app.log");
                // write log to file: timestamp, nodeID, operation, streamID, dataSize, dataRate
                //this.logFile.WriteLine(DateTime.Now.ToLongTimeString() + " " + this.myID + " Receive " + group_id + " " + Convert.ToString(((PubSubData)_data)._data.String.Length) + " " + Convert.ToString(((PubSubData)_data)._rate));
                //this.logFile.Close();
            }
        }

        #endregion

        #region Publish Data

        private void PublishData(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<string> _event_ = (QS._qss_x_.Properties_.Base_.IEvent_<string>)_event;
            string streamID = _event_._Object;
            //msgsize: byte
            int sent_msg_sz = this.myInterest[streamID].msgSize;
            // bitrate: kbps
            // kbps * 1000 / 8 / (1000 / alarmIntervalMS) =  kbps * alarmIntervalMS / 8;
            int total_data = this.myInterest[streamID].bitRate * alarmIntervalMS / 8;
            int dataSize = total_data;
            if (!this.streamMsg.ContainsKey(streamID))
            {
                this._logger.Log(this.myID + " invalid stream ID: " + streamID);
                return;
            }
            while (dataSize > 0)
            {
#if VERBOSE
                //this._logger.Log(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + " stream" + streamID + ": publish begin");

                Quilt.Core.GradientPeer gp = pubsub as Quilt.Core.GradientPeer;
                if (gp._share_state != null && gp._share_state._log != null)
                {
                    double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    gp._share_state._log.WriteLine(cur + "\tPubSubClient\t" + streamID + "\tPushed");
                    gp._share_state._log.Flush();
                }
#endif
                this.pubsubEndpoint.Interface.Publish(Convert.ToString(streamID), this.streamMsg[streamID]);

                dataSize -= sent_msg_sz;
                if ((double)sent_msg_sz / total_data < 1)
                {
#if VERBOSE
                    if (this._logger != null)
                    {
                        //this._logger.Log(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + " stream" + streamID + ": sleep " + 1000 * sent_msg_sz / total_data + "ms");
                    }
#endif

                    //System.Threading.Thread.Sleep(1000 * sent_msg_sz / total_data);
                }
            }
            this.myState[streamID] = StateUnion.SUBSCRIBED;
            //GC.Collect();
            //this._logger.Log(Convert.ToString(this.myID) + ": msgSize: " + sent_msg_sz + "total Byte: " + total_data + " time: " + Convert.ToString(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
            // write log to file: timestamp, streamID, sourceID, rate, destID, msg, msgType
            //this.logFile.WriteLine(Convert.ToString(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + "\t" + streamID + "\t" + this.myID + "\t1" + "\tNULL" + "\t" + Convert.ToString(total_data) + "\tSEND");
            //this.logFile.Close();
            if (this.loop)
            {
                this.streamAlarms[streamID].Reschedule();
            }
        }

        //private void PubCallback(IAsyncResult ar)
        //{
        //  this.pubDel.EndInvoke(ar);
        //}

        #endregion

        #region Connect & Disconnect

        private void Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {

        }

        private void Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            //start to unsubscribe when disconnected
            foreach (string i in this.myInterest.Keys)
            {
                if (this.myInterest[i].utility > 0.0)
                {
                    this.myState[i] = StateUnion.UNSUBSCRIBED;
                }
            }
        }

        #endregion
    }
}
