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
  /// Interaction logic for OldStyleCountdownControl.xaml
  /// </summary>
  public partial class OldStyleCountdownControl : UserControl
  {
    public event EventHandler OnCountdownCompleted;

    // This assumes a constant, immutable size for the control.
    // This will have to be made into this.ActualHeight/2, etc.
    private const int MidpointX = 960;
    private const int MidpointY = 540;
    private const int MovingRadius = 1500;
    private int counter;
    private Timer timer;
    private MediaPlayer mMediaPlayer;

    private const int startingCountdownSecs = 5;
    private const int timerIntervalMS = 20;
    private const int intervalsPerSec = 1000 / timerIntervalMS;
    private const double anglePerInterval = 2 * Math.PI / intervalsPerSec;
    private const double angleOffset = Math.PI * -1.0 / 2.0;

    private int countdownValue;

    private const string PathMarkup = "M 960,540 L ";

    public OldStyleCountdownControl()
    {
      InitializeComponent();

      mMediaPlayer = new MediaPlayer();
      mMediaPlayer.Open(new Uri(@"../../Sounds/Outtake_Blip.wav", UriKind.RelativeOrAbsolute));
      mMediaPlayer.Volume = 0.25;
      mMediaPlayer.IsMuted = true;
      mMediaPlayer.Play();

      countdownValue = startingCountdownSecs;

      tbCounter.Text = countdownValue.ToString();

      counter = 0;

      timer = new Timer(timerIntervalMS);
      timer.Elapsed += OnInterval;
      timer.Start();
    }

    private void OnInterval(object obj, ElapsedEventArgs args)
    {
      counter++;

      if(counter == intervalsPerSec)
      {
        counter = 0;
        tbCounter.Dispatcher.Invoke(() => { tbCounter.Text = (--countdownValue).ToString(); });

        PlayBlip();

        if(countdownValue == 0)
        {
          timer.Stop();
          timer.Dispose();
          MovingPath.Dispatcher.Invoke(() => { MovingPath.Data = Geometry.Parse(PathMarkup + MidpointX + "," + (MidpointY - MovingRadius).ToString()); });
          OnCountdownCompleted?.Invoke(this, new EventArgs());
          return;
        }
      }

      int posX = (int)(Math.Cos((counter * anglePerInterval) + angleOffset) * MovingRadius) + MidpointX;
      int posY = (int)(Math.Sin((counter * anglePerInterval) + angleOffset) * MovingRadius) + MidpointY;

      MovingPath.Dispatcher.Invoke(() => { MovingPath.Data = Geometry.Parse(PathMarkup + posX.ToString() + "," + posY.ToString()); });
    }

    private void PlayBlip()
    {
      this.Dispatcher.Invoke(() => 
      {
        mMediaPlayer.IsMuted = false;
        mMediaPlayer.Position = new TimeSpan(0);
        mMediaPlayer.Play();
      });
    }
  }
}
