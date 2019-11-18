using FamilyFeud.Controls;
using FamilyFeud.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
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
    private const double TRANSFORM_DISTANCE = 1920.0;

    private Size originalNonMaximizedSize;
    private List<Key> mAttachedKeys;
    private int currentQuestion;
    private MediaPlayer mMediaPlayer;

    private Game mGame;

    private SingleQuestionControl mPreviousQuestion;
    private SingleQuestionControl mActiveQuestion;
    private SingleQuestionControl mNextQuestion;

    public GameWindow(Game game)
    {
      InitializeComponent();

      if(game == null || game.NumRounds < 2)
      {
        throw new ArgumentException("Game cannot be null and must have at least two rounds");
      }

      mGame = game;

      ExistingQuestions = new SingleQuestionControl[mGame.NumRounds];

      mAttachedKeys = new List<Key>() { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8 };

      mActiveQuestion = new SingleQuestionControl(game.Rounds.ElementAt(0), true);
      mActiveQuestion.CacheMode = new BitmapCache() { EnableClearType = false, RenderAtScale = 1, SnapsToDevicePixels = false };
      mActiveQuestion.PreviousEnabled = false;
      ExistingQuestions[0] = mActiveQuestion;

      mNextQuestion = new SingleQuestionControl(game.Rounds.ElementAt(1));
      mNextQuestion.CacheMode = new BitmapCache() { EnableClearType = false, RenderAtScale = 1, SnapsToDevicePixels = false };
      ExistingQuestions[1] = mNextQuestion;

      currentQuestion = 0;

      SetActiveTransform(mActiveQuestion);
      SetNextTransform(mNextQuestion);

      AttachNextPrevClickEvents();

      this.KeyUp += KeyPressed;

      gParentGrid.Children.Add(mActiveQuestion);
      gParentGrid.Children.Add(mNextQuestion);

      mMediaPlayer = new MediaPlayer();
      mMediaPlayer.Open(new Uri(@"../../Sounds/Next_Question.wav", UriKind.RelativeOrAbsolute));
      mMediaPlayer.IsMuted = true;
      mMediaPlayer.MediaEnded += (s, e) =>
      {
        mMediaPlayer.IsMuted = true;
        mMediaPlayer.Pause();
        mMediaPlayer.Position = new TimeSpan(0, 0, 0);
      };
    }

    // Because controls need a parameterless constructor
    public GameWindow() : this(new Game())
    {

    }

    /// <summary>
    /// Reveals the answer associatied with the respetive key. Accepts any key
    /// specified in the attachedKeys list.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void KeyPressed(object sender, KeyEventArgs args)
    {
      if(!mAttachedKeys.Contains(args.Key))
      {
        return;
      }

      mActiveQuestion.RevealAnswer(args.Key - Key.D0);
    }

    #region RenderTransform for next/prev ---------------------------------------------

    private void SetNextTransform(SingleQuestionControl question)
    {
      if(question != null)
      {
        question.RenderTransform = new TranslateTransform(TRANSFORM_DISTANCE, 0); 
      }
    }

    private void SetPrevTransform(SingleQuestionControl question)
    {
      if(question != null)
      {
        question.RenderTransform = new TranslateTransform(TRANSFORM_DISTANCE * -1, 0); 
      }
    }

    private void SetActiveTransform(SingleQuestionControl question)
    {
      if(question != null)
      {
        question.RenderTransform = new TranslateTransform(0, 0);
      }
    }

    private void TransformToNextQuestion()
    {
      gParentGrid.Children.Remove(mPreviousQuestion);
      mPreviousQuestion = null;

      Duration duration = new Duration(new TimeSpan(0, 0, 1));
      DoubleAnimation nextToCurrent = new DoubleAnimation(TRANSFORM_DISTANCE, 0, duration);
      DoubleAnimation currentToPrev = new DoubleAnimation(0, TRANSFORM_DISTANCE * -1, duration);

      currentToPrev.AccelerationRatio = 0.5;
      nextToCurrent.AccelerationRatio = 0.5;
      currentToPrev.DecelerationRatio = 0.5;
      nextToCurrent.DecelerationRatio = 0.5;

      mNextQuestion.NextEnabled = currentQuestion + 1 < mGame.NumRounds - 1;

      nextToCurrent.Completed += (s, e) =>
      {
        mPreviousQuestion = mActiveQuestion;
        mActiveQuestion = mNextQuestion;

        currentQuestion++;

        AttachNextPrevClickEvents();

        if(currentQuestion < mGame.NumRounds - 1)
        {
          if(ExistingQuestions[currentQuestion + 1] == null)
          {
            mNextQuestion = new SingleQuestionControl(mGame.Rounds.ElementAt(currentQuestion + 1));
            mNextQuestion.CacheMode = new BitmapCache() { EnableClearType = false, RenderAtScale = 1, SnapsToDevicePixels = false };
            ExistingQuestions[currentQuestion + 1] = mNextQuestion;
          }
          else
          {
            mNextQuestion = ExistingQuestions[currentQuestion + 1];
          }

          SetNextTransform(mNextQuestion);
          gParentGrid.Children.Add(mNextQuestion);
        }
        else
        {
          mNextQuestion = null;
        }

        SetActiveTransform(mActiveQuestion);
        SetPrevTransform(mPreviousQuestion);
        SetNextTransform(mNextQuestion);

        mActiveQuestion.ShowQuestion();
      };

      mNextQuestion.RenderTransform.BeginAnimation(TranslateTransform.XProperty, nextToCurrent);
      mActiveQuestion.RenderTransform.BeginAnimation(TranslateTransform.XProperty, currentToPrev);

      mMediaPlayer.Position = new TimeSpan(0, 0, 0);
      mMediaPlayer.IsMuted = false;
      mMediaPlayer.Play();
    }

    private void TransformToPreviousQuestion()
    {
      gParentGrid.Children.Remove(mNextQuestion);
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
          if(ExistingQuestions[currentQuestion - 1] == null)
          {
            mPreviousQuestion = new SingleQuestionControl(mGame.Rounds.ElementAt(currentQuestion - 1));
            mPreviousQuestion.CacheMode = new BitmapCache() { EnableClearType = false, RenderAtScale = 1, SnapsToDevicePixels = false };
            ExistingQuestions[currentQuestion - 1] = mPreviousQuestion;
          }
          else
          {
            mPreviousQuestion = ExistingQuestions[currentQuestion - 1];
          }

          SetNextTransform(mPreviousQuestion);
          gParentGrid.Children.Add(mPreviousQuestion);
        }
        else
        {
          mPreviousQuestion = null;
        }

        SetActiveTransform(mActiveQuestion);
        SetPrevTransform(mPreviousQuestion);
        SetNextTransform(mNextQuestion);

        mActiveQuestion.ShowQuestion();
      };

      mPreviousQuestion.RenderTransform.BeginAnimation(TranslateTransform.XProperty, prevToCurrent);
      mActiveQuestion.RenderTransform.BeginAnimation(TranslateTransform.XProperty, currentToNext);
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
      if(currentQuestion < mGame.NumRounds - 1)
      {
        TransformToNextQuestion();
      }
    }

    private void PreviousClick(object sender, EventArgs args)
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
      e.Cancel = true;
      Hide();
    }

    #endregion

    #region Properties ------------------------------------------------------------

    private SingleQuestionControl[] ExistingQuestions { get; set; }

    #endregion
  }
}
