using FamilyFeud.CustomEventArgs;
using FamilyFeud.DataObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for QuestionBuilder.xaml
  /// </summary>
  public partial class QuestionBuilder : Window, INotifyPropertyChanged
  {
    private bool mIsNormalQuestion;
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<EventArgs<Question>> QuestionComplete;
    public event EventHandler<EventArgs<BonusQuestion>> BonusQuestionComplete;
  
    public QuestionBuilder()
    {
      PropertyChanged += PropertyChanged_RenameAnswerField;
      InitializeComponent();
      mIsNormalQuestion = true;
    }
  
    public QuestionBuilder(bool isBonus)
    {
      PropertyChanged += PropertyChanged_RenameAnswerField;
      InitializeComponent();
      mIsNormalQuestion = isBonus;
    }

    private void PropertyChanged_RenameAnswerField(object sender, PropertyChangedEventArgs args)
    {
      if(args.PropertyName.Equals(nameof(IsNormalQuestion)))
      {
        if(IsNormalQuestion)
        {
          tbAnswer1.Text = "Answer 1:";
        }
        else
        {
          tbAnswer1.Text = "Answer:";
        }
      }
    }

    private bool CheckNonEmptyStackPanelTextBoxes(StackPanel sp)
    {
      bool isNonEmpty = true;

      foreach(object o in sp.Children)
      {
        TextBox tb = o as TextBox;
        if(tb != null)
        {
          isNonEmpty &= string.IsNullOrWhiteSpace(tb.Text);
        }
      }

      return isNonEmpty;
    }
    
    public bool IsNormalQuestion
    {
      get
      {
        return mIsNormalQuestion;
      }
      set
      {
        if(value != mIsNormalQuestion)
        {
          mIsNormalQuestion = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNormalQuestion)));
        }
      }
    }

    public bool CanSave
    {
      get
      {
        if(mIsNormalQuestion)
        {
          return CheckNonEmptyStackPanelTextBoxes(spQuestion) &&
                 CheckNonEmptyStackPanelTextBoxes(spAnswer1) &&
                 CheckNonEmptyStackPanelTextBoxes(spAnswer2) &&
                 CheckNonEmptyStackPanelTextBoxes(spAnswer3) &&
                 CheckNonEmptyStackPanelTextBoxes(spAnswer4);
        }
        else
        {
          return CheckNonEmptyStackPanelTextBoxes(spQuestion) &&
                 CheckNonEmptyStackPanelTextBoxes(spAnswer1);
        }
      }
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs eventArgs)
    {
      int val;

      if(!int.TryParse((sender as TextBox).Text + eventArgs.Text, out val))
      {
        eventArgs.Handled = true;
      }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      
    }
  }
}
