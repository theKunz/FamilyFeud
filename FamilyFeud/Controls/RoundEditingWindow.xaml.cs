using FamilyFeud.DataObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonLib.CustomEventArgs;

namespace FamilyFeud.Controls
{
    /// <summary>
    /// Interaction logic for RoundEditingWindow.xaml
    /// </summary>
  public partial class RoundEditingWindow : Window, INotifyPropertyChanged
  {
    private Round mEditingRound;
    private Round mOriginalRound;
    private bool mIgnoreDirty;

    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<EventArgs<Round>> EditingComplete;

    public RoundEditingWindow()
    {
      InitializeComponent();
      Initialize(null);
    }

    public RoundEditingWindow(Round round)
    {
      InitializeComponent();
      Initialize(round);
    }

    protected override void OnClosing(CancelEventArgs args)
    {
      if(IsDirty && !mIgnoreDirty)
      {
        // This should hopefully get around any problems asynchronous code from the popup could cause.
        args.Cancel = true;

        if(MessageBox.Show("Unsaved changed", "You have unsaved changes. Are you sure you want to exit?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          mIgnoreDirty = true;
          Close();
        }
      }
    }

    private void Initialize(Round round)
    {
      if(round == null)
      {
        round = new Round();
      }

      mIgnoreDirty = false;

      mOriginalRound = round.Copy();
      mEditingRound = round.Copy();

      mEditingRound.PropertyChanged += EditingRoundPropertyChanged;
      mEditingRound.Question.PropertyChanged += EditingRoundPropertyChanged;
      foreach(Answer answer in mEditingRound.Answers)
      {
        answer.PropertyChanged += EditingRoundPropertyChanged;
      }

      DataContext = this;
    }

    private void EditingRoundPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDirty)));
    }

    public bool IsDirty
    {
      get
      {
        return !mEditingRound.Equals(mOriginalRound);
      }
    }

    public Round OriginalRound
    {
      get
      {
        return mOriginalRound;
      }
      private set
      {
        mOriginalRound = value ?? new Round();
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OriginalRound)));
      }
    }

    public Round EditingRound
    {
      get
      {
        return mEditingRound;
      }
      private set
      {
        mEditingRound = value ?? new Round();
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EditingRound)));
      }
    }
  }
}
