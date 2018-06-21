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
 * 1. Draw(RangePaintEventArgs e): sandhra.prakash@siemens.com: the border should be drawn even when editing.
 * 2. Draw(RangePaintEventArgs e): To support freezePanes enahancement.
 * 3. Draw(RangePaintEventArgs e): Transform is applied to focusRect and rectToDraw, otherwise drawing wont be proper incase of smoothscrolling
*/
#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevAge.Drawing;
using SourceGrid.Cells.Editors;

namespace SourceGrid.Decorators
{
	public class DecoratorSelection : DecoratorBase
	{
		public DecoratorSelection(Selection.SelectionBase selection)
		{
			mSelection = selection;
		}

		private Selection.SelectionBase mSelection;

		public override bool IntersectWith(Range range)
		{
			return mSelection.IntersectsWith(range);
		}

	    public override void Draw(RangePaintEventArgs e)
	    {
	        RangeRegion region = mSelection.GetSelectionRegion();

	        if (region.IsEmpty())
	            return;

            //sandhra.prakash@siemens.com: To support freezePanes enahancement
	        // get visible range for data area
            Range dataRange = mSelection.Grid.CompleteRange;
            
            System.Drawing.Brush brush = e.GraphicsCache.BrushsCache.GetBrush(mSelection.BackColor);

	        CellContext focusContext = new CellContext(e.Grid, mSelection.ActivePosition);
	        // get focus rectangle
	        // clipped to visible range
            Range focusClippedRange = dataRange.Intersect(new Range(mSelection.ActivePosition, mSelection.ActivePosition));
	        System.Drawing.Rectangle focusRect = e.Grid.PositionToVisibleRectangle(focusClippedRange.Start);

            //sandhra.prakash@siemens.com: To support smooth scrolling. Otherwise transform will not be applied.
            focusRect.X -= (int)e.GraphicsCache.Graphics.Transform.OffsetX;
            focusRect.Y -= (int)e.GraphicsCache.Graphics.Transform.OffsetY;

	        bool isFocusCellVisible = e.Grid.IsCellVisible(focusContext.Position, true);
	        //Draw each selection range
	        foreach (Range rangeToLoop in region)
	        {
	            // intersect given range with visible range
	            // this way we ensure we don't loop through thousands
	            // of rows to calculate rectToDraw
                Range rng = dataRange.Intersect(rangeToLoop);

                //sandhra.prakash@siemens.com: To support freezePanes enahancement
	            System.Drawing.Rectangle rectToDraw = e.Grid.RangeToVisibleRectangle(rng);

                //sandhra.prakash@siemens.com: To support smooth scrolling. Otherwise transform will not be applied.
                rectToDraw.X -= (int)e.GraphicsCache.Graphics.Transform.OffsetX;
                rectToDraw.Y -= (int)e.GraphicsCache.Graphics.Transform.OffsetY; 

	            if (rectToDraw == System.Drawing.Rectangle.Empty)
	                continue;

	            System.Drawing.Region regionToDraw = new System.Drawing.Region(rectToDraw);

                Range cellRange = e.Grid.PositionToCellRange(focusContext.Position);
                //sandhra.prakash@siemens.com: To support freezePanes enahancement: Only if the cell is visible exclude it. It can be hidden under a fixedrow\column
                if (rectToDraw.IntersectsWith(focusRect) && (isFocusCellVisible || rng.Contains(cellRange)))
	                regionToDraw.Exclude(focusRect);

	            e.GraphicsCache.Graphics.FillRegion(brush, regionToDraw);

	            var partType = GetBorderType(e.Grid, rng);

	            //Draw the border only if there isn't a editing cell
	            // and is the range that contains the focus or there is a single range
	            if ((rng.Contains(mSelection.ActivePosition) || region.Count == 1) && partType != BorderPartType.None)
	            {
	                //if (!focusContext.IsEditing())
	                //sandhra.prakash@siemens.com: the border should be drawn even when editing, 

                    mSelection.Border.Draw(e.GraphicsCache, rectToDraw, partType);

	            }
	        }

            //sandhra.prakash@siemens.com: To support freezePanes enahancement: Only if the cell is visible exclude it. It can be hidden under a fixedrow\column
            if (isFocusCellVisible) 
	        //Draw Focus
            {
	            System.Drawing.Brush brushFocus = e.GraphicsCache.BrushsCache.GetBrush(mSelection.FocusBackColor);
	            e.GraphicsCache.Graphics.FillRectangle(brushFocus, focusRect);
	        }
           
	    }

        private static BorderPartType GetBorderType(GridVirtual grid, Range rng)
        {
            //sandhra.prakash@siemens: To optimise calculations.
            return grid.ScrollingStyle.GetBorderType(rng);          
	        
	    }
	}
}
