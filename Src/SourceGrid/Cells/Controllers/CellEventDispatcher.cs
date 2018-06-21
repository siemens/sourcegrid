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
 1. OnKeyDown, OnKeyPress:For Enhancement: CheckBoxWithIcon Event needs to be available to checkbox controller, for it work as expected
*/
#endregion Copyright

using System;
using System.Windows.Forms;

namespace SourceGrid.Cells.Controllers
{
	/// <summary>
	/// This controller dispatch each event to the specified cell.
    /// This controller can be shared between multiple cells and is usually used as the default Grid.Controller. Removing this controller can cause unexpected behaviors.
	/// </summary>
    public class CellEventDispatcher : ControllerBase
	{
		/// <summary>
		/// The default behavior of a cell.
		/// </summary>
        public readonly static CellEventDispatcher Default = new CellEventDispatcher();

		public override void OnKeyDown (CellContext sender, KeyEventArgs e)
		{
			base.OnKeyDown(sender, e);

            //sandhra.prakash@siemens.com:For Enhancement: CheckBoxWithIcon Event needs to be available to checkbox controller, for it work as expected.
			if (sender.Cell != null && sender.Cell.Controller != null /*&& e.Handled == false*/)
				sender.Cell.Controller.OnKeyDown(sender, e);
		}

		public override void OnKeyPress (CellContext sender, KeyPressEventArgs e)
		{
			base.OnKeyPress(sender, e);

            //sandhra.prakash@siemens.com:For Enhancement: CheckBoxWithIcon Event needs to be available to checkbox controller, for it work as expected.
			if (sender.Cell != null && sender.Cell.Controller != null /*&& e.Handled == false*/)
				sender.Cell.Controller.OnKeyPress(sender, e);
		}

		public override void OnDoubleClick (CellContext sender, EventArgs e)
		{
			base.OnDoubleClick(sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnDoubleClick(sender, e);
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="sender"></param>
		/// <param name="e"></param>
		public override void OnClick (CellContext sender, EventArgs e)
		{
			base.OnClick(sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnClick(sender, e);
		}

		public override void OnFocusEntered(CellContext sender, EventArgs e)
		{
			base.OnFocusEntered(sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnFocusEntered(sender, e);
		}

		public override void OnFocusLeft(CellContext sender, EventArgs e)
		{
			base.OnFocusLeft (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnFocusLeft(sender, e);
		}


		/// <summary>
		/// Fired when the SetValue method is called.
		/// </summary>
		public override void OnValueChanged(CellContext sender, EventArgs e)
		{
			base.OnValueChanged(sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnValueChanged(sender, e);
		}

		/// <summary>
		/// Fired when editing is ended
		/// </summary>
		public override void OnEditEnded(CellContext sender, EventArgs e)
		{
			base.OnEditEnded (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnEditEnded(sender, e);
		}

		public override bool CanReceiveFocus(CellContext sender, EventArgs e)
		{
			bool canReceiveFocus = base.CanReceiveFocus (sender, e);
			if (canReceiveFocus == false)
				return false;

			if (sender.Cell == null)
				return false;
			
			if (sender.Cell.Controller != null)
				return sender.Cell.Controller.CanReceiveFocus(sender, e);

			return true;
		}

		public override void OnEditStarting(CellContext sender, System.ComponentModel.CancelEventArgs e)
		{
			base.OnEditStarting (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnEditStarting(sender, e);
		}

        public override void OnEditStarted(CellContext sender, EventArgs e)
        {
            base.OnEditStarted(sender, e);

            if (sender.Cell != null && sender.Cell.Controller != null)
                sender.Cell.Controller.OnEditStarted(sender, e);
        }

        public override void OnValueChanging(CellContext sender, ValueChangeEventArgs e)
        {
            base.OnValueChanging(sender, e);

            if (sender.Cell != null && sender.Cell.Controller != null)
                sender.Cell.Controller.OnValueChanging(sender, e);
        }

		public override void OnFocusEntering(CellContext sender, System.ComponentModel.CancelEventArgs e)
		{
			base.OnFocusEntering (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnFocusEntering(sender, e);
		}

		public override void OnFocusLeaving(CellContext sender, System.ComponentModel.CancelEventArgs e)
		{
			base.OnFocusLeaving (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnFocusLeaving(sender, e);
		}

		public override void OnKeyUp(CellContext sender, KeyEventArgs e)
		{
			base.OnKeyUp (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnKeyUp(sender, e);
		}

		public override void OnMouseDown(CellContext sender, MouseEventArgs e)
		{
			base.OnMouseDown (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnMouseDown(sender, e);
		}

		public override void OnMouseEnter(CellContext sender, EventArgs e)
		{
			base.OnMouseEnter (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnMouseEnter(sender, e);
		}

		public override void OnMouseLeave(CellContext sender, EventArgs e)
		{
			base.OnMouseLeave (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnMouseLeave(sender, e);
		}

		public override void OnMouseMove(CellContext sender, MouseEventArgs e)
		{
			base.OnMouseMove (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnMouseMove(sender, e);
		}

		public override void OnMouseUp(CellContext sender, MouseEventArgs e)
		{
			base.OnMouseUp (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnMouseUp(sender, e);
		}


	


		public override void OnDragDrop(CellContext sender, DragEventArgs e)
		{
			base.OnDragDrop (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnDragDrop(sender, e);
		}

		public override void OnDragEnter(CellContext sender, DragEventArgs e)
		{
			base.OnDragEnter (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnDragEnter(sender, e);
		}

		public override void OnDragLeave(CellContext sender, EventArgs e)
		{
			base.OnDragLeave (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnDragLeave(sender, e);
		}

		public override void OnDragOver(CellContext sender, DragEventArgs e)
		{
			base.OnDragOver (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnDragOver(sender, e);
		}

		public override void OnGiveFeedback(CellContext sender, GiveFeedbackEventArgs e)
		{
			base.OnGiveFeedback (sender, e);

			if (sender.Cell != null && sender.Cell.Controller != null)
				sender.Cell.Controller.OnGiveFeedback(sender, e);
		}
	}
}
