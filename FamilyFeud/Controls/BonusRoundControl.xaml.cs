using FamilyFeud.DataObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for BonusRoundControl.xaml
  /// </summary>
  public partial class BonusRoundControl : UserControl, IDisposable, INotifyPropertyChanged, IRoundControl
  {
    public event EventHandler NextClickEvent;
    public event EventHandler PreviousClickEvent;
    public event EventHandler OnTimerFinished;
    public event PropertyChangedEventHandler PropertyChanged;

    private MediaPlayer mDingMediaPlayer;
    private MediaPlayer mXMediaPlayer;
    private Timer countDownTimer;
    private const int TimerSeconds = 120;
    private int currTick;
    private BonusRound mBonusData;
    private Storyboard showXStory;

    private bool mNextEnabled;
    private bool mPrevEnabled;

    private bool[] ShownAnswers;

    public BonusRoundControl() :
      this(null)
    {
    }

    public BonusRoundControl(BonusRound bRound)
    {
      mNextEnabled = true;
      mPrevEnabled = true;

      InitializeComponent();

      mBonusData = bRound != null ? bRound : new BonusRound();

      ShownAnswers = new bool[bRound.BonusQuestions.Count];

      DataContext = this;

      mDingMediaPlayer = new MediaPlayer();
      mDingMediaPlayer.IsMuted = true;
      mDingMediaPlayer.Open(new Uri(@"../../Sounds/Bing-sound.mp3", UriKind.RelativeOrAbsolute));
      mDingMediaPlayer.MediaEnded += (obj, e) =>
      {
        mDingMediaPlayer.Position = new TimeSpan(0, 0, 0);
        mDingMediaPlayer.Pause();
      };

      mXMediaPlayer = new MediaPlayer();
      mXMediaPlayer.IsMuted = true;
      mXMediaPlayer.Open(new Uri(@"../../Sounds/Wrong_Buzzer.wav", UriKind.RelativeOrAbsolute));
      mXMediaPlayer.MediaEnded += (obj, e) =>
      {
        mXMediaPlayer.Position = new TimeSpan(0, 0, 0);
        mXMediaPlayer.Pause();
      };

      currTick = TimerSeconds;
      countDownTimer = InitNewTimer();

      RoutedEventHandler loadedEvent = null;
      loadedEvent = (s, e) =>
      {
        Loaded -= loadedEvent;
        int numQuestions = BonusData.BonusQuestions.Count;
        for(int i = 1; i <= numQuestions; i++)
        {
          (FindName("tbQ" + i) as TextBlock).Text = BonusData.BonusQuestions[i - 1].Question.QuestionText;
        }

        showXStory = Resources["sbShowX"] as Storyboard;
      };
      Loaded += loadedEvent;
    }

    public void RevealAnswer(int answerIndex)
    {
      int numQuestions = BonusData.BonusQuestions.Count;
      if(answerIndex < 0 || answerIndex > numQuestions || answerIndex >= 10)
      {
        return;
      }

      // 0 is treated as 10, to avoid a potential out of bound exception
      // we return if there is no 10th question
      if(answerIndex == 0 && numQuestions < 10)
      {
        return;
      }

      int controlIndex = answerIndex == 0 ? 10 : answerIndex;
      int dataIndex = answerIndex == 0 ? 9 : answerIndex - 1;

      if(ShownAnswers[dataIndex])
      {
        return;
      }

      ShownAnswers[dataIndex] = true;

      TextBlock answerTb = (FindName("tbQ" + controlIndex) as TextBlock);
      TextBlock valueTb = (FindName("tbA" + controlIndex) as TextBlock);

      answerTb.Text = BonusData.BonusQuestions[dataIndex].Answer.AnswerText;
      answerTb.Foreground = new SolidColorBrush(Color.FromRgb(255, 214, 7));

      valueTb.Text = BonusData.BonusQuestions[dataIndex].Answer.PointValue.ToString();
      valueTb.Foreground = new SolidColorBrush(Color.FromRgb(255, 214, 7));

      mDingMediaPlayer.Position = new TimeSpan(0, 0, 0); // In case the user is mashing reveal answer and one ding hasn't finished
      mDingMediaPlayer.IsMuted = false;
      mDingMediaPlayer.Volume = 1;
      mDingMediaPlayer.Play();
    }

    public void StartTimer()
    {
      EventHandler disp = null;

      if(currTick == 0 || countDownTimer == null)
      {
       currTick = TimerSeconds;
        disp = (s, e) =>
        {
          countDownTimer.Disposed -= disp;

          countDownTimer = InitNewTimer();
          StartTimer();
        };
        countDownTimer.Disposed += disp;
        countDownTimer.Dispose();
      }
      else
      {
        countDownTimer.Start();
      }
    }

    public void StopTimer()
    {
      if(countDownTimer == null)
      {
        return;
      }

      countDownTimer.Stop();
    }

    public void ShowX()
    {
      showXStory.Begin();
      mXMediaPlayer.IsMuted = false;
      mXMediaPlayer.Volume = 1;
      mXMediaPlayer.Play();
      showXStory.Begin();
    }

    private Timer InitNewTimer()
    {
      Timer newTimer = new Timer(1000);
      newTimer.AutoReset = true;
      newTimer.Elapsed += TimerTick;
      return newTimer;
    }

    private void TimerTick(object sender, ElapsedEventArgs args)
    {
      currTick--;
      Dispatcher.Invoke(() => { tbTimer.Text = string.Format("{0}:{1}", (currTick / 60).ToString(), (currTick % 60).ToString("D2")); });
      if(currTick == 0)
      {
        OnTimerFinished?.Invoke(this, new EventArgs());
        countDownTimer.Stop();
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

    public void Dispose()
    {
      countDownTimer.Stop();
      countDownTimer.Dispose();
    }

    public BonusRound BonusData
    {
      get
      {
        return mBonusData;
      }
      set
      {
        if(value != mBonusData)
        {
          mBonusData = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BonusData)));
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
        if(value != mNextEnabled)
        {
          mNextEnabled = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextEnabled)));
        }
      } 
    }

    public bool PreviousEnabled
    {
      get
      {
        return mPrevEnabled;
      }
      set
      {
        if(value != mPrevEnabled)
        {
          mPrevEnabled = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PreviousEnabled)));
        }
      }
    }
  }
}
