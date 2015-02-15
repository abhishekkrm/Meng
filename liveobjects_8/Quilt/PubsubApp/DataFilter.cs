/*

Copyright (c) 2010 Bo Peng. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the
following conditions
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
using Quilt.PubsubApp;
using QS.Fx.Base;

namespace Quilt.PubsubApp
{
    class DataFilter
    {
        public Dictionary<int, Dictionary<int, PubSubData>> _msgs;
        public Dictionary<string, int> _map;

        #region constructor

        public DataFilter()
        {
            this._msgs = new Dictionary<int, Dictionary<int, PubSubData>>();
            this._map = new Dictionary<string, int>();
            StreamReader file = new StreamReader("C:/stream.txt");
            string line;
            string[] toks;
            int stream_type;
            Dictionary<int, PubSubData> rate_msg;
            //read stream file
            while ((line = file.ReadLine()) != null)
            {
                //parse each line to get stream info
                toks = line.Split('\t');
                //file format:
                // streamID, streamType, duration, bitRate, msgSize
                if (!this._map.TryGetValue(toks[0], out stream_type))
                {
                    stream_type = Convert.ToInt32(toks[1]);
                    this._map[toks[0]] = stream_type;
                }
                
                if (!this._msgs.TryGetValue(stream_type, out rate_msg))
                {
                    this._msgs[stream_type] = new Dictionary<int, PubSubData>();
                    int sent_msg_sz = Convert.ToInt32(toks[4]);
                    // make msg
                    StringBuilder x_25 = new StringBuilder();
                    x_25.Append('a', (int)(sent_msg_sz * 0.25)/2 - 6);
                    this._msgs[stream_type][25] = new PubSubData(new Name(x_25.ToString()), 25);

                    StringBuilder x_50 = new StringBuilder();
                    x_50.Append('a', (int)(sent_msg_sz * 0.5)/2 - 12);
                    this._msgs[stream_type][50] = new PubSubData(new Name(x_50.ToString()), 50);

                    StringBuilder x_75 = new StringBuilder();
                    x_75.Append('a', (int)(sent_msg_sz * 0.75)/2 - 18);
                    this._msgs[stream_type][75] = new PubSubData(new Name(x_75.ToString()), 75);

                    StringBuilder x_100 = new StringBuilder();
                    x_100.Append('a', sent_msg_sz/2 - 25);
                    this._msgs[stream_type][100] = new PubSubData(new Name(x_100.ToString()), 100);
                }
            }
            file.Close();
        }
        
        #endregion

        #region shrink

        public PubSubData shrink(string stream_id, int new_rate)
        {
            return this._msgs[this._map[stream_id]][new_rate];
        }

        #endregion
    }
}
