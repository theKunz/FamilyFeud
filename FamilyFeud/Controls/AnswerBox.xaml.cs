using FamilyFeud.DataObjects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for AnswerBox.xaml
  /// </summary>
  public partial class AnswerBox : UserControl
  {
    private Storyboard showAnswerStory;
    private int? mAnswerIndex;
    private Answer mAnswerSource;
    MediaPlayer mMediaplayer;

    public AnswerBox()
    {
      InitializeComponent();

      Loaded += SetSpinAnimationOnLoad;

      DataContext = this;
      MouseLeftButtonUp += ShowAnswerMouse;
      mMediaplayer = new MediaPlayer();
      mMediaplayer.Open(new Uri(@"../../Sounds/Bing-sound.mp3", UriKind.RelativeOrAbsolute));
      mMediaplayer.IsMuted = true;
      mMediaplayer.Play();
    }

    private void ShowAnswerMouse(object sender, MouseButtonEventArgs args)
    {
      ShowAnswer();
    }

    private void FreezeSpinAnimationOnComplete(object sender, EventArgs args)
    {
      showAnswerStory.Completed -= FreezeSpinAnimationOnComplete;
      showAnswerStory.Freeze();
    }

    private void SetSpinAnimationOnLoad(object sender, RoutedEventArgs args)
    {
      showAnswerStory = AnimationContainer.Resources["uiSpin"] as Storyboard;
      showAnswerStory.Completed += FreezeSpinAnimationOnComplete;
      Loaded -= SetSpinAnimationOnLoad;
    }

    /// <summary>
    /// Begins the animation to reveal the answer. Will pre-emptively return if
    /// this AnswerBox's index in null.
    /// </summary>
    public void ShowAnswer()
    {
      if(AnswerIndex == null)
      {
        return;
      }

      ShowAnswerAnimation();
      DetachAnimationEvents();
    }
    
    private void ShowAnswerAnimation()
    {
      if(!(showAnswerStory?.IsFrozen).Value)
      {
        showAnswerStory?.Begin();
        mMediaplayer.IsMuted = false;
        mMediaplayer.Position = new TimeSpan(0);
        mMediaplayer.Play();
      }
    }

    private void DetachAnimationEvents()
    {
      MouseLeftButtonUp -= ShowAnswerMouse;
    }

    #region Properties -----------------------------------------------------------

    private static DependencyProperty AnswerSourceProperty = DependencyProperty.Register("AnswerSource",
                                                                             typeof(Answer),
                                                                             typeof(AnswerBox));
    /// <summary>
    /// Answer object that will be displayed by this control.
    /// </summary>
    public Answer AnswerSource
    {
      get
      {
        return GetValue(AnswerSourceProperty) as Answer;
      }
      set
      {
        mAnswerSource = value;
        SetValue(AnswerSourceProperty, mAnswerSource);
      }
    }

    public static DependencyProperty AnswerIndexProperty = DependencyProperty.Register("AnswerIndex",
                                                                                       typeof(int?),
                                                                                       typeof(AnswerBox));

    /// <summary>
    /// This number will be displayed on the front while the answer is hidden. Set to 0 or less or null
    /// to hide and disable reveal animation.
    /// </summary>
    public int? AnswerIndex
    {
      get
      {
        return GetValue(AnswerIndexProperty) as int?;
      }
      set
      {
        mAnswerIndex = (value.HasValue && value.Value <= 0) ? null : value;
        SetValue(AnswerIndexProperty, mAnswerIndex);
      }
    }

    #endregion
  }
}
