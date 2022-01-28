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
 * 1. EditablePanelThemed::Draw: for FreezePanes enhancement, draw method is extended by adding a parameter to specify which border should be drawn.
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
    public class EditablePanelThemed : EditablePanelBase
    {
        #region Constuctor
        /// <summary>
        /// Default constructor
        /// </summary>
        public EditablePanelThemed()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public EditablePanelThemed(EditablePanelThemed other)
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
            return new EditablePanelThemed(this);
        }

        /// <summary>
        /// Standard button used when the XP style are disabled.
        /// </summary>
        private EditablePanel mStandard = new EditablePanel();

        #region Helper methods
        protected VisualStyleElement GetBackgroundElement()
        {
            return VisualStyleElement.TextBox.TextEdit.Normal;
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

        public override BorderStyle BorderStyle
        {
            get{return base.BorderStyle;}
            set
            {
                base.BorderStyle = value;
                mStandard.BorderStyle = value;
            }
        }

        public override void Draw(GraphicsCache graphics, RectangleF area, BorderPartType partType)
        {
            if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsElementDefined(GetBackgroundElement()))
            {
                if (BorderStyle == BorderStyle.System)
                    GetRenderer(GetBackgroundElement()).DrawBackground(graphics.Graphics, Rectangle.Round(area));
            }
            else
                //sandhra.prakash@siemens.com: for FreezePanes enhancement, draw method is extended by adding a parameter to specify which border should be drawn.
                mStandard.Draw(graphics, area, partType);
        }

        private RectangleBorder mEquivalentPadding = new RectangleBorder(new BorderLine(Color.Empty, 2));

        public override System.Drawing.RectangleF GetContentRectangle(System.Drawing.RectangleF backGroundArea)
        {
            if (BorderStyle == BorderStyle.System)
                return mEquivalentPadding.GetContentRectangle(backGroundArea);
            else
                return backGroundArea;
        }

        public override System.Drawing.SizeF GetExtent(System.Drawing.SizeF contentSize)
        {
            if (BorderStyle == BorderStyle.System)
                return mEquivalentPadding.GetExtent(contentSize);
            else
                return contentSize;
        }

        public override RectanglePartType GetPointPartType(RectangleF area, PointF point, out float distanceFromBorder)
        {
            if (BorderStyle == BorderStyle.System)
                return mEquivalentPadding.GetPointPartType(area, point, out distanceFromBorder);
            else
            {
                distanceFromBorder = 0;
                return RectanglePartType.ContentArea;
            }
        }
    }
}
