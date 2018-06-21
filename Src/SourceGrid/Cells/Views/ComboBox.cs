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
 * 1.GetBaseElement : changed access from private to protected so that it is
        accessible in child class
 * 2. sandhra.prakash@siemens.com : take care of enabled disabled style. Part of Selectable readonly enhancement
 */
#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGrid.Cells.Views
{
    public class ComboBox : Cell
    {
        /// <summary>
        /// Represents a default CheckBox with the CheckBox image align to the Middle Center of the cell. You must use this VisualModel with a Cell of type ICellCheckBox.
        /// </summary>
        public new readonly static ComboBox Default = new ComboBox();

        #region Constructors

        static ComboBox()
        {
        }

        /// <summary>
        /// Use default setting and construct a read and write VisualProperties
        /// </summary>
        public ComboBox()
        {
            ElementDropDown.AnchorArea = new DevAge.Drawing.AnchorArea(float.NaN, 0, 0, 0, false, false);
        }

        /// <summary>
        /// Copy constructor. This method duplicate all the reference field (Image, Font, StringFormat) creating a new instance.
        /// </summary>
        /// <param name="p_Source"></param>
        public ComboBox(ComboBox p_Source)
            : base(p_Source)
        {
            ElementDropDown = (DevAge.Drawing.VisualElements.IDropDownButton)p_Source.ElementDropDown.Clone();
        }
        #endregion

        protected override void PrepareView(CellContext context)
        {
            base.PrepareView(context);

            PrepareVisualElementDropDown(context);
        }

        protected override IEnumerable<DevAge.Drawing.VisualElements.IVisualElement> GetElements()
        {
            if (ElementDropDown != null)
                yield return ElementDropDown;

            foreach (DevAge.Drawing.VisualElements.IVisualElement v in GetBaseElements())
                yield return v;
        }
        /// <summary>
        /// sandhra.prakash@siemens.com: changed access from private to protected so that it is
        /// accessible in child class
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<DevAge.Drawing.VisualElements.IVisualElement> GetBaseElements()
        {
            return base.GetElements();
        }

        private DevAge.Drawing.VisualElements.IDropDownButton mElementDropDown = new DevAge.Drawing.VisualElements.DropDownButtonThemed();
        /// <summary>
        /// Gets or sets the visual element used to draw the checkbox. Default is DevAge.Drawing.VisualElements.CheckBoxThemed.
        /// </summary>
        public DevAge.Drawing.VisualElements.IDropDownButton ElementDropDown
        {
            get { return mElementDropDown; }
            set { mElementDropDown = value; }
        }


        protected virtual void PrepareVisualElementDropDown(CellContext context)
        {
            if (!context.Cell.Editor.EnableEdit || context.Cell.Editor.ReadOnly)
            {
                //sandhra.prakash@siemens.com : take care of enabled disabled style. Part of Selectable readonly enhancement
                ElementDropDown.Style = DevAge.Drawing.ButtonStyle.Disabled;
            }
            else if (context.CellRange.Contains(context.Grid.MouseCellPosition))
            {
                ElementDropDown.Style = DevAge.Drawing.ButtonStyle.Hot;
            }
            else
            {
                ElementDropDown.Style = DevAge.Drawing.ButtonStyle.Normal;
            }
        }

        #region Clone
        /// <summary>
        /// Clone this object. This method duplicate all the reference field (Image, Font, StringFormat) creating a new instance.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new ComboBox(this);
        }
        #endregion
    }
}
