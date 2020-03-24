using FamilyFeud.Controls;
using FamilyFeud.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace FamilyFeud
{ 
  /// <summary>
  /// Interaction logic for GameWindow.xaml
  /// </summary>
  public partial class GameWindow : Window
  {
    public static SoundPlayer mSoundPlayerIntro = new SoundPlayer(@"../../Sounds/Opening_Theme.wav");

    private const double TRANSFORM_DISTANCE = 1920.0;

    private Size originalNonMaximizedSize;
    private List<Key> mAttachedKeys;
    private int currentQuestion;
    private MediaPlayer mMediaPlayerQuestion;
    private int mBonusRoundIndex;

    private Game mGame;

    private IRoundControl mPreviousQuestion;
    private IRoundControl mActiveQuestion;
    private IRoundControl mNextQuestion;

    public GameWindow(Game game)
    {
      InitializeComponent();

      if(game == null || game.NumRounds < 2)
      {
        throw new ArgumentException("Game cannot be null and must have at least two rounds");
      }

      mGame = game;
      
      mAttachedKeys = new List<Key>() { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0 };

      switch(mGame.BonusRoundLocation)
      {
        case BonusRoundLocation.Middle:
          mBonusRoundIndex = (int)(game.NumRounds / 2);
          ExistingQuestions = new IRoundControl[mGame.NumRounds + 1];
          break;
        case BonusRoundLocation.End:
          mBonusRoundIndex = (int)game.NumRounds;
          ExistingQuestions = new IRoundControl[mGame.NumRounds + 1];
          break;
        case BonusRoundLocation.None:
        default:
          mBonusRoundIndex = int.MaxValue;
          ExistingQuestions = new IRoundControl[mGame.NumRounds];
          break;
      }

      for(int i = 0; i < ExistingQuestions.Length; i++)
      {
        if(i == mBonusRoundIndex)
        {
          ExistingQuestions[i] = new BonusRoundControl(game.BonusRound);
        }
        else
        {
          int normalQuestionIndex = i <= mBonusRoundIndex ? i : i - 1;
          ExistingQuestions[i] = new SingleQuestionControl(mGame.Rounds.ElementAt(normalQuestionIndex));
        }

        (ExistingQuestions[i] as UIElement).CacheMode = new BitmapCache() { EnableClearType = false, RenderAtScale = 1, SnapsToDevicePixels = false };
      }

      oldStyleCountdown = new OldStyleCountdownControl();
      titleScreen = new TitleScreen();

      gParentGrid.Children.Add(oldStyleCountdown);

      oldStyleCountdown.OnCountdownCompleted += (obj, args) =>
      {
        this.Dispatcher.Invoke(() =>
        {
          gParentGrid.Children.Remove(oldStyleCountdown);
          gParentGrid.Children.Add(titleScreen);
          mNextQuestion = ExistingQuestions[0];
          SetNextTransform(mNextQuestion);
          // Note, this needs to be added after the titleScreen in order for the first question to be visible
          // when transitioning to it.
          gParentGrid.Children.Add(mNextQuestion as Control);
        });
      };
      
      mSoundPlayerIntro.Play();

      mMediaPlayerQuestion = new MediaPlayer();
      mMediaPlayerQuestion.Open(new Uri(@"../../Sounds/Next_Question.wav", UriKind.RelativeOrAbsolute));
      mMediaPlayerQuestion.IsMuted = true;
      mMediaPlayerQuestion.MediaEnded += (s, e) =>
      {
        mMediaPlayerQuestion.IsMuted = true;
        mMediaPlayerQuestion.Pause();
        mMediaPlayerQuestion.Position = new TimeSpan(0, 0, 0);
      };

      this.Closed += GameWindow_Closed;
    }

    // Because controls need a parameterless constructor
    public GameWindow() : this(new Game())
    {

    }

    public void BeginQuestions()
    {
      Duration duration = new Duration(new TimeSpan(0, 0, 1));
      DoubleAnimation nextToCurrent = new DoubleAnimation(TRANSFORM_DISTANCE, 0, duration);
      DoubleAnimation currentToPrev = new DoubleAnimation(0, TRANSFORM_DISTANCE * -1, duration);

      currentToPrev.AccelerationRatio = 0.5;
      nextToCurrent.AccelerationRatio = 0.5;
      currentToPrev.DecelerationRatio = 0.5;
      nextToCurrent.DecelerationRatio = 0.5;

      mNextQuestion.NextEnabled = ExistingQuestions.Count() > 1;
      mNextQuestion.PreviousEnabled = false;

      nextToCurrent.Completed += (s, e) =>
      {
        mPreviousQuestion = null;
        mActiveQuestion = mNextQuestion;
        gParentGrid.Children.Remove(titleScreen);

        currentQuestion = 0;

        AttachNextPrevClickEvents();

        if(currentQuestion < ExistingQuestions.Length - 1)
        {
          mNextQuestion = ExistingQuestions[currentQuestion + 1];

          SetNextTransform(mNextQuestion);
          gParentGrid.Children.Add(mNextQuestion as Control);
        }
        else
        {
          mNextQuestion = null;
        }

        SetActiveTransform(mActiveQuestion);
        SetNextTransform(mNextQuestion);

        (mActiveQuestion as SingleQuestionControl)?.ShowQuestion();
      };

      (mNextQuestion as Control).RenderTransform.BeginAnimation(TranslateTransform.XProperty, nextToCurrent);
      //titleScreen.RenderTransform.BeginAnimation(TranslateTransform.XProperty, currentToPrev);


      // In case the opening theme is still playing.
      mSoundPlayerIntro.Stop();

      mMediaPlayerQuestion.Position = new TimeSpan(0, 0, 0);
      mMediaPlayerQuestion.IsMuted = false;
      mMediaPlayerQuestion.Volume = 0.75;
      mMediaPlayerQuestion.Play();

      this.KeyUp += KeyPressed;
    }

    /// <summary>
    /// Reveals the answer associatied with the respetive key. Accepts any key
    /// specified in the attachedKeys list.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void KeyPressed(object sender, KeyEventArgs args)
    {
      if(mAttachedKeys.Contains(args.Key))
      {
        mActiveQuestion.RevealAnswer(args.Key - Key.D0);
      }
      else if(args.Key == Key.X)
      {
        ShowXOnActiveQuestion();
      }
    }

    public void ShowAnswerOnActiveQuestion(int answerIndex)
    {
      mActiveQuestion.RevealAnswer(answerIndex);
    }

    public void ShowXOnActiveQuestion()
    {
      if(mActiveQuestion is SingleQuestionControl)
      {
        ((SingleQuestionControl)mActiveQuestion).ShowX();
      }
    }

    #region RenderTransform for next/prev ---------------------------------------------

    private void SetNextTransform(IRoundControl question)
    {
      if(question != null)
      {
        (question as Control).RenderTransform = new TranslateTransform(TRANSFORM_DISTANCE, 0); 
      }
    }

    private void SetPrevTransform(IRoundControl question)
    {
      if(question != null)
      {
        (question as Control).RenderTransform = new TranslateTransform(TRANSFORM_DISTANCE * -1, 0); 
      }
    }

    private void SetActiveTransform(IRoundControl question)
    {
      if(question != null)
      {
        (question as Control).RenderTransform = new TranslateTransform(0, 0);
      }
    }

    private void TransformToNextQuestion()
    {
      gParentGrid.Children.Remove(mPreviousQuestion as Control);
      mPreviousQuestion = null;

      Duration duration = new Duration(new TimeSpan(0, 0, 1));
      DoubleAnimation nextToCurrent = new DoubleAnimation(TRANSFORM_DISTANCE, 0, duration);
      DoubleAnimation currentToPrev = new DoubleAnimation(0, TRANSFORM_DISTANCE * -1, duration);

      currentToPrev.AccelerationRatio = 0.5;
      nextToCurrent.AccelerationRatio = 0.5;
      currentToPrev.DecelerationRatio = 0.5;
      nextToCurrent.DecelerationRatio = 0.5;

      mNextQuestion.NextEnabled = currentQuestion + 1 < ExistingQuestions.Length - 1;

      nextToCurrent.Completed += (s, e) =>
      {
        mPreviousQuestion = mActiveQuestion;
        mActiveQuestion = mNextQuestion;

        currentQuestion++;

        AttachNextPrevClickEvents();

        if(currentQuestion < ExistingQuestions.Length - 1)
        {
          mNextQuestion = ExistingQuestions[currentQuestion + 1];

          SetNextTransform(mNextQuestion);
          gParentGrid.Children.Add(mNextQuestion as Control);
        }
        else
        {
          mNextQuestion = null;
        }

        SetActiveTransform(mActiveQuestion);
        SetNextTransform(mNextQuestion);

        (mActiveQuestion as SingleQuestionControl)?.ShowQuestion();
      };

      (mNextQuestion as Control).RenderTransform.BeginAnimation(TranslateTransform.XProperty, nextToCurrent);
      (mActiveQuestion as Control).RenderTransform.BeginAnimation(TranslateTransform.XProperty, currentToPrev);

      mMediaPlayerQuestion.Position = new TimeSpan(0, 0, 0);
      mMediaPlayerQuestion.IsMuted = false;
      mMediaPlayerQuestion.Play();
    }

    private void TransformToPreviousQuestion()
    {
      gParentGrid.Children.Remove(mNextQuestion as Control);
      mNextQuestion = null;

      Duration duration = new Duration(new TimeSpan(0, 0, 1));
      DoubleAnimation prevToCurrent = new DoubleAnimation(TRANSFORM_DISTANCE * -1, 0, duration);
      DoubleAnimation currentToNext = new DoubleAnimation(0, TRANSFORM_DISTANCE, duration);

      currentToNext.AccelerationRatio = 0.5;
      prevToCurrent.AccelerationRatio = 0.5;
      currentToNext.DecelerationRatio = 0.5;
      prevToCurrent.DecelerationRatio = 0.5;

      mPreviousQuestion.PreviousEnabled = currentQuestion - 1 > 0;

      prevToCurrent.Completed += (s, e) =>
      {
        mNextQuestion = mActiveQuestion;
        mActiveQuestion = mPreviousQuestion;

        currentQuestion--;

        AttachNextPrevClickEvents();

        if(currentQuestion > 0)
        {
          mPreviousQuestion = ExistingQuestions[currentQuestion - 1];
          SetNextTransform(mPreviousQuestion);
          gParentGrid.Children.Add(mPreviousQuestion as Control);
        }
        else
        {
          mPreviousQuestion = null;
        }

        SetActiveTransform(mActiveQuestion);
        SetPrevTransform(mPreviousQuestion);
        SetNextTransform(mNextQuestion);


        (mActiveQuestion as SingleQuestionControl)?.ShowQuestion();
      };

      (mPreviousQuestion as Control).RenderTransform.BeginAnimation(TranslateTransform.XProperty, prevToCurrent);
      (mActiveQuestion as Control).RenderTransform.BeginAnimation(TranslateTransform.XProperty, currentToNext);
    }

    private void AttachNextPrevClickEvents()
    {
      mActiveQuestion.PreviousClickEvent -= PreviousClick;
      mActiveQuestion.PreviousClickEvent += PreviousClick;

      mActiveQuestion.NextClickEvent -= NextClick;
      mActiveQuestion.NextClickEvent += NextClick;
    }

    private void NextClick(object sender, EventArgs args)
    {
      GoToNext();
    }

    private void PreviousClick(object sender, EventArgs args)
    {
      GoToPrevious();
    }

    public void GoToNext()
    {
      if(currentQuestion < ExistingQuestions.Length - 1)
      {
        TransformToNextQuestion();
      }
    }

    public void GoToPrevious()
    {
      if(currentQuestion > 0)
      {
        TransformToPreviousQuestion();
      }
    }

    #endregion

    #region Display Settings ----------------------------------------------------------

    private void GameWindow_StateChanged(object sender, EventArgs e)
    {
      if(WindowState == WindowState.Maximized)
      {
        // Set back to normal before setting mode and style to fix a bug where
        // if you maximize THEN set style/mode it will calculate "fullscreen" including
        // the singleborder window. Set to normal, remove the border, then return to maximized.
        this.StateChanged -= GameWindow_StateChanged;

        WindowState = WindowState.Normal;

        ResizeMode = ResizeMode.NoResize;
        WindowStyle = WindowStyle.None;

        WindowState = WindowState.Maximized;

        this.StateChanged += GameWindow_StateChanged;
      }
      else
      {
        ResizeMode = ResizeMode.CanResize;
        WindowStyle = WindowStyle.SingleBorderWindow;
      }
    }

    private void GameWindow_KeyDown(object sender, KeyEventArgs e)
    {
      if((e.Key == Key.Escape || e.Key == Key.F11) && WindowState == WindowState.Maximized)
      {
        SizeChanged -= GameWindow_SizeChanged;

        WindowState = WindowState.Normal;
        ResizeMode = ResizeMode.CanResize;        

        Height = originalNonMaximizedSize.Height;
        Width = originalNonMaximizedSize.Width;

        SizeChanged += GameWindow_SizeChanged;
      }
      else if(e.Key == Key.F11 && WindowState == WindowState.Normal)
      {
        SizeChanged -= GameWindow_SizeChanged;

        ResizeMode = ResizeMode.NoResize;
        WindowState = WindowState.Maximized;
        WindowStyle = WindowStyle.None;

        SizeChanged += GameWindow_SizeChanged;
      }
    }

    private void GameWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      originalNonMaximizedSize = e.PreviousSize;
    }

    private void GameWindow_ContentRendered(object sender, EventArgs e)
    {
      originalNonMaximizedSize = this.RenderSize;
    }

    private void GameWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      //e.Cancel = true;
      //Hide();
      
    }

    private void GameWindow_Closed(object sender, EventArgs args)
    {
      mSoundPlayerIntro.Stop();
    }

    #endregion

    #region Properties ------------------------------------------------------------

    private IRoundControl[] ExistingQuestions { get; set; }
    private OldStyleCountdownControl oldStyleCountdown { get; set; }
    private TitleScreen titleScreen { get; set; }

    #endregion
  }
}
