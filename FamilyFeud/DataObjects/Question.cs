//-----------------------------------------------------------------------------
// File Name: Question.cs
// Contents:  File contains the declaration of the Question class
//-----------------------------------------------------------------------------
// History:
//
//   TR Number      Author              Date            Description
//   ---------      ---------------     ------------    -----------------------
//   Original       aaronkunzer          3/26/2018 11:38:33 AM          Original Implementation
//-----------------------------------------------------------------------------
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using CommonLib.Constants;
using System.ComponentModel;

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
  /// Class containing the implementation of Question
  /// </summary>
  [Serializable]
  #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
  public class Question : INotifyPropertyChanged
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

    private string mQuestionText;

    #endregion

    #region Public Events -----------------------------------------------------

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Constructors ------------------------------------------------------
    
    /// <summary>
    /// Default constructor, initializes a Question object with an empty question text
    /// </summary>
    public Question()
    {
      QuestionText = CommonConst.EmptyString;
    }

    /// <summary>
    /// Initializes a Question object with the given question value
    /// </summary>
    /// <param name="questionText">
    /// Question string that will be displayed.
    /// </param>
    public Question(string questionText)
    {
      QuestionText = questionText;
    }

    #endregion

    #region Public Methods ----------------------------------------------------

    /// <summary>
    /// Overrides the ToString to provide more relevant information on this object.
    /// </summary>
    /// <returns>
    /// A string containing the Question's value. Identical to the QuestionText property.
    /// </returns>
    public override string ToString()
    {
      return QuestionText;
    }

    /// <summary>
    /// Checks for a equality between this and the given object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      if(!(obj is Question))
      {
        return false;
      }

      if(obj == this)
      {
        return true;
      }

      return (obj as Question).ToString().Equals(this.ToString());
    }

    /// <summary>
    /// Creates and returns a deep copy of this question
    /// </summary>
    /// <returns></returns>
    public Question Copy()
    {
      return new Question(String.Copy(mQuestionText));
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
    /// Question string for display
    /// </summary>
    public string QuestionText
    {
      get
      {
        return mQuestionText;
      }
      set
      {
        mQuestionText = string.IsNullOrEmpty(value) ? CommonConst.EmptyString : value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuestionText)));
      }
    }

    #endregion

    #region Interface Implementations -----------------------------------------
    // None.
    #endregion
  }
}
