//-----------------------------------------------------------------------------
// File Name: Answer.cs
// Contents:  File contains the declaration of the Answer class
//-----------------------------------------------------------------------------
// History:
//
//   TR Number      Author              Date            Description
//   ---------      ---------------     ------------    -----------------------
//   Original       aaronkunzer          3/23/2018 10:43:22 AM          Original Implementation
//-----------------------------------------------------------------------------
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using CommonLib.Constants;
using System.ComponentModel;
using System.Security.Cryptography;

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
  /// Class containing the implementation of Answer
  /// </summary>
  [Serializable]
  #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
  public class Answer : IComparable, INotifyPropertyChanged
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

    private string mAnswerString;
    private uint mPointValue;

    #endregion

    #region Public Events -----------------------------------------------------

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Constructors ------------------------------------------------------
    
    /// <summary>
    /// Default constructor, initializes answer with an empty answer with 0 point value
    /// </summary>
    public Answer()
    {
      mAnswerString = CommonConst.EmptyString;
      mPointValue = 0;
    }

    /// <summary>
    /// Initializes an answer object with the given answer and points
    /// </summary>
    /// <param name="answerText">
    /// Answer's value
    /// </param>
    /// <param name="points">
    /// Point value of the answer
    /// </param>
    public Answer(string answerText = CommonConst.EmptyString, uint points = 0)
    {
      AnswerText = answerText;
      PointValue = points;
    }

    #endregion

    #region Public Methods ----------------------------------------------------
    
    /// <summary>
    /// String override to display simple representation of this Answer and it's value
    /// </summary>
    /// <returns>
    /// Return's answer and value in the format Answer: PointValue
    /// </returns>
    public override string ToString()
    {
      return mAnswerString + " : " + mPointValue.ToString();  
    }

    /// <summary>
    /// Shallow equality check between this and the passed object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      if(!(obj is Answer))
      {
        return false;
      }

      if(obj == this)
      {
        return true;
      }

      return (obj as Answer).ToString().Equals(this.ToString());
    }

    /// <summary>
    /// Creates and returns a deep copy of this answer
    /// </summary>
    /// <returns></returns>
    public Answer Copy()
    {
      return new Answer(String.Copy(mAnswerString), mPointValue);
    }

    #endregion

    #region Protected Methods -------------------------------------------------
    // None.
    #endregion

    #region Private Methods ---------------------------------------------------
    // None.
    #endregion

    #region Properties --------------------------------------------------------

    public string AnswerText
    {
      get
      {
        return mAnswerString;
      }
      set
      {
        mAnswerString = string.IsNullOrEmpty(value) ? CommonConst.EmptyString : value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AnswerText"));
      }
    }

    public uint PointValue
    {
      get
      {
        return mPointValue;
      }
      set
      {
        mPointValue = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointValue"));
      }
    }

    #endregion

    #region Interface Implementations -----------------------------------------

    /// <summary>
    /// Implments CompareTo for sorting, determined by point value.
    /// </summary>
    /// <param name="comparable">
    /// Answer to compare this object against
    /// </param>
    /// <returns>
    /// Returns &gt; 0 if parameter point value &lt; caller point value
    /// Returns = 0 if caller point value = parameter point value
    /// Returns &lt; 0 if parameter point value &gt; caller point value
    /// </returns>
    public int CompareTo(object compareObj)
    {
      if (!(compareObj is Answer))
      {
        throw new InvalidCastException("Cannot compare type " + this.GetType().ToString() + " to type " + compareObj.GetType().ToString());
      }

      Answer comparable = compareObj as Answer;

      return (int)comparable.PointValue - (int)this.PointValue;
    }

    #endregion
  }
}
