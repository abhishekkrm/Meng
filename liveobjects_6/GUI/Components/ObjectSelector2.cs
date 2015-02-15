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
using System.Threading;

namespace QS.GUI.Components
{
    public partial class ObjectSelector2 : UserControl, IObjectSelector
    {
        public ObjectSelector2()
        {
            InitializeComponent();
        }

        private EventHandler selectionChanged;
        private object selectedObject;
        private IDictionary<object, TreeNode> mappingObjects2Nodes = new Dictionary<object, TreeNode>();
        private TreeNode currentlySelectedNode;

        public object SelectedActualObject
        {
            get 
            {
                if (currentlySelectedNode is ManagedNode)
                    return ((ManagedNode)currentlySelectedNode).AssociatedObject;
                else if (currentlySelectedNode is QS._qss_e_.Inspection_.AttributeNode)
                    return ((QS._qss_e_.Inspection_.AttributeNode)currentlySelectedNode).Attribute;
                else
                    return null;
            }
        }

        #region IObjectSelector Members - Part 3

        void IObjectSelector.Save()
        {
            // .......................................................................................................................
        }

        void IObjectSelector.ToClipboard()
        {
            // .......................................................................................................................
        }

        #endregion

        #region IObjectSelector Members - Part 2

        void IObjectSelector.Refresh()
        {
            TreeNode node = treeView1.SelectedNode as TreeNode;
            if (node != null)
                Refresh(node);
        }

        void IObjectSelector.RefreshRecursively()
        {
            TreeNode node = treeView1.SelectedNode as TreeNode;
            if (node != null)
                RefreshRecursively(node);
        }

        void IObjectSelector.Unroll()
        {
            TreeNode node = treeView1.SelectedNode as TreeNode;
            if (node != null)
                Unroll(node);
        }

        void IObjectSelector.ExpandAll()
        {
            TreeNode node = treeView1.SelectedNode as TreeNode;
            if (node != null)
                node.ExpandAll();
        }

        #endregion

        #region Refreshing

        private void Refresh(TreeNode node)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(int_Refresh1), node);
        }

        private void RefreshRecursively(TreeNode node)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(int_Refresh2), node);
        }

        private void int_Refresh1(object obj)
        {
            treeView1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(delegate { do_Refresh(obj as System.Windows.Forms.TreeNode, false); }));
        }

        private void int_Refresh2(object obj)
        {
            treeView1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(delegate { do_Refresh(obj as System.Windows.Forms.TreeNode, true); }));
        }

        private void do_Refresh(TreeNode node, bool recursively)
        {
            if (node != null)
            {
                node.ForeColor = Color.Red;
                try
                {
                    try
                    {
                        if (node is QS._qss_e_.Inspection_.AttributeNode)
                        {
                            (node as QS._qss_e_.Inspection_.AttributeNode).Refresh();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.ToString());
                    }

                    RefreshSelected();

                    do_Unroll(node, false);

                    if (recursively && node.Nodes.Count < 2)
                    {
                        foreach (TreeNode subnode in node.Nodes)
                            do_Refresh(subnode, true);
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                }
                finally
                {
                    node.ForeColor = Color.Black;
                }
            }
        }

        #endregion

        #region Unrolling

        private void Unroll(TreeNode node)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(int_Unroll1), node);
        }

        private void int_Unroll1(object obj)
        {
            treeView1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(delegate { do_Unroll(obj as System.Windows.Forms.TreeNode, false); }));
        }

        private void UnrollRecursively(TreeNode node)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(int_Unroll2), node);
        }

        private void int_Unroll2(object obj)
        {
            treeView1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(delegate { do_Unroll(obj as System.Windows.Forms.TreeNode, true); }));
        }

        private void do_Unroll(TreeNode node, bool recursively)
        {
            if (node != null)
            {
                node.ForeColor = Color.Green;
                try
                {
                    if (node is QS._qss_e_.Inspection_.AttributeNode)
                    {
                        QS._qss_e_.Inspection_.AttributeNode attributeNode = node as QS._qss_e_.Inspection_.AttributeNode;
                        treeView1.BeginUpdate();
                        try
                        {
                            attributeNode.Unroll();
                        }
                        finally
                        {
                            treeView1.EndUpdate();
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                }
                finally
                {
                    node.ForeColor = Color.Black;
                }
            }
        }

        #endregion

        #region Handling of Mouse Actions

        private void RefreshSelected()
        {
            if (currentlySelectedNode != null)
                selectedObject = Node2Object(currentlySelectedNode);
            else
                selectedObject = null;

            AnnounceChange();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            lock (this)
            {
                if (currentlySelectedNode != null)
                    currentlySelectedNode.BackColor = Color.White;

                currentlySelectedNode = e.Node;

                if (currentlySelectedNode != null)
                {
                    currentlySelectedNode.BackColor = Color.Yellow;
                    selectedObject = Node2Object(currentlySelectedNode);
                }
                else
                    selectedObject = null;

                AnnounceChange();
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (currentlySelectedNode != null)
                Unroll(currentlySelectedNode);
        }

        #endregion

        #region Indexes for Icons

        private enum Icon : int
        {
            ClosedFolder = 0, OpenFolder, Document, Property, ManagedComponent, Other
        }

        private static QS._qss_e_.Inspection_.AttributeNode.ImageAssignment imageAssignment =
            new QS._qss_e_.Inspection_.AttributeNode.ImageAssignment(
                (int)Icon.Property, (int)Icon.Property, (int)Icon.ClosedFolder, (int)Icon.OpenFolder);

        #endregion

        #region Creating a node for an object and extracting an object from a node

        private TreeNode Object2Node(object o, string name)
        {
            if (o is QS.Fx.Logging.ILogger)
                return new ManagedNode(o, ((name != null) ? name : "log"), Icon.Document, Icon.Document);
            else if (o is QS._core_c_.Components.AttributeSet)
                return new QS._qss_e_.Inspection_.AttributeNode((QS._core_c_.Components.AttributeSet)o, imageAssignment);
            else if (o is QS.Fx.Inspection.IAttribute)
                return new QS._qss_e_.Inspection_.AttributeNode(((QS.Fx.Inspection.IAttribute)o), imageAssignment);
            else if (o is QS.Fx.Inspection.IInspectable)
                return new QS._qss_e_.Inspection_.AttributeNode(((QS.Fx.Inspection.IInspectable)o).Attributes, imageAssignment);
            else if (o is QS._qss_e_.Management_.IManagedComponent)
                return new ManagedNode(o, ((name != null) ? name : "component"), Icon.ManagedComponent, Icon.ManagedComponent);
            else
                return new ManagedNode(o, ((name != null) ? name : o.GetType().Name), Icon.Other, Icon.Other);
        }

        private object Node2Object(TreeNode node)
        {
            if (node is ManagedNode)
                return ((ManagedNode)node).AssociatedObject;
            else if (node is QS._qss_e_.Inspection_.AttributeNode)
            {
                QS._qss_e_.Inspection_.AttributeNode attributeNode = (QS._qss_e_.Inspection_.AttributeNode) node;
                switch (attributeNode.AttributeClass)
                {
                    case QS.Fx.Inspection.AttributeClass.SCALAR:
                        return attributeNode.CachedValue;

                    case QS.Fx.Inspection.AttributeClass.COLLECTION:
                        return string.Empty;
                }
            }
            
            throw new Exception("Unknown node type.");
        }

        #endregion

        #region IObjectSelector Members - Part 1

        void IObjectSelector.Add(object o)
        {
            ((IObjectSelector)this).Add(o, null);
        }

        void IObjectSelector.Add(object o, string name)
        {
            lock (this)
            {
                if (mappingObjects2Nodes.ContainsKey(o))
                    throw new Exception("Already contains this node.");
                TreeNode node = Object2Node(o, name);
                mappingObjects2Nodes.Add(o, node);

                try
                {
                    treeView1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(
                        delegate
                        {
                            treeView1.BeginUpdate();
                            treeView1.Nodes.Add(node);
                            treeView1.EndUpdate();
                        }));
                }
                catch (Exception)
                {
                    treeView1.BeginUpdate();
                    treeView1.Nodes.Add(node);
                    treeView1.EndUpdate();
                }
            }
        }

        void IObjectSelector.Remove(object o)
        {
            lock (this)
            {
                if (!mappingObjects2Nodes.ContainsKey(o))
                    throw new Exception("Does not contain this object.");
                TreeNode node = mappingObjects2Nodes[o];

                mappingObjects2Nodes.Remove(o);

                treeView1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(
                    delegate
                    {
                        treeView1.BeginUpdate();
                        treeView1.Nodes.Remove(node);
                        treeView1.EndUpdate();
                    }));
            }            
        }

        object IObjectSelector.SelectedObject
        {
            get { return selectedObject; }
        }

        private void AnnounceChange()
        {
            if (selectionChanged != null)
                selectionChanged(this, null);
        }

        event EventHandler IObjectSelector.SelectionChanged
        {
            add { selectionChanged += value; }
            remove { selectionChanged -= value; }
        }

        #endregion

        #region Class ManagedNode

        private class ManagedNode : TreeNode
        {
            public ManagedNode(object associatedObject, string name, Icon selectedIndex, Icon unselectedIndex)
                : base(name, (int) selectedIndex, (int) unselectedIndex)
            {
                this.associatedObject = associatedObject;
            }

            private object associatedObject;

            public object AssociatedObject
            {
                get { return associatedObject; }
            }
        }

        #endregion
    }
}
