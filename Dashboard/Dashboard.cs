using Isis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Windows.Forms.DataVisualization.Charting;


namespace Dashboard
{
    [QS.Fx.Reflection.ComponentClass("1`1", "Dashboard", "This is a dashboard for Operator's console")]
    public partial class Dashboard  : QS.Fx.Component.Classes.UI, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>
    {
        //Required Data structures
        private Dictionary<String, String> graphBusListMap;
        private Dictionary<String, PictureBox> generatorControlMap;
        private Dictionary<String, List<LineControl>> lineControlMap;
        private Dictionary<String, BusControl> busControlMap;
        private Dictionary<String, LineLabelControl> lineDownControlMap;
        private String previousAffectListString = String.Empty;
        private String busDetailsString = String.Empty;
        private HashSet<String> lineDownSet;
        
        //The panels that comprise the dashboard
        private Panel circuitPanel;
        private Panel appsPanel;
        private Panel toolsPanel;
        private Panel graphPanel;
        private Chart voltageChart;
        private BusControl shownBus = null;
        private LineControlButton[,] buttons = null;
         
        //Initialize the timers
        private Timer timerRealTimeData = new Timer();
        internal Timer animationTimer = new Timer();
        internal System.Threading.Timer updateThreadingTimer = null;
        
        internal static Size initSize;
        internal int lineCounter = 0;
        delegate void StringArg(string who, string val);

        public Dashboard(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channel)
            : base(_mycontext)
        {
            /***** Isis-2 *****/
            IsisSystem.Start();
            //myGroup = Isis.Group.Lookup("Dashboard");
            //if (myGroup == null)
                myGroup = new Group("Dashboard");
            

            //Handler that receives and propogates the changes in the circuit to all the connected clients using ISIS 
            myGroup.Handlers[UPDATE] += (StringArg)delegate(string name, string val)
            {
                lock (this)
                {
                    switch (name.Trim())
                    {
                        case "BusDetailsPropagate": 
                              // To propogate the details of the bus to all the connected clients
                              UpdateBusControlMap(val);
                              break;
                        case "AffectedListPropagate":
                              // To propogate the list of affected/lines with warning to all the connected clients
                              UpdateAffectedLines(val); 
                              break;
                        case "LineDownPropagate":
                            // Propogate the details of the line that was just pulled down by an operator
                            UpdateLineDown(val);
                            break;
                        case "LineUpPropagate":
                            // Propogate the details of the line that was just set right and brought up by an operator
                            UpdateLineUp(val);
                            break;                       
                    }
                }
            };

            //Create the necessary data structures to keep track of the buses, lines etc
            CreateRequiredDataStructures();

            //Initialize all the components of the Dashboard
            InitializeBusInfoPanel();
            InitializeCircuitPanel();

            InitializeAppsPanel();
            CreateCircuitFromXML();

            //Adding apps to the Dashboard
            AddAppsToPanel();

            SetSizeValuesForPanels();
            ActivateTimers();
            
            this.BackColor = Color.Black; // Sets the background color for the main panel
            this.Controls.Add(this.circuitPanel); // add the circuit panel to the Dashboard
            this.Resize += Dashboard_Resize;
            
            // Join the group to be able receive messages from ISIS for the group
            myGroup.Join();
            
            _Refresh();


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

        /*
         * Initialize timers
         */
        private void ActivateTimers()
        {
            animationTimer.Tick += new EventHandler(timer1_Tick); // Everytime timer ticks, timer_Tick will be called
            animationTimer.Interval = (1) * (1);  // Timer will tick every milliseconds
            animationTimer.Enabled = false; // Disable the timer for now 

            // Set Circuit Timer
            updateThreadingTimer = new System.Threading.Timer(new System.Threading.TimerCallback(this.ThreadingTimer_Tick), null,
                           5000, System.Threading.Timeout.Infinite);
        }

        /*
         *Set size settings for the panels
         */
        private void SetSizeValuesForPanels()
        {
            busInfoPanel.MinimumSize = Constants.MINIMUM_PANEL_SIZE;
            graphPanel.MinimumSize = Constants.MINIMUM_PANEL_SIZE;
            initSize = busInfoPanel.Size;
        }

        /*
         *Create the circuit dynamically from Component.xml file 
         */
        private void CreateCircuitFromXML()
        {
            XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
            xmlDoc.Load("C:\\liveobjects\\libraries\\4D12F33758B74DAFBDE0D17E298AD01E\\1\\Data\\Component.xml");

            // Read elements from the Components.xml file to create the circuit dynamically
            XmlNodeList components = xmlDoc.GetElementsByTagName("component");
            float xSizeFactor = Constants.X_SIZE_FACTOR;
            float ySizeFactor = Constants.Y_SIZE_FACTOR;

            String previousId = "";

            for (int i = 0; i < components.Count; i++)
            {
                Control control = null;
                if (Convert.ToString(components[i].Attributes[1].Value).Equals("Generator"))
                {
                    //adding a generator
                    String id = components[i].Attributes[0].Value;
                    int x = Convert.ToInt32(components[i].ChildNodes[0].Attributes[0].InnerText);
                    int y = Convert.ToInt32(components[i].ChildNodes[1].Attributes[0].InnerText);
                    int rotation = Convert.ToInt32(components[i].ChildNodes[2].Attributes[0].InnerText);
                    control = AddGeneratorControl(id, x * xSizeFactor, y * ySizeFactor, rotation);

                    //Seemingly the code below is not required..Commenting it for now --gaurav
                    //Add Bus IDs of the generator
                    //AddGeneratorBusControl(components[i].ChildNodes[3].ChildNodes[0].Attributes[0].Value);
                    //AddGeneratorBusControl(components[i].ChildNodes[3].ChildNodes[1].Attributes[0].Value);
                }
                else if (Convert.ToString(components[i].Attributes[1].Value).Equals("Bus"))
                {
                    //adding a bus
                    String id = components[i].Attributes[0].Value;
                    int x1 = Convert.ToInt32(components[i].ChildNodes[0].Attributes[0].InnerText);
                    int y1 = Convert.ToInt32(components[i].ChildNodes[1].Attributes[0].InnerText);
                    int x2 = Convert.ToInt32(components[i].ChildNodes[2].Attributes[0].InnerText);
                    int y2 = Convert.ToInt32(components[i].ChildNodes[3].Attributes[0].InnerText);
                    bool isArrow = Convert.ToString(components[i].ChildNodes[4].Attributes[0].InnerText).Equals("line-arrow");
                    control = AddBusControl(id, x1 * xSizeFactor, y1 * ySizeFactor, x2 * xSizeFactor, y2 * ySizeFactor, isArrow);
                    control.Click += Bus_Click;
                    control.MouseHover += Bus_Hover;
                    control.MouseLeave += Bus_Leave;

                    float xLabelLoc = x1 - 15;
                    float yLabelLoc = y1;
                    LineLabelControl busLabel = AddLineLabelControl(id, id, xLabelLoc, yLabelLoc);
                    AddControl(busLabel); //Add Label to the panel
                }
                else if (Convert.ToString(components[i].Attributes[1].Value).Equals("Line"))
                {
                    //adding a line between buses
                    String id = components[i].ChildNodes[4].Attributes[0].Value;
                    if (previousId != id)
                        lineCounter++;

                    int x1 = Convert.ToInt32(components[i].ChildNodes[0].Attributes[0].InnerText);
                    int y1 = Convert.ToInt32(components[i].ChildNodes[1].Attributes[0].InnerText);
                    int x2 = Convert.ToInt32(components[i].ChildNodes[2].Attributes[0].InnerText);
                    int y2 = Convert.ToInt32(components[i].ChildNodes[3].Attributes[0].InnerText);
                    control = AddLineControl(id, x1 * xSizeFactor, y1 * ySizeFactor, x2 * xSizeFactor, y2 * ySizeFactor, lineCounter.ToString());

                    previousId = id;
                }
                if (control != null)
                    this.circuitPanel.Controls.Add(control);
            }
        }

        /*
         * Initialize the panel that contains the circuit
         * */
        private void InitializeCircuitPanel()
        {
            circuitPanel = new Panel();
            circuitPanel.Size = new Size(1000, 744);
            //circuitPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            circuitPanel.BackColor = Color.Black;
            circuitPanel.Location = new Point(300,100);
            circuitPanel.AutoScroll=true;
        }

        /*
         * Initialize all the data structures required to control the buses, lines etc
         * */
        private void CreateRequiredDataStructures()
        {
            generatorControlMap = new Dictionary<String, PictureBox>();
            lineControlMap = new Dictionary<String, List<LineControl>>();
            busControlMap = new Dictionary<String, BusControl>();
            lineDownControlMap = new Dictionary<string, LineLabelControl>();
            lineDownSet = new HashSet<string>();
            graphBusListMap = new Dictionary<string, string>();
        }

        /*
         * Adding applications to the dashboard
         * First create button using creatLineDownApplication
         * Then create event handlers for the button
         * Then create an application that will fit into the panel on the right hand side of the circuit
         * */
        private void AddAppsToPanel()
        {
            //Line down application
            CreateLineDownApplication();

            //Graph and Info Panel Application
            CreateGraphInfoPanelApplication();
        }

        /*
         *Create the line down application that allows the operators to bring a line down or up based on the 
         *voltage phase angle
         */
        private void CreateLineDownApplication()
        {
            //Create button to launch application
            Button lineDownAppButton  =CreateButtonForApplication("LineDown", "Line Down");
            lineDownAppButton.Click += LineDownAppButton_Click;

            //Create the application
            InitializeToolsPanel();
        }

        /*
         *Create the graph panel and info panel that gives the information about a particular bus and the voltage values 
         *for the bus. Click on any of the buses to see the information corresponding to that particular bus
         */
        private void CreateGraphInfoPanelApplication()
        {
            //Create the button to launch the application
            Button graphButton = CreateButtonForApplication("Graph", "Graph");
            graphButton.Click += AppButton_Graph_Click; // Trigger a custom event to set the values of the graph upon click

            //Create the application
            InitializeGraphPanel();
        }

        /*
         *Create a button for an application 
         */
        private Button CreateButtonForApplication(String appName, String label)
        {
            Button appButton = new Button();
            appButton.BackColor = Constants.APP_BUTTON_COLOR;
            appButton.ForeColor = Constants.APP_BUTTON_FORE_COLOR;
            appButton.Text = label;
            appButton.Name = appName;
            appButton.Size = Constants.APP_BUTTON_SIZE;
            appButton.Location = new Point(5, 30 + 50 * appsPanel.Controls.Count);
            appsPanel.Controls.Add(appButton);
            return appButton;
        }

        /*
         *Show or Hide the graph panel when clicked on 'Graph' button 
         */
        private void AppButton_Graph_Click(object sender, EventArgs e)
        {
            toolsPanel.Visible = !toolsPanel.Visible;
            graphPanel.Visible = !graphPanel.Visible;
            busInfoPanel.Visible = !busInfoPanel.Visible;
            if (!busInfoPanel.Visible)
            {
                busInfoPanel.Hide();
                graphPanel.Hide();
            }
            else
            {
                //Initialize the values corresponding to the first bus in the circuit
                BusControl busControl = busControlMap["1"] ;
                if (busControl != null)
                {
                    if (!String.IsNullOrEmpty(busControl.getBusName()))
                    {
                        setPanelValues(busControl);
                        setGraphValues(busControl);
                    }
                }
                busInfoPanel.Show();
                graphPanel.Show();
            }       
        }


        /*
         *Event handler to open the tools panel that allows the operators to control the lines in the circuit
         *with buttons
         */
        private void LineDownAppButton_Click(object sender, EventArgs e)
        {
            toolsPanel.Visible = !toolsPanel.Visible;
            graphPanel.Visible = !graphPanel.Visible;
            busInfoPanel.Visible = !busInfoPanel.Visible;
            if (!busInfoPanel.Visible)
            {
                busInfoPanel.Hide();
                graphPanel.Hide();
            }
            else
            {
                busInfoPanel.Show();
                graphPanel.Show();
            }    
        }

        /*
         * Initialize the apps panel that will house the buttons to launch the apps in Dashboard
         */
        private void InitializeAppsPanel()
        {
            appsPanel = new Panel();
            appsPanel.Size = new Size(200, this.Size.Height);
            appsPanel.BackColor = Color.FromArgb(255, 20, 20, 20);
            appsPanel.Dock = DockStyle.Left;
            appsPanel.AutoScroll = true;
            
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(255, 50, 50, 50);
            headerPanel.Size = new Size(200, 30);


            Label header = new Label();
            header.TextAlign = ContentAlignment.MiddleCenter;
            header.Text = "APPS";
            header.ForeColor = Color.White;
            header.Size = new Size(200, 30);
        

            headerPanel.Controls.Add(header);
            appsPanel.Controls.Add(headerPanel);
            this.Controls.Add(appsPanel);   
        }

        /*
         * Initialize the graph panel and a chart that will show the voltage vs time graph for the given bus
         * This panel will be updated at constant time intervals
         */ 
        private void InitializeGraphPanel()
        {
            // Initialize the panel
            graphPanel = new Panel();
            graphPanel.Size = new Size(280, 280);
            graphPanel.BackColor = Color.Black;
            graphPanel.ForeColor = Color.Green;

            graphPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            graphPanel.Dock = DockStyle.Right;
            this.Controls.Add(graphPanel);

            this.Controls.Remove(busInfoPanel);
            this.graphPanel.Controls.Add(busInfoPanel);

            graphPanel.Hide();

            //Create a chart
            voltageChart = new Chart();
            voltageChart.Dock = DockStyle.Bottom;
            graphPanel.Controls.Add(voltageChart);
            voltageChart.Series.Add("Series1");
            voltageChart.BackColor = System.Drawing.Color.Black;
            voltageChart.ForeColor = System.Drawing.Color.Green;
            voltageChart.BackSecondaryColor = System.Drawing.Color.White;
            voltageChart.Series["Series1"].ChartType = SeriesChartType.Line;
            String[] voltageArray = {"1","2","3","4","5"};// random values

            for (int i = 0; i < voltageArray.Length; i++)
            {
                voltageChart.Series["Series1"].Points.AddXY(Convert.ToDouble(i + 1), Convert.ToDouble(voltageArray[i]));
            }
            // Adjust X axis scale
            voltageChart.ChartAreas.Add("chart");
            voltageChart.ChartAreas["chart"].AxisX.Minimum = 0;
            voltageChart.ChartAreas["chart"].AxisX.Maximum = 10;
            voltageChart.Show();                       
        }

        /*
         * Initialize the tools panel with N buttons where N is equal to the number of lines in the given circuit
         * Each button corresponds to a line and is numbered accordingly
         * Operators can use the buttons to control the lines.
         * Also, the color and label of each button is similar to the color and number of the lines that it controls
         */ 
        private void InitializeToolsPanel()
        {
            toolsPanel = new Panel();
            toolsPanel.Size = new Size(300, this.Size.Height); //Height equal to height of parent window
            toolsPanel.BackColor = Color.Black;
            toolsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            toolsPanel.Dock = DockStyle.Right; //attached to the right of the parent window
            int numLines = 60;
            buttons = new LineControlButton[numLines / 5 + 1, 5]; // 5 buttons per row
            int i = 0;
            int j = 0;
            //For each line in the circuit, add a corresponding control button
            foreach (KeyValuePair<string, List<LineControl>> pair in this.lineControlMap)
	        {
                LineControl lineControl = null;
                if (pair.Value.Count > 1)
                    lineControl = pair.Value[1];
                else
                    lineControl = pair.Value[0];
                String name = lineControl.getName();
                String id = lineControl.getId();
                i = (Convert.ToInt32(name) - 1) / 5;
                j = (Convert.ToInt32(name) - 1) % 5;

                buttons[i, j] = new LineControlButton(name, id);
                buttons[i, j].Name = name;
                buttons[i, j].ID = id;
                buttons[i, j].MouseEnter += LineControlButton_MouseEnter;
                buttons[i, j].MouseLeave += LineControlButton_MouseLeave;
    
                //Set the location of the buttons    
                Point parentPos = toolsPanel.Location;
                buttons[i, j].Location = new Point(j  * buttons[i,j].Size.Width + 15, i  * buttons[i,j].Size.Height + 15);
                toolsPanel.Controls.Add(buttons[i, j]);
            } 
            this.Controls.Add(toolsPanel);                  
        }
        
        private void LineControlButton_MouseEnter(object sender, EventArgs e)
        {
            LineControlButton lineControlButton = sender as LineControlButton;
            if (lineControlButton != null)
            {
                foreach(LineControl lineControl in this.lineControlMap[lineControlButton.ID])
                {
                    lineControl.highlightControl(true);
                }
            }
        }

        private void LineControlButton_MouseLeave(object sender, EventArgs e)
        {
            LineControlButton lineControlButton = sender as LineControlButton;
            if (lineControlButton != null)
            {
                foreach (LineControl lineControl in this.lineControlMap[lineControlButton.ID])
                {
                    lineControl.highlightControl(false);
                }
            }
        }

        delegate void AddToolsPanelControl_LineDown_Callback(LineControl lineControl);

        private void AddToolsPanelControl_LineDown(LineControl lineControl)
        {

            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                AddToolsPanelControl_LineDown_Callback d = new AddToolsPanelControl_LineDown_Callback(AddToolsPanelControl_LineDown);
                this.Invoke(d, new object[] { lineControl });
            }
            else
            {
                String name = lineControl.getName();
                int i = (Convert.ToInt32(name) - 1) / 5;
                int j = (Convert.ToInt32(name) - 1) % 5;
                if (buttons[i, j] != null)
                {
                    buttons[i, j].BackColor = Constants.WARNING_LINE_COLOR;
                    buttons[i, j].State = 1;
                    Control control = buttons[i, j];
                    control.Click += LineDownButton_Click;
                }
            }
        }


        /*Click on any of the 'Yellow' buttons to pull the corresponding line down.
         *The button as well as the line just brought down will then be 'Red'
         *No updates will take place on these line unless, the operator explicitely
         *brings the line back up
         */
        private void LineDownButton_Click(object sender, EventArgs e)
        {
            LineControlButton button = sender as LineControlButton;
            if (button != null)
            {
                //check the status of the button and ignore the event if the button is not in 'WARNING' state
                if (button.State != Constants.BUTTON_STATE_WARNING)
                    return;

                String lineControlId = button.ID;
                if (lineControlId != null)
                {

                    String lineDownSetCommaSeperated = lineControlId;
                    foreach (String lineDownID in lineDownSet)
                        lineDownSetCommaSeperated += "_" + lineDownID;
                    lineDownSetCommaSeperated = lineDownSetCommaSeperated.TrimStart('_');
                    /*
                     * Use Isis OrderedSend to propogate the line down event
                     * Use OrderedSend so that the events are reached in the order they were generated in
                     */
                    myGroup.OrderedSend(UPDATE, "LineDownPropagate", lineDownSetCommaSeperated);
                }
            }
        }

        delegate void AddToolsPanelControl_LineUp_Callback(LineControl lineControl);

        private void AddToolsPanelControl_LineUp(LineControl lineControl)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                AddToolsPanelControl_LineUp_Callback d = new AddToolsPanelControl_LineUp_Callback(AddToolsPanelControl_LineUp);
                this.Invoke(d, new object[] { lineControl });
            }
            else
            {
                String name = lineControl.getName();
                //Find the button that needs to be updated
                int i = (Convert.ToInt32(name) - 1) / 5;
                int j = (Convert.ToInt32(name) - 1) % 5;
                if (buttons[i, j] != null)
                {
                    //Change the status of the buttons and lines
                    buttons[i, j].BackColor = Constants.LINE_DOWN_COLOR;
                    buttons[i, j].State = Constants.BUTTON_STATE_DOWN;
                    buttons[i, j].Click += LineUpButton_Click;
                }
            }
        }

        /*
         * Event triggered when a 'Red' button is clicked to bring it back up
         */ 
        private void LineUpButton_Click(object sender, EventArgs e)
        {
            LineControlButton button = sender as LineControlButton;
             if (button != null)
             {
                 //If the button is in a different state other than 'Down', do nothing
                 if (button.State != Constants.BUTTON_STATE_DOWN)
                     return;

                 String lineControlId = button.ID;
                 if (lineControlId != null)
                 {
                     /*
                      * Use Isis OrderedSend to propogate the line up event
                      * Use OrderedSend so that the events are reached in the order they were generated in
                      */
                     myGroup.OrderedSend(UPDATE, "LineUpPropagate", lineControlId);
                 }
             }
        }

        /*
         * Update the bus control map received by Ordered send of BusDetailsPropogate
         */
        private void UpdateBusControlMap(String busDetailsStr)
        {
            String[] busListExcludingFirst = null;
            String busList = null;
            String[] oneBusDetail = busDetailsStr.Split(Constants.SEPARATOR_DELIMITER.ToCharArray());//Split by comma to get details
            int len = oneBusDetail.Length;
            try
            {
                for (int i = 0; i < len; i++)
                {
                    String[] dictionaryKey = oneBusDetail[i].Split(Constants.COMMA_DELIMITER.ToCharArray());
                    BusControl newBusControl = this.busControlMap[dictionaryKey[0]];
                    newBusControl.setBusDetails(oneBusDetail[i]);

                    if (this.graphBusListMap.ContainsKey(dictionaryKey[0]))
                    {
                        busList = this.graphBusListMap[dictionaryKey[0]];
                        busList = String.Concat(busList, ",", dictionaryKey[5]);
                        busListExcludingFirst = busList.Split(Constants.COMMA_DELIMITER.ToCharArray());
                        if (busListExcludingFirst.Length > 10)
                        {
                            busListExcludingFirst = busList.Split(Constants.COMMA_DELIMITER.ToCharArray(), 2, StringSplitOptions.None);
                            busList = busListExcludingFirst[1];
                        }
                        this.graphBusListMap[dictionaryKey[0]] = busList;
                    }
                    else
                    {
                        busList = dictionaryKey[5];
                        this.graphBusListMap[dictionaryKey[0]] = busList;
                    }
                }
                if (busInfoPanel.Visible == true && graphPanel.Visible == true)
                {
                    //If the busInfoPanel and graphPanel are visible, then update the details
                    if (shownBus != null)
                    {
                        setPanelValues(shownBus);
                        setGraphValues(shownBus);
                        refreshPanels();
                    }
                }
            }catch(Exception e)
            {
                    MessageBox.Show("Update Bus Control Map :Exception");
            }

        }

        /*
         * Timer that in intervals OrderSends the affected list of lines and bus details 
         */
        private void ThreadingTimer_Tick(object state)
        {          
            lock (this)
            {
                String busDetailsPlusAffectedStr = GetServerResponseForOperation(Constants.OPERATION_AFFECTEDLIST);
                String[] busDetailsPlusAffectedSplitStr = busDetailsPlusAffectedStr.Split(Constants.SEPARATOR_COLON_FOR_SERVER_DATA.ToCharArray());    //bus details: affected list
                                          
                if(busDetailsPlusAffectedSplitStr.Length >= 2)
                {
                    if (!String.IsNullOrEmpty(busDetailsPlusAffectedSplitStr[1]))
                        myGroup.OrderedSend(UPDATE, "AffectedListPropagate", busDetailsPlusAffectedSplitStr[1]);

                    if (!String.IsNullOrEmpty(busDetailsPlusAffectedSplitStr[0]))
                        myGroup.OrderedSend(UPDATE, "BusDetailsPropagate", busDetailsPlusAffectedSplitStr[0]);    

                    String lineDownSetCommaSeperated = null;
                        foreach (String lineDownID in lineDownSet)
                            lineDownSetCommaSeperated += "_" + lineDownID;
                        if (lineDownSetCommaSeperated != null)
                        {
                            lineDownSetCommaSeperated = lineDownSetCommaSeperated.TrimStart('_');
                            myGroup.OrderedSend(UPDATE, "LineDownPropagate", lineDownSetCommaSeperated);
                        }

                    previousAffectListString = busDetailsPlusAffectedSplitStr[1];
                }
                updateThreadingTimer = new System.Threading.Timer(new System.Threading.TimerCallback(this.ThreadingTimer_Tick), null,
                           Constants.UPDATE_CIRCUIT_INTERVAL, System.Threading.Timeout.Infinite);
            }                     
        }       

        /* Update the affected lines : the lines whose state have been changed due to voltage phase angle differences
         */
        private void UpdateAffectedLines(string affectedListString)
        {
            HashSet<String> lineDownButtonAddedSet = new HashSet<string>();
            String[] affectedLines = affectedListString.Split(Constants.SEPARATOR_DELIMITER.ToCharArray());
            String[] previousAffectedLines = previousAffectListString.Split(Constants.SEPARATOR_DELIMITER.ToCharArray());
           
            foreach (KeyValuePair<string, List<LineControl>> pair in this.lineControlMap)
	        {
                foreach (LineControl lineControl in pair.Value)
                    if (!lineDownSet.Contains(lineControl.getId()))
                    {
                        lineControl.UpdateLine(Constants.NORMAL_LINE_COLOR);
                       
                        int i = (Convert.ToInt32(lineControl.getName()) - 1) / 5;
                        int j = (Convert.ToInt32(lineControl.getName()) - 1) % 5;
                        if (buttons[i, j] != null)
                        {
                            buttons[i, j].BackColor = Constants.NORMAL_LINE_COLOR;
                            buttons[i, j].State = 0;
                            buttons[i, j].Click -= LineDownButton_Click;
                            buttons[i, j].Click -= LineUpButton_Click;
                        }
                    }
            }

            // Iterate for each affected lines
            foreach(String lineId in affectedLines)
            {
                if (this.lineControlMap.ContainsKey(lineId.Replace("\r\n", "")))
                {
                    int lineCounter = 0;
                    foreach (LineControl lineControl in lineControlMap[lineId.Replace("\r\n", "")])
                    {
                        if (lineControl.getName() == "26")
                            continue;
                        if (!lineDownSet.Contains(lineControl.getId()))
                        {
                            if (!lineDownButtonAddedSet.Contains(lineControl.getId()))
                            {
                                if (lineControlMap[lineId.Replace("\r\n", "")].Count > 1)
                                    AddToolsPanelControl_LineDown(lineControlMap[lineId.Replace("\r\n", "")][1]);
                                else
                                    AddToolsPanelControl_LineDown(lineControlMap[lineId.Replace("\r\n", "")][0]);


                                lineDownButtonAddedSet.Add(lineControl.getId());
                                
                            }
                            lineControl.UpdateLine(Constants.WARNING_LINE_COLOR);
                            lineCounter++;
                        }
                    }
                }                    
            }            
        }

        // This delegate enables asynchronous calls for adding
        // the control in Form
        delegate void AddControlCallback(LineLabelControl control);

        private void AddControl(LineLabelControl control)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                AddControlCallback d = new AddControlCallback(AddControl);
                this.Invoke(d, new object[] { control });
            }
            else
            {
                this.circuitPanel.Controls.Add(control);             
                control.BringToFront();
            }
        }

        // this is to update the line down color
        private void UpdateLineDown(String lineDownIdValues)
        {
            String[] lineDownValues = lineDownIdValues.Split('_');
            foreach (String lineDownId in lineDownValues)
            {                
                if (lineControlMap.ContainsKey(lineDownId))
                {
                    // Remove Button from LineDownPanel
                    foreach (LineControl lineControl in lineControlMap[lineDownId])
                    {
                        lineControl.UpdateLine(Constants.LINE_DOWN_COLOR);
                        int i = (Convert.ToInt32(lineControl.getName()) - 1) / 5;
                        int j = (Convert.ToInt32(lineControl.getName()) - 1) % 5;
                        if (buttons[i, j] != null)
                        {
                            buttons[i, j].BackColor = Constants.LINE_DOWN_COLOR;
                            buttons[i, j].State = Constants.BUTTON_STATE_DOWN;
                            buttons[i, j].Click -= LineDownButton_Click; //Remove the event handler till it becomes 'Yellow' again
                        }
                     }

                    // Add to LineUpPanel
                    if (!lineDownSet.Contains(lineDownId))//changed
                    {
                        if (lineControlMap[lineDownId].Count > 1)
                            AddToolsPanelControl_LineUp(lineControlMap[lineDownId][1]);
                        else 
                            AddToolsPanelControl_LineUp(lineControlMap[lineDownId][0]);
                    }
                    lineDownSet.Add(lineDownId);                    
                }
            }
        }

        private void UpdateLineUp(string lineControlId)
        {
            if (lineControlId != null)
            {
                if (lineDownSet.Contains(lineControlId))
                {
                    lineDownSet.Remove(lineControlId);
                    foreach (LineControl lineControl in lineControlMap[lineControlId])
                    {
                        lineControl.UpdateLine(Constants.NORMAL_LINE_COLOR);
                        int i = (Convert.ToInt32(lineControl.getName()) - 1) / 5;
                        int j = (Convert.ToInt32(lineControl.getName()) - 1) % 5;
                        if (buttons[i, j] != null)
                        {
                            buttons[i, j].BackColor = Constants.NORMAL_LINE_COLOR;
                            buttons[i, j].State = Constants.BUTTON_STATE_NORMAL;
                            buttons[i, j].Click -= LineUpButton_Click;
                            buttons[i, j].Click -= LineDownButton_Click; 
                        }

                    }
                }
            }
        }

        #region LDO
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
        

        private void Dashboard_Resize(object sender, System.EventArgs e)
        {
            Control dashboard = (Control)sender;

            int margin = 100;
            int circuitPanelWidth = dashboard.Size.Width - appsPanel.Width - busInfoPanel.Width - 2*margin;
            if (toolsPanel.Visible)
            {
                circuitPanelWidth = dashboard.Size.Width - appsPanel.Width - toolsPanel.Width - 2*margin;
            }
            int circuitPanelHeight = dashboard.Size.Height - margin;

            circuitPanel.Size = new System.Drawing.Size(circuitPanelWidth, circuitPanelHeight);
            circuitPanel.Location = new Point(appsPanel.Width + margin, margin);
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

        #endregion

        #region Server Communication

        public String GetServerResponseForOperation(String operation)
        {
      
            // Data buffer for incoming data.
            byte[] bytes = new byte[4096];
            String affectedListString = String.Empty;

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                ipAddress = IPAddress.Parse("127.0.0.1");//192.168.0.7");//"128.84.34.26");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5300);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes(operation);

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    Console.WriteLine("after send");
                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    affectedListString = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    Console.ReadLine();
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return affectedListString;
        }

        #endregion

        #region Click Handler for Panel
        private void Bus_Click(object sender, EventArgs e)
        {

            BusControl busControl = sender as BusControl;
            if (busControl != null)
            {
                if (!busControl.getBusName().Equals(String.Empty))
                {
                    setPanelValues(busControl);
                    setGraphValues(busControl);
                }
            }           
        }

        #endregion

        #region mouse hover
        private void Bus_Hover(object sender, EventArgs e)
        {
            
            BusControl busControl = sender as BusControl;
            if (busControl != null)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Hand;
            }
        }
        private void Bus_Leave(object sender, EventArgs e)
        {

            BusControl busControl = sender as BusControl;
            if (busControl != null)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
        }

        #endregion

        #region Panel Animation

        private void InfoPanel_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /* Enable asynchronus calling */
        delegate void RefreshPanelsCallback();

        private void refreshPanels()
        {
            if (this.busInfoPanel.InvokeRequired)
            {
                RefreshPanelsCallback d = new RefreshPanelsCallback(refreshPanels);
                this.Invoke(d, new object[] { });
                return;
            }
            busInfoPanel.Refresh();
            graphPanel.Refresh();
        }

        /* Enable asynchronus calling */
        delegate void SetPanelValesCallback(BusControl busControl);
        
        private void setPanelValues(BusControl busControl)
        {
            if (this.busIdValue.InvokeRequired)
            {
                SetPanelValesCallback d = new SetPanelValesCallback(setPanelValues);
                this.Invoke(d, new object[] { busControl });
                return;
            }
            shownBus = busControl;
            this.busIdValue.Text = busControl.getBusNo();
            this.busNameValue.Text = busControl.getBusName();
            this.busTypeValue.Text = busControl.getBusType();
            this.busAreaNumberValue.Text = busControl.getAreaNumber();
            this.busVoltageValue.Text = busControl.getBusVoltage();
            this.busBaseKiloVoltageValue.Text = busControl.getBusBaseKiloVoltage();
            this.busPhaseAngleValue.Text = busControl.getVoltagePhaseAngle();

        }

        /* Enable asynchronus calling */
        delegate void SetGraphValesCallback(BusControl busControl);

        private void setGraphValues(BusControl busControl)
        {
            try
            {
                if (this.voltageChart.InvokeRequired)
                {
                    SetGraphValesCallback d = new SetGraphValesCallback(setGraphValues);
                    this.Invoke(d, new object[] { busControl });
                    return;
                }

                this.voltageChart.Series.RemoveAt(0);
                this.voltageChart.Series.Add("Series1");
                this.voltageChart.Series["Series1"].ChartType = SeriesChartType.Line;
                String voltageList = this.graphBusListMap[busControl.getBusNo()];
                String[] voltageArray = voltageList.Split(Constants.COMMA_DELIMITER.ToCharArray());
                this.voltageChart.BackColor = System.Drawing.Color.Gray;
                this.voltageChart.ForeColor = System.Drawing.Color.Green;
                this.voltageChart.ChartAreas["chart"].BackColor = System.Drawing.Color.White;
                for (int i = 0; i < voltageArray.Length; i++)
                {
                    voltageChart.Series["Series1"].Points.AddXY(Convert.ToDouble(i), Convert.ToDouble(voltageArray[i]));
                }
                // Adjust X axis scale
                voltageChart.ChartAreas["chart"].AxisX.Minimum = 0;
                voltageChart.ChartAreas["chart"].AxisX.Maximum = 9;
            }
            catch(Exception e)
            {
                MessageBox.Show("Set Graph Values : Exception");
            }
         }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            Point locationForBusInfoPanel = new Point();
            Point locationForGraphInfoPanel = new Point();
            Point locationOfToolsPanel = toolsPanel.Location;
            locationForBusInfoPanel.X = locationOfToolsPanel.X;// -busInfoPanel.Size.Width;
            locationForBusInfoPanel.Y = 0;
            locationForGraphInfoPanel.X = locationForBusInfoPanel.X;
            locationForGraphInfoPanel.Y = busInfoPanel.Height + 15;
            busInfoPanel.Location = locationForBusInfoPanel;
            graphPanel.Location = locationForGraphInfoPanel;
            animationTimer.Stop();
            busInfoPanel.Visible = true;
            busInfoPanel.Size = initSize;
            busInfoPanel.Show();
            graphPanel.Visible = true;
            graphPanel.Show();             
        }

        private void Panel_Click(object sender, EventArgs e)
        {
            animationTimer.Start();
        }

        #endregion

        #region Add Controls

        private Control AddGeneratorBusControl(String id)
        {
            BusControl busControl = new BusControl(id);
            this.busControlMap.Add(id, busControl);
            return busControl;
        }
        private Control AddBusControl(String id, float x1, float y1, float x2, float y2, bool isArrow)
        {
            BusControl busControl = new BusControl(id, x1, y1, x2, y2);
            busControl.Location = new Point((int)x1, (int)y1);
            if (this.busControlMap.ContainsKey(id))
                this.busControlMap.Remove(id);
            this.busControlMap.Add(id, busControl);

            if (isArrow)
            {
                PictureBox arrowElement = AddArrowToBus();
                arrowElement.Location = new Point((int)(x1 + x2) / 2, (int)y1 + 1);
                this.circuitPanel.Controls.Add(arrowElement);
            }

            return busControl;
        }

        private PictureBox AddArrowToBus()
        {
            PictureBox arrowBox = new PictureBox();
            arrowBox.Image = new Bitmap("C:\\liveobjects\\libraries\\4D12F33758B74DAFBDE0D17E298AD01E\\1\\Data\\arrow.png");
            arrowBox.Size = new System.Drawing.Size(10, 27);

            return arrowBox;
        }

        private Control AddLineControl(String id, float x1, float y1, float x2, float y2, String name)
        {
            LineControl lineControl = new LineControl(id, x1, y1, x2, y2, name);
            lineControl.Location = new Point((int)x1, (int)y1);
            if (!this.lineControlMap.ContainsKey(id))
            {
                List<LineControl> lineControlList = new List<LineControl>();
                lineControlList.Add(lineControl);
                this.lineControlMap.Add(id, lineControlList);
            }
            else
            {
                List<LineControl> lineControlList = this.lineControlMap[id];
                lineControlList.Add(lineControl);

            }

            return lineControl;
        }
        private LineLabelControl AddLineLabelControl(String id, String name, float x, float y)
        {
            LineLabelControl linedownControl = new LineLabelControl(id,name);
            linedownControl.Location = new Point((int)x, (int)y);
            this.lineDownControlMap.Add(id, linedownControl);
            return linedownControl;
        }

        private PictureBox AddGeneratorControl(String id, float x, float y, int rotation)
        {
            PictureBox pictureBox = new PictureBox();

            if (rotation == 0)
                pictureBox.Image = new Bitmap("C:\\liveobjects\\libraries\\4D12F33758B74DAFBDE0D17E298AD01E\\1\\Data\\generator.png");
            else
                pictureBox.Image = new Bitmap("C:\\liveobjects\\libraries\\4D12F33758B74DAFBDE0D17E298AD01E\\1\\Data\\generator_rotated180.png");

            pictureBox.Location = new Point((int)x, (int)y);

            pictureBox.Size = new Size(34, 75);

            this.generatorControlMap.Add(id, pictureBox);

            return pictureBox;
        }

        #endregion
    }
}