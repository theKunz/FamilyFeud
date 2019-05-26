using FamilyFeud.CustomEventArgs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for NumericUpDown.xaml
  /// </summary>
  public partial class NumericUpDown : UserControl
  {
    public event EventHandler<ArithmeticEventArgs> ValueChanged;

    public enum LoopingType
    {
      LoopToZero,
      LoopToMinMax,
      NoLoop
    }

    public NumericUpDown()
    {
      InitializeComponent();
      Min = int.MinValue;
      Max = int.MaxValue;
      LoopType = LoopingType.LoopToMinMax;
      Value = 0;
    }

    private void btnUp_Click(object sender, RoutedEventArgs e)
    {
      Value++;
    }

    private void btnDown_Click(object sender, RoutedEventArgs e)
    {
      Value--;
    }

    public static DependencyProperty ValueProperty = DependencyProperty.Register("Value",
                                                                                 typeof(int),
                                                                                 typeof(NumericUpDown));

    public int Value
    {
      get
      {
        return (GetValue(ValueProperty) as int?).Value;
      }
      set
      {
        int oldVal;
        int newVal;
        int delta;

        newVal = value;

        if(LoopType == LoopingType.NoLoop)
        {
          if(newVal > Max || newVal < Min)
          {
            return;
          }
        }
        else
        {
          if(newVal > Max || newVal < Min)
          {
            newVal = LoopType == LoopingType.LoopToZero ? 0 :
                                    newVal > Max ? newVal = Min :
                                    newVal = Max;
          }
        }

        oldVal = (GetValue(ValueProperty) as int?).Value;

        delta = newVal - oldVal;

        SetValue(ValueProperty, newVal);

        //OnPropertyChanged(new DependencyPropertyChangedEventArgs(ValueProperty, oldVal, newVal));
        ValueChanged?.Invoke(this, new ArithmeticEventArgs(delta < 0 ? delta * -1 : delta, 
                                                           delta < 0 ? ArithmeticEventArgs.Operand.Subtract : ArithmeticEventArgs.Operand.Add));
      }
    }

    public int Min
    {
      get;set;
    }

    public int Max
    {
      get;set;
    }

    public LoopingType LoopType
    {
      get;set;
    }

  }
}
