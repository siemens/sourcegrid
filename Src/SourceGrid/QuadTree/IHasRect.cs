using SourceGrid;
using System;
using System.Drawing;
using Range = SourceGrid.Range;

namespace QuadTreeLib
{
    /// <summary>
    /// An interface that defines and object with a rectangle
    /// </summary>
    public interface IHasRect
    {
        Range Rectangle { get; }
    }
}
