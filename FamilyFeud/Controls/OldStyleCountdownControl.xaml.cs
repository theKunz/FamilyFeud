﻿using System;
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
using System.Windows.Threading;

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
    Random rand;

    private const int startingCountdownSecs = 5;
    private const int timerIntervalMS = 20;
    private const int intervalsPerSec = 1000 / timerIntervalMS;
    private const double anglePerInterval = 2 * Math.PI / intervalsPerSec;
    private const double angleOffset = Math.PI * -1.0 / 2.0;

    private Action StainLineAction;
    private Action CounterUpdateAction;
    private Action LineAction;

    private int countdownValue;

    private const string PathMarkup = "M 960,540 L ";

    public OldStyleCountdownControl()
    {
      InitializeComponent();

      rand = new Random((int)DateTime.UtcNow.Ticks);

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

      StainLineAction = counter % 2 == 0 ? SetStainLines() : null;

      if(counter == intervalsPerSec)
      {
        counter = 0;
        countdownValue--;

        if(countdownValue == 0)
        {
          timer.Stop();
          timer.Dispose();
          this.Dispatcher.Invoke(() => { MovingPath.Data = Geometry.Parse(PathMarkup + MidpointX + "," + (MidpointY - MovingRadius).ToString()); });
          OnCountdownCompleted?.Invoke(this, new EventArgs());
          return;
        }

        CounterUpdateAction = () => { tbCounter.Text = countdownValue.ToString(); };
      }
      else
      {
        CounterUpdateAction = null;
      }

      int posX = (int)(Math.Cos((counter * anglePerInterval) + angleOffset) * MovingRadius) + MidpointX;
      int posY = (int)(Math.Sin((counter * anglePerInterval) + angleOffset) * MovingRadius) + MidpointY;

      LineAction = () => { MovingPath.Data = Geometry.Parse(PathMarkup + posX.ToString() + "," + posY.ToString()); };

      this.Dispatcher.Invoke(() =>
      {
        StainLineAction?.Invoke();
        CounterUpdateAction?.Invoke();
        LineAction?.Invoke();
      });
    }

    private Action SetStainLines()
    {
      int[] horizontals = Enumerable.Repeat(0, 5).Select(i => rand.Next(1, MidpointX * 2)).ToArray();
      return () =>
      {
        try
        {
          StainLine0.Data = Geometry.Parse($"M {horizontals[0]},-1 L {horizontals[0]},{MidpointY * 2 + 1}");
          StainLine1.Data = Geometry.Parse($"M {horizontals[1]},-1 L {horizontals[1]},{MidpointY * 2 + 1}");
          StainLine2.Data = Geometry.Parse($"M {horizontals[2]},-1 L {horizontals[2]},{MidpointY * 2 + 1}");
          StainLine3.Data = Geometry.Parse($"M {horizontals[3]},-1 L {horizontals[3]},{MidpointY * 2 + 1}");
          StainLine4.Data = Geometry.Parse($"M {horizontals[4]},-1 L {horizontals[4]},{MidpointY * 2 + 1}");
        }
        catch
        {
          // do nothing, forcibly stopping execution in debug mode sometimes causes an error here.
        }
      };
    }
  }
}
