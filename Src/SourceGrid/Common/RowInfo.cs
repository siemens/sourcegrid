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
 * 1. Visible : Unnecessarily events for visibility changed shouldnot be fired
*/
#endregion Copyright
using System;
using System.ComponentModel;

namespace SourceGrid
{
	/// <summary>
	/// Row Information
	/// </summary>
	public class RowInfo
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="p_Grid"></param>
		public RowInfo(GridVirtual p_Grid)
		{
			m_Grid = p_Grid;
			m_Height = Grid.DefaultHeight;
		}
	
		private int m_Height;
		/// <summary>
		/// Height of the current row
		/// </summary>
		public int Height
		{
			get{return m_Height;}
			set
			{
				if (value < 0)
					value = 0;
	
				if (m_Height != value)
				{
					m_Height = value;
					((RowInfoCollection)m_Grid.Rows).OnRowHeightChanged(new RowInfoEventArgs(this));
				}
			}
		}
	
		//private int m_Index;
		/// <summary>
		/// Index of the current row
		/// </summary>
		public int Index
		{
			get{return ((RowInfoCollection)Grid.Rows).IndexOf(this);}
		}
	
		private GridVirtual m_Grid;
		/// <summary>
		/// Attached Grid
		/// </summary>
		[Browsable(false)]
		public GridVirtual Grid
		{
			get{return m_Grid;}
		}
	
		public Range Range
		{
			get
			{
				if (m_Grid == null)
					throw new SourceGridException("Invalid Grid object");
	
				return new Range(Index, 0, Index, Grid.Columns.Count - 1);
			}
		}
		private object m_Tag;
		/// <summary>
		/// A property that the user can use to insert custom informations associated to a specific row
		/// </summary>
		[Browsable(false)]
		public object Tag
		{
			get{return m_Tag;}
			set{m_Tag = value;}
		}
	
		private AutoSizeMode m_AutoSizeMode = AutoSizeMode.Default;
		/// <summary>
		/// Flags for autosize and stretch
		/// </summary>
		public AutoSizeMode AutoSizeMode
		{
			get{return m_AutoSizeMode;}
			set{m_AutoSizeMode = value;}
		}
	
		/// <summary>
		/// Gets or sets if the row is visible.
		/// Internally set the height to 0 to hide a row.
		/// </summary>
		public bool Visible
		{
			get 
			{ 
				return Grid.Rows.IsRowVisible(this.Index);
			}
			set
			{
                //sandhra.prakash@siemens.com: Unnecessarily events for visibility changed shouldnot be fired
                if (Grid.Rows.IsRowVisible(Index) != value)
				    Grid.Rows.ShowRow(this.Index, value);
			}
		}
	}
}
