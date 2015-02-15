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
using System.Xml.Serialization;
using System.IO;

namespace QS._qss_x_.Scenarios_
{
    public sealed class Scenario_Multicast_ : QS._qss_x_.Simulations_.IScenario_
    {
        #region _Create

        QS._qss_x_.Simulations_.ITask_[] QS._qss_x_.Simulations_.IScenario_._Create(int _nnodes, IDictionary<string, QS.Fx.Reflection.Xml.Parameter> _parameters)
        {
            List<QS._qss_x_.Simulations_.ITask_> _tasks = new List<QS._qss_x_.Simulations_.ITask_>();
            if (_nnodes < 2)
                throw new Exception("Not enough nodes.");
            double _mttb = (double) _parameters["MTTB"].Value;
            double _mttr = (double) _parameters["MTTR"].Value;
            double _mttf = (double) _parameters["MTTF"].Value;
            int _senders = Math.Min((int) _parameters["Senders"].Value, _nnodes - 1);
            int _receivers = _nnodes - 1 - _senders;
            int _messages = (int) _parameters["Messages"].Value;
            double _rate = (double) _parameters["Rate"].Value;
            string _level_0 = _serialize(_parameters["Level_0"]);
            string _level_1 = _serialize(_parameters["Level_1"]);
            string _level_2 = _serialize(_parameters["Level_2"]);
            _tasks.Add(new QS._qss_x_.Simulations_.Task_("server", 0, double.PositiveInfinity, 0, _normalize(_server())));
            for (int _i = 1; _i <= _senders; _i++)
                _tasks.Add(new QS._qss_x_.Simulations_.Task_(
                    ((_senders == 1) ? "sender" : ("sender_" + _i.ToString())), 
                    _mttb, double.PositiveInfinity, 0, _normalize(_client(_level_0, _level_1, _level_2, _messages, _rate))));
            for (int _i = 1; _i <= _receivers; _i++)
                _tasks.Add(new QS._qss_x_.Simulations_.Task_(
                    ((_receivers == 1) ? "receiver" : ("receiver_" + _i.ToString())), _mttb, _mttf, _mttr, _normalize(_client(_level_0, _level_1, _level_2, 0, 0))));
            return _tasks.ToArray();
        }

        #endregion

        #region _normalize

        private string _normalize(string _s)
        {
            QS.Fx.Reflection.Xml.Root _oooo;
            using (StringReader _rrrr = new StringReader(_s.Trim()))
            {
                _oooo = (QS.Fx.Reflection.Xml.Root)(new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Deserialize(_rrrr);
            }
            StringBuilder _ssss = new StringBuilder();
            using (StringWriter _wwww = new StringWriter(_ssss))
            {
                (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Serialize(_wwww, _oooo);
            }
            return _ssss.ToString();
        }

        #endregion

        #region _serialize

        private string _serialize(QS.Fx.Reflection.Xml.Parameter _data)
        {
            StringBuilder _s = new StringBuilder();
            using (StringWriter _writer = new StringWriter(_s))
            {
                (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Parameter))).Serialize(_writer, _data);
            }
            string _o = _s.ToString();
            int _i = _o.IndexOf("<Value", _o.IndexOf(">", _o.IndexOf("<Parameter")));
            int _j = _o.LastIndexOf("</Parameter>");
            return _o.Substring(_i, _j - _i);
        }

        #endregion

        #region _server

        private string _server()
        {
            return
@"
                <?xml version=""1.0"" encoding=""utf-16""?>
                <Root xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                  <Object xsi:type=""ReferenceObject"" xsi:nil=""true"" />
                </Root>
";
        }

        #endregion

        #region _client

        private string _client(string _properties_0, string _properties_1, string _properties_2, int _length, double _rate)
        {
            return
@"
                <?xml version=""1.0"" encoding=""utf-16""?>
                <Root xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">

                  <Object xsi:type=""ReferenceObject"" id=""F01AA89B005A4B119702B6A076816160"">
                    <Attribute id=""9F4C608A9A6D44FFAD8A2FDC662E185B"" value=""Properties Framework Multicast Client"" />
                    <Parameter id=""channel"">
                      <Value xsi:type=""ReferenceObject"" id=""22B987492B164910877120F9E85EE8A6"">
                        <Attribute id=""9F4C608A9A6D44FFAD8A2FDC662E185B"" value=""Properties Framework Multicast Channel"" />
                        <Parameter id=""MessageClass"">
                          <Value xsi:type=""ValueClass"" id=""148B8C6B886A49029CD81610CB75EEE6"" />
                        </Parameter>
                        <Parameter id=""CheckpointClass"">
                          <Value xsi:type=""ValueClass"" id=""148B8C6B886A49029CD81610CB75EEE6"" />
                        </Parameter>
"
                +
                    _delegation
                    (
                        _aggregation
                        (
                            _properties_0,
                            _delegation
                            (
                                _aggregation
                                (
                                    _properties_1,
                                    _delegation
                                    (
                                        _aggregation
                                        (
                                            _properties_2,
                                            _no_delegation()
                                        )
                                    )
                                )
                            )
                        )
                    )
                +
@"
                        <Parameter id=""batching"">
                          <Value xsi:type=""xsd:double"">0.1</Value>
                        </Parameter>
                        <Parameter id=""debug"">
                          <Value xsi:type=""xsd:boolean"">true</Value>
                        </Parameter>
                      </Value>
                    </Parameter>
                    <Parameter id=""length"">
                      <Value xsi:type=""xsd:int"">
"
                + _length.ToString() +
@"
                      </Value>
                    </Parameter>
                    <Parameter id=""rate"">
                      <Value xsi:type=""xsd:double"">
"
                + _rate.ToString() + 
@"
                      </Value>
                    </Parameter>
                    <Parameter id=""debug"">
                      <Value xsi:type=""xsd:boolean"">true</Value>
                    </Parameter>
                  </Object>
                </Root>
";
        }

        #endregion

        #region _no_delegation

        private string _no_delegation()
        {
            return
@"
                <Parameter id=""identifier"">
                  <Value xsi:type=""Identifier"" xsi:nil=""true""/>
                </Parameter>
                <Parameter id=""delegation"">
                  <Value xsi:type=""ReferenceObject"" xsi:nil=""true"" />
                </Parameter>                                
";
        }

        #endregion

        #region _delegation

        private string _delegation(string _object)
        {
            return
@"
                <Parameter id=""identifier"">
                  <Value xsi:type=""Identifier"" value=""1"" />
                </Parameter>
                <Parameter id=""delegation"">
                  <Value xsi:type=""ReferenceObject"" id=""34BFC487AF954C7FAED21947F36EF364"">
                    <Attribute id=""9F4C608A9A6D44FFAD8A2FDC662E185B"" value=""Properties Framework Delegation Channel (Local)"" />
                    <Parameter id=""IdentifierClass"">
                      <Value xsi:type=""ValueClass"" id=""24D8914967A443BE93FD68B4F3CE82A5"" />
                    </Parameter>
                    <Parameter id=""ObjectClass"">
                      <Value xsi:type=""ObjectClass"" id=""FDD770B290844EB49B3D1B38566D374E"" />
                    </Parameter>
                    <Parameter id=""identifier"">
                      <Value xsi:type=""Identifier"" value=""1"" />
                    </Parameter>
                    <Parameter id=""object"">
"
                + _object +
@"
                    </Parameter>
                    <Parameter id=""debug"">
                      <Value xsi:type=""xsd:boolean"">true</Value>
                    </Parameter>
                  </Value>
                </Parameter>
";
        }

        #endregion

        #region _aggregation

        private string _aggregation(string _properties, string _delegation)
        {
            return
@"
              <Value xsi:type=""ReferenceObject"" id=""2C1D2D7BD5EB4F0080FFD0F8E9C7D7B2"">
                <Attribute id=""9F4C608A9A6D44FFAD8A2FDC662E185B"" value=""Properties Framework Aggregation Component"" />
                <Parameter id=""properties"">
"
                + _properties +
@"
                </Parameter>
"
                + _delegation +
@"
                <Parameter id=""debug"">
                  <Value xsi:type=""xsd:boolean"">true</Value>
                </Parameter>
              </Value>
";
        }

        #endregion
    }
}
