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

namespace QS._qss_x_.Inspection_.Hierarchy_
{
    public partial class HierarchyVisualizer : UserControl, IHierarchyVisualizer
    {
        #region Constructor

        public HierarchyVisualizer()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields

        private IHierarchyView hierarchyview;
        private int nobjects;
        private IHierarchyViewObject[] objects;
        private int[] minranks, maxranks;
        private IDictionary<object, int> map;
        private int maxrank;
        private IList<int>[] incoming, outgoing;
        private double[] _ordering;
        private List<KeyValuePair<double, int>> ordering;
        private IList<int> bottomlayer;
        private IList<int>[] layers;
        private PointF[] positions;
        private Graphics g;
        private RectangleF window;
        private System.Drawing.Font font = new Font(FontFamily.GenericSansSerif, 18, FontStyle.Bold);
        private Pen linepen = new Pen(Color.Blue, 5), linepen2 = new Pen(Color.Red, 5);
        private bool selected, selectedisobj;
        private int selectedind, selectedfrom, selectedto;
        private IHierarchyViewObject selectedobj;
        private IHierarchyViewConnection selectedconn;
        private EventHandler selectionchangecallback;
        private PointF m;
        private bool firecallback;

        #endregion

        #region IHierarchyVisualizer Members

        IHierarchyView IHierarchyVisualizer.HierarchyView
        {
            get { return hierarchyview; }
            set
            {
                lock (this)
                {
                    if (value != hierarchyview)
                    {
                        hierarchyview = value;
                        if (hierarchyview != null)
                            _Recalculate();
                    }
                }
            }
        }

        event EventHandler IHierarchyVisualizer.OnSelectionChanged
        {
            add 
            {
                lock (this)
                {
                    selectionchangecallback += value;
                }
            }
            
            remove 
            {
                lock (this)
                {
                    selectionchangecallback -= value;
                }
            }
        }

        bool IHierarchyVisualizer.Selected
        {
            get { return selected; }
        }

        IHierarchyViewObject IHierarchyVisualizer.SelectedObject
        {
            get { return selectedobj; }
        }

        IHierarchyViewConnection IHierarchyVisualizer.SelectedConnection
        {
            get { return selectedconn; }
        }

        #endregion

        #region _Recalculate

        private void _Recalculate()
        {
            List<IHierarchyViewObject> l = new List<IHierarchyViewObject>();
            foreach (IHierarchyViewObject o in hierarchyview.Objects)
                l.Add(o);
            objects = l.ToArray();
            nobjects = objects.Length;
            minranks = new int[nobjects];
            maxranks = new int[nobjects];
            map = new Dictionary<object, int>();
            maxrank = 0;
            selected = false;
            incoming = new IList<int>[nobjects];
            outgoing = new IList<int>[nobjects];
            ordering = new List<KeyValuePair<double, int>>();
            _ordering = new double[nobjects];

            for (int ind = 0; ind < nobjects; ind++)
            {
                minranks[ind] = -1;
                maxranks[ind] = -1;
                _ordering[ind] = -1;
                object id = objects[ind].ID;
                if (map.ContainsKey(id))
                    throw new Exception("Two objects with different identifiers have been found.");
                map.Add(id, ind);
            }

            for (int ind = 0; ind < nobjects; ind++)
                _ComputeRank1(ind);

            for (int ind = 0; ind < nobjects; ind++)
                _ComputeRank2(ind);

            layers = new IList<int>[maxrank + 1];
            for (int ind = 0; ind < layers.Length; ind++)
                layers[ind] = new List<int>();

            bottomlayer = new List<int>();
            for (int ind = 0; ind < nobjects; ind++)
            {
                if (minranks[ind] == 0)
                    bottomlayer.Add(ind);
            }

            int k = 0;
            foreach (int ind in bottomlayer)
                _ordering[ind] = (((double) (k++)) + 0.5) / ((double)(bottomlayer.Count));

            for (int ind = 0; ind < nobjects; ind++)
                _ComputeOrder(ind);

            for (int ind = 0; ind < nobjects; ind++)
                ordering.Add(new KeyValuePair<double, int>(_ordering[ind], ind));

            ordering.Sort(
                new Comparison<KeyValuePair<double, int>>(
                    delegate(KeyValuePair<double, int> o1, KeyValuePair<double, int> o2)
                    {
                        int result = o1.Key.CompareTo(o2.Key);
                        if (result == 0)
                            result = o1.Value.CompareTo(o2.Value);
                        return result;
                    }));

            foreach (KeyValuePair<double, int> o in ordering)
                layers[maxranks[o.Value]].Add(o.Value);

            positions = new PointF[nobjects];

            for (int ind = 0; ind < layers.Length; ind++)
            {
                double y = 1 - (((double) ind) + 0.5) / ((double) (maxrank + 1));

                if (true) // ind == 0)
                {
                    int xind = 0;
                    foreach (int nodeind in layers[ind])
                    {
                        double x = (((double) xind) + 0.5) / ((double) (layers[ind].Count));
                        positions[nodeind] = new PointF((float) x, (float) y);
                        xind++;
                    }
                }
                else
                {
                    List<KeyValuePair<double, int>> _positions = new List<KeyValuePair<double, int>>();

                    int xind = 0;
                    foreach (int nodeind in layers[ind])
                    {
                        double x = 0;
                        int nx = 0;
                        foreach (int incomingind in incoming[nodeind])
                        {
                            x += positions[incomingind].X;
                            nx++;
                        }

                        x /= nx;
                        _positions.Add(new KeyValuePair<double, int>(x, nodeind));
                        xind++;
                    }

                    _positions.Sort(
                        new Comparison<KeyValuePair<double, int>>(
                            delegate(KeyValuePair<double, int> o1, KeyValuePair<double, int> o2) 
                            {
                                int result = o1.Key.CompareTo(o2.Key);
                                if (result == 0)
                                    result = o1.Value.CompareTo(o2.Value);
                                return result;
                            }));

                    xind = 0;
                    foreach (KeyValuePair<double, int> _position in _positions)
                    {
                        double x = (((double) xind) + 0.5) / ((double)(layers[ind].Count));
                        positions[_position.Value] = new PointF((float)x, (float)y);
                        xind++;
                    }
                }
            }
        }

        #endregion

        #region _ComputeRank1

        private void _ComputeRank1(int index)
        {
            if (minranks[index] < 0)
            {
                IHierarchyViewObject obj = objects[index];
                int rank = 0;
                incoming[index] = new List<int>();
                foreach (IHierarchyViewConnection conn in obj.Incoming)
                {
                    IHierarchyViewObject input = conn.Incoming;
                    int inputindex = map[input.ID];
                    incoming[index].Add(inputindex);
                    _ComputeRank1(inputindex);
                    int inputrank = minranks[inputindex];
                    rank = Math.Max(rank, inputrank + 1);
                }
                minranks[index] = rank;
                maxrank = Math.Max(maxrank, rank);
            }
        }

        #endregion

        #region _ComputeRank2

        private void _ComputeRank2(int index)
        {
            if (maxranks[index] < 0)
            {
                IHierarchyViewObject obj = objects[index];
                int rank = maxrank;
                outgoing[index] = new List<int>();
                foreach (IHierarchyViewConnection conn in obj.Outgoing)
                {
                    IHierarchyViewObject output = conn.Outgoing;
                    int outputindex = map[output.ID];
                    outgoing[index].Add(outputindex);
                    _ComputeRank2(outputindex);
                    int outputrank = maxranks[outputindex];
                    rank = Math.Min(rank, outputrank - 1);
                }
                maxranks[index] = rank;
            }
        }

        #endregion

        #region _ComputeOrder

        private void _ComputeOrder(int index)
        {
            if (_ordering[index] < 0)
            {
                double x = 0;
                int nx = 0;
                foreach (int incomingind in incoming[index])
                {
                    _ComputeOrder(incomingind);
                    x += _ordering[incomingind];
                    nx++;
                }
                x /= nx;
                _ordering[index] = x;
            }
        }

        #endregion

        #region HierarchyVisualizer_Paint

        private void HierarchyVisualizer_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            try
            {
                g.Clear(Color.White);
                window = g.VisibleClipBounds;
                lock (this)
                {
                    selected = false;
                    if (hierarchyview != null)
                    {
                        for (int ind = 0; !selected && ind < nobjects; ind++)
                        {
                            IHierarchyViewObject obj = objects[ind];
                            string s = obj.Name;
                            PointF position = new PointF(window.Left + positions[ind].X * window.Width,
                                window.Top + positions[ind].Y * window.Height);
                            SizeF ssize = g.MeasureString(s, font);

                            if (m.X >= (position.X - ssize.Width / 2) && m.X <= (position.X + ssize.Width / 2) &&
                                m.Y >= (position.Y - ssize.Height / 2) && m.Y <= (position.Y + ssize.Height / 2))
                            {
                                selected = true;
                                selectedisobj = true;
                                selectedind = ind;
                                selectedobj = obj;
                                selectedconn = null;
                                selectedfrom = -1;
                                selectedto = -1;
                                break;
                            }
                        }

                        for (int ind = 0; ind < nobjects; ind++)
                        {
                            PointF p1 = new PointF(window.Left + positions[ind].X * window.Width, window.Top + positions[ind].Y * window.Height);
                            IHierarchyViewObject obj = objects[ind];

                            double x1 = window.Left + positions[ind].X * window.Width;
                            double y1 = window.Top + positions[ind].Y * window.Height;

                            foreach (IHierarchyViewConnection conn in obj.Outgoing)
                            {
                                IHierarchyViewObject other = conn.Outgoing;
                                int otherind = map[other.ID];

                                PointF p2 = new PointF(window.Left + positions[otherind].X * window.Width, 
                                    window.Top + positions[otherind].Y * window.Height);

                                if (!selected)
                                {
                                    double x2 = window.Left + positions[otherind].X * window.Width;
                                    double y2 = window.Top + positions[otherind].Y * window.Height;

                                    double dx = x2 - x1, dy = y2 - y1;
                                    double lambda = (dx * (m.X - x1) + dy * (m.Y - y1)) / (dx * dx + dy * dy);

                                    if (lambda > 0 && lambda < 1)
                                    {
                                        double ix = x1 + lambda * dx, iy = y1 + lambda * dy;
                                        double distance = Math.Sqrt((m.X - ix) * (m.X - ix) + (m.Y - iy) * (m.Y - iy));

                                        if (distance < 10)
                                        {
                                            selected = true;
                                            selectedisobj = false;
                                            selectedind = -1;
                                            selectedobj = null;
                                            selectedfrom = ind;
                                            selectedto = otherind;
                                            selectedconn = conn;
                                        }
                                    }
                                }

                                if (selected && !selectedisobj && ReferenceEquals(conn, selectedconn))
                                    g.DrawLine(linepen2, p1, p2);
                                else
                                    g.DrawLine(linepen, p1, p2);
                            }
                        }

                        for (int ind = 0; ind < nobjects; ind++)
                        {
                            PointF p1 = new PointF(window.Left + positions[ind].X * window.Width, window.Top + positions[ind].Y * window.Height);
                            IHierarchyViewObject obj = objects[ind];

                            double x1 = window.Left + positions[ind].X * window.Width;
                            double y1 = window.Top + positions[ind].Y * window.Height;

                            foreach (IHierarchyViewConnection conn in obj.Outgoing)
                            {
                                IHierarchyViewObject other = conn.Outgoing;
                                int otherind = map[other.ID];

                                PointF p2 = new PointF(window.Left + positions[otherind].X * window.Width,
                                    window.Top + positions[otherind].Y * window.Height);

                                if (selected && !selectedisobj && ReferenceEquals(conn, selectedconn))
                                {
                                    string s = conn.Name;
                                    PointF pp = new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
                                    SizeF ssize = g.MeasureString(s, font);
                                    PointF pm = new PointF((float)pp.X - ssize.Width / 2, (float)pp.Y - ssize.Height / 2);
                                    g.FillRectangle(Brushes.Yellow, pm.X - 3, pm.Y - 3, ssize.Width + 6, ssize.Height + 6);
                                    g.DrawString(s, font, Brushes.Black, pm);
                                }
                            }
                        }

                        for (int ind = 0; ind < nobjects; ind++)
                        {
                            IHierarchyViewObject obj = objects[ind];
                            string s = obj.Name;
                            PointF position = new PointF(window.Left + positions[ind].X * window.Width, 
                                window.Top + positions[ind].Y * window.Height);
                            SizeF ssize = g.MeasureString(s, font);
                            PointF p = new PointF((float)position.X - ssize.Width / 2, (float)position.Y - ssize.Height / 2);

                            if (selected && selectedisobj && ReferenceEquals(obj, selectedobj))
                            {
                                g.FillRectangle(Brushes.Yellow, p.X - 3, p.Y - 3, ssize.Width + 6, ssize.Height + 6);
                                g.DrawRectangle(linepen2, p.X - 5, p.Y - 5, ssize.Width + 10, ssize.Height + 10);
                            }
                            else
                            {
                                g.FillRectangle(Brushes.Ivory, p.X - 3, p.Y - 3, ssize.Width + 6, ssize.Height + 6);
                                g.DrawRectangle(linepen, p.X - 5, p.Y - 5, ssize.Width + 10, ssize.Height + 10);
                            }
                            g.DrawString(s, font, Brushes.Black, p);
                        }
                    }

                    if (firecallback)
                    {
                        firecallback = false;
                        if (selectionchangecallback != null)
                            selectionchangecallback(this, null);
                    }
                }
            }
            catch (Exception exc)
            {
                g.Clear(Color.White);
                System.Drawing.Font font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold);
                g.DrawString(exc.ToString(), font, Brushes.Red, 1, 1);
            }
        }

        #endregion

        #region HierarchyVisualizer_MouseClick

        private void HierarchyVisualizer_MouseClick(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                m = new PointF(e.X, e.Y);
                firecallback = true;
            }

            Refresh();
        }

        #endregion
    }
}
