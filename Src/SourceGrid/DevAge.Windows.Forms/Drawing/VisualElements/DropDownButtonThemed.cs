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
 * 1. ProportionateSize(ref RectangleF area): Sandhra.prakash@siemens.com : 
            This is added to keep the width and height of the 
            dropdown button proportinate to eachother in the view
 * 2. OnDraw(GraphicsCache graphics, RectangleF area) : sandhra.prakash@siemens.com : 
            The new function was called here, to proportionate the size(width and height) 
            of the dropdown button.
*/
#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace DevAge.Drawing.VisualElements
{
    [Serializable]
    public class DropDownButtonThemed : DropDownButtonBase
    {
        #region Constuctor
        /// <summary>
        /// Default constructor
        /// </summary>
        public DropDownButtonThemed()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public DropDownButtonThemed(DropDownButtonThemed other)
            : base(other)
        {
        }
        #endregion
        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new DropDownButtonThemed(this);
        }

        #region Properties
        /// <summary>
        /// Standard button used when the XP style are disabled.
        /// </summary>
        private DropDownButton mStandardButton = new DropDownButton();

        public override ButtonStyle Style
        {
            get { return base.Style; }
            set { base.Style = value; mStandardButton.Style = value; }
        }
        #endregion

        #region Helper methods
        protected VisualStyleElement GetBackgroundElement()
        {
            if (Style == ButtonStyle.Hot)
                return VisualStyleElement.ComboBox.DropDownButton.Hot;
            else if (Style == ButtonStyle.Pressed)
                return VisualStyleElement.ComboBox.DropDownButton.Pressed;
            else if (Style == ButtonStyle.Disabled)
                return VisualStyleElement.ComboBox.DropDownButton.Disabled;
            else
                return VisualStyleElement.ComboBox.DropDownButton.Normal;
        }

        /// <summary>
        /// Gets the System.Windows.Forms.VisualStyles.VisualStyleRenderer to draw the specified element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected System.Windows.Forms.VisualStyles.VisualStyleRenderer GetRenderer(VisualStyleElement element)
        {
            return new System.Windows.Forms.VisualStyles.VisualStyleRenderer(element);
        }
        #endregion

        protected override void OnDraw(GraphicsCache graphics, RectangleF area)
        {
            // Sandhra.prakash@siemens.com : This is added to keep the width and height of the 
            // dropdown button same in the view
            ProportionateSize(ref area);
            if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsElementDefined(GetBackgroundElement()))
            {
                GetRenderer(GetBackgroundElement()).DrawBackground(graphics.Graphics, Rectangle.Round(area));

                if (Style == ButtonStyle.Focus)
                {
                    using (MeasureHelper measure = new MeasureHelper(graphics))
                    {
                        ControlPaint.DrawFocusRectangle(graphics.Graphics, Rectangle.Round(area));
                    }
                }
            }
            else
                mStandardButton.Draw(graphics, area);
        }

        /// <summary>
        /// Sandhra.prakash@siemens.com : This is added to keep the width and height of the 
        /// dropdown button same in the view
        /// </summary>
        /// <param name="area"></param>
        private static void ProportionateSize(ref RectangleF area)
        {
            if (area.Width > area.Height)
            {
                area.X += area.Width - area.Height;
                area.Width = area.Height;
            }
            else
            {
                area.Height = area.Width;
            }
        }

        protected override SizeF OnMeasureContent(MeasureHelper measure, SizeF maxSize)
        {
            if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsElementDefined(GetBackgroundElement()))
            {
                var size = GetRenderer(GetBackgroundElement()).GetPartSize(measure.Graphics, ThemeSizeType.True);
                // in Win-7 machines size is returned as 7
                // Increase it to 16, so it matches the standard drop-down width
                if (size.Width < 16)
                	size.Width = 16;

                return size;
            }
            else
                return mStandardButton.Measure(measure, Size.Empty, maxSize);
        }

        //public override RectangleF GetBackgroundContentRectangle(MeasureHelper measure, RectangleF backGroundArea)
        //{
        //    backGroundArea = base.GetBackgroundContentRectangle(measure, backGroundArea);

        //    if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsElementDefined(GetBackgroundElement()))
        //        return GetRenderer(GetBackgroundElement()).GetBackgroundContentRectangle(measure.Graphics, Rectangle.Round(backGroundArea));
        //    else
        //        return mStandardButton.GetBackgroundContentRectangle(measure, backGroundArea);
        //}

        //public override SizeF GetBackgroundExtent(MeasureHelper measure, SizeF contentSize)
        //{
        //    if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsElementDefined(GetBackgroundElement()))
        //    {
        //        Rectangle content = new Rectangle(new Point(0, 0), Size.Ceiling(contentSize));
        //        contentSize = GetRenderer(GetBackgroundElement()).GetBackgroundExtent(measure.Graphics, content).Size;
        //    }
        //    else
        //        contentSize = mStandardButton.GetBackgroundExtent(measure, contentSize);

        //    return base.GetBackgroundExtent(measure, contentSize);
        //}
    }
}
