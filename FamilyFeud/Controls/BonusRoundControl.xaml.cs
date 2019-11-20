using FamilyFeud.DataObjects;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for BonusRoundControl.xaml
  /// </summary>
  public partial class BonusRoundControl : UserControl, IDisposable
  {
    public event EventHandler OnTimerFinished;

    private Timer countDownTimer;
    private const int TimerSeconds = 60;
    private int currTick;


    public BonusRoundControl() :
      this(null)
    {
    }

    public BonusRoundControl(BonusRound bRound)
    {
      InitializeComponent();

      currTick = 0;
      countDownTimer = InitNewTimer();
      Loaded += (s, e) =>
      {
        StartTimer();
      };
    }

    public void StartTimer()
    {
      EventHandler disp = null;

      if(currTick == TimerSeconds || countDownTimer == null)
      {
        currTick = 0;
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

    private Timer InitNewTimer()
    {
      Timer newTimer = new Timer(1000);
      newTimer.AutoReset = true;
      newTimer.Elapsed += TimerTick;
      return newTimer;
    }

    private void TimerTick(object sender, ElapsedEventArgs args)
    {
      Dispatcher.Invoke(new Action(() => { tbTimer.Text = (TimerSeconds - ++currTick).ToString("0:##"); }));
      if(currTick == TimerSeconds)
      {
        OnTimerFinished?.Invoke(this, new EventArgs());
        countDownTimer.Stop();
      }
    }

    public void Dispose()
    {
      countDownTimer.Stop();
      countDownTimer.Dispose();
    }
  }
}
