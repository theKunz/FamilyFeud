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
    private IEnumerable<Round> mSelectedRounds;
    private IEnumerable<BonusQuestion> mSelectedBonusQuestions;
    private bool mHasBonusRound;
    private bool mBonusAtEnd;

    public GameBuildingCompletedEventArgs(IEnumerable<Round> selectedRounds, IEnumerable<BonusQuestion> selectedBonusQuestions, bool hasBonusRound, bool bonusAtEnd)
    {
      mSelectedRounds = selectedRounds;
      mSelectedBonusQuestions = selectedBonusQuestions;
      mHasBonusRound = hasBonusRound;

      mBonusAtEnd = hasBonusRound && bonusAtEnd;
    }

    public IEnumerable<Round> SelectedRounds
    {
      get
      {
        return mSelectedRounds;
      }
    }

    public IEnumerable<BonusQuestion> SelectedBonusQuestions
    {
      get
      {
        return mSelectedBonusQuestions;
      }
    }

    public bool HasBonusRound
    {
      get
      {
        return mHasBonusRound;
      }
    }

    public bool BonusAtEnd
    {
      get
      {
        return mBonusAtEnd;
      }
    }
  }
}
