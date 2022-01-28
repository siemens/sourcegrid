# SourceGrid

## v6.0.0 (January 2022)

- Upgraded project file format to SDK-style.
- Changed target framework from .NET 3.5 to .NET 6.
- Moved DevAge.* source files into SourceGrid folder.
- Fixed obsolete code. For example: ContextMenu is replaced by ContextMenuStrip.
- Removed unused code files.

## Major Changes of v4.40.4580

- Enhancement: Smooth horizontal and vertical scrolling
- Enhanced Freeze panes(FixedRow and FixedColumn) and made it independent of Header row\column count
- Introduced a boundary(defined by user) to stop auto scrolling
- Filter row support in DataGrid
- Support for Drag and drop of cells
- Performance improvement while loading grid- CreateControl
- Selectable readonly cells
- Introduced a disabled cell mode
- Fixed bugs in clipboard, spanning etc.
