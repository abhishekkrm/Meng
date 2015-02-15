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

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

#endregion

namespace QS.GUI.Components
{
	public partial class ComponentSelector : UserControl
	{
		public ComponentSelector()
		{
			InitializeComponent();
		}

		private LogWindow logWindow;
		private QS._qss_e_.Management_.IManagedComponent managedComponent;

		public LogWindow LogWindow
		{
			get { return logWindow; }
			set { logWindow = value; }
		}

//		public void Refresh()
//		{
//			managedComponent.Rebuild();
//		}

		public QS._qss_e_.Management_.IManagedComponent ManagedComponent
		{
			get { return managedComponent; }
			set 
			{
				select(null);

				managedComponent = value;
				treeView1.BeginUpdate();

				treeView1.Nodes.Clear();

				if (managedComponent != null)
					treeView1.Nodes.Add(new ComponentNode(managedComponent));

				treeView1.EndUpdate();
			}
		}

		private ComponentNode currentlySelected = null;
		private void select(ComponentNode node)
		{
			if (currentlySelected != null)
			{
				if (currentlySelected.ManagedComponent.Log != null)
					currentlySelected.ManagedComponent.Log.Console = null;
			}

			currentlySelected = node;
			if (currentlySelected != null)
			{
				logWindow.Clear();
				if (currentlySelected.ManagedComponent.Log != null)
					currentlySelected.ManagedComponent.Log.Console = logWindow;
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			select(e.Node as ComponentNode);
		}

		#region Class CompomentNode

		private class ComponentNode : TreeNode
		{
			public ComponentNode(QS._qss_e_.Management_.IManagedComponent managedComponent)
				: base(managedComponent.Name)
			{
				this.managedComponent = managedComponent;
				Rebuild();
			}

			public void Rebuild()
			{
				Name = managedComponent.Name;

				if (managedComponent.Subcomponents != null && managedComponent.Subcomponents.Length > 0)
				{
					ImageIndex = 0;
					SelectedImageIndex = 1;
				}
				else
				{
					ImageIndex = SelectedImageIndex = 2;
				}

				Nodes.Clear();

				if (managedComponent.Subcomponents != null)
				{
					foreach (QS._qss_e_.Management_.IManagedComponent subcomponent in managedComponent.Subcomponents)
					{
						if (subcomponent != null)
							Nodes.Add(new ComponentNode(subcomponent));
					}
				}
			}

			private QS._qss_e_.Management_.IManagedComponent managedComponent;

			public QS._qss_e_.Management_.IManagedComponent ManagedComponent
			{
				get { return managedComponent; }
			}
		}

		#endregion

		private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				treeView1.BeginUpdate();
				((ComponentNode)treeView1.SelectedNode).Rebuild();
				treeView1.EndUpdate();
			}
		}
	}
}
