//-----------------------------------------------------------------------------
// File Name: Round.cs
// Contents:  File contains the declaration of the Round class
//-----------------------------------------------------------------------------
// History:
//
//   TR Number      Author              Date            Description
//   ---------      ---------------     ------------    -----------------------
//   Original       aaronkunzer          3/23/2018 11:37:53 AM          Original Implementation
//-----------------------------------------------------------------------------
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using ExtensionMethods;
using CommonLib.Constants;
using System.ComponentModel;
using CommonLib.CustomEventArgs;
using FamilyFeud.Controls;

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
  /// Class containing the implementation of Question and multiple Answers
  /// </summary>
  [Serializable]
  #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
  public class Round : INotifyPropertyChanged, IQuestioner, IEditable
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

    private ObservableCollection<Answer> mAnswers;
    private Question mQuestion;

    #endregion

    #region Public Events -----------------------------------------------------

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Constructors ------------------------------------------------------

    /// <summary>
    /// For Serialization Purposes, Used to create a round with Empty Values
    /// </summary>
    public Round()
    {
      mQuestion = new Question();
      mAnswers = new ObservableCollection<Answer>();
    }

    /// <summary>
    /// Initializes a new Round object with the given optional question and answers
    /// </summary>
    /// <param name="question">
    /// Question for the round
    /// </param>
    /// <param name="answers">
    /// List of answers for the round
    /// </param>
    public Round(string question, IEnumerable<Answer> answers)
    {
      if(question == null || answers == null)
      {
        throw new ArgumentNullException();
      }

      mAnswers = new ObservableCollection<Answer>();
      mAnswers.AddRange(answers);

      mQuestion = new Question(question);
    }


    /// <summary>
    /// Initializes a new Round object with the given optional question and answers
    /// </summary>
    /// <param name="question">
    /// Question object for the round
    /// </param>
    /// <param name="answers">
    /// List of answers for the round
    /// </param>
    public Round(Question question, IEnumerable<Answer> answers)
    {
      if(question == null || answers == null)
      {
        throw new ArgumentNullException();
      }

      mAnswers = new ObservableCollection<Answer>();
      mAnswers.AddRange(answers);

      mQuestion = question;
    }

    #endregion

    #region Public Methods ----------------------------------------------------

    /// <summary>
    /// Builds a string representation of this round, displaying the round and it's various answers.
    /// </summary>
    /// <returns>
    /// String with a format of the question followed by the answers and their pointvalue on unique lines.
    /// </returns>
    public override string ToString()
    {
      string retVal;

      retVal = CommonConst.EmptyString;

      retVal += Question.ToString() + Environment.NewLine;
      foreach(Answer answer in Answers)
      {
        retVal +=  '\t' + answer.ToString() + Environment.NewLine;
      }

      return retVal;
    }

    /// <summary>
    /// Shallow equality check between this and the passed object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      Round round;

      if(!(obj is Round))
      {
        return false;
      }

      if(obj == this)
      {
        return true;
      }

      round = obj as Round;

      if(round.NumAnswers != this.NumAnswers || 
         (round.Question == null && this.Question != null) ||
         (round.Question != null && this.Question == null) ||
         (round.Answers == null && this.Answers != null) ||
         (round.Answers != null && this.Answers == null))
      {
        return false;
      }

      if(!(round.Question == null && this.Question == null) &&
         !round.Question.Equals(this.Question))
      {
        return false;
      }

      if(round.Answers == null && this.Answers == null)
      {
        return true;
      }

      for(int i = 0; i < this.NumAnswers; i++)
      {
        if(!this.Answers[i].Equals(round.Answers[i]))
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Creates and returns a deep copy of this round
    /// </summary>
    /// <returns></returns>
    public Round Copy()
    {
      Round ret;

      ret = new Round();

      ret.Answers.AddRange(this.Answers.Select(ans => ans.Copy()));
      ret.Question = this.Question.Copy();

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
    /// Answers to the question
    /// </summary>
    public ObservableCollection<Answer> Answers
    {
      get
      {
        return mAnswers;
      }
      set
      {
        mAnswers = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Answers)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumAnswers)));
      }
    }

    /// <summary>
    /// Number of answers, provided for more readable xaml binding
    /// </summary>
    [XmlIgnore]
    public uint NumAnswers
    {
      get
      {
        return Answers != null ? unchecked((uint)Answers.Count) : 0;
      }
    }

    /// <summary>
    /// The question that is asked this round
    /// </summary>
    public Question Question
    {
      get
      {
        return mQuestion;
      }
      set
      {
        mQuestion = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Question)));
      }
    }

    [XmlIgnore]
    public Action<object> EditAction
    {
      get
      {
        return (object obj) =>
        {
          QuestionBuilder qb = new QuestionBuilder(this);
          EventHandler<EventArgs<Round>> qComplete;
          EventHandler bqClosed;

          qComplete = null;
          qComplete = (object s, EventArgs<Round> args) =>
          {
            qb.QuestionComplete -= qComplete;

            this.Question = args.Data.Question;
            this.Answers = args.Data.Answers;

          };

          bqClosed = null;
          bqClosed = (object s, EventArgs args) =>
          {
            qb.QuestionComplete -= qComplete;
            qb.Closed -= bqClosed;
          };

          qb.Closed += bqClosed;
          qb.QuestionComplete += qComplete;

          qb.Title = "Edit Question";
          qb.ShowDialog();
        };
      }
      set
      {
        return;
      }
    }

    #endregion

    #region Interface Implementations -----------------------------------------
    // None.
    #endregion
  }
}
