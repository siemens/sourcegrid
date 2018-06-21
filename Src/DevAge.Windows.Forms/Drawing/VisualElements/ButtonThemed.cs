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

/*Changes:
* 1. OnDraw(GraphicsCache graphics, RectangleF area) :With the default implementation it was seen that the ractangle is not drawn at the desired location.
                        Inorder to correct that we provide an overridable method which can make minor adjustments to area
 * 2. GetFocusRectangle(MeasureHelper measure, RectangleF backgroundArea) - new method to correct the drawing of focusRectangle
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
    public class ButtonThemed : ButtonBase
    {
        #region Constuctor
        /// <summary>
        /// Default constructor
        /// </summary>
        public ButtonThemed()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public ButtonThemed(ButtonThemed other)
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
            return new ButtonThemed(this);
        }

        #region Properties
        /// <summary>
        /// Standard button used when the XP style are disabled.
        /// </summary>
        private Button mStandardButton = new Button();

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
                return VisualStyleElement.Button.PushButton.Hot;
            else if (Style == ButtonStyle.Pressed)
                return VisualStyleElement.Button.PushButton.Pressed;
            else if (Style == ButtonStyle.Disabled)
                return VisualStyleElement.Button.PushButton.Disabled;
            else if (Style == ButtonStyle.NormalDefault)
                return VisualStyleElement.Button.PushButton.Default;
            else
                return VisualStyleElement.Button.PushButton.Normal;
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
            base.OnDraw(graphics, area);

            if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsElementDefined(GetBackgroundElement()))
            {
                GetRenderer(GetBackgroundElement()).DrawBackground(graphics.Graphics, Rectangle.Round(area));

                if (Style == ButtonStyle.Focus)
                {
                    using (MeasureHelper measure = new MeasureHelper(graphics))
                    {
                        //sandhra.prakash@siemens.com: With the default implementation it was seen that the ractangle is not drawn at the desired location.
                        //Inorder to correct that we provide an overridable method which can make minor adjustments to area
                        RectangleF focusRectangle = GetFocusRectangle(measure, area);
                        ControlPaint.DrawFocusRectangle(graphics.Graphics, Rectangle.Round(focusRectangle));
                    }
                }
            }
            else
                mStandardButton.Draw(graphics, area);
        }

        /// <summary>
        /// sandhra.prakash@siemens.com: added FocusRectangle method Calculator. Focus rectangle is not appearing properly.
        /// So here we provide a default implementation. and in UIGrid we override it to correct it.
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="backgroundArea"></param>
        /// <returns></returns>
        protected virtual RectangleF GetFocusRectangle(MeasureHelper measure, RectangleF backgroundArea)
        {
            return GetBackgroundContentRectangle(measure, backgroundArea);
        }
        public override RectangleF GetBackgroundContentRectangle(MeasureHelper measure, RectangleF backGroundArea)
        {
            backGroundArea = base.GetBackgroundContentRectangle(measure, backGroundArea);

            if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsElementDefined(GetBackgroundElement()))
                return GetRenderer(GetBackgroundElement()).GetBackgroundContentRectangle(measure.Graphics, Rectangle.Round(backGroundArea));
            else
                return mStandardButton.GetBackgroundContentRectangle(measure, backGroundArea);
        }

        public override SizeF GetBackgroundExtent(MeasureHelper measure, SizeF contentSize)
        {
            if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsElementDefined(GetBackgroundElement()))
            {
                Rectangle content = new Rectangle(new Point(0, 0), Size.Ceiling(contentSize));
                contentSize = GetRenderer(GetBackgroundElement()).GetBackgroundExtent(measure.Graphics, content).Size;
            }
            else
                contentSize = mStandardButton.GetBackgroundExtent(measure, contentSize);

            return base.GetBackgroundExtent(measure, contentSize);
        }
    }
}
