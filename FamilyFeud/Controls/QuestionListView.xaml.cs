using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using FamilyFeud.DataObjects;
using CommonLib.Commands;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for QuestionListView.xaml
  /// </summary>
  public partial class QuestionListView : UserControl, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private string mHeader;
    private ObservableCollection<IQuestioner> mItemSource;

    public QuestionListView()
    {
      InitializeComponent();

      mHeader = string.Empty;
      mItemSource = new ObservableCollection<IQuestioner>();

      DataContext = this;
    }

    private void Edit(IEditable editableObj)
    {
      editableObj.EditAction?.Invoke(editableObj);
    }

    private void Delete(IEditable deleteObj)
    {
      if(deleteObj is IQuestioner)
      {
        ItemSource.Remove(deleteObj as IQuestioner);
      }
    }

    /// <summary>
    /// Header for this listview
    /// </summary>
    public string Header
    {
      get
      {
        return mHeader;
      }
      set
      {
        if(!mHeader.Equals(value))
        {
          mHeader = value ?? string.Empty;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Header)));
        }
      }
    }

    /// <summary>
    /// Items to be displayed
    /// </summary>
    public ObservableCollection<IQuestioner> ItemSource
    {
      get
      {
        return mItemSource;
      }
      set
      {
        if (mItemSource != value)
        {
          mItemSource = value ?? new ObservableCollection<IQuestioner>();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSource)));
        }
      }
    }

    private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;

      Edit(menuItem.Tag as IEditable);
    }

    private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;

      Delete(menuItem.Tag as IEditable);
    }
  }
}
