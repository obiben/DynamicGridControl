# DynamicGridControl
WPF control to create grids dynamically.

This is an improvement from this control, tailored to suit my own needs: for now this only means being able to modify the DataSource at runtime and seeing changes be made to the grid.
http://www.codeproject.com/Articles/775352/WPF-Virtualizing-Grid-Control?msg=5262175#xx5262175xx

Description copied from source for perennity:
##Introduction
Recently, I found myself searching the web for a highly flexible, virtualizing WPF grid control, to present a large set of data to the user in an time-table like manner, who should then be able to pick one item to proceed with.

I didn't find anything appropriate and after playing around with virtualizing stack panels which didn't satisfy my needs of flexibility and responsiveness, I decided to implement a reusable grid by myself.

##Features
- Highly responsive (long tasks are executed asynchronously, progress bar is shown while loading).
- Highly customizable (Gridlines, Headers, Content may be styles by using DataTemplates).
- Fast (1Mio. data items with integer column/header-information are loaded within 2 seconds).
- Makes use of multiple CPU cores (using TPL).
- Automatically transforms a 1-dimensional source array to a 2-dimensional grid (by using two source item's properties to calculate the x/y position).

##Sample

![Image a sample grid](https://github.com/obiben/DynamicGridControl/blob/master/DynamicGrid.PNG?raw=true)

The data source for the control shown above is a `IEnumerable<SampleGridItem>`. The `SampleGridItem` looks like this:

```csharp
public class SampleGridItem
{
    public string ProductName { get; set; }
    public DateTime ProductionDate { get; set; }
    public int ProductionCount { get; set; } 
}
```

## Using the control - Step by Step
### 1. Include the project you can download on this page

Download, unpack and add it as a project reference.

### 2. Create a new class and derive from DynamicGridControl<>.

Because WPF can't handle generic classes, you need to derive from DynamicGridControl

```csharp
public class DynamicGridSampleControl : DynamicGridControl<SampleGridItem, string, DateTime>
{
```
The type parameters specify:

TDataSource: The type of the data items (which are later bound to the DataSource property).
TRow: The type of the data item's property which contains row information.
TCol: The type of the data item's property which contains column information.
As you can see, row and column can be of any type.

### 3. Create a default constructor

To tell the control, which properties of the data source contain row / header information, you assign the first two delegates (columnSelector / rowSelector).

The third delegate controls what happens, when multiple data items relate to the same row / column - combination.

```csharp
public DynamicGridSampleControl()
   : base(

   item => item.ProductName,


   item => item.ProductionDate,


   items => new SampleGridItem()
   {
       ProductName = items.First().ProductName,
       ProductionDate = items.First().ProductionDate,
       ProductionCount = items.Sum(item => item.ProductionCount)
   })
```
### 4. Sorting?

If you want to sort the row / column headers, assign a RowComparer / ColumnComparer.

Since the row / column id generic and can be any CLR type, you need to add logic on how to compare the items. Most of the time, you can use a type's CompareTo() -method:

```csharp
this.ColumnComparer = (a, b) => a.CompareTo(b);
```

### 5. Create a static constructor to create a default style

Tell WPF which style to use when creating your Grid:

```csharp
static DynamicGridSampleControl()
{
    DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicGridSampleControl), new FrameworkPropertyMetadata(typeof(DynamicGridSampleControl)));
}
```

### 6. Copy the default style from the sample project

Copy the default Style (DynamicGridSampleControlDefaultStyle.xaml) and adjust the namespaces.

Most of the time, there is no need to modify the control template, except for the following cases:

You want to alter the gridline's style (they already provide an IsOdd-Property if you want to make every second row/column pink for example).
You know what you're doing Smile | :)

### 7. Create your control and bind your data to the DataSource - Property

_IMPORTANT:_ Wrap your control within a ScrollViewer which's CanContentScroll is set to True.
Otherwise you won't be able to scroll.

```xml
 <ScrollViewer CanContentScroll="True"
          HorizontalScrollBarVisibility="Auto"
          VerticalScrollBarVisibility="Auto">
    <local:DynamicGridSampleControl ItemWidth="100" ItemHeight="30" DataSource="{Binding Data}" />
</ScrollViewer> 
```

### 8. Style it !

You can:

style the cells by setting the DataItemTemplate / DataItemTemplateSelector.
style the headers by setting the HeaderTemplate / HeaderTemplateSelector.
style the wait-layer by setting the WaitLayerTemplate.
How it works
Simple.

If you assign the DataSource, a distinct column / row - table is build (asynchronously & TPL).
Then a two-dimensional itemsCache is build for fast lookup while scrolling (asynchronously).

To display the data, for each visible cell, a bindable ViewModel (DynamicGridDataItem, etc.) is created that wraps the current content of the cell in its Content property (much like ItemContainerGenerator's recycle behavior).

If you scroll or resize the window, the visible cells' contents are updated by simply accessing the itemsCache.
