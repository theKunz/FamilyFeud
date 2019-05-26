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

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for SingleQuestionControl.xaml
  /// </summary>
  public partial class SingleQuestionControl : UserControl, INotifyPropertyChanged
  {
    private int mNumAnswers;
    private Round mRound;
    private bool mNextEnabled;
    private bool mPreviousEnabled;
    private MediaPlayer mMediaPlayer;

    public event EventHandler NextClickEvent;
    public event EventHandler PreviousClickEvent;
    public event PropertyChangedEventHandler PropertyChanged;

    public SingleQuestionControl() : this(new ObservableCollection<Answer>(), CommonConst.EmptyString)
    {
      // Blank for now, need a parameterless constructor to be used as a control
    }

    public SingleQuestionControl(ObservableCollection<Answer> answers, string question) : this(new Round(question, answers))
    {
      // Blank for now
    }

    public SingleQuestionControl(Round round) 
    {
      InitializeComponent();

      Loaded += (object sender, RoutedEventArgs args) =>
      {
        ItemSource = round;
      };

      mNumAnswers = round.Answers.Count;

      KeyUp += KeyPressed;

      mMediaPlayer = new MediaPlayer();
    }

    private void SetAnswerSources()
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
    }

    private void KeyPressed(object sender, KeyEventArgs args)
    {
      RevealAnswer(args.Key - Key.D0);
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

    public bool NextEnabled
    {
      get
      {
        return true;
        //return mNextEnabled;
      }
      set
      {
        mNextEnabled = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NextEnabled"));
      }
    }

    public bool PreviousEnabled
    {
      get
      {
        return true;
        //return mPreviousEnabled;
      }
      set
      {
        mPreviousEnabled = value;
        PropertyChanged(this, new PropertyChangedEventArgs("PreviousEnabled"));
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
        SetAnswerSources();
      }
    }
  }
}
