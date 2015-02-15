using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Net.Sockets;

using Isis;

namespace ConsoleApplication1
{
    [QS.Fx.Reflection.ComponentClass("1`1", "Graph", "A simple plain graph with a graphical user interface.")]
    public partial class Chart : QS.Fx.Component.Classes.UI, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>
    {
        private Random random = new Random();
        private int pointIndex = 0;

        delegate void stringArg(string who, string val);

        public Chart(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channel)
            : base(_mycontext)
        {

            /***** Isis-2 *****/
            IsisSystem.Start();
            myGroup = Isis.Group.Lookup("Graph");
            if (myGroup == null)
                myGroup = new Group("Graph");

            myGroup.Handlers[UPDATE] += (stringArg)delegate(string name, string val)
            {
                lock (this)
                {
                    string _new_text = val;
                    if (!_new_text.Equals(this._text))
                    {
                        this._text = _new_text;
                    }

                }
            };

            myGroup.Join();

            InitializeComponent();
            
            this._channelendpoint_graph = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>(this);

            if (_channel != null)
            {
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText> _channelproxy =
                    _channel.Dereference(_mycontext);

                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
                        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                            _otherendpoint =
                                _channelproxy.Channel;

                this._channelconnection_graph = ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint_graph).Connect(_otherendpoint);
            }     
        }

        [QS.Fx.Base.Inspectable("channelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channelendpoint_graph;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _channelconnection_graph;

        private string _text = string.Empty;
        private bool _editing;
        private DateTime _lastchanged;
        private System.Threading.Timer _timer;

        Group myGroup;
        int UPDATE = 1;
        TcpClient client;
        Stream s;
        StreamReader sr = null;
        string ip = "127.0.0.1";

        private const int EditingTimeoutInMilliseconds1 = 1000;
        private const int EditingTimeoutInMilliseconds2 = 100;
        private KeyPressEventArgs e;

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Initialize(
            QS.Fx.Value.Classes.IText _checkpoint)
        {
            lock (this)
            {
                
                string _new_text = (_checkpoint != null) ? _checkpoint.Text : null;
                if (_new_text == null)
                {
                    _new_text = string.Empty;
                    client = new TcpClient(ip, 9090);
                    s = client.GetStream();
                    sr = new StreamReader(s);
                }
                if (!_new_text.Equals(this._text))
                {
                    this._text = _new_text;
                      
                }
                _Refresh();  
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Receive(
            QS.Fx.Value.Classes.IText _message)
        {
            /*lock (this)
            {
                string _new_text = _message.Text;
                if (!_new_text.Equals(this._text))
                {
                    this._text = _new_text;
                    _Refresh();
                }
            }*/
        }

        QS.Fx.Value.Classes.IText
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Checkpoint()
        {
            return new QS.Fx.Value.UnicodeText(this._text);
        }

        private void _Refresh()
        {
            lock (this)
            {
                if (!_editing)
                {
                    if (InvokeRequired)
                        BeginInvoke(new QS.Fx.Base.Callback(this._Refresh));
                    else
                    {
                        if (sr != null)
                        {
                            this._text = sr.ReadLine();

                            //_channelendpoint_graph.Interface.Send(new QS.Fx.Value.UnicodeText(_text));
                            /******** Isis-2 ******/
                            myGroup.Send(UPDATE, "Test", this._text);
                            /**********************/
                        }
                        int numberOfPointsInChart = 100;
                        int numberOfPointsAfterRemoval = 90;
                        String[] parts;
                        String[] pparts;

                        int x;
                        double y;
                        parts = this._text.Split(',');
                        /*if (this._text.Equals("") || this._text.Equals(String.Empty))
                        {
                            return;
                        }*/
                        foreach (String point in parts)
                        {
                            try
                            {
                                pparts = point.Split(' ');
                                x = Convert.ToInt32(pparts[0]);
                                y = Convert.ToDouble(pparts[1]);
                                chart1.Series[0].Points.AddXY(x, y);
                                pointIndex = x + 1;
                            }
                            catch (Exception d)
                            {
                            }
                        }
                        
                        chart1.ResetAutoValues();

                        // Keep a constant number of points by removing them from the left
                        while (chart1.Series[0].Points.Count > numberOfPointsInChart)
                        {
                            // Remove data points on the left side
                            while (chart1.Series[0].Points.Count > numberOfPointsAfterRemoval)
                            {
                                chart1.Series[0].Points.RemoveAt(0);
                            }

                            // Adjust X axis scale
                            chart1.ChartAreas["Default"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                            chart1.ChartAreas["Default"].AxisX.Maximum = chart1.ChartAreas["Default"].AxisX.Minimum + numberOfPointsInChart;
                        }

                        // Invalidate chart
                        chart1.Invalidate();
                        _textbox_KeyPress(this, e);
                    }
                }
            }
        }

        private void _textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            lock (this)
            {
                if (_editing)
                    _lastchanged = DateTime.Now;
                else
                {
                    _editing = true;
                    _lastchanged = DateTime.Now;
                    _timer = new System.Threading.Timer(new System.Threading.TimerCallback(this._TimerCallback), null,
                        EditingTimeoutInMilliseconds1, System.Threading.Timeout.Infinite);
                }
            }
        }

        private void _TimerCallback(object o)
        {
            lock (this)
            {
                if (_editing)
                {
                    int _remaining_milliseconds =
                        EditingTimeoutInMilliseconds1 - ((int)Math.Floor((DateTime.Now - _lastchanged).TotalMilliseconds));
                    if (_remaining_milliseconds > EditingTimeoutInMilliseconds2)
                        _timer = new System.Threading.Timer(new System.Threading.TimerCallback(this._TimerCallback), null,
                            _remaining_milliseconds, System.Threading.Timeout.Infinite);
                    else
                    {
                        if (InvokeRequired)
                            BeginInvoke(new System.Threading.TimerCallback(this._TimerCallback), new object[] { null });
                        else
                        {
                            _editing = false;
                            string _text = this._text;
                            this.Log(_text);
                        }
                    }
                }
                _Refresh();
            }
        }
    }
}
