
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using SourceGrid.Cells;  

namespace SourceGrid
{
   public partial class Grid
   {
      /// <summary>
      /// Overriden to return a custom accessibility object instance. 
      /// </summary>
      /// <returns>AccessibleObject for the grid control.</returns>
      protected override AccessibleObject CreateAccessibilityInstance()
      {
         return new GridAccessibleObject(this); 
      }

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

/*Changes in class GridRowAccessibleObject:
* 1. Grid property was made protected so that it is accessible in child classes.
*/
#endregion Copyright

      /// <summary>
      /// Provides information about the grid control in the acessibility tree. 
      /// </summary>
      public class GridAccessibleObject : Control.ControlAccessibleObject
      {
          /// <summary>
          /// Gets the Grid
          /// sandhra.prakash@siemens.com : Grid property was made protected so 
          /// that it is accessible in child classes.
          /// </summary>
          protected Grid Grid { get; private set; }

         /// <summary>
         /// Creates a new GridAccessibleObject
         /// </summary>
         /// <param name="owner">Owner</param>
         public GridAccessibleObject(Grid owner) : base(owner)
         {
            Grid = owner;
         }

         /// <summary>
         /// Returns the role of the grid control
         /// </summary>
         public override AccessibleRole Role
         {
            get { return AccessibleRole.Table; }
         }

         /// <summary>
         /// Retrieves the accessible name of the grid control
         /// </summary>
         public override string Name
         {
            get { return Grid.Name; } 
         }

         /// <summary>
         /// Retrieves the accessibility object of a row in the grid.  
         /// </summary>
         /// <param name="index">Index of child</param>
         /// <returns>AccessibleObject of a child element</returns>
         public override AccessibleObject GetChild(int index)
         {
            return new GridRowAccessibleObject(Grid.Rows[index], this); 
         }

         /// <summary>
         /// Returns the number of children
         /// </summary>
         /// <returns>The number of rows in the grid.</returns>
         public override int GetChildCount()
         {
            return Grid.RowsCount; 
         }
      }

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

/*Changes in class GridRowAccessibleObject:
* 1. System.Drawing.Rectangle Bounds: Sandhra.prakash@siemens.com: 
    hscroll and vscroll value can be negative so before using it as index for 
    list of columns or list of rows check is added for negative numbers. Else
    application will crash.
 2. GridRow property was made protected so that it is accessible in child classes.
*/
#endregion Copyright

      /// <summary>
      /// Provides for information about grid rows in the acessibility tree. 
      /// </summary>
      public class GridRowAccessibleObject : AccessibleObject
      {
          /// <summary>
          /// Gets the GridRow
          /// sandhra.prakash@siemens.com : GridRow property was made protected so 
          /// that it is accessible in child classes.
          /// </summary>
          protected GridRow GridRow { get; private set; }
         private GridAccessibleObject parent;

         /// <summary>
         /// Creates a new GridRowAccessibleObject
         /// </summary>
         /// <param name="gridRow">The grid row</param>
         /// <param name="parent">The grid row's parent grid</param>
         public GridRowAccessibleObject(GridRow gridRow, GridAccessibleObject parent)
            : base()
         {
            this.GridRow = gridRow;
            this.parent = parent; 
         }

         /// <summary>
         /// Gets the bounding rectangle in screen coordinates. 
         /// </summary>
         public override System.Drawing.Rectangle Bounds
         {
            get
            {
               // Return empty rectangle if row is not visible 
               if (!GridRow.Visible)
                  return System.Drawing.Rectangle.Empty;
               
               int vScroll = 0;
               if (GridRow.Grid.VerticalScroll.Enabled)
                  vScroll = GridRow.Grid.VScrollBar.Value;

               // Check if the row is before the current scroll position 
               if (GridRow.Index < vScroll)
                  return System.Drawing.Rectangle.Empty; 
               
               // Get the row height 
               int rowHeight = GridRow.Height;

               int hScroll = 0;
               if (GridRow.Grid.HorizontalScroll.Enabled)
                  hScroll = GridRow.Grid.HScrollBar.Value;

               // Calculate row width 
               int rowWidth = 0;
               for (int i = hScroll; i < GridRow.Grid.Columns.Count; i++)
               {
                   //Sandhra.prakash@siemens.com : Check added so that column index will not be negative.
                   if (i < 0)
                       continue;
                  GridColumns cols = (GridColumns) GridRow.Grid.Columns;
                  rowWidth += cols[i].Width; 
               }
               
               // Get the row x position 
               int x = parent.Bounds.X; 

               // Calculate the row y position
               int y = parent.Bounds.Top; 
               for (int i = vScroll; i < GridRow.Index; i++)
               {
                   //Sandhra.prakash@siemens.com : Check added so that row index will not be negative.
                   if (i < 0)
                       continue;

                  GridRows rows = (GridRows)GridRow.Grid.Rows;
                  y += rows[i].Height;
               }
               
               System.Drawing.Rectangle rowRect = new System.Drawing.Rectangle(x, y, rowWidth, rowHeight);

               // Double check if row is inside current grid view 
               if (parent.Bounds.IntersectsWith(rowRect)) 
                  return rowRect;
               else 
                  return System.Drawing.Rectangle.Empty;
            }
         }

         /// <summary>
         /// Returns the name of the grid row. 
         /// </summary>
         public override string Name
         {
            get { return "Row " + GridRow.Index; }
         }

         /// <summary>
         /// Returns the role of the grid row.  
         /// </summary>
         public override AccessibleRole Role
         {
            get { return AccessibleRole.Row; }
         }

         /// <summary>
         /// Returns the parent grid of the grid row.  
         /// </summary>
         public override AccessibleObject Parent
         {
            get { return parent; }
         }

         /// <summary>
         /// Returns a child cell of the row.  
         /// </summary>
         /// <param name="index">Index of child</param>
         /// <returns>AccessibleObject of a child element</returns>
         public override AccessibleObject GetChild(int index)
         {
            ICellVirtual[] cells = GridRow.Grid.GetCellsAtRow(GridRow.Index);
            if (index < cells.Length)
            {
               Cell cell = (Cell)cells[index];

               if (cell == null)
                  return null; 

               return new GridRowCellAccessibleObject(cell, this);
               
            }
            else
            {
               Cell cell = (Cell)cells[cells.Length - 1];
               return new GridRowCellAccessibleObject(cell, this); 
            }
         }

         /// <summary>
         /// Returns the number of children. 
         /// </summary>
         /// <returns>Number of cells in a row</returns>
         public override int GetChildCount()
         {
            return GridRow.Grid.GetCellsAtRow(GridRow.Index).Length; 
         }
      }

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

/*Changes in class GridRowAccessibleObject:
* 1. Cell property was made protected so that it is accessible in child classes.
*/
#endregion Copyright

      /// <summary>
      /// Provides information about grid cells in the accessibility tree.  
      /// </summary>
      public class GridRowCellAccessibleObject : AccessibleObject
      {
          /// <summary>
          /// Gets the cell 
          /// sandhra.prakash@siemens.com :Cell property was made protected so that 
          /// it is accessible in child classes.
          /// </summary>
          protected Cell Cell { get; private set; }
         GridRowAccessibleObject parent; 

         /// <summary>
         /// Creates a new GridRowCellAccessibleObject
         /// </summary>
         /// <param name="cell">The grid cell</param>
         /// <param name="parent">The parent row of the cell</param>
         public GridRowCellAccessibleObject(Cell cell, GridRowAccessibleObject parent) : base()
         {
            this.Cell = cell;
            this.parent = parent;
         }

         /// <summary>
         /// Gets the bounding rectange in screen coordinates. 
         /// </summary>
         public override System.Drawing.Rectangle Bounds
         {
            get
            {
               // If the parent row isn't visible, neither is the cell
               if (parent.Bounds == System.Drawing.Rectangle.Empty)
                  return System.Drawing.Rectangle.Empty;

               int hScroll = 0;
               if (Cell.Grid.HorizontalScroll.Enabled)
                  hScroll = Cell.Grid.HScrollBar.Value;

               // If we scrolled horizontally past the cell it won't be visible anymore
               if (Cell.Column.Index < hScroll)
                  return System.Drawing.Rectangle.Empty;

               int cellWidth = Cell.Column.Width;
               int cellHeight = Cell.Row.Height;

               int x = parent.Bounds.X;

               
               for (int i = hScroll; i < Cell.Column.Index; i++)
               {
                  GridColumns cols = (GridColumns)Cell.Grid.Columns;
                  x += cols[i].Width;
               }

               int y = parent.Bounds.Y;

               System.Drawing.Rectangle cellRect = new System.Drawing.Rectangle(x, y, cellWidth, cellHeight); 

               // Double check cell if cell is inside current grid view
               if (parent.Bounds.IntersectsWith(cellRect))
                  return cellRect;
               else 
                  return System.Drawing.Rectangle.Empty;
            }
         }

         /// <summary>
         /// Returns the role of the grid cell. 
         /// </summary>
         public override AccessibleRole Role
         {
            get { return AccessibleRole.Cell; }
         }

         /// <summary>
         /// Returns the name of the cell.
         /// </summary>
         /// <remarks>
         /// If display text not found, returns a default name containing the column index.
         /// </remarks>
         public override string Name
         {
            get
            {
               if (Cell.DisplayText != null)
                  return Cell.DisplayText;
               return "Column " + Cell.Column.Index; 
            }
         }

         /// <summary>
         /// Set or get the value of the cell.
         /// </summary>
         public override string Value
         {
            get { return Cell.DisplayText; }
            set { Cell.Value = value; }
         }

         /// <summary>
         /// Returns the parent row of the cell. 
         /// </summary>
         public override AccessibleObject Parent
         {
            get { return parent; }
         }
      }
   }
}
