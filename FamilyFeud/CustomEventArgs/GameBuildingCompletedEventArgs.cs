using FamilyFeud.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFeud.CustomEventArgs
{
  public delegate void GameBuildingCompletedEventHandler(object sender, GameBuildingCompletedEventArgs args);

  public class GameBuildingCompletedEventArgs : EventArgs
  {
    public GameBuildingCompletedEventArgs(
      IEnumerable<Round> selectedRounds, 
      IEnumerable<BonusQuestion> selectedBonusQuestions, 
      bool hasBonusRound, 
      bool bonusAtEnd)
    {
      SelectedRounds = selectedRounds;
      SelectedBonusQuestions = selectedBonusQuestions;
      HasBonusRound = hasBonusRound;

      BonusAtEnd = hasBonusRound && bonusAtEnd;
    }

    public GameBuildingCompletedEventArgs(
      IEnumerable<Round> selectedRounds,
      IEnumerable<BonusQuestion> selectedBonusQuestions,
      IEnumerable<Round> newRounds,
      IEnumerable<BonusQuestion> newBonusQuestions,
      bool hasBonusRound,
      bool bonusAtEnd)
    {
      SelectedRounds = selectedRounds;
      SelectedBonusQuestions = selectedBonusQuestions;
      HasBonusRound = hasBonusRound;

      NewRounds = newRounds;
      NewBonusQuestions = newBonusQuestions;

      BonusAtEnd = hasBonusRound && bonusAtEnd;
    }

    public IEnumerable<Round> SelectedRounds { get; }

    public IEnumerable<BonusQuestion> SelectedBonusQuestions { get; }

    public IEnumerable<Round> NewRounds { get; }

    public IEnumerable<BonusQuestion> NewBonusQuestions { get; }

    public bool HasBonusRound { get; }

    public bool BonusAtEnd { get; }
  }
}
