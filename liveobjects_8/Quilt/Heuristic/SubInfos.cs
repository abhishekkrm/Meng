/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quilt.Heuristic
{
    public class SubInfos
    {
        #region Fields

        private static string SRC = "";
        //public Dictionary<string, Dictionary<double, int>> _franctions = new Dictionary<string,Dictionary<double,int>>();
        public List<int> _rates = new List<int>();
        public Dictionary<string, List<string>> _terminals = new Dictionary<string, List<string>>();
        public Dictionary<string, int> _bws = new Dictionary<string, int>();
        public Dictionary<string, Dictionary<string, int>> _utilities = new Dictionary<string, Dictionary<string, int>>(); //terminal, stream, utility
        public Dictionary<string, string> _sourceID = new Dictionary<string, string>();

        public Dictionary<string, Interest> _streamInfo = new Dictionary<string, Interest>();
        public struct Interest
        {
            public string streamID;
            public int dur;
            public int bitRate;//bandwidth
            public int msgSize;
            public double utility;//utility

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
        private StreamReader interestFile;
        private StreamReader streamFile;

        #endregion

        #region Constructor

        public SubInfos(string _interest_file, string _stream_file)
        {
            this.SetSubInfo(_interest_file, _stream_file);
            //TODO: read file to parse information
        }

        public SubInfos()
        {
            // use default path
            this.SetSubInfo("C:/interest.txt", "C:/stream.txt");
            //TODO: set on the run
        }

        #endregion

        #region private methods

        private void SetSubInfo(string _interest_file, string _stream_file)
        {
            this.interestFile = new StreamReader(_interest_file);
            this.streamFile = new StreamReader(_stream_file);
            this.getInterest();

        }

        private void getInterest()
        {
            string line = null;
            string[] toks;
            //read stream file
            while ((line = this.streamFile.ReadLine()) != null)
            {
                //parse each line to get stream info
                toks = line.Split('\t');
                //file format:
                // streamID, streamType, duration, bitRate, msgSize, sourceID
                this._streamInfo.Add(toks[0], new Interest(toks[0], Convert.ToInt32(toks[2]), Convert.ToInt32(toks[3]), Convert.ToInt32(toks[4]), 0.0));
                if (!this._sourceID.ContainsKey(toks[0]))
                {
                    this._sourceID.Add(toks[0], "cs" + toks[5]);
                }
                // bindwidth: kbps
                this._bws[toks[0]] = Convert.ToInt32(toks[3])*10;
            }

            while ((line = this.interestFile.ReadLine()) != null)
            {
                // parse each line to get interest of this node
                toks = line.Split('\t');
                // fill this._utilities structure
                // file format:
                // userID, streamID, rate, serverID
                if (!this._rates.Contains(Convert.ToInt32(toks[2])))
                {
                    this._rates.Add(Convert.ToInt32(toks[2]));
                }
                if (this._utilities.ContainsKey(toks[3]))
                {
                    // already has this nodeID in the structurde
                    if (this._utilities[toks[3]].ContainsKey(toks[1]))
                    {
                        // already has this streamID
                        if (this._utilities[toks[3]][toks[1]] < Convert.ToInt32(toks[2]))
                        {
                            this._utilities[toks[3]][toks[1]] = Convert.ToInt32(toks[2]);
                        }
                    }
                    else
                    {
                        this._utilities[toks[3]][toks[1]] = (int)(Convert.ToDouble(toks[2]));
                    }
                }
                else
                {
                    Dictionary<string, int> stream_info = new Dictionary<string, int>();
                    stream_info[toks[1]] = (int)(Convert.ToDouble(toks[2]));
                    this._utilities[toks[3]] = stream_info;
                }
                // fill this._terminals structure
                if (this._terminals.ContainsKey(toks[1]))
                {
                    // already has the streamID
                    if (!this._terminals[toks[1]].Contains(toks[3]))
                    {
                        // this terminal hasn't been added for this stream
                        this._terminals[toks[1]].Add(toks[3]);
                    }
                }
                else
                {
                    List<string> terminals = new List<string>();
                    terminals.Add(toks[3]);
                    this._terminals[toks[1]] = terminals;
                }
            }
            //this.setSource();  
        }

        public string getSource(string streamID)
        {
            string sourceID;
            if (!this._sourceID.TryGetValue(streamID, out sourceID))
                return "";
            return sourceID;
        }
        #endregion
    }
}
