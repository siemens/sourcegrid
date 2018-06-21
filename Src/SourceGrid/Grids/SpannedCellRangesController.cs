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
 *1.  Update(Range newRange) : check ensures that if the once merged cell decided to split - that cell range is removed from collection
 * 2.  Update(Range newRange) : If there is no existing match add it to the collection rather than throwing exception.
 * 3. chinnari.prasad@siemens.com : ShrinkOrRemoveSpannedRows(int startIndex, int count) : Updated the calculation of row span
 * 4. UpdateOrAdd(Range newRange) :On trying to Update SpannedCellRangesController passes only start range. Hence oldRange is not the actual range which should be removed.
            scenerio : 1. Set a Cell's columnspan to 1. then after loading entire grid change the span to 2. Again after loading entire grid change the span to 3. 
             *Now try making all these cells null individually.(In UIGridExtender set displayController = null) Result - Crash
*/
#endregion Copyright

using QuadTreeLib;
using System;
using System.Collections.Generic;

namespace SourceGrid
{
	public class SpannedCellRangesController : ISpannedCellRangesController
	{
		Grid m_grid = null;
		
		public ISpannedRangesCollection SpannedRangesCollection {get; private set;}
		
		public Grid Grid {
			get { return m_grid; }
		}
		
		/// <summary>
		/// Adds or updates given range.
		/// Updates range only when existing range with given start position is found
		/// </summary>
		/// <param name="newRange"></param>
		public void UpdateOrAdd(Range newRange)
		{
			if (newRange.Equals(Range.Empty))
				throw new ArgumentException("Range can not be empty");
			Range? index = this.SpannedRangesCollection.FindRangeWithStart(newRange.Start);
		    if (index == null)
		        this.SpannedRangesCollection.Add(newRange);
		    else
		    {
                //sandhra.prakash@siemens.com: On trying to Update SpannedCellRangesController passes only start range. Hence oldRange is not the actual range which should be removed.
                /*scenerio : 1. Set a Cell's columnspan to 1. then after loading entire grid change the span to 2. Again after loading entire grid change the span to 3. 
                 *Now try making all these cells null individually.(In UIGridExtender set displayController = null) Result - Crash*/
		        Range? spannedRange = this.SpannedRangesCollection.FindRangeWithStart(newRange.Start);
		        if (spannedRange.HasValue)
		        {
                    this.SpannedRangesCollection.Update(spannedRange.Value, newRange);
		        }
		    }
		}
		
		/// <summary>
		/// Updates range whose start position matches.
		/// If no matches found, an exception is thrown
		/// //sandhra.prakash@siemens.com: If no match is found Add it to the range.
		/// </summary>
		/// <param name="newRange"></param>
		public void Update(Range newRange)
		{
			Range? index = this.SpannedRangesCollection.FindRangeWithStart(newRange.Start);
		    if (index == null)
		    {
                //sandhra.prakash@siemens.com: If no match is found Add it to the range.
                UpdateOrAdd(newRange);
                return;
		    }
            //sandhra.prakash@siemens.com: this ensures that if the once merged cell decided to split - that cell range is removed from collection
            if (newRange.Start == newRange.End)
                SpannedRangesCollection.Remove(index.Value);
            else
			    this.SpannedRangesCollection.Update(index.Value, newRange);
		}
		
		
		public SpannedCellRangesController(Grid grid, ISpannedRangesCollection spannedRangeCollection)
			:this(grid)
		{
			SpannedRangesCollection = spannedRangeCollection;
		}
		
		public SpannedCellRangesController(Grid grid)
		{
			this.m_grid = grid;
			SpannedRangesCollection = new SpannedRangesList();
		}
		
		public void MoveLeftSpannedRanges(int startIndex, int moveCount)
		{
			foreach (var range in this.SpannedRangesCollection.ToArray())
			{
				if (range.Start.Column <= startIndex)
					continue;
				var newRange = new Range(range.Start.Row, range.Start.Column - moveCount,
				                         range.End.Row, range.End.Column - moveCount);
				this.SpannedRangesCollection.Update(range, newRange);
			}
		}
		
		public void MoveUpSpannedRanges(int startIndex, int moveCount)
		{
			foreach (var range in this.SpannedRangesCollection.ToArray())
			{
				if (range.Start.Row <= startIndex)
					continue;
				var newRange = new Range(range.Start.Row - moveCount, range.Start.Column,
				                         range.End.Row - moveCount, range.End.Column);
				this.SpannedRangesCollection.Update(range, newRange);
			}
		}
		
		public void Swap(int rowIndex1, int rowIndex2)
		{
			var wholeGrid = new Range(rowIndex1, 0, rowIndex1, int.MaxValue);
			var wholeGrid2 = new Range(rowIndex2, 0, rowIndex2, int.MaxValue);
			
			var firstRanes = this.SpannedRangesCollection.GetRanges(wholeGrid);
			var secondRanges = this.SpannedRangesCollection.GetRanges(wholeGrid2);
			
			foreach (var rangineInFirst in firstRanes)
			{
				if (rangineInFirst.RowsCount > 1)
					throw new SourceGridException("Can not swap rows if they contain spanned ranged which extend more than one row");
				var newRange = new Range(rowIndex2, rangineInFirst.Start.Column, rowIndex2, rangineInFirst.End.Column);
				this.SpannedRangesCollection.Update(rangineInFirst, newRange);
			}
			
			foreach (var rangesInSecond in secondRanges)
			{
				if (rangesInSecond.RowsCount > 1)
					throw new SourceGridException("Can not swap rows if they contain spanned ranged which extend more than one row");
				var newRange = new Range(rowIndex1, rangesInSecond.Start.Column, rowIndex1, rangesInSecond.End.Column);
				this.SpannedRangesCollection.Update(rangesInSecond, newRange);
			}
		}
		 
		
		public void MoveDownSpannedRanges(int startIndex, int moveCount)
		{
			foreach (var range in this.SpannedRangesCollection.ToArray())
			{
				if (range.Start.Row < startIndex)
					continue;
				var newRange = new Range(range.Start.Row + moveCount, range.Start.Column,
				                         range.End.Row + moveCount, range.End.Column);
				this.SpannedRangesCollection.Update(range, newRange);
			}
		}
		
		public void MoveRightSpannedRanges(int startIndex, int moveCount)
		{
			foreach (var range in this.SpannedRangesCollection.ToArray())
			{
				if (range.Start.Column < startIndex)
					continue;
				var newRange = new Range(range.Start.Row, range.Start.Column + moveCount,
				                         range.End.Row, range.End.Column + moveCount);
				this.SpannedRangesCollection.Update(range, newRange);
			}
		}
		
		
		public void RemoveSpannedCellReferencesInRows(int startIndex, int count)
		{
			foreach (var range in this.SpannedRangesCollection.ToArray())
			{
				if ((range.Start.Row >= startIndex) &&
				    (range.Start.Row < startIndex + count))
				{
					this.SpannedRangesCollection.Remove(range);
				}
			}
		}
		
		public void RemoveSpannedCellReferencesInColumns(int startIndex, int count)
		{
			foreach (var range in this.SpannedRangesCollection.ToArray())
			{
				if ((range.Start.Column >= startIndex) &&
				    (range.Start.Column < startIndex + count))
				{
					this.SpannedRangesCollection.Remove(range);
				}
			}
		}
		
		public void ExpandSpannedColumns(int startIndex, int count)
		{
			foreach (var range in this.SpannedRangesCollection.ToArray())
			{
				bool isInsideStart = (range.Start.Column >= startIndex) && (range.Start.Column <= startIndex + count - 1);
				bool isInsideEnd = (range.End.Column >= startIndex) && (range.End.Column <= startIndex + count - 1);
				bool isInside = isInsideStart || isInsideEnd;
				if (isInside)
				{
					var cell = m_grid[range.Start];
					cell.ColumnSpan += count;
				}
			}
		}
		
		public void ExpandSpannedRows(int startIndex, int count)
		{
			foreach (var range in this.SpannedRangesCollection.ToArray())
			{
				bool isInsideStart = (range.Start.Row >= startIndex) && (range.Start.Row <= startIndex + count - 1);
				bool isInsideEnd = (range.End.Row >= startIndex) && (range.End.Row <= startIndex + count - 1);
				bool isInside = isInsideStart || isInsideEnd;
				if (isInside)
				{
					var cell = m_grid[range.Start];
					cell.RowSpan += count;
				}
			}
		}
		
		public  void ShrinkOrRemoveSpannedRows(int startIndex, int count)
		{
			const int startCol = 0;
			const int endCol = 1000;
			var removeRange = new Range(startIndex, startCol, startIndex + count - 1, endCol);
			foreach (var range in this.SpannedRangesCollection.ToArray())
			{
				var intersection = range.Intersect(removeRange);
				if (intersection.IsEmpty() == false)
				{
					var cell = m_grid[range.Start];
                    //chinnari.prasad@siemens.com : Getting the correct row span. Previous implementation
                    // was not considering the row sapn of spanned row
                    int span = GetRowSpan(cell, startIndex, count);
					if (span <= 1 && cell.ColumnSpan <= 1)
					{
						cell.RowSpan = 1;
						this.SpannedRangesCollection.Remove(range);
					} else
						cell.RowSpan = span;
				}
			}
		}
		
		public  void ShrinkOrRemoveSpannedColumns(int startIndex, int count)
		{
			const int startRow = 0;
			const int endRow = 1000;
			var removeRange = new Range(startRow, startIndex, endRow, startIndex + count - 1);
			foreach (var range in this.SpannedRangesCollection.ToArray())
			{
				var intersection = range.Intersect(removeRange);
				if (intersection.IsEmpty() == false)
				{
					var cell = m_grid[range.Start];
					var span = cell.ColumnSpan - count;
					if (span <= 1 && cell.ColumnSpan <= 1)
					{
						cell.ColumnSpan = 1;
						this.SpannedRangesCollection.Remove(range);
					} else
						cell.ColumnSpan = span;
				}
			}
        }

        #region Private Implementation
        //chinnari.prasad@siemens.com : Getting the correct row span. Previous implementation
        // was not considering the row sapn of current row header
        private static int GetRowSpan(Cells.ICell cell, int startIndex, int count)
        {
            const int defaultRowSpan = 1;

            if (cell == null)
            {
                return defaultRowSpan;
            }

            int endIndex = startIndex + count - 1;
            int startSpannedIndex = cell.Range.Start.Row;
            int endSpannedIndex = cell.Range.End.Row;
            int rowSpan = cell.RowSpan;

            if (startSpannedIndex >= startIndex)
            {
                return rowSpan;
            }
            if (endSpannedIndex < endIndex)
            {
                return (startIndex - startSpannedIndex);
            }
            rowSpan = rowSpan - count;
            return rowSpan;
        }

        #endregion Private Implementation
    }
}
