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
 * 1. Additional item added to RectanglePartType (part of custom border in UIGrid)
 * 2. BorderPartType enum added : for FreezePanes enhancement, draw method is extended by adding a parameter to specify which border should be drawn.
*/
#endregion Copyright
using System;

namespace DevAge.Drawing
{

    /// <summary>
    /// Specifies alignment of content on the drawing surface.
    /// The same enum as System.Drawing.ContentAlignment. Rewritten for compatibility with the Compact Framework.
    /// </summary>
    public enum ContentAlignment
    {
        /// <summary>
        /// Content is vertically aligned at the bottom, and horizontally aligned at the center.  
        /// </summary>
        BottomCenter = 512,
        /// <summary>
        /// Content is vertically aligned at the bottom, and horizontally aligned on the left.  
        /// </summary>
        BottomLeft = 256,
        /// <summary>
        /// Content is vertically aligned at the bottom, and horizontally aligned on the right.
        /// </summary>
        BottomRight = 1024,
        /// <summary>
        /// Content is vertically aligned in the middle, and horizontally aligned at the center.  
        /// </summary>
        MiddleCenter = 32,
        /// <summary>
        /// Content is vertically aligned in the middle, and horizontally aligned on the left.  
        /// </summary>
        MiddleLeft = 16,
        /// <summary>
        /// Content is vertically aligned in the middle, and horizontally aligned on the right. 
        /// </summary>
        MiddleRight = 64,
        /// <summary>
        /// Content is vertically aligned at the top, and horizontally aligned at the center.  
        /// </summary>
        TopCenter = 2,
        /// <summary>
        /// Content is vertically aligned at the top, and horizontally aligned on the left. 
        /// </summary>
        TopLeft = 1,
        /// <summary>
        /// Content is vertically aligned at the top, and horizontally aligned on the right.
        /// </summary>
        TopRight = 4
    }

    
    public enum RectanglePartType
    {
        None = 0,
        ContentArea = 1,
        LeftBorder = 2,
        TopBorder = 3,
        RightBorder = 4,
        BottomBorder = 5,
        DragRectangle =6,
    }

    /// <summary>
    /// sandhra.prakash@siemens.com: for FreezePanes enhancement, draw method is extended by adding a parameter to specify which border should be drawn.
    /// </summary>
    [Flags]
    public enum BorderPartType
    {
        None = 0,
        LeftBorder = 1 << 1,
        TopBorder = 1 << 2,
        RightBorder = 1 << 3,
        BottomBorder = 1 << 4,
        DragRectangle = 1 << 5,
        All = LeftBorder | TopBorder | RightBorder | BottomBorder | DragRectangle,
    }
    public enum Gradient3DBorderStyle
    {
        Raised = 1,
        Sunken = 2
    }

    public enum ElementsDrawMode
    {
        /// <summary>
        /// Draw each element over the previous
        /// </summary>
        Covering = 1,
        /// <summary>
        /// Align each element with the previous if an alignment is specified.
        /// </summary>
        Align = 2
    }


    public enum ControlDrawStyle
    {
        Normal = 1,
        Pressed = 2,
        Hot = 3,
        Disabled = 4
    }

    public enum ButtonStyle
    {
        Normal = 1,
        Pressed = 2,
        Hot = 3,
        Disabled = 4,
        NormalDefault = 5,
        Focus = 6
    }

    public enum HeaderSortStyle
    {
        None = 0,
        Ascending = 1,
        Descending = 2
    }

    public enum CheckBoxState
    {
        Undefined = 0,
        Checked = 1,
        Unchecked = 2
    }

    public enum BorderStyle
    {
        None,
        System
    }

    public enum BackgroundColorStyle
    {
        None = 0,
        Linear = 1,
        Solid = 2
    }
}
