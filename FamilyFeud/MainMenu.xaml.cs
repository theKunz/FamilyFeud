using FamilyFeud.Controls;
using FamilyFeud.DataObjects;
using FamilyFeud.CustomEventArgs;
using CommonLib.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using FamilyFeud.Helpers;
using System.Collections.Specialized;

namespace FamilyFeud
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    private GameWindow gameWindow;
    private ObservableCollection<IQuestioner> mQuestions;
    private ObservableCollection<IQuestioner> mBonusQuestions;
    private const int MaxRandomGameSize = 10;
    private const int MinRandomGameSize = 3;
    private static Random Rand = new Random((int)DateTime.UtcNow.Ticks); // ew dirty conversion from long to int
    private Game currentGame;
    public event PropertyChangedEventHandler PropertyChanged;

    private enum ButtonState
    {
      NoGame,
      NewGame,
      Questions,
      BonusRound
    }

    public MainWindow()
    {
      InitializeComponent();

      Loaded += (s, args) =>
      {
        BackgroundRoot.Height = 1080.0 - SystemParameters.CaptionHeight * 3 + 3;
        ChangeButtonState(ButtonState.NoGame);
      };

      this.KeyUp += KeyPressed;

      LoadSaveData();
      mBonusQuestions.CollectionChanged += (sender, args) =>
      {
        SaveFileHelpers.UpdateBonusRoundSaveData(mBonusQuestions);
      };
      mQuestions.CollectionChanged += (sender, args) =>
      {
        SaveFileHelpers.UpdateRoundSaveData(mQuestions);
      };
      QuestionList.ItemSource = mQuestions;
      BonusQuestionList.ItemSource = mBonusQuestions;

      this.DataContext = this;
    }

    private void MainMenu_Closed(object sender, EventArgs e)
    {
      gameWindow?.Close();
      App.Current.Shutdown();
    }

    private void btnBeginRandomGame_Click(object sender, RoutedEventArgs e)
    {
      ObservableCollection<Round> selectedRounds;
      Round currRound;
      int numRounds;

      selectedRounds = new ObservableCollection<Round>();
      numRounds = mQuestions.Count < MaxRandomGameSize ? mQuestions.Count : MaxRandomGameSize;

      while(selectedRounds.Count < numRounds)
      {
        currRound = mQuestions[Rand.Next() % mQuestions.Count] as Round;

        if(!selectedRounds.Any(r => currRound.Equals(r)))
        {
          selectedRounds.Add(currRound.Copy());
        }
      }

      StartGame(selectedRounds, null, false, false);
    }

    private void btnBeginCustomGame_Click(object sender, RoutedEventArgs e)
    {
      GameBuilder gb = new GameBuilder(mQuestions.Cast<Round>(), mBonusQuestions.Cast<BonusQuestion>());
      gb.GameBuildingCompleted += GameBuildingCompleted;
      gb.Show();
    }

    private void GameBuildingCompleted(object sender, GameBuildingCompletedEventArgs args)
    {
      ObservableCollection<Round> rounds = new ObservableCollection<Round>();
      BonusRound bonusRound = new BonusRound();

      // Create a deep copy to prevent editing of rounds and Bonus Questions from messing up existing games.
      // If you disable editing while a game is occurring, you can remove this.
      foreach(Round round in args.SelectedRounds)
      {
        rounds.Add(round.Copy());
      }

      foreach(BonusQuestion bonusQuestion in args.SelectedBonusQuestions)
      {
        bonusRound.BonusQuestions.Add(bonusQuestion.Copy());
      }

      foreach(Round round in args.NewRounds)
      {
        var matchingQuestion = mQuestions.FirstOrDefault(b => b.Question.Equals(round.Question));
        if(matchingQuestion != null)
        {
          mQuestions.Remove(matchingQuestion);
        }
        mQuestions.Insert(0, round.Copy());
      }

      foreach(BonusQuestion bonusQuestion in args.NewBonusQuestions)
      {
        var matchingQuestion = mBonusQuestions.FirstOrDefault(b => b.Question.Equals(bonusQuestion.Question));
        if(matchingQuestion != null)
        {
          mBonusQuestions.Remove(matchingQuestion);
        }
        mBonusQuestions.Insert(0, bonusQuestion.Copy());
      }

      StartGame(rounds, bonusRound, args.HasBonusRound, args.BonusAtEnd);
    }

    private void StartGame(IEnumerable<Round> questions, BonusRound bonusRound, bool includeBonusRound, bool isBonusRoundAtEnd)
    {
      EventHandler onClosed;
      
      gameWindow?.Close();

      currentGame = new Game(questions, bonusRound)
      {
        BonusRoundLocation = !includeBonusRound || bonusRound == null || bonusRound.BonusQuestions.Count == 0 ? 
                              BonusRoundLocation.None :
                             isBonusRoundAtEnd ?                                                                
                              BonusRoundLocation.End :
                             BonusRoundLocation.Middle    
      };

      onClosed = null;
      onClosed = (object sender, EventArgs args) =>
      {
        gameWindow.Closed -= onClosed;
        gameWindow.PropertyChanged -= OnPropertyChanged;
        ChangeButtonState(ButtonState.NoGame);
        CurrentRoundAnswers = new ObservableCollection<Answer>();
        currentGame = null;
      };
      gameWindow = new GameWindow(currentGame);
      gameWindow.PropertyChanged += OnPropertyChanged;
      gameWindow.Closed += onClosed;
      ChangeButtonState(ButtonState.NewGame);
      gameWindow.PropertyChanged += (sender, args) =>
      {
        if(args.PropertyName == nameof(GameWindow.IsQuestionShown))
        {
          if(gameWindow.IsQuestionShown != IsActiveQuestionShown)
          {
            ShowCurrentQuestionOverlay_Click(null, null);
          }
        }
      };

      gameWindow.Show();
    }

    private ObservableCollection<Answer> mCurrentRoundAnswers;
    public ObservableCollection<Answer> CurrentRoundAnswers
    {
      get
      {
        return mCurrentRoundAnswers;
      }
      set
      {
        if(value != mCurrentRoundAnswers)
        {
          mCurrentRoundAnswers = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentRoundAnswers)));
        }
      }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      if(args.PropertyName == nameof(GameWindow.CurrentRound))
      {
        if(gameWindow.CurrentRound is SingleQuestionControl)
        {
          IsActiveQuestionShown = true;
          ShowCurrentQuestionOverlay.Content = "Hide Current Question";
          ChangeButtonState(ButtonState.Questions);
          CurrentRoundAnswers = new ObservableCollection<Answer>((gameWindow.CurrentRound as SingleQuestionControl).ItemSource.Answers);
          GoToPreviousQuestion.IsEnabled = gameWindow.CurrentRound.PreviousEnabled;
          GoToNextQuestion.IsEnabled = gameWindow.CurrentRound.NextEnabled;
        }
        else if(gameWindow.CurrentRound is BonusRoundControl)
        {
          ChangeButtonState(ButtonState.BonusRound);
          CurrentRoundAnswers = new ObservableCollection<Answer>((gameWindow.CurrentRound as BonusRoundControl).BonusData.BonusQuestions.Select(bq => bq.Answer));
          GoToPreviousQuestion.IsEnabled = gameWindow.CurrentRound.PreviousEnabled;
          GoToNextQuestion.IsEnabled = gameWindow.CurrentRound.NextEnabled;
        }
        else
        {
          CurrentRoundAnswers = new ObservableCollection<Answer>();
        }

        for(int i = 0; i < 10; i++)
        {
          (this.FindName("btnShow" + i) as Button).IsEnabled = i < CurrentRoundAnswers.Count;
        }
      }
    }

    public void LoadSaveData()
    {
      mQuestions = new ObservableCollection<IQuestioner>(SaveFileHelpers.LoadRoundSaveData());
      mBonusQuestions = new ObservableCollection<IQuestioner>(SaveFileHelpers.LoadBonusRoundSaveData());
    }

    private void ChangeButtonState(ButtonState newState)
    {
      bool bonusExists = currentGame != null ? currentGame.BonusRoundLocation != BonusRoundLocation.None : false;

      if(newState == ButtonState.NoGame)
      {
        StartIntro.Visibility = Visibility.Collapsed;
        ShowCurrentQuestionOverlay.Visibility = Visibility.Collapsed;
        GoToFirstQuestion.Visibility = Visibility.Collapsed;
        GoToNextQuestion.Visibility = Visibility.Collapsed;
        GoToPreviousQuestion.Visibility = Visibility.Collapsed;
        btnShow0.Visibility = Visibility.Collapsed;
        btnShow1.Visibility = Visibility.Collapsed;
        btnShow2.Visibility = Visibility.Collapsed;
        btnShow3.Visibility = Visibility.Collapsed;
        btnShow4.Visibility = Visibility.Collapsed;
        btnShow5.Visibility = Visibility.Collapsed;
        btnShow6.Visibility = Visibility.Collapsed;
        btnShow7.Visibility = Visibility.Collapsed;
        btnShow8.Visibility = Visibility.Collapsed;
        btnShow9.Visibility = Visibility.Collapsed;
        WrongAnswer.Visibility = Visibility.Collapsed;
        EndGame.Visibility = Visibility.Collapsed;
        StartBonusTimer.Visibility = Visibility.Collapsed;
        btnGetBonusSheet.Visibility = Visibility.Collapsed;
        
      }
      else if(newState == ButtonState.NewGame)
      {
        StartIntro.Visibility = Visibility.Visible;
        StartIntro.IsEnabled = true;
        ShowCurrentQuestionOverlay.Visibility = Visibility.Collapsed;
        GoToFirstQuestion.Visibility = Visibility.Visible;
        GoToFirstQuestion.IsEnabled = false;
        GoToNextQuestion.Visibility = Visibility.Collapsed;
        GoToPreviousQuestion.Visibility = Visibility.Collapsed;
        btnShow0.Visibility = Visibility.Collapsed;
        btnShow1.Visibility = Visibility.Collapsed;
        btnShow2.Visibility = Visibility.Collapsed;
        btnShow3.Visibility = Visibility.Collapsed;
        btnShow4.Visibility = Visibility.Collapsed;
        btnShow5.Visibility = Visibility.Collapsed;
        btnShow6.Visibility = Visibility.Collapsed;
        btnShow7.Visibility = Visibility.Collapsed;
        btnShow8.Visibility = Visibility.Collapsed;
        btnShow9.Visibility = Visibility.Collapsed;
        WrongAnswer.Visibility = Visibility.Collapsed;
        EndGame.Visibility = Visibility.Visible;
        Grid.SetColumn(EndGame, 0);
        Grid.SetColumnSpan(EndGame, 2);
        StartBonusTimer.Visibility = Visibility.Collapsed;
        btnGetBonusSheet.Visibility = Visibility.Visible;
        Grid.SetRow(btnGetBonusSheet, 7);
        Grid.SetColumnSpan(btnGetBonusSheet, 2);
        btnGetBonusSheet.IsEnabled = bonusExists;
      }
      else if(newState == ButtonState.Questions)
      {
        StartIntro.Visibility = Visibility.Collapsed;
        ShowCurrentQuestionOverlay.Visibility = Visibility.Visible;
        GoToFirstQuestion.Visibility = Visibility.Collapsed;
        GoToNextQuestion.Visibility = Visibility.Visible;
        GoToPreviousQuestion.Visibility = Visibility.Visible;
        btnShow0.Visibility = Visibility.Visible; // Answer 1
        btnShow1.Visibility = Visibility.Visible; // Answer 2
        btnShow2.Visibility = Visibility.Visible; // Answer 3
        btnShow3.Visibility = Visibility.Visible; // Answer 4
        btnShow4.Visibility = Visibility.Visible; // Answer 5
        btnShow5.Visibility = Visibility.Visible; // Answer 6
        btnShow6.Visibility = Visibility.Visible; // Answer 7
        btnShow7.Visibility = Visibility.Visible; // Answer 8
        btnShow8.Visibility = Visibility.Collapsed; // Answer 9
        btnShow9.Visibility = Visibility.Collapsed; // Answer 10
        WrongAnswer.Visibility = Visibility.Visible;
        Grid.SetRow(WrongAnswer, 9);
        EndGame.Visibility = Visibility.Visible;
        Grid.SetColumn(EndGame, 1);
        Grid.SetColumnSpan(EndGame, 1);
        StartBonusTimer.Visibility = Visibility.Collapsed;
        btnGetBonusSheet.Visibility = Visibility.Visible;
        Grid.SetRow(btnGetBonusSheet, 13);
        Grid.SetColumnSpan(btnGetBonusSheet, 1);
        btnGetBonusSheet.IsEnabled = bonusExists;
      }
      else if(newState == ButtonState.BonusRound)
      {
        StartIntro.Visibility = Visibility.Collapsed;
        ShowCurrentQuestionOverlay.Visibility = Visibility.Collapsed;
        GoToFirstQuestion.Visibility = Visibility.Collapsed;
        GoToNextQuestion.Visibility = Visibility.Visible;
        GoToPreviousQuestion.Visibility = Visibility.Visible;
        btnShow0.Visibility = Visibility.Visible; // Answer 1
        btnShow1.Visibility = Visibility.Visible; // Answer 2
        btnShow2.Visibility = Visibility.Visible; // Answer 3
        btnShow3.Visibility = Visibility.Visible; // Answer 4
        btnShow4.Visibility = Visibility.Visible; // Answer 5
        btnShow5.Visibility = Visibility.Visible; // Answer 6
        btnShow5.Visibility = Visibility.Visible; // Answer 6
        btnShow6.Visibility = Visibility.Visible; // Answer 7
        btnShow7.Visibility = Visibility.Visible; // Answer 8
        btnShow8.Visibility = Visibility.Visible; // Answer 9
        btnShow9.Visibility = Visibility.Visible; // Answer 10
        WrongAnswer.Visibility = Visibility.Visible;
        Grid.SetRow(WrongAnswer, 11);
        EndGame.Visibility = Visibility.Visible;
        Grid.SetColumn(EndGame, 1);
        Grid.SetColumnSpan(EndGame, 1);
        StartBonusTimer.Visibility = Visibility.Visible;
        btnGetBonusSheet.Visibility = Visibility.Visible;
        Grid.SetRow(btnGetBonusSheet, 13);
        Grid.SetColumnSpan(btnGetBonusSheet, 1);
        btnGetBonusSheet.IsEnabled = bonusExists;
      }
    }

    private void btnAddBonusQuestion_Click(object sender, RoutedEventArgs e)
    {
      QuestionBuilder qb = new QuestionBuilder(true);
      EventHandler<EventArgs<BonusQuestion>> bqComplete;
      EventHandler bqClosed;

      bqComplete = null;
      bqComplete = (object s, EventArgs<BonusQuestion> args) =>
      {
        qb.BonusQuestionComplete -= bqComplete;

        if(mBonusQuestions.Any(b => args.Data.Question.Equals(b.Question)))
        {
          if(MessageBox.Show("This question already exists. Overwrite?", "Question Already Exists ", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
          {
            mBonusQuestions.Remove(mBonusQuestions.First(b => args.Data.Question.Equals(b.Question)));
            mBonusQuestions.Insert(0, args.Data);
          }
        }
        else
        {
          mBonusQuestions.Insert(0, args.Data);
        }

      };

      bqClosed = null;
      bqClosed = (object s, EventArgs args) =>
      {
        qb.BonusQuestionComplete -= bqComplete;
        qb.Closed -= bqClosed;
      };

      qb.Closed += bqClosed;
      qb.BonusQuestionComplete += bqComplete;

      qb.Title = "Add Bonus Question";
      qb.ShowDialog();
    }

    private void btnAddQuestion_Click(object sender, RoutedEventArgs e)
    {
      QuestionBuilder qb = new QuestionBuilder(false);
      EventHandler<EventArgs<Round>> qComplete;
      EventHandler bqClosed;

      qComplete = null;
      qComplete = (object s, EventArgs<Round> args) =>
      {
        qb.QuestionComplete -= qComplete;

        
        if (mQuestions.Any(existing => args.Data.Question.Equals(existing.Question)))
        {
          if(MessageBox.Show("This question already exists. Overwrite?", "Question Already Exists", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
          {
            mQuestions.Remove(mQuestions.First(b => args.Data.Question.Equals(b.Question)));
            mQuestions.Insert(0, args.Data);
          }
        }
        else
        {
          mQuestions.Insert(0, args.Data);
        }
      };

      bqClosed = null;
      bqClosed = (object s, EventArgs args) =>
      {
        qb.QuestionComplete -= qComplete;
        qb.Closed -= bqClosed;
      };

      qb.Closed += bqClosed;
      qb.QuestionComplete += qComplete;

      qb.Title = "Add Question";
      qb.ShowDialog();
    }

    private void btnShowAnswer_Click(object sender, RoutedEventArgs e)
    {
      if(currentGame == null)
      {
        return;
      }

      Button clickedButton = sender as Button;

      int answerToShow = int.Parse(clickedButton.Name.Substring(clickedButton.Name.Length - 1)) + 1;

      answerToShow = answerToShow == 10 ? 0 : answerToShow;

      ShowAnswer(answerToShow);
    }

    private void KeyPressed(object sender, KeyEventArgs args)
    {
      Key[] numKeys = { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0 };

      if(numKeys.Contains(args.Key))
      {
        ShowAnswer(args.Key - Key.D0);
      }
      else if(args.Key == Key.X)
      {
        ShowX();
      }
      else if(args.Key == Key.Right)
      {
        TransitionNextQuestion();
      }
      else if(args.Key == Key.Left)
      {
        TransitionPreviousQuestion();
      }
      else if(args.Key == Key.H && IsActiveQuestionShown)
      {
        ShowCurrentQuestionOverlay_Click(this, new RoutedEventArgs());
      }
      else if(args.Key == Key.S && !IsActiveQuestionShown)
      {
        ShowCurrentQuestionOverlay_Click(this, new RoutedEventArgs());
      }
    }

    private void ShowAnswer(int answerIndex)
    {
      gameWindow?.ShowAnswerOnActiveQuestion(answerIndex);
    }

    private void ShowX()
    {
      gameWindow?.ShowXOnActiveQuestion();
    }

    private void TransitionNextQuestion()
    {
      gameWindow?.GoToNext();
    }

    private void TransitionPreviousQuestion()
    {
      gameWindow?.GoToPrevious();
    }

    private void WrongAnswer_Click(object sender, RoutedEventArgs e)
    {
      ShowX();
    }

    private void GoToPreviousQuestion_Click(object sender, RoutedEventArgs e)
    {
      TransitionPreviousQuestion();
    }

    private void GoToNextQuestion_Click(object sender, RoutedEventArgs e)
    {
      TransitionNextQuestion();
    }

    private void GoToFirstQuestion_Click(object sender, RoutedEventArgs e)
    {
      gameWindow.BeginQuestions();
    }

    private bool bonusTimerCounting = false;

    private void StartBonusTimer_Click(object sender, RoutedEventArgs e)
    {
      if(!bonusTimerCounting)
      {
        StartBonusTimer.Content = "Pause Countdown";
        gameWindow?.BeginBonusRoundCountdown();
      }
      else
      {
        StartBonusTimer.Content = "Start Countdown";
        gameWindow?.StopBonusRoundCountdown();
      }

      bonusTimerCounting = !bonusTimerCounting;
    }

    private void StartIntro_Click(object sender, RoutedEventArgs e)
    {
      gameWindow?.BeginIntro();
      StartIntro.IsEnabled = false;
      GoToFirstQuestion.IsEnabled = true;
    }

    private void EndGame_Click(object sender, RoutedEventArgs e)
    {
      gameWindow?.Close();
    }

    private bool IsActiveQuestionShown;
    private void ShowCurrentQuestionOverlay_Click(object sender, RoutedEventArgs e)
    {
      if(IsActiveQuestionShown)
      {
        gameWindow?.HideCurrentQuestionOverlay();
        ShowCurrentQuestionOverlay.Content = "Show Current Question";
      }
      else
      {
        gameWindow?.ShowCurrentQuestionOverlay();
        ShowCurrentQuestionOverlay.Content = "Hide Current Question";
      }

      IsActiveQuestionShown = !IsActiveQuestionShown;
    }

    private Window mHotkeyWindow;
    private void btnHotkeys_Click(object sender, RoutedEventArgs e)
    {
      if(mHotkeyWindow != null)
      {
        mHotkeyWindow.Activate();
        return;
      }

      mHotkeyWindow = new Window();
      mHotkeyWindow.Content = new HotkeyListControl();
      mHotkeyWindow.SizeToContent = SizeToContent.WidthAndHeight;
      mHotkeyWindow.ResizeMode = ResizeMode.NoResize;
      mHotkeyWindow.Icon = new BitmapImage(new Uri(@"pack://application:,,,/FamilyFeud;component/Images/FamilyFeudIcon.png", UriKind.RelativeOrAbsolute));
      mHotkeyWindow.Title = "Family Feud";
      mHotkeyWindow.Closed += (send, args) =>
      {
        mHotkeyWindow = null;
      };
      mHotkeyWindow.Show();
    }

    private void btnGetBonusSheet_Click(object sender, RoutedEventArgs e)
    {
      if(currentGame != null)
      {
        ImagePrinter.DownloadBonusRoundImage(currentGame.BonusRound);
      }
    }
  }
}
