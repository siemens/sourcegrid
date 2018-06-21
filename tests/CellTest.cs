// Copyright (c) Siemens AG, 2018.
// Author  : Sandhra Prakash, sandhra.prakash@siemens.com

using System;
using System.Globalization;
using NUnit.Framework;
using SourceGrid.Cells;

namespace SourceGrid.Tests
{
    [TestFixture]
    public class CellTest
    {
        #region ColumnSpan & RowSpan

        [Test]
        public void ColumnSpan_PlaceCellOnSpannedRegion_ThrowOverlappingCellException()
        {
            Grid grid = new Grid();
            grid.Redim(2, 2);
            grid[0, 0] = new Cell("Text Span");
            grid[0, 0].ColumnSpan = 2;

            //Todo: sandhra: check if this situation needs to be handled by reducing the spanning?
            Assert.Throws<OverlappingCellException>(() => grid[0, 1] = new Cell("This throws OverlappingCellException"));
        }

        [Test]
        public void RowSpan_PlaceCellOnSpannedRegion_ThrowOverlappingCellException()
        {
            Grid grid = new Grid();
            grid.Redim(2, 2);
            grid[0, 0] = new Cell("Text Span");
            grid[0, 0].RowSpan = 2;

            //Todo: sandhra: check if this situation needs to be handled by reducing the spanning?
            Assert.Throws<OverlappingCellException>(() => grid[1, 0] = new Cell("This should throw OverlappingCellException"));
        }

        [Test]
        public void ColumnSpan_ToCauseOverlappingWithNonSpannedCell_OccupiesRegion()
        {
            Grid grid = new Grid();
            grid.Redim(2, 6);
            grid[0, 4] = new Cell("spanned cell");
            grid[0, 5] = new Cell("overlapping cell");

            grid[0, 4].ColumnSpan = 2;

            Assert.That(grid[0, 5], Is.EqualTo(grid[0, 4]),  "Cell didnot occupy the cells in the spanned region");
            Assert.That(grid[0, 4], Is.Not.Null, "spanned cell was deleted");
            Assert.That(grid[0, 4].ColumnSpan, Is.EqualTo(2), "Columnspan is not set as expected");
        }

        [Test]
        public void RowSpan_ToCauseOverlappingWithNonSpannedCell_OccupiesRegion()
        {
            Grid grid = new Grid();
            grid.Redim(6, 2);
            grid[4, 0] = new Cell("spanned cell");
            grid[5, 0] = new Cell("overlapping cell");

            grid[4, 0].RowSpan = 2;

            Assert.That(grid[5, 0], Is.EqualTo(grid[4, 0]), "Cell didnot occupy the cells in the spanned region");
            Assert.That(grid[4, 0], Is.Not.Null, "spanned cell was deleted");
            Assert.That(grid[4, 0].RowSpan, Is.EqualTo(2), "Columnspan is not set as expected");
        }

        [Test]
        public void ColumnSpan_IncreaseSpanToExistingSpannedCells_RemoveCellsInSpannedRegion()
        {
            Grid grid = new Grid();
            grid.Redim(2, 6);
            grid[0, 0] = new Cell("Cell to be spanned");
            grid[0, 1] = new Cell("Spanned Cell") { ColumnSpan = 3 };// 0,1 to 0,3

            grid[0, 0].ColumnSpan = 3;//spanned region - 0,0 to 0,2

            Assert.That(grid[0, 3], Is.Null, @"Did not occupy spanned cell\couldnt delete the spanned cell");
            Assert.That(grid[0, 0].ColumnSpan, Is.EqualTo(3), "Column span is not set as expected");
            Assert.That(grid[0, 2], Is.EqualTo(grid[0, 0]), "Cell didnot span as expected");
        }

        [Test]
        public void RowSpan_IncreaseSpanToExistingSpannedCells_RemoveCellsInSpannedRegion()
        {
            Grid grid = new Grid();
            grid.Redim(6, 2);
            grid[0, 0] = new Cell("Cell to be spanned");
            grid[1, 0] = new Cell("Spanned Cell") { RowSpan = 3 };// 1,0 to 3,0

            grid[0, 0].RowSpan = 3;//spanned region - 0,0 to 2,0

            Assert.That(grid[3, 0], Is.Null, @"Did not occupy spanned cell\couldnt delete the spanned cell");
            Assert.That(grid[0, 0].RowSpan, Is.EqualTo(3), "Row span is not set as expected");
            Assert.That(grid[2, 0], Is.EqualTo(grid[0, 0]), "Cell didnot span as expected");
        }

        [Test]
        public void ColumnSpan_DecreaseSpan_ReduceSpanning()
        {
            Grid grid = new Grid();
            grid.Redim(2, 12);
            grid[0, 0] = new Cell("Cell to be spanned") { ColumnSpan = 3 };
            grid[0, 3] = new Cell("Spanned Cell") { ColumnSpan = 3 };

            grid[0, 0].ColumnSpan = 1;

            Assert.That(grid[0, 1], Is.Null);
        }

        [Test]
        public void RowSpan_DecreaseSpan_ReduceSpanning()
        {
            Grid grid = new Grid();
            grid.Redim(12, 2);
            grid[0, 0] = new Cell("Cell to be spanned") { RowSpan = 3 };
            grid[3, 0] = new Cell("Spanned Cell") { RowSpan = 3 };

            grid[0, 0].RowSpan = 1;

            Assert.That(grid[1, 0], Is.Null);
        }

        [Test]
        public void ColumnSpan_IncreaseSpanOutOfBounds_ThrowsNothing()
        {
            Grid grid = new Grid();
            grid.Redim(2, 2);
            grid[0, 0] = new Cell();

            Assert.Throws<ArgumentException>(() => grid[0, 0].ColumnSpan = 9);
        }

        [Test]
        public void RowSpan_GridNull_SpanChangedRangeEmpty()
        {
            Cell cell = new Cell("Cell to be spanned");

            cell.RowSpan = 2;

            //Todo: sandhra: check if this case is ok? span changed range didnot
            Assert.That(cell.RowSpan, Is.EqualTo(2));
            Assert.That(cell.Range, Is.EqualTo(Range.Empty));
        }

        [Test]
        public void ColumnSpan_GridNull_SpanChangedRangeEmpty()
        {
            Cell cell = new Cell("Cell to be spanned");

            cell.ColumnSpan = 2;

            //Todo: sandhra: check if this case is ok? span changed range didnot
            Assert.That(cell.ColumnSpan, Is.EqualTo(2));
            Assert.That(cell.Range, Is.EqualTo(Range.Empty));
        }
        #endregion ColumnSpan & RowSpan

        #region range

        [Test]
        public void Range_GridNull_ReturnsEmptyRange()
        {
            Cell cell = new Cell("Unbound cell");
            Assert.That(cell.Grid, Is.Null);

            Assert.That(cell.Range, Is.EqualTo(Range.Empty));
        }

        [Test]
        public void Range_GridNull_ReturnsSpannedRange()
        {
            Grid grid = new Grid();
            grid.Redim(4,4);
            grid[0,0] = new Cell("cell");
            grid[0, 0].ColumnSpan = 3;

            Assert.That(grid[0, 0].Range, Is.EqualTo(new Range(0,0,0,2)));
        }

        #endregion range

        #region DisplayText

        [Test]
        public void DisplayText_StringTypeValidText_ReturnsTextPassed()
        {
            const string text = "Display text";
            Cell cell = new Cell(typeof(string));
            cell.Value = text;

            Assert.That(cell.DisplayText, Is.EqualTo(text));
        }

        [Test]
        public void DisplayText_StringTypeNullText_ReturnsEmpty()
        {
            Cell cell = new Cell(null, typeof(string));

            Assert.That(cell.DisplayText, Is.Empty);
        }
        [Test]
        public void DisplayText_NumericType_ReturnAsString()
        {
            const int number = 1234;
            Cell cell = new Cell(number, typeof(Int32));

            Assert.That(cell.DisplayText, Is.EqualTo(number.ToString(CultureInfo.InvariantCulture)));
        }

        #endregion DisplayText

        #region Value

        [Test]
        public void Value_StringTypeValidText_ReturnsTextPassed()
        {
            const string text = "Display text";
            Cell cell = new Cell(text,typeof(string));

            Assert.That(cell.Value, Is.EqualTo(text));
        }

        [Test]
        public void Value_StringTypeNullText_ReturnsNull()
        {
            Cell cell = new Cell(null, typeof(string));

            Assert.That(cell.Value, Is.Null);
        }

        [Test]
        public void Value_NumericType_ReturnAsString()
        {
            const int number = 1234;
            Cell cell = new Cell(number, typeof(Int32));

            Assert.That(cell.Value, Is.EqualTo(number));
        }

        #endregion Value
        
        #region ToString

        [Test]
        public void ToString_StringTypeValidText_ReturnsTextPassed()
        {
            const string text = "Display text";
            Cell cell = new Cell(typeof(string));
            cell.Value = text;

            Assert.That(cell.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void ToString_StringTypeNullText_ReturnsEmpty()
        {
            Cell cell = new Cell(null, typeof(string));

            Assert.That(cell.ToString(), Is.Empty);
        }
        [Test]
        public void ToString_NumericType_ReturnAsString()
        {
            const int number = 1234;
            Cell cell = new Cell(number, typeof(Int32));

            Assert.That(cell.ToString(), Is.EqualTo(number.ToString(CultureInfo.InvariantCulture)));
        }

        #endregion ToString
    }
}