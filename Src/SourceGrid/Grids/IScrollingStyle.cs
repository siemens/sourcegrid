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
 * 
*/
#endregion Copyright

using System.Collections.Generic;
using System.Drawing;
using DevAge.Drawing;

namespace SourceGrid
{
    public interface IScrollingStyle
    {
        int? FirstVisibleScrollableRow { get; }
        int? LastVisibleScrollableRow { get; }
        int? FirstVisibleScrollableColumn { get;  }
        int? LastVisibleScrollableColumn { get; }

        void CustomScrollPageUp(int line);
        void CustomScrollPageDown(int line);

        Range GetFixedLeftRange();

        Range GetFixedTopRange();
        Range GetScrollableRange();
        bool IsColumnHiddenUnderFixedColumn(int column, bool includePartial);

        bool IsRowHiddenUnderFixedRows(int rowIndex, bool includePartial);
        void CustomScrollLineDown();
        void CustomScrollLineRight();

        /// <summary>
        /// Return the scroll position that must be set to show a specific cell.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="visibleRows"></param>
        /// <param name="newScrollPosition"></param>
        /// <param name="visibleColumns"></param>
        /// <returns>Return false if the cell is already visible, return true is the cell is not currently visible.</returns>
        bool GetScrollPositionToShowCell(Position position, List<int> visibleColumns, List<int> visibleRows,
            out Point newScrollPosition);

        void RecalcHScrollBar(int cols);
        void RecalcVScrollBar(int rows);

        void Paint(System.Windows.Forms.PaintEventArgs e);
        void PaintMergedCell(GraphicsCache graphics, Range cellRange, CellContext cellContext);
        
        List<int> ColumnsInsideRegion(int x, int width, bool returnsPartial, bool returnsFixedColumns);
        List<int> RowsInsideRegion(int y, int height, bool returnsPartial, bool returnsFixedRows);
        
        int GetLeft(int column);
        int GetTop(int row);

        Rectangle RangeToVisibleRectangle(Range range);

        BorderPartType GetBorderType(Range rng);

        void ScrollOnPoint(Point mousePoint);
    }
}