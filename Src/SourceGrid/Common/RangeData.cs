
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
// Author            : chinnari.prasad@siemens.com 
// In Charge for Code: chinnari.prasad@siemens.com
//------------------------------------------------------------------------ 

/*Changes :
 1. chinnari.prasad@siemens.com : LoadData method is changed to support cut and copy activities.
    enhancement : Enable clipboard modes
 2. chinnari.prasad@siemens.com : WriteData method is changed to support paste activity.
    enhancement : Enable clipboard modes
 3. chinnari.prasad@siemens.com : StringArrayToString method is changed to get the proper string after copy.
    enhancement : Enable clipboard modes
*/
#endregion Copyright

using System;

namespace SourceGrid
{
	/// <summary>
	/// RangeData class represents a range of data. Can represent a range of data in string format. Usually used for drag and drop and clipboard copy/paste operations.
	/// See Controllers\Clipboard, Controllers\SelectionDrag and Controllers\SelectionDrop.
	/// </summary>
	[Serializable]
	public class RangeData
	{
		/// <summary>
		/// The string constant used with the System.Windows.Forms.DataFormats.GetFormat to register the clipboard format RangeData object.
		/// </summary>
		public const string RANGEDATA_FORMAT = "SourceGrid.RangeData";

		/// <summary>
		/// Static constructor
		/// </summary>
		static RangeData()
		{
			//Register custom DataFormats
			System.Windows.Forms.DataFormats.GetFormat(RANGEDATA_FORMAT);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public RangeData()
		{
		}
		
		public RangeData(GridVirtual mSourceGrid) : this()
		{
			this.mSourceGrid = mSourceGrid;
		}
		

		[Obsolete]
		private Position mStartDragPosition;
		private Range mSourceRange;
		private object[,] mSourceValues;
		[NonSerialized]
		private GridVirtual mSourceGrid;
		[Obsolete]
		private CutMode mCutMode = CutMode.None;

		[NonSerialized]
		private System.Windows.Forms.DataObject mClipboardDataObject = null;

		/// <summary>
		/// Range source
		/// </summary>
		public Range SourceRange
		{
			get{return mSourceRange;}
		}

		/// <summary>
		/// String array for values.
		/// </summary>
		public object[,] SourceValues
		{
			get{return mSourceValues;}
		}

		/// <summary>
		/// Starting drag position. Used only for calculating drop destination range.
		/// </summary>
		[Obsolete]
		public Position StartDragPosition
		{
			get{return mStartDragPosition;}
		}

		/// <summary>
		/// Working grid.
		/// </summary>
		public GridVirtual SourceGrid
		{
			get{return mSourceGrid;}
		}

		/// <summary>
		/// Cut mode. Default is none.
		/// </summary>
		[Obsolete]
		public CutMode CutMode
		{
			get{return mCutMode;}
		}

		#region Loading data
		
		[Obsolete("Use LoadData method without startDragPosition")]
		public void LoadData(GridVirtual sourceGrid, Range sourceRange, Position startDragPosition, CutMode cutMode)
		{
			LoadData(sourceGrid, sourceRange, Position.Empty, cutMode);
		}
		
		/// <summary>
		/// Load the specified range data into a string array. This method use the cell editor to get the value.
		/// </summary>
		/// <param name="sourceGrid"></param>
		/// <param name="sourceRange"></param>
		/// <param name="cutMode">Cut mode. Can be used to remove the data from the source when pasting it to the destination or immediately.</param>
		public static RangeData LoadData(GridVirtual sourceGrid, Range sourceRange, CutMode cutMode)
		{
			RangeData data = new RangeData(sourceGrid);
			//mCutMode = cutMode;
			data.mSourceRange= sourceRange;
			data.mSourceValues = new object[sourceRange.RowsCount, GetVisibleColumnCount(sourceGrid, sourceRange)];

			int arrayRow = 0;
			for (int r = sourceRange.Start.Row; r <= sourceRange.End.Row; r++, arrayRow++)
			{
				int arrayCol = 0;
				for (int c = sourceRange.Start.Column; c <= sourceRange.End.Column; c++)
				{
					if (sourceGrid.Columns.IsColumnVisible(c) == false)
						continue;
					Position posCell = new Position(r, c);
					Cells.ICellVirtual cell = sourceGrid.GetCell(posCell);
					CellContext cellContext = new CellContext(sourceGrid, posCell, cell);
					/*if (cell != null && cell.Editor != null && cell.Editor.IsStringConversionSupported())
						data.mSourceValues[arrayRow, arrayCol] = cell.Editor.ValueToString( cell.Model.ValueModel.GetValue(cellContext) );
					else if (cell != null)
						data.mSourceValues[arrayRow, arrayCol] = cellContext.DisplayText;*/

                    //chinnari.prasad@siemens.com : LoadData method is changed to support cut and copy activities.
				    if (cell != null)
				    {
				        if (cutMode == CutMode.CutImmediately)
				        {
				            if ((cell.ClipboardModes & ClipboardMode.Cut) == ClipboardMode.Cut)
				            {
				                data.mSourceValues[arrayRow, arrayCol] = cellContext.Value;
				            }
				        }
				        else if (cutMode == CutMode.None)
				        {
				            if ((cell.ClipboardModes & ClipboardMode.Copy) == ClipboardMode.Copy)
				            {
				                data.mSourceValues[arrayRow, arrayCol] = cellContext.Value;
				            }
				        }
				    }
					arrayCol++;
				}
			}

			data.mClipboardDataObject = new System.Windows.Forms.DataObject();
			data.mClipboardDataObject.SetData(RANGEDATA_FORMAT, data);
            string[,] values = DataToStringArray(sourceGrid, data.mSourceRange);
			data.mClipboardDataObject.SetData(typeof(string), StringArrayToString(values));

            //Cut Data
            //chinnari.prasad@siemens.com : Cut operation is moved below the copy operation to retain the data
            if (cutMode == CutMode.CutImmediately && sourceGrid != null)
            {
                sourceGrid.ClearValues(new RangeRegion(sourceRange), ClipboardMode.Cut);
            }

			return data;
		}
		
		private static int GetVisibleColumnCount(GridVirtual sourceGrid, Range sourceRange)
		{
			int visibleCount = 0;
			for (int c = sourceRange.Start.Column; c <= sourceRange.End.Column; c++)
			{
				if (sourceGrid.Columns.IsColumnVisible(c))
					visibleCount++;
			}
			return visibleCount;
		}

		/// <summary>
		/// Load the data from a Tab delimited string of data. Each column is separated by a Tab and each row by a LineFeed character.
		/// </summary>
		public void LoadData(string data)
		{
			mSourceGrid = null;
			StringToData(data, out mSourceRange, out mSourceValues);

			mClipboardDataObject = new System.Windows.Forms.DataObject();
			mClipboardDataObject.SetData(RANGEDATA_FORMAT, this);
            mClipboardDataObject.SetData(typeof(string), StringArrayToString(mSourceValues as string[,]));
		}
		#endregion

		#region Write data
		/// <summary>
		/// Write the current loaded array string in the specified grid range. This method use the cell editor to set the value.
		/// </summary>
		public void WriteData(GridVirtual sourceGrid, Position destinationPosition)
		{
			int sourceRow = this.SourceValues.GetUpperBound(0) ;
			int sourceColumn = this.SourceValues.GetUpperBound(1);
			var dataRow = 0;
			for (int r = destinationPosition.Row; r <= destinationPosition.Row  + sourceRow; r++)
			{
				int dataColumn = 0;
				for (int c = destinationPosition.Column; c <= destinationPosition.Column + sourceColumn; c++)
				{
					Position posCell = new Position(r, c);
					Cells.ICellVirtual cell = sourceGrid.GetCell(posCell);
					CellContext cellContext = new CellContext(sourceGrid, posCell, cell);

                    //chinnari.prasad@siemens.com : WriteData method is changed to support paste activity.
				    if (cell != null && (cell.ClipboardModes & ClipboardMode.Paste) == ClipboardMode.Paste && 
                        cell.Editor != null && mSourceValues[dataRow, dataColumn] != null)
				    {
                        cell.Editor.SetCellValue(cellContext, mSourceValues[dataRow, dataColumn]);
				    }
					dataColumn++;
				}
				dataRow++;
			}
			
			
			/*int sourceRow = this.SourceValues.Length - 1;
			int sourceColumn = this.SourceValues.Rank - 1;
			//Calculate the destination Range merging the source range
			var destinationRange = new Range(destinationPosition,
			                                 new Position(destinationPosition.Row + sourceRow, destinationPosition.Column + sourceColumn));
			Range newRange = mSourceRange;
			newRange.MoveTo(destinationRange.Start);
			if (newRange.End.Column > destinationRange.End.Column)
				newRange.End = new Position(newRange.End.Row, destinationRange.End.Column);
			if (newRange.End.Row > destinationRange.End.Row)
				newRange.End.Row = destinationRange.End.Row;

			for (int r = newRange.Start.Row; r <= newRange.End.Row; r++)
			{
				int dataColumn = 0;
				for (int c = newRange.Start.Column; c <= newRange.End.Column ; c++)
				{
					//if (sourceGrid.Columns.IsColumnVisible(c) == false)
					//	continue;
					Position posCell = new Position(r, c);
					Cells.ICellVirtual cell = sourceGrid.GetCell(posCell);
					CellContext cellContext = new CellContext(sourceGrid, posCell, cell);

					if (cell != null && cell.Editor != null && mSourceValues[r - newRange.Start.Row, dataColumn] != null)
						cell.Editor.SetCellValue(cellContext, mSourceValues[r - newRange.Start.Row, dataColumn] );
					dataColumn++;
				}
			}*/
		}
		#endregion

		/// <summary>
		/// Convert a string buffer into a Range object and an array of string.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="range"></param>
		/// <param name="values"></param>
		protected virtual void StringToData(string data, out Range range, out object[,] values)
		{
			//tolgo uno dei due caratteri di a capo per usare lo split
			data = data.Replace("\x0D\x0A","\x0A");
			string[] rowsData = data.Split('\x0A','\x0D');
			
			//Check if the last row is not null (some application put a last \n character at the end of the cells, for example excel)
			int rows = rowsData.Length;
			if (rows > 0 &&
			    (rowsData[rows - 1] == null ||
			     rowsData[rows - 1].Length == 0) )
				rows--;

			if (rows == 0)
			{
				range = Range.Empty;
				values = new string[0,0];
				return;
			}

			//Calculate the columns based on the first rows! Note: probably is better to calculate the maximum columns.
			string[] firstColumnsData = rowsData[0].Split('\t');
			int cols = firstColumnsData.Length;

			range = new Range(0, 0, rows - 1, cols - 1);
			values = new string[rows, cols];

			int arrayRow = 0;
			for (int r = range.Start.Row; r < range.Start.Row + rows; r++, arrayRow++)
			{
				string rowData = rowsData[arrayRow];
				string[] columnsData = rowData.Split('\t');
				int arrayCol = 0;
				for (int c = range.Start.Column; c <= range.End.Column; c++, arrayCol++)
				{
					if (arrayCol < columnsData.Length)
						values[arrayRow, arrayCol] = columnsData[arrayCol];
					else
						values[arrayRow, arrayCol] = "";
				}
			}
		}

        /// <summary>
        /// Convert an array of strings into a string.
        /// Normally using a tab delimited for columns and a LineFeed for rows.
        /// </summary>
        /// <returns></returns>
        protected static string StringArrayToString(string[,] values)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            int nr = values.GetLength(0);
            int nc = values.GetLength(1);
            for (int r = 0; r < nr; ++r)
            {
                for (int c = 0; c < nc; ++c)
                {
                    builder.Append(values[r, c]);
                    //chinnari.prasad@siemens.com : StringArrayToString method is changed to get the proper string after copy.
                    if (c != nc - 1)
                        builder.Append('\t');
                }

                //chinnari.prasad@siemens.com : StringArrayToString method is changed to get the proper string after copy.
                if (r != nr - 1)
                    builder.Append("\x0D\x0A");
                else
                    builder.Append("\x0D");
            }

            return builder.ToString();
        }

		/// <summary>
		/// Convert a range and an array of string into a string. Normally using a tab delimited for columns and a LineFeed for rows.
		/// </summary>
		/// <returns></returns>
        protected static string[,] DataToStringArray(GridVirtual sourceGrid, Range range)
		{
            int numberOfRows = range.End.Row - range.Start.Row + 1;
            int numberOfCols = range.End.Column - range.Start.Column + 1;
            string[,] values = new string[numberOfRows, numberOfCols];

            int arrayRow = 0;
            for (int r = range.Start.Row; r <= range.End.Row; r++, arrayRow++)
            {
                int arrayCol = 0;
                for (int c = range.Start.Column; c <= range.End.Column; c++, arrayCol++)
                {
                    String val = String.Empty;

                    Position posCell = new Position(r, c);
                    Cells.ICellVirtual cell = sourceGrid.GetCell(posCell);
                    CellContext cellContext = new CellContext(sourceGrid, posCell, cell);

                    if (cell != null && cell.Editor != null && cell.Editor.IsStringConversionSupported())
                        values[arrayRow, arrayCol] = cell.Editor.ValueToString(cell.Model.ValueModel.GetValue(cellContext));
                    else if (cell != null)
                        values[arrayRow, arrayCol] = cellContext.DisplayText;
                }
            }

            return values;
		}

		/// <summary>
		/// Calculate the destination range for the drop or paste operations.
		/// </summary>
		/// <param name="destinationGrid"></param>
		/// <param name="dropDestination"></param>
		/// <returns></returns>
		[Obsolete("Completely not used. Will be removed in future versions")]
		public Range FindDestinationRange(GridVirtual destinationGrid, Position dropDestination)
		{
			if (dropDestination.IsEmpty())
				return Range.Empty;

			Position destinationStart = new Position(dropDestination.Row + (mSourceRange.Start.Row - mStartDragPosition.Row),
			                                         dropDestination.Column + (mSourceRange.Start.Column - mStartDragPosition.Column) );

			destinationStart = Position.Max(destinationStart, new Position(0, 0));

			Range destination = mSourceRange;
			destination.MoveTo( destinationStart );

			destination = destination.Intersect(destinationGrid.CompleteRange);

			return destination;
		}


		/// <summary>
		/// Copy the specified RangeData object the the clipboard
		/// </summary>
		/// <param name="rangeData"></param>
		public static void ClipboardSetData(RangeData rangeData)
		{
			if (rangeData.mClipboardDataObject == null)
				throw new SourceGridException("No data loaded, use the LoadData method");
			System.Windows.Forms.Clipboard.SetDataObject(rangeData.mClipboardDataObject);
		}

		/// <summary>
		/// Get a RangeData object from the clipboard. Return null if the clipboard doesn't contains valid data formats.
		/// </summary>
		/// <returns></returns>
		public static RangeData ClipboardGetData()
		{
			System.Windows.Forms.IDataObject dtObj = System.Windows.Forms.Clipboard.GetDataObject();
			RangeData rngData = null;
			if (dtObj.GetDataPresent(RANGEDATA_FORMAT))
				rngData = (RangeData)dtObj.GetData(RANGEDATA_FORMAT);

            // if RANGEDATA_FORMAT or GetData returns null use string buffer as rngData
            if (rngData == null)
            {
                // get unicode text instead of text
                if (dtObj.GetDataPresent(System.Windows.Forms.DataFormats.UnicodeText, true))
                {
                    string buffer = (string)dtObj.GetData(System.Windows.Forms.DataFormats.UnicodeText, true);
                    rngData = new RangeData();
                    rngData.LoadData(buffer);
                }
            }

			return rngData;
		}
	}
}
