//-----------------------------------------------------------------------------
// File Name: BonusRound.cs
// Contents:  File contains the declaration of the BonusRound class
//-----------------------------------------------------------------------------
// History:
//
//   TR Number      Author              Date            Description
//   ---------      ---------------     ------------    -----------------------
//   Original       aaronkunzer          3/26/2018 11:24:52 AM          Original Implementation
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using ExtensionMethods;
using CommonLib.Constants;
using System.Collections.ObjectModel;

namespace FamilyFeud.DataObjects
{
  #region Namespace Enumerations ----------------------------------------------
  // None.
  #endregion

  #region Namespace Delegates -------------------------------------------------
  // None.
  #endregion

  #region Namespace Structs ---------------------------------------------------
  // None.
  #endregion

  #region Namespace Interfaces ------------------------------------------------
  // None.
  #endregion

  /// <summary>
  /// Class containing the implementation of BonusRound
  /// </summary>
  [Serializable]
  public class BonusRound : INotifyPropertyChanged
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

    private ObservableCollection<BonusQuestion> mBonusQuestions;

    #endregion

    #region Public Events -----------------------------------------------------

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Constructors ------------------------------------------------------
    
    /// <summary>
    /// Initializes a BonusRound object with an empty set of questions and answers
    /// </summary>
    public BonusRound()
    {
      BonusQuestions = new ObservableCollection<BonusQuestion>();
    }

    /// <summary>
    /// Initializes a BonusRound object with the given sets of questions and answers
    /// </summary>
    /// <param name="questions">
    /// Enumerable ObservableCollection of questions
    /// </param>
    /// <param name="answers">
    /// Enumerable ObservableCollection of answers
    /// </param>
    public BonusRound(IEnumerable<Question> questions, IEnumerable<Answer> answers)
    {
      if(questions == null || answers == null)
      {
        throw new ArgumentNullException();
      }

      if(questions.Count() != answers.Count())
      {
        throw new IndexOutOfRangeException("Unequal number of questions to answers in BonusRound initialization");
      }

      BonusQuestions = new ObservableCollection<BonusQuestion>();

      for(int i = 0; i < questions.Count(); i++)
      {
        mBonusQuestions.Add(new BonusQuestion(questions.ElementAt(i), answers.ElementAt(i)));
      }
    }

    /// <summary>
    /// Initializes a BonusRound object with the given sets of questions and answers
    /// </summary>
    /// <param name="questionSet">
    /// KVP set of questions and answers
    /// </param>
    public BonusRound(IEnumerable<BonusQuestion> questionSet)
    {
      if(questionSet == null)
      {
        throw new ArgumentNullException();
      }

      BonusQuestions = new ObservableCollection<BonusQuestion>();
      BonusQuestions.AddRange(questionSet);
    }

    #endregion

    #region Public Methods ----------------------------------------------------

    public override string ToString()
    {
      string retStr = "Bonus Round:" + Environment.NewLine;

      foreach(BonusQuestion item in mBonusQuestions)
      {
        retStr += '\t' + item.Question.ToString() + " -> " + item.Answer.ToString() + Environment.NewLine;
      }

      return retStr;
    }

    /// <summary>
    /// Shallow equality check between this and the passed object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      BonusRound br;

      if(!(obj is BonusRound))
      {
        return false;
      }

      if(obj == this)
      {
        return true;
      }

      br = obj as BonusRound;

      if(mBonusQuestions == null && br.BonusQuestions == null)
      {
        return true;
      }

      if((mBonusQuestions == null && br.BonusQuestions != null) ||
         (mBonusQuestions != null && br.BonusQuestions == null) ||
          mBonusQuestions.Count != br.BonusQuestions.Count)
      {
        return false;
      }

      for(int i = 0; i < mBonusQuestions.Count; i++)
      {
        if(!mBonusQuestions[i].Question.Equals(br.BonusQuestions[i].Question) ||
           !mBonusQuestions[i].Answer.Equals(br.BonusQuestions[i].Answer))
        {
          return false;
        }
      }

      return true;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Creates a deep copy of this object. Items contained in its properties will also be deep copies.
    /// </summary>
    /// <returns></returns>
    public BonusRound Copy()
    {
      BonusRound ret = new BonusRound();

      ret.BonusQuestions.AddRange(mBonusQuestions.Select(item => new BonusQuestion(item.Question.Copy(), item.Answer.Copy())));

      return ret;
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
    /// Dictionary containing a Key-Value pair, Key = Question, Value = Answer
    /// </summary>
    public ObservableCollection<BonusQuestion> BonusQuestions
    {
      get
      {
        return mBonusQuestions;
      }
      set
      {
        mBonusQuestions = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BonusQuestions"));
      }
    }

    #endregion

    #region Interface Implementations -----------------------------------------
    // None.
    #endregion
  }
}
