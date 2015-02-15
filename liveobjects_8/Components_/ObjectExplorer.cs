/* Copyright (c) 2004-2009, 
 * Revant Kapoor (rk368@cornell.edu),
 * Yilin Qin (yq33@cornell.edu),
 * Krzysztof Ostrowski. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the 
    documentation and/or other materials provided with the distribution.
3. Neither the name of the Cornell University nor the names of its contributors may be used to endorse or promote products derived from 
    this software without specific prior written permission.

This software is provided by Krzysztof Ostrowski ''as is'' and any express or implied warranties, including, but not limited to, the implied
warranties of merchantability and fitness for a particular purpose are disclaimed. in no event shall krzysztof ostrowski be liable for any direct, 
indirect, incidental, special, exemplary, or consequential damages (including, but not limited to, procurement of substitute goods or services;
loss of use, data, or profits; or business interruption) however caused and on any theory of liability, whether in contract, strict liability, or tort
(including negligence or otherwise) arising in any way out of the use of this software, even if advised of the possibility of such damage. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Xml.Serialization;


namespace QS._qss_x_.ObjectExplorer_
{
    [QS.Fx.Reflection.ComponentClass("581644C5ED8D48769BBB7141962F7B07", "ObjectExplorer", "Object Explorer")]
    public partial class ObjectExplorer : UserControl, QS.Fx.Object.Classes.IUI
    {
        #region Fields              
        private QS.Fx.Endpoint.Internal.IExportedUI myendpoint;
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderendpoint;
        private QS.Fx.Endpoint.IConnection _loaderconnection;
        private ObjectExplorerHandler handler;
        private IList<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _list = new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();
        private bool bRefreshing = false;          
        #endregion

        public ObjectExplorer(QS.Fx.Object.IContext _mycontext, 
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> _loader)
        {
            InitializeComponent();
            this.myendpoint = _mycontext.ExportedUI(this);
            this._loaderendpoint = _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();
            this._loaderconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._loaderendpoint).Connect(_loader.Dereference(_mycontext).Endpoint);
            this.handler = new ObjectExplorerHandler(_mycontext, this);
            
        }

        #region IUI Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.IUI.UI
        {
            get { return this.myendpoint; }
        }

        #endregion

        #region UserControl1_load
        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
        #endregion


        #region updateTreeView
        
        public void updateTreeView()
        {
            if (this.bRefreshing)
            {
                updateTreeView(this.treeView2);
            }
        }

        public void updateTreeView(Control oControl)
        {
            IEnumerator iter = this.treeView2.Nodes.GetEnumerator();
            while (iter.MoveNext())
            {
                updateNode((TreeNode)iter.Current);
            }           

        }

        private void updateNode(TreeNode node)
        {
            string str1 = ((QS.Fx.Value.Classes.IExplorableMetadata<object>)(node.Tag)).Name;
            string str2 = ((QS.Fx.Value.Classes.IExplorableMetadata<object>)(node.Tag)).Value.ToString();
            node.Text = ((QS.Fx.Value.Classes.IExplorableMetadata<object>)(node.Tag)).Name +
                ":" + ((QS.Fx.Value.Classes.IExplorableMetadata<object>)(node.Tag)).Value.ToString();
           
            IEnumerator iter = node.Nodes.GetEnumerator();

            while (iter.MoveNext())
            { 
                updateNode((TreeNode)iter.Current);
            }
        }
        #endregion

        private void OnControl1DoubleClick(object sender, EventArgs e)
        {
           
        }

       
        private void Drop(string _objectxml)
        {
            
            if (_loaderendpoint.IsConnected)
            {   
                // adding objectxml to handler
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = this._loaderendpoint.Interface.Load(_objectxml);
                QS.Fx.Attributes.IAttribute _nameattribute;
                _objectref.Attributes.Get( QS.Fx.Attributes.AttributeClasses.CLASS_name, out  _nameattribute);
                String _key = _nameattribute.Value;
                 
                handler.Add(_objectref);
                TreeNode node = treeView1.Nodes.Add(_key);
                node.Tag = _objectref;               
                            
                
            }
        }

        private void collapseChildren(TreeNode node)
        {
            TreeNode child = node.FirstNode;
            // travel recursively to delete children nodes, which may include grandchildren nodes
            while (child != null)
            {
                if (child.GetNodeCount(false) > 0)
                    collapseChildren(child);
                child = child.NextNode;
            }
            node.Collapse();
            node.Nodes.Clear();
            handler.hideChildren(((QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>)node.Tag));
        }

        private void collapseChildrenForMetadata(TreeNode node)
        {
            TreeNode child = node.FirstNode;
            // travel recursively to delete children nodes, which may include grandchildren nodes
            while (child != null)
            {
                if (child.GetNodeCount(false) > 0)
                    collapseChildren(child);
                child = child.NextNode;
            }
            node.Collapse();
            node.Nodes.Clear();
        }

        private void OnTreeView1NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (this.bRefreshing)
                this.bRefreshing = false;
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> node =
                (QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>)e.Node.Tag;
            
            if (e.Node.Nodes.Count == 0)
            {
                IDictionary<String, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> children = handler.getChildren(node);
                IEnumerator<KeyValuePair<String, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> iterator = children.GetEnumerator();

                while (iterator.MoveNext())
                {
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objectref = iterator.Current.Value;
                    TreeNode nodeChild = e.Node.Nodes.Add(iterator.Current.Key);                       
                    nodeChild.Tag = objectref;
                    nodeChild.Expand();
                }

                e.Node.Expand();
            }
            else
            {
               // collapseChildren(e.Node);
            }

            // get property of the selected object
            this.treeView2.Nodes.Clear();
            IEnumerable<QS.Fx.Value.Classes.IExplorableMetadata<object>> slist = handler.getProperties(node);
            if (slist != null)
            {
                IEnumerator<QS.Fx.Value.Classes.IExplorableMetadata<object>> slist_iter = slist.GetEnumerator();
                IEnumerator iter = treeView2.Nodes.GetEnumerator();
                while (iter.MoveNext())
                {
                    TreeNode current = (TreeNode)iter.Current;
                }                
                // print out the value in the list to the right edit box
                while (slist_iter.MoveNext())
                {
                    TreeNode nodeChild = treeView2.Nodes.Add(slist_iter.Current.Name + ":" + slist_iter.Current.Value.ToString());
                    nodeChild.Tag = slist_iter.Current;

                }
            }
            this.bRefreshing = true;
        }


       
        #region OntreeView1DragDrop
        private void OntreeView1DragDrop(object sender, DragEventArgs e)
        {            
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] _filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string _filename in _filenames)
                {
                    string _text;
                    using (StreamReader _streamreader = new StreamReader(_filename))
                    {
                        _text = _streamreader.ReadToEnd();
                    }
                    Drop(_text);
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Text, false))
            {
                string _text = (string)e.Data.GetData(DataFormats.Text);
                Drop(_text);
            }
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText, false))
            {
                string _text = (string)e.Data.GetData(DataFormats.UnicodeText);
                Drop(_text);
            }
            else
                throw new Exception("The drag and drop operation cannot continue because none of the data formats was recognized.");

        }
        #endregion

        #region OnTreeView1DragEner
        private void OnTreeView1DragEnter(object sender, DragEventArgs e)
        {
            lock (this)
            {
                if (_loaderendpoint.IsConnected )
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop, false) ||
                        e.Data.GetDataPresent(DataFormats.Text, false) || e.Data.GetDataPresent(DataFormats.UnicodeText, false))
                    {
                        e.Effect = DragDropEffects.All;
                    }
                }
                else
                    e.Effect = DragDropEffects.None;
            }
        }
        #endregion

        #region OnTreeView2NodeMouseClick
        private void OnTreeView2NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            QS.Fx.Value.Classes.IExplorableMetadata<object> node =
                (QS.Fx.Value.Classes.IExplorableMetadata<object>)e.Node.Tag;

            if (e.Node.Nodes.Count != 0)
            {
                //collapseChildrenForMetadata(e.Node);
            }
            else
            {
                // get property of the selected object
                IEnumerable<QS.Fx.Value.Classes.IExplorableMetadata<object>> list = handler.getPropertyChildren(node);
                foreach (QS.Fx.Value.Classes.IExplorableMetadata<object> child in list)
                {
                    TreeNode nChild = e.Node.Nodes.Add(child.Name + ":" + child.Value.ToString());
                    nChild.Tag = child;
                    nChild.Expand();
                }
                e.Node.Expand();
            }

        }
        #endregion

        #region OnTreeView2NodeMouseDoubleClick
        private void OnTreeView2NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            e.Node.BeginEdit();
        }
        #endregion

        #region OnTreeView2AfterLabelEdit
        private void OnTreeView2AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            e.Node.EndEdit(true);
            if (e.Label != null)
            {
                QS.Fx.Value.Classes.IExplorableMetadata<object> node =
                    (QS.Fx.Value.Classes.IExplorableMetadata<object>)e.Node.Tag;
                if (!handler.updateMetadata(node, e.Label))
                    System.Windows.Forms.MessageBox.Show("update failed.");
            }
        }
        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>       

        #endregion
    }
}
