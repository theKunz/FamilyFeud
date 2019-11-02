using CommonLib.CustomEventArgs;
using FamilyFeud.Controls;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
/// <summary>
/// This class is largely a wrapper method for relating a Question with a single answer,
/// used by the BonusRound class.
/// </summary>
namespace FamilyFeud.DataObjects
{
  [Serializable]
  public class BonusQuestion : INotifyPropertyChanged, IQuestioner, IEditable
  {
    private Question mQuestion;
    private Answer mAnswer;

    public event PropertyChangedEventHandler PropertyChanged;

    public BonusQuestion()
    {
      mAnswer = new Answer();
      mQuestion = new Question();
    }

    public BonusQuestion(Question question, Answer answer)
    {
      mQuestion = question;
      mAnswer = answer;
    }

    public BonusQuestion(string question, string answer, uint pointValue)
    {
      mQuestion = new Question(question);
      mAnswer = new Answer(answer, pointValue);
    }

    public override string ToString()
    {
      return (Question.ToString() ?? string.Empty) + " : " + (Answer.ToString() ?? string.Empty) + Environment.NewLine;
    }

    public BonusQuestion Copy()
    {
      return new BonusQuestion(Question.Copy(), Answer.Copy());
    }

    public Question Question
    {
      get
      {
        return mQuestion;
      }
      set
      {
        if(!mQuestion.Equals(value))
        {
          mQuestion = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Question)));
        }
      }
    }

    public Answer Answer
    {
      get
      {
        return mAnswer;
      }
      set
      {
        if(!mAnswer.Equals(value))
        {
          mAnswer = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Answer)));
        }
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
          EventHandler<EventArgs<BonusQuestion>> bqComplete;
          EventHandler bqClosed;

          bqComplete = null;
          bqComplete = (object s, EventArgs<BonusQuestion> args) =>
          {
            qb.BonusQuestionComplete -= bqComplete;

            this.Answer = args.Data.Answer;
            this.Question = args.Data.Question;

          };

          bqClosed = null;
          bqClosed = (object s, EventArgs args) =>
          {
            qb.BonusQuestionComplete -= bqComplete;
            qb.Closed -= bqClosed;
          };

          qb.Closed += bqClosed;
          qb.BonusQuestionComplete += bqComplete;

          qb.Title = "Edit Bonus Question";
          qb.ShowDialog();
        };
      }
      set
      {
        return;
      }
    }
  }
}
