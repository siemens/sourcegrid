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
 * 1. OnMouseMove(CellContext sender, MouseEventArgs e): Sandhra.prakash@siemens.com: 
 this was added to avoid the automatic scrolling issue that occurs if drag and drop is executed.
 2. OnMouseDown : Defect Fix - After using Shift to select continuous cells
            click on one of the selected to cell to initate drag and drop, this will reset 
            the selection. To avoid resetting of selection and enable autoscrolling.
 3. mScrollTimer_Tick : requirement from syscon to stop autoscroll if 
            cursor is moved out of the bounds of the grid.
 4. m_DragSizeRectangle - Damps unwanted drag movements
 5. OnMouseDown : when change is made to accomodate FreezePanes, 
                scrolltracking should be enabled to the whole dataArea. and not just scrollrect
 6. OnMouseMove: when user defined freezePane enhancement is done, 
            the first visible row and col is the first datacells. which can be scrolled into 
 7. OnMouseMove : sandhra.prakash@siemens.com: if the below if condition is not commented, DragSelection of datacells which is inside fixedRow or fixedColumn will not work
 8. ScrollTimer_Tick : To correct the issue: make fixedRows\FixedColumns extend to the dataCells. 
            Now move the vertical\horizontal scrollbar to hide some of the datacells inside the fixedrows\fixedcolumns.
            Click on the datacell that is in the fixedTop\fixedLeft\FixedTopLeft area. This will start autoscrolling.
 9. OnMouseMove - need not depend on LastVisibleScrollableRow\LastVisibleScrollableColumn. as it seems to be unnessecary\redundant.
*/
#endregion Copyright


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SourceGrid.Cells.Controllers
{
    /// <summary>
    /// A cell controller used to handle mouse selection
    /// </summary>
    public class MouseSelection : ControllerBase
    {
        //public static MouseSelection Default = new MouseSelection();

        /// <summary>
        /// Controls which mouse buttons invoke mouse selection.
        /// Default is MouseButtons.Left
        /// </summary>
        public MouseButtons MouseButtons { get; set; }

        
        /// <summary>
        /// Sandhra.prakash@siemens.com: Damps unwanted drag movements
        /// </summary>
        private Rectangle m_DragSizeRectangle;

        public MouseSelection()
        {
            MouseButtons = MouseButtons.Left;
        }


        public override void OnMouseDown(CellContext sender, MouseEventArgs e)
        {
            base.OnMouseDown(sender, e);

            var pressed = e.Button & MouseButtons;
            if (pressed == MouseButtons.None)
            {
                return;
            }

            //sandhra.prakash@siemens.com: Remember the point where the mouse down occurred. The DragSize indicates
            // the size that the mouse can move before a drag event should be started.  
            Size dragSize = SystemInformation.DragSize;

            // sandhra.prakash@siemens.com: Create a rectangle using the DragSize, with the mouse position being
            // at the center of the rectangle.
            m_DragSizeRectangle = new Rectangle(
                new Point(e.X - SystemInformation.DragSize.Width / 2,
                    e.Y - SystemInformation.DragSize.Height / 2), dragSize);

            GridVirtual grid = sender.Grid;
            
            //Sandhra.prakash@siemens.com: To Fix autoscrolling issue on pressing shift and then moving mouse
            //after releasing shift
            Range rg = new Range(sender.Position);
            bool inRange = false;
            if (grid.Selection.GetSelectionRegion() != null && grid.Selection.GetSelectionRegion().GetCellsPositions().Count > 1)
            {
                inRange = grid.Selection.GetSelectionRegion().IntersectsWith(rg);// return;
            }
            //Check the control and shift key status
            bool controlPress = ((Control.ModifierKeys & Keys.Control) == Keys.Control &&
                                 (grid.SpecialKeys & GridSpecialKeys.Control) == GridSpecialKeys.Control);

            bool shiftPress = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift &&
                               (grid.SpecialKeys & GridSpecialKeys.Shift) == GridSpecialKeys.Shift);
            
            //Sandhra.prakash@siemens.com: Defect Fix - After using Shift to select continuous cells
            //click on one of the selected cell to initate drag and drop, this will reset 
            //the selection. To avoid resetting of selection and enable autoscrolling.
            if (!shiftPress && !controlPress && inRange)
            {
                grid.Focus();

                
                if (grid.Selection.EnableDragSelection)
                {
                    //sandhra.prakash@siemens.com: this condition is necessary for 
                    //excel like selection in dataGrid
                    grid.Selection.ResetSelection(true);
                    grid.Selection.Focus(sender.Position, true);
                }

                /*sandhra.prakash@siemens.com: when change is made to accomodate FreezePanes, 
                //scrolltracking should be enabled to the whole dataArea. and not just scrollrect */
                // begin scroll tracking only if mouse was clicked
                // in scrollable area
                Rectangle scrollRect1 = grid.GetDataArea();
                if (scrollRect1.Contains(e.Location))
                    BeginScrollTracking(grid);

                grid.Selection.Invalidate();
                return;
            }
            
            //Default click handler
            if (shiftPress == false ||
                grid.Selection.EnableMultiSelection == false)
            {
                //Handle Control key
                bool mantainSelection = grid.Selection.EnableMultiSelection && controlPress;// && inRange;

                //If the cell is already selected and the user has the ctrl key pressed then deselect the cell
                if (controlPress && grid.Selection.IsSelectedCell(sender.Position) && grid.Selection.ActivePosition != sender.Position)
                    grid.Selection.SelectCell(sender.Position, false);
                else
                    grid.Selection.Focus(sender.Position, !mantainSelection);
            }
            else //handle shift key
            {
                //sandhra.prakash@siemens.com: on pressing ctrl and then using shift it resets the selection - to avod this
                //grid.Selection.ResetSelection(true);
                Range rangeToSelect = new Range(grid.Selection.ActivePosition, sender.Position);
                grid.Selection.SelectRange(rangeToSelect, true);
            }

            /*sandhra.prakash@siemens.com: when change is made to accomodate FreezePanes, 
                //scrolltracking should be enabled to the whole dataArea. and not just scrollrect */
            // begin scroll tracking only if mouse was clicked
            // in scrollable area
            Rectangle scrollRect = grid.GetDataArea();
            if (scrollRect.Contains(e.Location))
                BeginScrollTracking(grid);

            grid.Selection.Invalidate();
        }


        public override void OnMouseUp(CellContext sender, MouseEventArgs e)
        {
            base.OnMouseUp(sender, e);
            
            var pressed = e.Button & MouseButtons;

            if (pressed == MouseButtons.None)
            {
               return;
            }

            //Sandhra.prakash@siemens.com: To Fix autoscrolling issue on pressing shift and then moving mouse
            //after releasing shift
            GridVirtual grid = sender.Grid;

            Range rg = new Range(sender.Position);
            bool inRange = false;
            if (grid.Selection.GetSelectionRegion() != null && grid.Selection.GetSelectionRegion().GetCellsPositions().Count > 1)
            {
                inRange = grid.Selection.GetSelectionRegion().IntersectsWith(rg);// return;
            }
            //Check the control and shift key status
            bool controlPress = ((Control.ModifierKeys & Keys.Control) == Keys.Control &&
                                 (grid.SpecialKeys & GridSpecialKeys.Control) == GridSpecialKeys.Control);

            bool shiftPress = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift &&
                               (grid.SpecialKeys & GridSpecialKeys.Shift) == GridSpecialKeys.Shift);

            //sandhra.prakash@siemens.com: The below one is not needed in the case of grid
            //with excel like selection. say dataGrid but is absolutely necessary for UIGrid.
            if (!shiftPress && !controlPress && inRange
                && !sender.Grid.Selection.EnableDragSelection)
            {
                grid.Selection.ResetSelection(true);
                grid.Selection.Focus(sender.Position, true);
            }
            sender.Grid.MouseSelectionFinish();

            EndScrollTracking();
        }

       
        /// <summary>
        /// Used for mouse multi selection and mouse scrolling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnMouseMove(CellContext sender, MouseEventArgs e)
        {
            var pressed = e.Button & MouseButtons;

            if (pressed == MouseButtons.None)
            {
                //Sandhra.prakash@siemens.com: this was added to avoid the automatic 
                //scrolling issue that occurs if drag and drop is executed.
                EndScrollTracking();
                //sandhra.prakash@siemens.com: dragSelection with right mouse button is not needed.
                return;
            }

            if (mCapturedGrid != null && mCapturedMouseMoveEventArgs != null)
            {
                mCapturedMouseMoveSender = CellContext.Empty;
                mCapturedMouseMoveEventArgs = null;
            }

            base.OnMouseMove(sender, e);

            //First check if the multi selection is enabled and the active position is valid
            if (sender.Grid.Selection.EnableMultiSelection == false ||
                sender.Grid.MouseDownPosition.IsEmpty() )
                return;

            //Sandhra.prakash@siemens.com: To Fix autoscrolling issue on pressing shift and then moving mouse
            //after releasing shift
            if (!sender.Grid.Selection.EnableDragSelection) { return; }

            //DragSizeRectangle check helps in correcting the issue - Click on an half visible cell. 
            //the grid scrolls into view making the cell visible, but also causes multiple selection 
            if (m_DragSizeRectangle.Contains(e.Location))
            {
                return;
            }

            //sandhra.prakash@siemens.com: lastVisibleRow and lastVisibleColumn is not needed here, because its unnecessary or redundant
            // preprare helper variables
            //int? lastVisibleRow = sender.Grid.Rows.LastVisibleScrollableRow;
            //int? lastVisibleColumn = sender.Grid.Columns.LastVisibleScrollableColumn;

            /*sandhra.prakash@siemens.com: when user defined freezePane enhancement is done, 
            the first visible row and col is the first datacells. which can be scrolled into */
            int? firstVisibleRow =  sender.Grid.HeaderRowCount;
            int? firstVisibleColumn = sender.Grid.HeaderColumnCount;

            //Check if the mouse position is valid and try to fix it if it's not
            int? row = sender.Grid.Rows.RowAtPoint(e.Y);
            int? col = sender.Grid.Columns.ColumnAtPoint(e.X);
            //sandhra.prakash@siemens.com: this check is unnecessary.
            //if (!row.HasValue)
            //{
            //    if (e.Y <= 0)
            //        row = firstVisibleRow;
            //    else
            //        row = lastVisibleRow;
            //}
            //if (!col.HasValue)
            //{
            //    if (e.X <= 0)
            //        col = firstVisibleColumn;
            //    else
            //        col = lastVisibleColumn;
            //}

            if (!col.HasValue || !row.HasValue)
                return;

            //// Fix mouse position so it does not go out of visible range
            //if (lastVisibleRow.HasValue && row.Value > lastVisibleRow.Value)
            //    row = lastVisibleRow;
            //if (lastVisibleColumn.HasValue && col.Value > lastVisibleColumn.Value)
            //    col = lastVisibleColumn;

            if (firstVisibleRow.HasValue && row < firstVisibleRow.Value)
                row = firstVisibleRow;
            if (firstVisibleColumn.HasValue && col < firstVisibleColumn.Value)
                col = firstVisibleColumn;

            Position mousePosition = new Position(row.Value, col.Value);
            
            /*sandhra.prakash@siemens.com: if the below if condition is not commented, DragSelection of datacells which is inside fixedRow or fixedColumn will not work*/
            //If the position type is different I don't continue
            // bacause this can cause problem for example when selection the fixed rows when the scroll is on a position > 0
            // that cause all the rows to be selected
            //if (sender.Grid.GetPositionType(mousePosition) !=
            //    sender.Grid.GetPositionType(sender.Grid.Selection.ActivePosition))
            //    return;
           
            sender.Grid.ChangeMouseSelectionCorner(mousePosition);
            sender.Grid.Selection.Invalidate();
            if (mCapturedGrid != null)
            {
                mCapturedMouseMoveSender = sender;
                mCapturedMouseMoveEventArgs = e;
            }
        }

        /// <summary>
        /// Ends scroll tracking on double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnDoubleClick(CellContext sender, EventArgs e)
        {
            base.OnDoubleClick(sender, e);

            EndScrollTracking();
        }

        private Timer mScrollTimer;
        private GridVirtual mCapturedGrid;
        private CellContext mCapturedMouseMoveSender = CellContext.Empty;
        private MouseEventArgs mCapturedMouseMoveEventArgs = null;

        /// <summary>
        /// Start the timer to scroll the visible area
        /// </summary>
        /// <param name="grid"></param>
        private void BeginScrollTracking(GridVirtual grid)
        {
            //grid.Capture = true;
            mCapturedGrid = grid;

            if (mScrollTimer == null)
            {
                mScrollTimer = new Timer();
                mScrollTimer.Interval = 100;
                mScrollTimer.Tick += this.mScrollTimer_Tick;
            }
            mScrollTimer.Enabled = true;
        }
        /// <summary>
        /// Stop the timer
        /// </summary>
        private void EndScrollTracking()
        {
            //grid.Capture = false;
            if (mScrollTimer != null)
                mScrollTimer.Enabled = false;
            mCapturedGrid = null;
            mCapturedMouseMoveEventArgs = null;
            mCapturedMouseMoveSender = CellContext.Empty;
        }

        private void mScrollTimer_Tick(object sender, EventArgs e)
        {
            if (mCapturedGrid == null)
                return;
            if (mCapturedGrid.IsDisposed)
            {
                EndScrollTracking();
                return;
            }

            //If grid has lost focus end scroll tracking
            if (!mCapturedGrid.Focused)
            {
                EndScrollTracking();
                return;
            }

            Point mousePoint = mCapturedGrid.PointToClient(Control.MousePosition);
            
            //sandhra.prakash@siemens.com : requirement from syscon to stop autoscroll if 
            //cursor is moved out of the bounds of the grid 
            Rectangle rectangle = new Rectangle(mCapturedGrid.Location,mCapturedGrid.Size);

            bool isPointInGrid = rectangle.Contains(mousePoint);
            if (!isPointInGrid && mCapturedGrid.IsCustomAreaAutoScrollEnabled)
            {
                return;
            }

            /*sandhra.prakash@siemens.com: To correct the issue: make fixedRows\FixedColumns extend to the dataCells. 
            Now move the vertical\horizontal scrollbar to hide some of the datacells inside the fixedrows\fixedcolumns.
            Click on the datacell that is in the fixedTop\fixedLeft\FixedTopLeft area. This will start autoscrolling.*/
            if (m_DragSizeRectangle.Contains(mousePoint))
            {
                return;
            }

            //Scroll the view if required
            mCapturedGrid.ScrollOnPoint(mousePoint);

            // If we are scrolling view fire mouse move event to change also selection
            if (mCapturedMouseMoveEventArgs != null)
            {
                this.OnMouseMove(mCapturedMouseMoveSender,
                                    new MouseEventArgs(mCapturedMouseMoveEventArgs.Button, mCapturedMouseMoveEventArgs.Clicks,
                                                    mousePoint.X, mousePoint.Y, mCapturedMouseMoveEventArgs.Delta));
            }
        }

    }
}
