// Copyright (c) Siemens AG, 2018.
// Author  : Sandhra Prakash, sandhra.prakash@siemens.com

using NUnit.Framework;
using SourceGrid.Cells;

namespace SourceGrid.Tests
{
    class GridTest
    {

        #region GetCell

        [Test]
        public void GetCell_NonExistent_ReturnsNull()
        {
            Grid grid = new Grid();
            grid.Redim(2, 2);

            Assert.That(grid.GetCell(0, 0), Is.Null);
        }

        [Test]
        public void GetCell_OutOfBounds_ReturnsNull()
        {
            Grid grid = new Grid();
            grid.Redim(5, 5);

            Assert.That(grid.GetCell(new Position(10, 10)), Is.Null);
        }

        [Test]
        public void GetCell_ValidateCellReturnFromSpannedRegion_ReturnsParentCell()
        {
            Grid grid = new Grid();
            grid.Redim(2, 2);
            Cell visibleCell = new Cell("Text Span", typeof(string));
            grid[0, 0] = visibleCell;
            grid[0, 0].RowSpan = 2;
            grid[0, 0].ColumnSpan = 2;

            //ActAndAssert
            Assert.That(grid.GetCell(0, 0), Is.EqualTo(visibleCell));
            Assert.That(grid.GetCell(0, 1), Is.EqualTo(visibleCell));
            Assert.That(grid.GetCell(1, 0), Is.EqualTo(visibleCell));
            Assert.That(grid.GetCell(1, 1), Is.EqualTo(visibleCell));
        }

        #endregion GetCell

        #region InsertCell\this[row, col]

        [Test]
        public void InsertCell_NullCell_ThrowsNothing()
        {
            Grid grid = new Grid();
            grid.Redim(2, 2);

            Assert.That(() => grid[0, 0] = null, Throws.Nothing);
        }

        [Test]
        public void InsertCell_ParentCellMadeNull_SpannedCellRemoved()
        {
            Grid grid = new Grid();
            grid.Redim(2, 2);
            grid[0, 0] = new Cell();
            grid[0, 0].ColumnSpan = 2;

            grid[0, 0] = null;

            Assert.That(grid.GetCell(0, 1), Is.Null);
            Assert.That(() => grid[0, 1] = new Cell("New Cell"), Throws.Nothing);
        }


        [Test]
        public void InsertCell_OutOfBounds_ThrowsNothing()
        {
            Grid grid = new Grid();
            grid.Redim(2, 2);

            Assert.That(() => grid[9, 0] = new Cell(), Throws.Nothing);
        }

        #endregion InsertCell\this[row, col]
        
    }
}
