using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Util.WPF
{
    public static class MyGrids
    {
        public static Grid GetRowItem(List<int> columnSizes, StackPanel parent)
        {
            var rowGrid = GetRow(columnSizes, parent);
            rowGrid.Margin = new Thickness(1, 2, 1, 0);

            return rowGrid;
        }

        public static Grid GetRow(List<int> columnSizes, StackPanel parent)
        {
            var rowGrid = Get(columnSizes, 1, parent);

            return rowGrid;
        }

        public static Grid GetRow(int row, int column, Grid parent, List<int> columnSizes)
        {
            var rowGrid = Get(row, column, parent, columnSizes, 1);

            return rowGrid;
        }

        public static Grid Get(int row, int column, Grid parent, List<int> columnSizes, int rowQuantity)
        {
            var grid = Get(columnSizes, rowQuantity);
            UtilWPF.SetGridPosition(grid, row, column, parent);

            return grid;
        }

        public static Grid Get(List<int> columnSizes, int rowQuantity, StackPanel parent)
        {
            var grid = Get(columnSizes, rowQuantity);
            parent.Children.Add(grid);

            return grid;
        }

        private static Grid Get(List<int> columnSizes, int rowQuantity)
        {
            var grid = new Grid();

            foreach (var size in columnSizes)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(size, GridUnitType.Star)
                });
            }

            for (int i = 1; i <= rowQuantity; i++)
                grid.RowDefinitions.Add(new RowDefinition());

            return grid;
        }
    }
}
