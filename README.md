![Build](https://github.com/siemens/sourcegrid/workflows/Build/badge.svg)
![Publish NuGet Packages](https://github.com/siemens/sourcegrid/workflows/Publish%20NuGet%20Packages/badge.svg)
## SourceGrid

SourceGrid is a free open source grid control. Supports virtual grid, custom cells and editors, advanced formatting options and many others features
SourceGrid is a Windows Forms control written entirely in C#, goal is to create a simple but flexible grid to use in all of the cases in which it is necessary to visualize or to change a series of data in a table format. There are a lot of controls of this type available, but often are expensive, difficult to be customize or not compatible with .NET. SourceGrid allows users to have customizable datasource which is not in DataSet format.

The SourceGrid project, initially overseen by contributors on http://sourcegrid.codeplex.com/ and led by Davide Icardi and Darius Damalakas, has undergone a transition in response to a period of inactivity and a lack of ongoing maintenance from its contributors.
Recognizing the importance of sustaining and enhancing the project, it has been moved here and active maintenance and development efforts are being directed. This move aims to ensure the continued vitality of the SourceGrid project and foster a collaborative environment for its advancement.

![Overview Image](/img/SourceGrid_Overview.jpg)

For more detailed information, Refer article at [CodeProject](https://www.codeproject.com/Articles/3531/SourceGrid-Open-Source-C-Grid-Control)

# Preconditions

There only a few preconditions which must be fulfilled.

* Visual Studio 2022
* Net 8.0

# Changes:
1. Enhancement: Smooth horizontal and vertical scrolling
2. Enhanced Freeze panes(FixedRow and FixedColumn) and made it independent of Header row\column count
3. Introduced a boundary(defined by user) to stop auto scrolling
4. Filter row support in DataGrid
5. Support for Drag and drop of cells
6. Performance improvement while loading grid- CreateControl
7. Selectable readonly cells
8. Introduced a disabled cell mode
9. Fixed bugs in clipboard, spanning etc

Refer [Changes_Wiki](https://github.com/siemens/sourcegrid/wiki/Changes) for more information

# License
This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/siemens/sourcegrid/blob/master/LICENSE) file for details 


