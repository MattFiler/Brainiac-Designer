////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Brainiac.Design
{
	internal partial class ErrorCheckDialog : Form
	{
		private BehaviorTreeList _behaviorTreeList= null;
		internal BehaviorTreeList BehaviorTreeList
		{
			get { return _behaviorTreeList; }
			set { _behaviorTreeList= value; }
		}

		private BehaviorTreeView _behaviorTreeView= null;
		internal BehaviorTreeView BehaviorTreeView
		{
			get { return _behaviorTreeView; }
			set { _behaviorTreeView= value; }
		}

		internal ErrorCheckDialog()
		{
			InitializeComponent();
		}

		private void listView_DoubleClick(object sender, EventArgs e)
		{
			// check if there is an item selected
			if(listView.SelectedItems.Count <1)
				return;

			// check if this item has a node connected to it. The no-errors message doesn't
			Nodes.BehaviorNode node= (Nodes.BehaviorNode) listView.SelectedItems[0].Tag;
			if(node ==null)
				return;

			// show the behaviour and select the node.
			NodeViewData nvd= _behaviorTreeList.ShowNode((Nodes.Node)node);
			_behaviorTreeView.SelectedNode= nvd;
		}
	}
}