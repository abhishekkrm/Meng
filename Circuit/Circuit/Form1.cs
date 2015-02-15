using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using Isis;

namespace Circuit
{
    [QS.Fx.Reflection.ComponentClass("1`1", "NetworkTopology", "This is a circuit topology")]
    public partial class Form1 : QS.Fx.Component.Classes.UI, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>
    {
        private Label[] labels;
        private PictureBox[] picture_box;
        private int[] size_x;
        private int[] size_y;

        delegate void stringArg(string who, string val);

        public Form1(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channel)
            : base(_mycontext)
        {
            /***** Isis-2 *****/
            
            IsisSystem.Start();
            myGroup = Isis.Group.Lookup("Circuit");
            if (myGroup == null)
                myGroup = new Group("Circuit");

            myGroup.Handlers[UPDATE] += (stringArg)delegate(string name, string val)
            {
                lock (this)
                {
                    string _new_text = val;
                    if (!_new_text.Equals(this._text))
                    {
                        this._text = _new_text;
                        // _Refresh();
                    }

                }
            };
            myGroup.Join();

            

            XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
            xmlDoc.Load("C:\\liveobjects\\libraries\\4D12F33758B74DAFBDE0D17E298AD01E\\1\\Data\\labels.xml"); // Load the XML document from the specified file   

            // Get elements
            XmlNodeList labelss = xmlDoc.GetElementsByTagName("label");

            labels = new Label[labelss.Count];
            picture_box = new PictureBox[labelss.Count];
                        
            size_x = new int[labelss.Count];
            size_y = new int[labelss.Count];

            for (int i = 0; i < labels.Length; i++)
            {
                this.labels[i] = new System.Windows.Forms.Label();
                this.labels[i].Visible = true;
                this.labels[i].AutoSize = true;
                this.labels[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.labels[i].Location = new System.Drawing.Point(
                    Convert.ToInt32(labelss[i].ChildNodes[0].Attributes[0].InnerText),
                    Convert.ToInt32(labelss[i].ChildNodes[1].Attributes[0].InnerText));
                size_x[i] = Convert.ToInt32(labelss[i].ChildNodes[0].Attributes[0].InnerText);
                size_y[i] = Convert.ToInt32(labelss[i].ChildNodes[1].Attributes[0].InnerText);
                this.labels[i].Name = "label" + (i + 1);
                this.labels[i].TabIndex = (i + 1);
                this.labels[i].Text = "label" + (i + 1);
                this.labels[i].Size = new System.Drawing.Size(2, 2);
                this.Controls.Add(this.labels[i]);
                this.labels[i].Text = new Random().Next(0, 10 * (i + 1)) + "";
                this.labels[i].ForeColor = Color.Black;
                this.labels[i].BackColor = Color.White;


                /*this.picture_box[i] = new System.Windows.Forms.PictureBox();
                this.picture_box[i].Image = new Bitmap("C:\\liveobjects\\libraries\\4D12F33758B74DAFBDE0D17E298AD01E\\1\\Data\\reddot.png");
                this.picture_box[i].Location = new Point(
                    Convert.ToInt32(labelss[i].ChildNodes[0].Attributes[0].InnerText),
                    Convert.ToInt32(labelss[i].ChildNodes[1].Attributes[0].InnerText));
                this.picture_box[i].Size = new System.Drawing.Size(13, 13);
                this.Controls.Add(this.picture_box[i]);
                this.picture_box[i].Visible = false;*/

            }

            InitializeComponent();

            this._channelendpoint_circuit = _mycontext.DualInterface<
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

                this._channelconnection_circuit = ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint_circuit).Connect(_otherendpoint);
            }
           
        }

        private void mouseDoubleClickPBox(object sender, MouseEventArgs e) 
        {
            myGroup.Send(UPDATE, "Test", "newf" + "C:\\liveobjects\\examples\\SmartGrid\\objects\\Graph.liveobject");
        }

        private string _text = string.Empty;
        private bool _editing;
        private DateTime _lastchanged;
        private System.Threading.Timer _timer;

        private bool primary = false;
        private int normal = 0;
        private Random rnd;
        Group myGroup;
        int UPDATE = 1;

        private const int EditingTimeoutInMilliseconds1 = 1000;
        private const int EditingTimeoutInMilliseconds2 = 100;
        private KeyPressEventArgs e;

        [QS.Fx.Base.Inspectable("channelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channelendpoint_circuit;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _channelconnection_circuit;

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
            for (int i = 0; i < labels.Length; i++)
            {
                this.labels[i].Left = size_x[i] * control.Size.Width / 804;
                this.labels[i].Top = size_y[i] * control.Size.Height / 498;
                try
                {
                    float size = Math.Min(control.Size.Width, control.Size.Height) * 0.03F;
                    this.labels[i].Font = new System.Drawing.Font("Microsoft Sans Serif", size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }
                catch (Exception d)
                {
                }
            }
        }

        QS.Fx.Value.Classes.IText QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Checkpoint()
        {
            return new QS.Fx.Value.UnicodeText(this._text);
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Initialize(QS.Fx.Value.Classes.IText _checkpoint)
        {
            lock (this)
            {
                string _new_text = (_checkpoint != null) ? _checkpoint.Text : null;
                if (_new_text == null)
                {
                    _new_text = string.Empty;
                    primary = true;
                    rnd = new Random();
                }
                if (!_new_text.Equals(this._text))
                {
                    this._text = _new_text;
                }
                //MessageBox.Show("Calling refresh");
                _Refresh();
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Receive(QS.Fx.Value.Classes.IText _message)
        {
           /* lock (this)
            {
                string _new_text = _message.Text;
                if (!_new_text.Equals(this._text))
                {
                    this._text = _new_text;
                    _Refresh();
                }
            }*/
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
                        string[] values;
                        int i = 0;

                        if (primary)
                        {
                            //MessageBox.Show("primary");
                            this._text = "";
                            this._text += normal;
                            for (i = 0; i < (labels.Length); i++)
                            {
                                if ((i == 17 || i == 21) && normal == 1)
                                {
                                    this._text += "," + rnd.Next(8,13);
                                }
                                else
                                    this._text += "," + rnd.Next(108, 113);
                            }
                            //_channelendpoint_circuit.Interface.Send(new QS.Fx.Value.UnicodeText(this._text));
                            /******** Isis-2 ******/
                            myGroup.Send(UPDATE, "Test", this._text);
                            /**********************/
                           // MessageBox.Show("Update sent");
                        }
                        System.Threading.Thread.Sleep(200);
                        values = this._text.Split(',');
                       // MessageBox.Show("text"  + this._text);
                        if (values[0].Equals("0"))
                        {
                            this.pictureBox1.Image = Image.FromFile("C:\\liveobjects\\libraries\\4D12F33758B74DAFBDE0D17E298AD01E\\1\\Data\\without_labels.png");
                        }
                        else if (values[0].Equals("1"))
                        {
                            this.pictureBox1.Image = Image.FromFile("C:\\liveobjects\\libraries\\4D12F33758B74DAFBDE0D17E298AD01E\\1\\Data\\red_labels.png");
                        }
                        else if (values[0].Equals("2"))
                        {
                            this.pictureBox1.Image = Image.FromFile("C:\\liveobjects\\libraries\\4D12F33758B74DAFBDE0D17E298AD01E\\1\\Data\\green_labels.png");
                        }
                        for (i = 0; i < labels.Length; i++)
                        {
                            this.labels[i].Text = values[i+1];
                        }
                        //MessageBox.Show("image and values set");
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

        private void timerRealTimeData_Tick(object sender, System.EventArgs e)
        {
            if (normal == 0)
            {
                normal = 1;
                this.timerRealTimeData.Interval = 60000;
            }
            else if (normal == 1)
            {
                normal = 2;
                this.timerRealTimeData.Interval = 30000;
            }
            else if (normal == 2)
            {
                normal = 0;
                this.timerRealTimeData.Interval = 30000;
            }
        }
    }
}
