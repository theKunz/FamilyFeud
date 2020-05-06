using CommonLib.Constants;
using FamilyFeud.DataObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for SingleQuestionControl.xaml
  /// </summary>
  public partial class SingleQuestionControl : UserControl, INotifyPropertyChanged, IRoundControl
  {
    private int mNumAnswers;
    private Round mRound;
    private bool mNextEnabled;
    private bool mPreviousEnabled;
    private MediaPlayer mXMediaPlayer;
    private Storyboard showQuestionStory;
    private Storyboard showXStory;
    private bool mIsQuestionShown = false;
    private bool mShowQuestionOnLoad;
    private int XCount;

    public event EventHandler NextClickEvent;
    public event EventHandler PreviousClickEvent;
    public event PropertyChangedEventHandler PropertyChanged;

    public SingleQuestionControl() : this(new ObservableCollection<Answer>(), CommonConst.EmptyString)
    {
      // Blank for now, need a parameterless constructor to be used as a control
    }

    public SingleQuestionControl(ObservableCollection<Answer> answers, string question, bool showQuestionOnLoad = false) : this(new Round(question, answers), showQuestionOnLoad)
    {
      // Blank for now.
    }

    public SingleQuestionControl(Round round, bool showQuestionOnLoad = false) 
    {
      InitializeComponent();

      mShowQuestionOnLoad = showQuestionOnLoad;

      PreviousEnabled = true;
      NextEnabled = true;

      ItemSource = round;
      
      Loaded += SQC_Loaded;

      mNumAnswers = round.Answers.Count;

      KeyUp += KeyPressed;


      DataContext = this;

      XCount = 0;

      mXMediaPlayer = new MediaPlayer();
      mXMediaPlayer.IsMuted = true;
      mXMediaPlayer.MediaEnded += (sender, args) =>
      {
        mXMediaPlayer.Stop();
        mXMediaPlayer.Position = new TimeSpan(0);
      };
      mXMediaPlayer.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"/Sounds/Wrong_Buzzer.wav", UriKind.RelativeOrAbsolute));
    }

    private void SQC_Loaded(object sender, RoutedEventArgs args)
    {
      Loaded -= SQC_Loaded;

      showQuestionStory = Resources["sbQuestionOpacityStory"] as Storyboard;
      showQuestionStory.Completed += (object storySender, EventArgs storyArgs) =>
      {
        IsQuestionShown = !IsQuestionShown;
        if(!IsQuestionShown)
        {
          bdrQuestionForeground.Visibility = Visibility.Collapsed;
          bdrQuestionBackground.Visibility = Visibility.Collapsed;
        }
      };

      showXStory = Resources["sbShowX"] as Storyboard;
      showXStory.Completed += (object storySender, EventArgs storyArgs) =>
      {
        XCount++;
      };

      if(mShowQuestionOnLoad)
      {
        ShowQuestion();
      }
    }

    private void SetQuestionInfo()
    {
      AnswerBox currAnswer;

      for(int i = 0; i < 8; i++)
      {
        currAnswer = FindName("Answer" + (i + 1).ToString()) as AnswerBox;

        if(ItemSource != null && i < ItemSource.NumAnswers)
        {
          currAnswer.AnswerSource = ItemSource.Answers[i];
          currAnswer.AnswerIndex = i + 1;
        }
        else
        {
          currAnswer.AnswerSource = null;
          currAnswer.AnswerIndex = null;
        }
      }

      tbQuestion.Text = ItemSource.Question.QuestionText;
    }

    private void KeyPressed(object sender, KeyEventArgs args)
    {
      if(args.Key == Key.X)
      {
        ShowX();
      }
      else
      {
        RevealAnswer(args.Key - Key.D0);
      }
    }

    private void ButtonNext_Click(object sender, RoutedEventArgs e)
    {
      NextClickEvent?.Invoke(sender, e);
    }

    private void ButtonPrev_Click(object sender, RoutedEventArgs e)
    {
      PreviousClickEvent?.Invoke(sender, e);
    }

    /// <summary>
    ///  Function is not idempotent. It will show 1 X, then 2 X's, then perpetually 3 X's
    /// </summary>
    public void ShowX()
    {
      string targetName = XCount == 0 ? WrongAnswer1.Name :
                          XCount == 1 ? WrongAnswer2.Name :
                          WrongAnswer3.Name;

      foreach(Timeline animation in showXStory.Children)
      {
        Storyboard.SetTargetName((DoubleAnimation)animation, targetName);
      }
      mXMediaPlayer.IsMuted = false;
      mXMediaPlayer.Volume = 1;
      mXMediaPlayer.Play();
      showXStory.Begin();
    }

    /// <summary>
    /// Reveals the answer at the given index. Ordering is column-wise from the left.
    /// No action will occur with invalid indexes.
    /// </summary>
    /// <param name="answerIndex"></param>
    public void RevealAnswer(int answerIndex)
    {
      if(answerIndex < 1 || answerIndex > 8 || answerIndex > mNumAnswers)
      {
        return;
      }

      (FindName("Answer" + answerIndex) as AnswerBox).ShowAnswer();
    }

    public void ShowQuestion()
    {
      if(!mIsQuestionShown)
      {
        bdrQuestionForeground.Visibility = Visibility.Visible;
        bdrQuestionBackground.Visibility = Visibility.Visible;
        (showQuestionStory.Children[0] as DoubleAnimation).From = 0.0;
        (showQuestionStory.Children[0] as DoubleAnimation).To = 0.9;
        (showQuestionStory.Children[1] as DoubleAnimation).From = 0.0;
        (showQuestionStory.Children[1] as DoubleAnimation).To = 1.0;
        showQuestionStory.Begin();
      }
    }

    public void HideQuestion()
    {
      if(mIsQuestionShown)
      {
        (showQuestionStory.Children[0] as DoubleAnimation).From = 0.9;
        (showQuestionStory.Children[0] as DoubleAnimation).To = 0.0;
        (showQuestionStory.Children[1] as DoubleAnimation).From = 1.0;
        (showQuestionStory.Children[1] as DoubleAnimation).To = 0.0;
        showQuestionStory.Begin();
      }
    }

    public bool IsQuestionShown
    {
      get
      {
        return mIsQuestionShown;
      }
      set
      {
        if(mIsQuestionShown != value)
        {
          mIsQuestionShown = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsQuestionShown)));
        }
      }
    }

    public bool NextEnabled
    {
      get
      {
        return mNextEnabled;
      }
      set
      {
        mNextEnabled = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextEnabled)));
      }
    }

    public bool PreviousEnabled
    {
      get
      {
        return mPreviousEnabled;
      }
      set
      {
        mPreviousEnabled = value;
        PropertyChanged(this, new PropertyChangedEventArgs(nameof(PreviousEnabled)));
      }
    }

    public Round ItemSource
    {
      get
      {
        return mRound;
      }
      set
      {
        mRound = value;
        SetQuestionInfo();
      }
    }

    private void bdrQuestion_MouseUp(object sender, MouseButtonEventArgs args)
    {
      HideQuestion();
    }
  }
}
