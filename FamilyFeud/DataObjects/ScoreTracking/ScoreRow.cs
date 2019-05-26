using CommonLib.Constants;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace FamilyFeud.DataObjects
{
  public class ScoreRow : INotifyPropertyChanged, IComparable
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public ScoreRow(int numQuestions, string teamName = CommonConst.EmptyString)
    {
      Scores = new ObservableCollection<PointVal>();
      Name = teamName;

      numQuestions = numQuestions < 1 ? 1 : numQuestions;

      for (int i = 0; i < numQuestions; i++)
      {
        PointVal pv = new PointVal();
        pv.PropertyChanged += (s, e) =>
        {
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Scores"));
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScoreTotal"));
        };

        Scores.Add(pv);
      }

      Scores.CollectionChanged += (s, e) =>
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Scores"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScoreTotal"));
      };


    }

    private string mName;
    private ObservableCollection<PointVal> mScores;

    public string Name
    {
      get
      {
        return mName;
      }
      set
      {
        mName = value == null ? CommonConst.EmptyString : value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
      }
    }

    public ObservableCollection<PointVal> Scores
    {
      get
      {
        return mScores;
      }
      set
      {
        mScores = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Scores"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScoreTotal"));
      }
    }

    public int ScoreTotal
    {
      get
      {
        return Scores == null ? 0 :
               Scores.Where(p => p.PointValue.HasValue).Select(p => p.PointValue.Value).Sum();
      }
    }

    public int CompareTo(object obj)
    {
      if(obj == null)
      {
        return -1;
      }
      
      if(!obj.GetType().Equals(typeof(ScoreRow)))
      {
        throw new ArgumentException("Compared object must be an instance of ScoreRow");
      }

      return (obj as ScoreRow).ScoreTotal - this.ScoreTotal;
    }
  }
}
