using CommonLib.Constants;
using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FamilyFeud.DataObjects
{
  [Serializable]
  #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
  public class Game : INotifyPropertyChanged
  {
    #region Public Enumerations -----------------------------------------------
    // None.
    #endregion

    #region Private Enumerations ----------------------------------------------
    // None.
    #endregion

    #region Public Structs ----------------------------------------------------
    // None.
    #endregion

    #region Private Structs ---------------------------------------------------
    // None.
    #endregion

    #region Public Constants --------------------------------------------------
    // None.
    #endregion

    #region Private Constants -------------------------------------------------
    // None.
    #endregion

    #region Private Data Members ----------------------------------------------

    private BonusRound mBonusRound;
    private ObservableCollection<Round> mRounds;

    #endregion

    #region Public Events -----------------------------------------------------

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Constructors ------------------------------------------------------

    /// <summary>
    /// Default constructor, initializes a Question object with an empty question text
    /// </summary>
    public Game()
    {
      mBonusRound = null;
      mRounds = new ObservableCollection<Round>();
    }

    /// <summary>
    /// Initializes a Question object with the given question value
    /// </summary>
    /// <param name="questionText">
    /// Question string that will be displayed.
    /// </param>
    public Game(IEnumerable<Round> rounds, BonusRound bonusRound = null)
    {
      mBonusRound = bonusRound;
      mRounds = new ObservableCollection<Round>(rounds);
    }

    #endregion

    #region Public Methods ----------------------------------------------------

    /// <summary>
    /// Overrides the ToString to provide more relevant information on this object.
    /// </summary>
    /// <returns>
    /// A string containing the string representation of each round and the optional
    /// bonus round if it exists.
    /// </returns>
    public override string ToString()
    {
      string retStr = CommonConst.EmptyString;

      foreach(Round round in mRounds)
      {
        retStr += round.ToString();
      }

      retStr += mBonusRound.ToString() ?? CommonConst.EmptyString;

      return retStr;
    }

    /// <summary>
    /// Shallow equality check between this and the passed object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      Game game;

      if(!(obj is Game))
      {
        return false;
      }

      if(obj == this)
      {
        return true;
      }

      game = obj as Game;

      if (game.NumRounds != this.NumRounds ||
         (game.BonusRound == null && this.BonusRound != null) ||
         (game.BonusRound != null && this.BonusRound == null))
      {
        return false;
      }

      if (!(game.BonusRound == null && this.BonusRound == null) &&
         !game.BonusRound.Equals(this.BonusRound))
      {
        return false;
      }

      for (int i = 0; i < this.NumRounds; i++)
      {
        if (!this.Rounds[i].Equals(game.Rounds[i]))
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Creates a deep copy of this object. Items contained in its properties will also be deep copies.
    /// </summary>
    /// <returns></returns>
    public Game Copy()
    {
      Game copy;

      copy = new Game();

      copy.BonusRound = this.BonusRound?.Copy();
      copy.Rounds.AddRange(this.Rounds.Select(round => round.Copy()));

      return copy;
    }

    #endregion

    #region Protected Methods -------------------------------------------------
    // None.
    #endregion

    #region Private Methods ---------------------------------------------------
    // None.
    #endregion

    #region Properties --------------------------------------------------------

    /// <summary>
    /// List of rounds that make up this game
    /// </summary>
    public ObservableCollection<Round> Rounds
    {
      get
      {
        return mRounds;
      }
      set
      {
        mRounds = value ?? new ObservableCollection<Round>();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rounds"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumRounds"));
      }
    }

    /// <summary>
    /// Number of answers, provided for more readable xaml binding
    /// </summary>
    [XmlIgnore]
    public uint NumRounds
    {
      get
      {
        return Rounds != null ? unchecked((uint)Rounds.Count) : 0;
      }
    }

    /// <summary>
    /// The optional bonus round
    /// </summary>
    public BonusRound BonusRound
    {
      get
      {
        return mBonusRound;
      }
      set
      {
        mBonusRound = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BonusRound"));
      }
    }

    #endregion

    #region Interface Implementations -----------------------------------------
    // None.
    #endregion
  }
}
