using FamilyFeud.DataObjects;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using FamilyFeud.CustomEventArgs;
using System;
using CommonLib.CustomEventArgs;
using System.Linq;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for GameBuilder.xaml, used to let the user construct a game
  /// </summary>
  public partial class GameBuilder : Window, INotifyPropertyChanged
  {
    private const int MinQuestions = 3;

    private const int MaxBonusQuestions = 10;

    private ObservableCollection<Round> mAvailableRounds;
    private ObservableCollection<Round> mChosenRounds;
    private ObservableCollection<BonusQuestion> mAvailableBonusQuestions;
    private ObservableCollection<BonusQuestion> mChosenBonusQuestions;
    private List<Round> mNewRounds;
    private List<BonusQuestion> mNewBonusQuestions;

    private QuestionBuilder mQuickQuestion;

    public event PropertyChangedEventHandler PropertyChanged;
    public event GameBuildingCompletedEventHandler GameBuildingCompleted;

    public GameBuilder(IEnumerable<Round> availableRounds = null,
                       IEnumerable<BonusQuestion> availableBonusQuestions = null)
    {
      InitializeComponent();

      mChosenRounds = new ObservableCollection<Round>();
      mChosenBonusQuestions = new ObservableCollection<BonusQuestion>();

      mAvailableRounds = availableRounds == null ? new ObservableCollection<Round>() : 
                                                   new ObservableCollection<Round>(availableRounds);
      mAvailableBonusQuestions = availableBonusQuestions == null ? new ObservableCollection<BonusQuestion>() : 
                                                                   new ObservableCollection<BonusQuestion>(availableBonusQuestions);

      mNewRounds = new List<Round>();
      mNewBonusQuestions = new List<BonusQuestion>();

      DataContext = this;
    }
    
    /// <summary>
    /// List of available rounds that the user has not selected for use in a game
    /// </summary>
    public ObservableCollection<Round> AvailableRounds
    {
      get
      {
        return mAvailableRounds;
      }
      private set
      {
        if(value != mAvailableRounds)
        {
          mAvailableRounds = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AvailableRounds)));
        }
      }
    }

    /// <summary>
    /// List of rounds the user has chosen for use in a game
    /// </summary>
    public ObservableCollection<Round> ChosenRounds
    {
      get
      {
        return mChosenRounds;
      }
      private set
      {
        if(value != mChosenRounds)
        {
          mChosenRounds = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChosenRounds)));
        }
      }
    }

    /// <summary>
    /// List of available Bonus Questions not chosen by the user
    /// </summary>
    public ObservableCollection<BonusQuestion> AvailableBonusQuestions
    {
      get
      {
        return mAvailableBonusQuestions;
      }
      private set
      {
        if(value != mAvailableBonusQuestions)
        {
          mAvailableBonusQuestions = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AvailableBonusQuestions)));
        }
      }
    }

    /// <summary>
    /// List of Bonus Questions chosen by the user for use in a game
    /// </summary>
    public ObservableCollection<BonusQuestion> ChosenBonusQuestions
    {
      get
      {
        return mChosenBonusQuestions;
      }
      private set
      {
        if(value != mChosenBonusQuestions)
        {
          mChosenBonusQuestions = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChosenBonusQuestions)));
        }
      }
    }

    private void ChooseRound_Click(object sender, RoutedEventArgs e)
    {
      MoveItems<Round>(mAvailableRounds, mChosenRounds, lvSelectableRounds.SelectedItems);
      btnDone.IsEnabled = mChosenRounds.Count >= MinQuestions;
      lvSelectedRounds.Items.MoveCurrentToLast();
    }

    private void UnChooseRound_Click(object sender, RoutedEventArgs e)
    {
      MoveItems<Round>(mChosenRounds, mAvailableRounds, lvSelectedRounds.SelectedItems);
      btnDone.IsEnabled = mChosenRounds.Count >= MinQuestions;
      lvSelectableRounds.Items.MoveCurrentToLast();
    }

    private void ChooseBonusQuestion_Click(object sender, RoutedEventArgs e)
    {
      MoveItems<BonusQuestion>(mAvailableBonusQuestions, mChosenBonusQuestions, lvSelectableBonusQuestions.SelectedItems, MaxBonusQuestions);
      btnChooseBonus.IsEnabled = lvSelectedBonusQuestions.Items.Count < MaxBonusQuestions;
    }

    private void UnChooseBonusQuestion_Click(object sender, RoutedEventArgs e)
    {
      MoveItems<BonusQuestion>(mChosenBonusQuestions, mAvailableBonusQuestions, lvSelectedBonusQuestions.SelectedItems);
      btnChooseBonus.IsEnabled = lvSelectedBonusQuestions.Items.Count < MaxBonusQuestions;
    }

    private void MoveItems<T>(ObservableCollection<T> colToRemoveFrom, ObservableCollection<T> colToAddTo, IList selectedItems, int addToMax = -1)
      where T : class
    {
      List<T> tempList;

      if (selectedItems == null || 
          selectedItems.Count == 0 || 
          !(selectedItems[0] is T))
      {
        return;
      }

      if(colToRemoveFrom == colToAddTo)
      {
        return;
      }

      tempList = new List<T>();

      foreach(object obj in selectedItems)
      {
        tempList.Add(obj as T);
      }

      foreach(T currItem in tempList)
      {
        if(addToMax < 0 || colToAddTo.Count < addToMax)
        {
          colToRemoveFrom.Remove(currItem);
          colToAddTo.Add(currItem);
        }
      }
    }

    private void btnDone_Click(object sender, RoutedEventArgs e)
    {
      bool hasBonusRound = !rbNone.IsChecked.Value && ChosenBonusQuestions.Count > 0;

      GameBuildingCompleted?.Invoke(this, new GameBuildingCompletedEventArgs(ChosenRounds, 
                                                                             ChosenBonusQuestions,
                                                                             mNewRounds,
                                                                             mNewBonusQuestions,
                                                                             hasBonusRound, 
                                                                             rbEnd.IsChecked.Value));
      Close();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void rbNone_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if(rbNone.IsChecked.HasValue && !rbNone.IsChecked.Value)
      {
        if(btnChooseBonus != null)
        {
          btnChooseBonus.IsEnabled = lvSelectableBonusQuestions?.Items == null ? true :
                                     lvSelectedBonusQuestions.Items.Count < MaxBonusQuestions;
        }

        if(btnUnchooseBonus != null)
        {
          btnUnchooseBonus.IsEnabled = true;
        }
      }
      else
      {
        if(btnChooseBonus != null)
        {
          btnChooseBonus.IsEnabled = false;
        }

        if(btnUnchooseBonus != null)
        {
          btnUnchooseBonus.IsEnabled = false;
        }
      }
    }

    private void btnNewQuestion_Click(object sender, RoutedEventArgs args)
    {
      if(mQuickQuestion != null)
      {
        return;
      }

      EventHandler<EventArgs<Round>> questionComplete;
      EventHandler closed;

      mQuickQuestion = new QuestionBuilder(false);

      questionComplete = null;
      questionComplete = (s, e) =>
      {
        mQuickQuestion.QuestionComplete -= questionComplete;
        Func<Round, bool> predicate = (b) => { return e.Data.Question.Equals(b.Question); };

        if(AvailableRounds.Any(predicate))
        {
          if(MessageBox.Show("This question already exists. Overwrite?", "Question Already Exists ", 
                             MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
          {
            AvailableRounds.Remove(AvailableRounds.First(predicate));
            AvailableRounds.Insert(0, e.Data.Copy());
            mNewRounds.Insert(0, e.Data.Copy());
          }
        }
        else if(ChosenRounds.Any(predicate))
        {
          if(MessageBox.Show("This question already exists. Overwrite?", "Question Already Exists ",
                             MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
          {
            int i = ChosenRounds.IndexOf(ChosenRounds.First(predicate));
            ChosenRounds.Remove(ChosenRounds.First(predicate));
            ChosenRounds.Insert(i, e.Data.Copy());
            mNewRounds.Insert(0, e.Data.Copy());
          }
        }
        else
        {
          AvailableRounds.Insert(0, e.Data.Copy());
          mNewRounds.Insert(0, e.Data.Copy());
        }
      };

      closed = null;
      closed += (s, e) =>
      {
        mQuickQuestion.Closed -= closed;

        mQuickQuestion = null;
      };

      mQuickQuestion.QuestionComplete += questionComplete;
      mQuickQuestion.Closed += closed;

      mQuickQuestion.ShowDialog();
    }

    private void btnNewBonus_Click(object sender, RoutedEventArgs args)
    {
      if(mQuickQuestion != null)
      {
        return;
      }

      EventHandler<EventArgs<BonusQuestion>> bonusQuestionComplete;
      EventHandler closed;

      mQuickQuestion = new QuestionBuilder(true);

      bonusQuestionComplete = null;
      bonusQuestionComplete = (s, e) =>
      {
        mQuickQuestion.BonusQuestionComplete -= bonusQuestionComplete;
        Func<BonusQuestion, bool> predicate = (b) => { return e.Data.Question.Equals(b.Question); };

        if(AvailableBonusQuestions.Any(predicate))
        {
          if(MessageBox.Show("This bonus question already exists. Overwrite?", "Question Already Exists ",
                             MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
          {
            AvailableBonusQuestions.Remove(AvailableBonusQuestions.First(predicate));
            AvailableBonusQuestions.Insert(0, e.Data.Copy());
            mNewBonusQuestions.Insert(0, e.Data.Copy());
          }
        }
        else if(ChosenBonusQuestions.Any(predicate))
        {
          if(MessageBox.Show("This bonus question already exists. Overwrite?", "Question Already Exists ",
                             MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
          {
            int i = ChosenBonusQuestions.IndexOf(ChosenBonusQuestions.First(predicate));
            ChosenBonusQuestions.Remove(ChosenBonusQuestions.First(predicate));
            ChosenBonusQuestions.Insert(i, e.Data.Copy());
            mNewBonusQuestions.Insert(0, e.Data.Copy());
          }
        }
        else
        {
          mNewBonusQuestions.Insert(0, e.Data.Copy());
          AvailableBonusQuestions.Insert(0, e.Data.Copy());
        }

      };

      closed = null;
      closed += (s, e) =>
      {
        mQuickQuestion.Closed -= closed;

        mQuickQuestion = null;
      };

      mQuickQuestion.BonusQuestionComplete += bonusQuestionComplete;
      mQuickQuestion.Closed += closed;

      mQuickQuestion.ShowDialog();
    }
  }

}
