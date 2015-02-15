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
using System.Windows.Forms;

namespace QS._qss_x_.Administrative_Console_
{
    public class NamespaceNode : TreeNode
    {
        #region Constructor

        public NamespaceNode(Service_Old_.IService service, ulong identifier)
        {
            this.service = service;
            this.identifier = identifier;
            _Refresh(true, true);
        }

        #endregion

        #region Fields

        private Service_Old_.IService service;
        private ulong identifier;
        private int category;
        private string name;
        private bool isfolder, hassubfolders;

        #endregion

        #region _Refresh

        public void _Refresh(bool populate, bool recursively)
        {
            QS._qss_x_.Service_Old_.ObjectInfo info; 
            service.NamespaceGetObjectInfo(identifier, out info);
            Text = info.Name;
            ImageIndex = (int) info.Category;
            SelectedImageIndex = (int) info.Category;
#if !MONO
            ToolTipText = "id = " + identifier.ToString("x");
#endif
            Nodes.Clear();
            if (populate && info.IsContainer && info.HasObjects)
            {
                ulong[] ids;
                service.NamespaceGetSubObjects(identifier, out ids);
                foreach (ulong id in ids)
                {
                    NamespaceNode subnode = new NamespaceNode(service, id);
                    Nodes.Add(subnode);
                    subnode._Refresh(recursively, recursively);
                }
                Expand();
            }
        }

        #endregion

        #region _Select

        public void _Select()
        {
        }

        #endregion

        #region _Deselect

        public void _Deselect()
        {
#if !MONO
            ContextMenu = null;
#endif
        }

        #endregion

        #region _Menu

        public void _Menu(System.Drawing.Point location)
        {
            QS._qss_x_.Service_Old_.ActionInfo[] infos;
            service.NamespaceGetActionInfos(identifier, out infos);
            System.Windows.Forms.ContextMenu menu = new ContextMenu();
            foreach (QS._qss_x_.Service_Old_.ActionInfo info in infos)
            {
                _Action _action = new _Action(this, info.Name, info.Identifier, info.Context);
                _action.Click += new EventHandler(_action._ClickCallback);
                menu.MenuItems.Add(_action);
            }
#if !MONO
            ContextMenu = menu;
            ContextMenu.Show(TreeView, location);
#endif
        }        

        #endregion

        #region Class _Action

        private class _Action : MenuItem
        {
            public _Action(NamespaceNode node, string name, ulong identifier, ulong context)
                : base(name)
            {
                this.node = node;
                this.name = name;
                this.identifier = identifier;
                this.context = context;
            }

            private NamespaceNode node;
            private string name;
            private ulong identifier, context;

            public void _ClickCallback(object sender, EventArgs e)
            {
#if !MONO
                node.ContextMenu = null;
#endif
                node.service.NamespaceInvokeAction(node.identifier, identifier, context);
            }
        }

        #endregion
    }
}
