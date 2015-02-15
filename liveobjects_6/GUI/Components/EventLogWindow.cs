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
    public partial class EventLogWindow : UserControl, QS.Fx.Logging.IEventLogger, QS.Fx.Diagnostics.IDiagnosticsComponent
    {
        public EventLogWindow()
        {
            InitializeComponent();

            comboBox1.Items.Add(new EventClassRef(null));
            foreach (QS.Fx.Logging.IEventClass eventClass in QS._qss_c_.Logging_1_.EventClasses.Classes)
                comboBox1.Items.Add(new EventClassRef(eventClass));
            comboBox1.SelectedIndex = 0;

            comboBox2.Items.Add(new LocationRef(null));
            comboBox2.SelectedIndex = 0;
        }

        #region Class EventClassRef

        private class EventClassRef
        {
            public EventClassRef(QS.Fx.Logging.IEventClass eventClass)
            {
                this.EventClass = eventClass;
            }

            public QS.Fx.Logging.IEventClass EventClass;

            public override string ToString()
            {
                return this.EventClass != null ? this.EventClass.Name : "(no selection)";
            }
        }

        #endregion

        #region Class LocationRef

        private class LocationRef
        {
            public LocationRef(string location)
            {
                this.Location = location;
            }

            public string Location;

            public override string ToString()
            {
                return this.Location != null ? this.Location : "(no selection)";
            }
        }

        #endregion

        private EventClassRef selectedEventClassRef;
        private LocationRef selectedLocationRef;
        private ICollection<string> availableLocations = 
            new System.Collections.ObjectModel.Collection<string>();
        private List<QS.Fx.Logging.IEvent> events = new List<QS.Fx.Logging.IEvent>();

        #region Handling the GUI stuff

        private const int MinColumnWidth = 50, MaxColumnWidth = 200;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                selectedEventClassRef = comboBox1.SelectedItem as EventClassRef;
                listView1.BeginUpdate();
                listView1.Columns.Clear();
                listView1.Columns.Add("Time");
                listView1.Columns.Add("Location");
                listView1.Columns.Add("Source");
                listView1.Columns.Add("Description");
                if (selectedEventClassRef != null && selectedEventClassRef.EventClass != null)
                    foreach (string property in selectedEventClassRef.EventClass.Properties)
                        listView1.Columns.Add(property);
                listView1.Items.Clear();
                foreach (QS.Fx.Logging.IEvent ev in events)
                    AppendEvent(ev);
                foreach (ColumnHeader columnHeader in listView1.Columns)
                {
                    columnHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                    if (columnHeader.Width < MinColumnWidth)
                        columnHeader.Width = MinColumnWidth;
                    else if (columnHeader.Width > MaxColumnWidth)
                        columnHeader.Width = MaxColumnWidth;
                }
                listView1.EndUpdate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                listView1.BeginUpdate();
                foreach (ColumnHeader columnHeader in listView1.Columns)
                {
                    columnHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                    if (columnHeader.Width < MinColumnWidth)
                        columnHeader.Width = MinColumnWidth;
                    else if (columnHeader.Width > MaxColumnWidth)
                        columnHeader.Width = MaxColumnWidth;
                }
                listView1.EndUpdate();
            }
        }

        private void AppendEvent(QS.Fx.Logging.IEvent e)
        {
            bool isSelected = selectedLocationRef != null && selectedLocationRef.Location != null &&
                e.Location != null && selectedLocationRef.Location.Equals(e.Location);

            if (selectedEventClassRef != null && selectedEventClassRef.EventClass != null)
            {
                if (ReferenceEquals(e.Class, selectedEventClassRef.EventClass))
                    listView1.Items.Add(new EventRef(e, true, isSelected));
            }
            else
                listView1.Items.Add(new EventRef(e, false, isSelected));

            if (e.Location != null && !availableLocations.Contains(e.Location))
            {
                availableLocations.Add(e.Location);
                comboBox2.Items.Add(new LocationRef(e.Location));
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                System.Collections.IEnumerator selectedGuys = listView1.SelectedItems.GetEnumerator();
                if (selectedGuys.MoveNext())
                {
                    EventRef eventRef = selectedGuys.Current as EventRef;
                    if (eventRef != null)
                        richTextBox1.Text = eventRef.Event.ToString();
                    else
                        richTextBox1.Text = "(error)";
                }
                else
                    richTextBox1.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                updateMarking();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                lock (this)
                {
                    selectedLocationRef = comboBox2.SelectedItem as LocationRef;
                    updateMarking();
                }
            }
        }

        private void updateMarking()
        {
            listView1.BeginUpdate();
            foreach (EventRef eventRef in listView1.Items)
            {
                bool isSelected = selectedLocationRef != null && selectedLocationRef.Location != null &&
                    eventRef.Event.Location != null && selectedLocationRef.Location.Equals(eventRef.Event.Location);
                eventRef.SetSelected(isSelected);
            }
            listView1.EndUpdate();
        }

        #endregion

        #region IEventLogger Members

        void QS.Fx.Logging.IEventLogger.Log(QS.Fx.Logging.IEvent eventToLog)
        {
            lock (this)
            {
                events.Add(eventToLog);
                AppendEvent(eventToLog);
            }
        }

        #endregion

        #region Class EventRef

        private class EventRef : ListViewItem
        {
            public EventRef(QS.Fx.Logging.IEvent e, bool show_properties, bool is_selected) 
                : base(Event2Strings(e, show_properties))
            {
                this.Event = e;
                if (is_selected)
                    this.BackColor = Color.Yellow;
            }

            public QS.Fx.Logging.IEvent Event;

            public void SetSelected(bool is_selected)
            {
                if (is_selected)
                    this.BackColor = Color.Yellow;
                else
                    this.BackColor = Color.White;
            }

            private static string[] Event2Strings(QS.Fx.Logging.IEvent e, bool show_properties)
            {
                if (show_properties)
                {
                    List<string> items = new List<string>();
                    items.Add(e.Time.ToString("000000.000000000"));
                    items.Add(e.Location != null ? e.Location : "");
                    items.Add(e.Source != null ? e.Source : "");
                    items.Add(e.Description != null ? e.Description : "" );
                    foreach (string property in e.Class.Properties)
                    {
                        object o = e[property];
                        items.Add(o != null ? o.ToString() : "");
                    }
                    return items.ToArray();
                }
                else
                    return new string[] { e.Time.ToString("000000.000000000"), e.Location != null ? e.Location : "",
                        e.Source != null ? e.Source : "", e.Description != null ? e.Description : "" };
            }
        }

        #endregion

        #region IDiagnosticsComponent Members

        QS.Fx.Diagnostics.ComponentClass QS.Fx.Diagnostics.IDiagnosticsComponent.Class
        {
            get { return QS.Fx.Diagnostics.ComponentClass.EventLogger; }
        }

        bool QS.Fx.Diagnostics.IDiagnosticsComponent.Enabled
        {
            get { return true; }
            set
            {
                if (value != true)
                    throw new NotSupportedException("This component is always enabled by default.");
            }
        }

        void QS.Fx.Diagnostics.IDiagnosticsComponent.ResetComponent()
        {
            try
            {
                reset();
            }
            catch (Exception)
            {
                try
                {
                    listView1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(delegate { reset(); }));
                }
                catch (Exception)
                {
                }
            }
        }

        private void reset()
        {
            lock (this)
            {
                events.Clear();
                availableLocations.Clear();

                listView1.BeginUpdate();
                listView1.Items.Clear();
                listView1.EndUpdate();

                comboBox2.Items.Clear();
                comboBox2.Items.Add(new LocationRef(null));
                comboBox2.SelectedIndex = 0;
            }
        }

        #endregion
    }
}
