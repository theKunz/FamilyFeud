using CommonLib.CustomEventArgs;
using FamilyFeud.DataObjects;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for QuestionBuilder.xaml
  /// </summary>
  public partial class QuestionBuilder : Window, INotifyPropertyChanged
  {
    private bool mIsNormalQuestion;
    private bool mCanSave;
    private Round mRound;
    private BonusQuestion mBonusQuestion;
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<EventArgs<Round>> QuestionComplete;
    public event EventHandler<EventArgs<BonusQuestion>> BonusQuestionComplete;

    // TODO: Add edit existing question

    public QuestionBuilder()
    {
      mIsNormalQuestion = true;
      mCanSave = false;
      PropertyChanged += PropertyChanged_RenameAnswerField;
      Loaded += OnLoaded_UpdateIsNormalQuestion;
      DataContext = this;
      InitializeComponent();
    }

    public QuestionBuilder(bool isBonusQuestion)
    {
      mIsNormalQuestion = !isBonusQuestion;
      mCanSave = false;
      PropertyChanged += PropertyChanged_RenameAnswerField;
      Loaded += OnLoaded_UpdateIsNormalQuestion;
      DataContext = this;
      InitializeComponent();
    }

    public QuestionBuilder(Round round)
    {
      mIsNormalQuestion = true;
      mCanSave = true;
      mRound = round.Copy();
      PropertyChanged += PropertyChanged_RenameAnswerField;
      Loaded += OnLoaded_UpdateIsNormalQuestion;
      Loaded += OnLoaded_InitializeRound;
      DataContext = this;
      InitializeComponent();
    }

    public QuestionBuilder(BonusQuestion bonusQuestion)
    {
      mIsNormalQuestion = false;
      mCanSave = true;
      mBonusQuestion = bonusQuestion.Copy();
      PropertyChanged += PropertyChanged_RenameAnswerField;
      Loaded += OnLoaded_UpdateIsNormalQuestion;
      Loaded += OnLoaded_InitializeBonus;
      DataContext = this;
      InitializeComponent();
    }

    private void OnLoaded_UpdateIsNormalQuestion(object sender, RoutedEventArgs args)
    {
      Loaded -= OnLoaded_UpdateIsNormalQuestion;

      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNormalQuestion)));
    }

    private void OnLoaded_InitializeRound(object sender, RoutedEventArgs args)
    {
      Loaded -= OnLoaded_InitializeRound;

      if(mRound == null)
      {
        return;
      }

      tbQuestion.Text = mRound.Question.QuestionText;

      TextBox answerTb;
      TextBox pointTb;

      for(int i = 0; i < mRound.Answers.Count; i++)
      {
        answerTb = FindName("tbAnswer" + (i + 1)) as TextBox;
        pointTb = FindName("tbAnswer" + (i + 1) + "Points") as TextBox;

        answerTb.Text = mRound.Answers.ElementAt(i).AnswerText;
        pointTb.Text = mRound.Answers.ElementAt(i).PointValue.ToString();
      }
    }

    private void OnLoaded_InitializeBonus(object sender, RoutedEventArgs args)
    {
      Loaded -= OnLoaded_InitializeBonus;

      if(mBonusQuestion == null)
      {
        return;
      }

      tbQuestion.Text = mBonusQuestion.Question.QuestionText;
      tbAnswer1.Text = mBonusQuestion.Answer.AnswerText;
      tbAnswer1Points.Text = mBonusQuestion.Answer.PointValue.ToString();
    }

    private void PropertyChanged_RenameAnswerField(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName.Equals(nameof(IsNormalQuestion)))
      {
        if (IsNormalQuestion)
        {
          labelAnswer1.Text = "Answer 1:";
        }
        else
        {
          labelAnswer1.Text = "Answer:";
        }
      }
    }

    private bool CheckNonEmptyStackPanelTextBoxes(StackPanel sp)
    {
      bool isNonEmpty = true;

      foreach (object o in sp.Children)
      {
        TextBox tb = o as TextBox;
        if (tb != null)
        {
          isNonEmpty &= !string.IsNullOrWhiteSpace(tb.Text);
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
        if (value != mIsNormalQuestion)
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
        return mCanSave;
      }
      private set
      {
        if (value != mCanSave)
        {
          mCanSave = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanSave)));
        }
      }
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs eventArgs)
    {
      eventArgs.Handled = !uint.TryParse((sender as TextBox).Text + eventArgs.Text, out _);
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if (mIsNormalQuestion)
      {
        Round round = new Round();
        round.Question = new Question(tbQuestion.Text);
        round.Answers = new ObservableCollection<Answer>();

        if (CheckNonEmptyStackPanelTextBoxes(spAnswer1)) { round.Answers.Add(new Answer(tbAnswer1.Text, uint.Parse(tbAnswer1Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer2)) { round.Answers.Add(new Answer(tbAnswer2.Text, uint.Parse(tbAnswer2Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer3)) { round.Answers.Add(new Answer(tbAnswer3.Text, uint.Parse(tbAnswer3Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer4)) { round.Answers.Add(new Answer(tbAnswer4.Text, uint.Parse(tbAnswer4Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer5)) { round.Answers.Add(new Answer(tbAnswer5.Text, uint.Parse(tbAnswer5Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer6)) { round.Answers.Add(new Answer(tbAnswer6.Text, uint.Parse(tbAnswer6Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer7)) { round.Answers.Add(new Answer(tbAnswer7.Text, uint.Parse(tbAnswer7Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer8)) { round.Answers.Add(new Answer(tbAnswer8.Text, uint.Parse(tbAnswer8Points.Text))); }

        round.Answers = new ObservableCollection<Answer>(round.Answers.OrderByDescending(a => a.PointValue));

        QuestionComplete?.Invoke(this, new EventArgs<Round>(round));
      }
      else
      {
        BonusQuestion bonusQuestion = new BonusQuestion()
        {
          Question = new Question(tbQuestion.Text),
          Answer = new Answer(tbAnswer1.Text, uint.Parse(tbAnswer1Points.Text))
        };

        BonusQuestionComplete?.Invoke(this, new EventArgs<BonusQuestion>(bonusQuestion));
      }

      this.Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs args)
    {
      if (IsNormalQuestion)
      {
        CanSave = CheckNonEmptyStackPanelTextBoxes(spQuestion) &&
                 (CheckNonEmptyStackPanelTextBoxes(spAnswer1) ||
                  CheckNonEmptyStackPanelTextBoxes(spAnswer2) ||
                  CheckNonEmptyStackPanelTextBoxes(spAnswer3) ||
                  CheckNonEmptyStackPanelTextBoxes(spAnswer4) ||
                  CheckNonEmptyStackPanelTextBoxes(spAnswer5) ||
                  CheckNonEmptyStackPanelTextBoxes(spAnswer6) ||
                  CheckNonEmptyStackPanelTextBoxes(spAnswer7) ||
                  CheckNonEmptyStackPanelTextBoxes(spAnswer8));
      }
      else
      {
        CanSave = CheckNonEmptyStackPanelTextBoxes(spQuestion) &&
                  CheckNonEmptyStackPanelTextBoxes(spAnswer1);
      }
    }
  }
}
