using CommonLib.Constants;
using FamilyFeud.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FamilyFeud.Helpers;
using FamilyFeud.DataObjects;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for TeamScoreTracker.xaml
  /// </summary>
  public partial class TeamScoreTracker : UserControl, INotifyPropertyChanged
  {

    #region Private Data Members & Constants ----------------------------------

    private int numTotalQuestions;
    private const int RoundMaxScore = 999;
    private const int RoundMinScore = 0;
    private ObservableCollection<ScoreRow> mItemsSource;
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Constructors ------------------------------------------------------

    public TeamScoreTracker()
    {
      InitializeComponent();

      numTotalQuestions = 13;

      Reset();

      Loaded += (s, e) =>
      {
        ScoreDataGrid.ItemsSource = ItemsSource;

        DataGridColumn newCol;
        TextBlock tb;

        for(int i = 0; i < numTotalQuestions; i++)
        {
          tb = new TextBlock();
          tb.Text = "Round " + i.ToString();
          tb.Style = App.Current.Resources["FamilyFeudTextBlockStyle"] as Style;
          tb.Background = new SolidColorBrush(Colors.Transparent);

          newCol = new DataGridTemplateColumn()
          {
            Header = tb,
            Width = 125,
            HeaderStyle = App.Current.Resources["FamilyFeudGridHeaderStyle"] as Style,
            CellTemplate = Resources["StandardDataGridCellTemplate"] as DataTemplate,
          };

          ScoreDataGrid.Columns.Add(newCol);
        }

        ScoreDataGrid.Height = (ItemsSource.Count * 50) + 100;
      };
    }

    #endregion

    #region Public Methods ----------------------------------------------------
    
    /// <summary>
    /// Sets the number of columns in this team score tracker control. This will reset any data currently held.
    /// </summary>
    /// <param name="numColumns">
    /// Number of columns for the control.
    /// </param>
    public void SetColumnCount(int numColumns)
    {
      numTotalQuestions = numColumns < 1 ? 1 : numColumns > 100 ? 100 : numColumns;
      Reset();
    }


    public void Reset()
    {
      ItemsSource = new ObservableCollection<ScoreRow>()
      {
        new ScoreRow(numTotalQuestions, CommonConst.EmptyString),
        new ScoreRow(numTotalQuestions, CommonConst.EmptyString),
        new ScoreRow(numTotalQuestions, CommonConst.EmptyString),
        new ScoreRow(numTotalQuestions, CommonConst.EmptyString),
        new ScoreRow(numTotalQuestions, CommonConst.EmptyString),
        new ScoreRow(numTotalQuestions, CommonConst.EmptyString),
        new ScoreRow(numTotalQuestions, CommonConst.EmptyString)
      };

      if(ScoreDataGrid != null)
      {
        ScoreDataGrid.ItemsSource = ItemsSource;
        ScoreDataGrid.Height = (ItemsSource.Count * 50) + 100;
      }
    }

    #endregion

    #region Private Methods ---------------------------------------------------

    private void SetCellBindings(DataGridRow row)
    {
      DataGridCell cell;
      TextBox cellTB;

      row.Loaded += (a, e) =>
      {
        for(int i = 0; i < (row.Item as ScoreRow).Scores.Count; i++)
        {
          cell = ScoreDataGrid.GetCell(row, i + 2);
          cellTB = cell.FindFirstVisualChild<TextBox>() as TextBox;
          cellTB.SetBinding(TextBox.TextProperty, 
                            new Binding()
                            {
                              Path = new PropertyPath("Scores[" + i.ToString() + "].PointValue"),
                              Mode = BindingMode.TwoWay,
                              UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                            });
          cellTB.PreviewTextInput += (sender, eventArgs) =>
          {
            int val;

            if(string.IsNullOrEmpty((sender as TextBox).Text + eventArgs.Text))
            {
              return;
            }
            
            eventArgs.Handled = !int.TryParse((sender as TextBox).Text + eventArgs.Text, out val) || val < RoundMinScore || val > RoundMaxScore;
          };
        }
      };
    }

    private void ScoreDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
      SetCellBindings(e.Row);
    }

    private void AddRow_Click(object sender, RoutedEventArgs e)
    {
      ScoreRow newRow = new ScoreRow(numTotalQuestions, CommonConst.EmptyString);

      ItemsSource.Add(newRow);
      ScoreDataGrid.Height = (ItemsSource.Count * 50) + 60;
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
      Reset();
    }

    private void TeamNameBorder_MouseUp(object sender, MouseButtonEventArgs e)
    {
      TextBox nameTB;

      nameTB = (sender as Border).FindFirstVisualChild<TextBox>();
      if(nameTB == null)
      {
        return;
      }

      FocusManager.SetFocusedElement(this, nameTB);
      Keyboard.Focus(nameTB);
    }

    private void ScoreHeader_Click(object sender, RoutedEventArgs e)
    {
      bool isLowToHigh = true;
      bool isHighToLow = true;
      ScoreRow selectedRow;
      int currPosition;
      int selectedPosition;

      if(ItemsSource.Count <= 1)
      {
        return;
      }

      for(int i = 1; i < ItemsSource.Count; i++)
      {
        isHighToLow &= ItemsSource[i].CompareTo(ItemsSource[i - 1]) >= 0;
        isLowToHigh &= ItemsSource[i].CompareTo(ItemsSource[i - 1]) <= 0;
      }

      // Writing our own lazy selection sort here since the ObservableCollection doesn't support Sort() and creating new lists via OrderBy() is unecessary.
      // Since we're expecting ~15 teams at most, then the n^2 is acceptable
      if ((!isHighToLow && !isLowToHigh) || isLowToHigh)
      {
        //ScoreDataGrid.ItemsSource = ItemsSource = new ObservableCollection<ScoreRow>(ItemsSource.OrderBy(i => i.ScoreTotal).ToList());

        currPosition = 0;
        for(int i = 0; i < ItemsSource.Count - 1; i++)
        {
          selectedRow = ItemsSource[i];
          selectedPosition = i;
          for(int j = i + 1; j < ItemsSource.Count; j++)
          {
            if(selectedRow.CompareTo(ItemsSource[j]) > 0)
            {
              selectedRow = ItemsSource[j];
              selectedPosition = j;
            }
          }
          ItemsSource.Move(selectedPosition, currPosition);
          currPosition++;
        }

      }
      else if(isHighToLow)
      {
        currPosition = 0;
        for (int i = 0; i < ItemsSource.Count - 1; i++)
        {
          selectedRow = ItemsSource[i];
          selectedPosition = i;
          for (int j = i + 1; j < ItemsSource.Count; j++)
          {
            if (selectedRow.CompareTo(ItemsSource[j]) < 0)
            {
              selectedRow = ItemsSource[j];
              selectedPosition = j;
            }
          }
          ItemsSource.Move(selectedPosition, currPosition);
          currPosition++;
        }
      }
    }

    #endregion

    #region Properties --------------------------------------------------------

    private ObservableCollection<ScoreRow> ItemsSource
    {
      get
      {
        return mItemsSource;
      }
      set
      {
        if(value != mItemsSource)
        {
          mItemsSource = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemsSource)));
        }
      }
    }

    #endregion

    #region Private Classes ---------------------------------------------------
    // None.
    #endregion
  }
}
