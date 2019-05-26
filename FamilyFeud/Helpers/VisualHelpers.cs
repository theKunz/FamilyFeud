using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace FamilyFeud.Helpers
{
  public static class VisualHelpers
  {

    public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int columnIndex = 0)
    {
      if (row == null) return null;

      var presenter = row.FindFirstVisualChild<DataGridCellsPresenter>();
      if (presenter == null) return null;

      var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
      if (cell != null) return cell;

      grid.ScrollIntoView(row, grid.Columns[columnIndex]);
      cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);

      return cell;
    }

    public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj)
        where T : DependencyObject
    {
      if (depObj != null)
      {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
          DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
          if (child != null && child is T)
          {
            yield return (T)child;
          }

          foreach (T childOfChild in FindVisualChildren<T>(child))
          {
            yield return childOfChild;
          }
        }
      }
    }

    public static childItem FindFirstVisualChild<childItem>(this DependencyObject obj)
        where childItem : DependencyObject
    {
      foreach(childItem child in obj.FindVisualChildren<childItem>())
      {
        return child;
      }

      return null;
    }
  }
}
