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
// Copyright (C) Siemens AG 2017    
//------------------------------------------------------------------------ 
// Project           : UIGrid
// Author            : Sandhra.Prakash@siemens.com
// In Charge for Code: Sandhra.Prakash@siemens.com
//------------------------------------------------------------------------ 

/*Changes :
 * 1. OnPaint : Draw method signature is changed. as the IBorder Draw method was changed to add the 
                part of border which needs to be drawn. Change needs to be reflected here as well
*/
#endregion Copyright

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevAge.Drawing;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Summary description for EditableControlBase.
	/// </summary>
	public class EditableControlBase : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EditableControlBase()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserMouse, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, false);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			BackColor = DefaultColor;

            mContainer.Background = mBackground;
            mContainer.Border = mEditablePanel;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

        private DevAge.Drawing.VisualElements.Container mContainer = new DevAge.Drawing.VisualElements.Container();
        private DevAge.Drawing.VisualElements.EditablePanelThemed mEditablePanel = new DevAge.Drawing.VisualElements.EditablePanelThemed();
        private DevAge.Drawing.VisualElements.BackgroundSolid mBackground = new DevAge.Drawing.VisualElements.BackgroundSolid();

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

            using (DevAge.Drawing.GraphicsCache cache = new DevAge.Drawing.GraphicsCache(e.Graphics, e.ClipRectangle))
            {
                //sandhra.prakash@siemens.com: as the IBorder Draw method was changed to add the 
                //part of border which needs to be drawn. Change needs to be reflected here as well
                mEditablePanel.Draw(cache, ClientRectangle, BorderPartType.All);
            }
		}

		private static Color DefaultColor = System.Drawing.Color.FromKnownColor( System.Drawing.KnownColor.Window );
		[DefaultValue( typeof(Color), "Window")]
		public new Color BackColor
		{
			get{return base.BackColor;}
            set { mBackground.BackColor = value; base.BackColor = value; }
		}

        [DefaultValue(DevAge.Drawing.BorderStyle.System)]
        public new DevAge.Drawing.BorderStyle BorderStyle
		{
            get { return mEditablePanel.BorderStyle; }
            set { mEditablePanel.BorderStyle = value; OnBorderStyleChanged(EventArgs.Empty); }
		}

		protected virtual void OnBorderStyleChanged(EventArgs e)
		{
			Invalidate();
		}

		public override Rectangle DisplayRectangle
		{
			get
			{
                using (DevAge.Drawing.MeasureHelper measure = new DevAge.Drawing.MeasureHelper(this))
                {
                    return Rectangle.Round(mContainer.GetContentRectangle(measure, base.DisplayRectangle));
                }
			}
		}

		protected void SetContentAndButtonLocation(System.Windows.Forms.Control content, System.Windows.Forms.Control rightButton)
		{
			Rectangle displayRectangle = DisplayRectangle;

			rightButton.Bounds = new Rectangle(displayRectangle.Right - 18, 
				displayRectangle.Y, 
				18, 
				displayRectangle.Height);

            int width = rightButton.Location.X - DisplayRectangle.X;
            content.Bounds = new Rectangle(displayRectangle.Location, new Size(width, displayRectangle.Height));
		}
	}


}
