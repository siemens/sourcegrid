#region Copyright
/*SourceGrid LICENSE (MIT style)

Copyright (c) 2005 - 2012 http://sourcegrid.codeplex.com/, Davide Icardi, Darius Damalakas

Permission is hereby granted, free of charge, to any person obtaining 
a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE. */

//------------------------------------------------------------------------ 
// Copyright (C) Siemens AG 2016    
//------------------------------------------------------------------------ 
// Project           : UIGrid
// Author            : Sandhra.Prakash@siemens.com
// In Charge for Code: Sandhra.Prakash@siemens.com
//------------------------------------------------------------------------ 

/*Changes :
 * 1. ApplyToolTipText(CellContext sender, EventArgs e) -  defect fix- If ToolTipTitle is updated dynamically
                    the tooltip was not getting displayed. it is because Grid.ToolTipText is used
                    to show tooltip on the screen, it calls SetToolTip function of ToolTip.
                    Immediately after that the tooltip title was being set which hides the tooltip.
                   
*/
#endregion Copyright

using System;

namespace SourceGrid.Cells.Controllers
{
	/// <summary>
	/// Allow to customize the tooltiptext of a cell. This class read the tooltiptext from the ICellToolTipText.GetToolTipText.  This behavior can be shared between multiple cells.
	/// </summary>
	public class ToolTipText : ControllerBase
	{
		/// <summary>
		/// Default tooltiptext
		/// </summary>
		public readonly static ToolTipText Default = new ToolTipText();

		#region IBehaviorModel Members
		public override void OnMouseEnter(CellContext sender, EventArgs e)
		{
			base.OnMouseEnter(sender, e);

			ApplyToolTipText(sender, e);
            
		}

		public override void OnMouseLeave(CellContext sender, EventArgs e)
		{
			base.OnMouseLeave(sender, e);

			ResetToolTipText(sender, e);
		}
		#endregion

        private string mToolTipTitle = string.Empty;
        public string ToolTipTitle
        {
            get { return mToolTipTitle; }
            set { mToolTipTitle = value; }
        }

        private System.Windows.Forms.ToolTipIcon mToolTipIcon = System.Windows.Forms.ToolTipIcon.None;
        public System.Windows.Forms.ToolTipIcon ToolTipIcon
        {
            get { return mToolTipIcon; }
            set { mToolTipIcon = value; }
        }

        private bool mIsBalloon = false;
        public bool IsBalloon
        {
            get { return mIsBalloon; }
            set { mIsBalloon = value; }
        }

        private System.Drawing.Color mBackColor = System.Drawing.Color.Empty;
        public System.Drawing.Color BackColor
        {
            get { return mBackColor; }
            set { mBackColor = value; }
        }
        private System.Drawing.Color mForeColor = System.Drawing.Color.Empty;
        public System.Drawing.Color ForeColor
        {
            get { return mForeColor; }
            set { mForeColor = value; }
        }


		/// <summary>
		/// Change the cursor with the cursor of the cell
		/// </summary>
		protected virtual void ApplyToolTipText(CellContext sender, EventArgs e)
		{
			Models.IToolTipText toolTip;
			if ( (toolTip = (Models.IToolTipText)sender.Cell.Model.FindModel(typeof(Models.IToolTipText))) != null)
			{
                string text = toolTip.GetToolTipText(sender);
                if (text != null && text.Length > 0)
                {
                    sender.Grid.ToolTip.ToolTipTitle = ToolTipTitle;
                    sender.Grid.ToolTip.ToolTipIcon = ToolTipIcon;
                    sender.Grid.ToolTip.IsBalloon = IsBalloon;
                    if (BackColor.IsEmpty == false)
                        sender.Grid.ToolTip.BackColor = BackColor;
                    if (ForeColor.IsEmpty == false)
                        sender.Grid.ToolTip.ForeColor = ForeColor;

                    //sandhra.prakash@siemens.com: defect fix- If ToolTipTitle is updated dynamically
                    //the tooltip was not getting displayed. it is because Grid.ToolTipText is used
                    //to show tooltip on the screen, it calls SetToolTip function of ToolTip.
                    //Immediately after that the tooltip title was being set which hides the tooltip.
                    sender.Grid.ToolTipText = text;
                }
			}
		}

		/// <summary>
		/// Reset the original cursor
		/// </summary>
		protected virtual void ResetToolTipText(CellContext sender, EventArgs e)
		{
			if ( sender.Cell.Model.FindModel(typeof(Models.IToolTipText)) != null)
			{
                sender.Grid.ToolTipText = null;
			}
		}
	}
}
