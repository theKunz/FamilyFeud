using FamilyFeud.CustomEventArgs;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using ExtensionMethods;
using CommonLib.Constants;
using System.Collections.Generic;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for AddSubtractControl.xaml
  /// </summary>
  public partial class AddSubtractControl : UserControl
  {
    private const string AllowedChars = "^[0-9]*$";
    private List<Int32> AllowedKeys;
    private string oldVal;

    public event EventHandler<ArithmeticEventArgs> OnAddOrSubtract;

    public AddSubtractControl()
    {
      InitializeComponent();

      AllowedKeys = new List<Int32>();
      oldVal = CommonConst.EmptyString;
      tbValue.Text = CommonConst.EmptyString;
    }

    private void btnSubtract_Click(object sender, RoutedEventArgs args)
    {
      if(tbValue.Text.Equals(CommonConst.EmptyString))
      {
        return;
      }

      OnAddOrSubtract?.Invoke(this, new ArithmeticEventArgs(int.Parse(tbValue.Text), ArithmeticEventArgs.Operand.Subtract));
      tbValue.Text = CommonConst.EmptyString;
    }

    private void btnAdd_Click(object sender, RoutedEventArgs args)
    {
      if(tbValue.Text.Equals(CommonConst.EmptyString))
      {
        return;
      }

      OnAddOrSubtract?.Invoke(this, new ArithmeticEventArgs(int.Parse(tbValue.Text), ArithmeticEventArgs.Operand.Add));
      tbValue.Text = CommonConst.EmptyString;
    }

    private void tbValue_Pasting(object sender, DataObjectPastingEventArgs args)
    {
      string text;

      if(args.DataObject.GetDataPresent(typeof(string)))
      {
        text = args.DataObject.GetData(typeof(string)) as string;
        if(!IsTextAllowed(text))
        {
          args.CancelCommand();
        }
      }
      else
      {
        args.CancelCommand();
      }
    }

    private void TextChanged(object sender, TextChangedEventArgs args)
    {
      if(IsTextAllowed(tbValue.Text))
      {
        oldVal = tbValue.Text;
      }
      else
      {
        args.Handled = true;

        tbValue.TextChanged -= TextChanged;
        tbValue.Text = oldVal;
        tbValue.TextChanged += TextChanged;
      }
    }

    private bool IsTextAllowed(string text)
    {
      return Regex.IsMatch(text, AllowedChars);
    }

  }
}
