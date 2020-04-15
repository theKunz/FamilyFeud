using FamilyFeud.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace FamilyFeud.Helpers
{
  public static class SaveFileHelpers
  {
    private static XmlSerializer Serializer = new XmlSerializer(typeof(List<object>),
                                                                new Type[]
                                                                {
                                                                  typeof(Game),
                                                                  typeof(Round),
                                                                  typeof(BonusRound),
                                                                  typeof(Question),
                                                                  typeof(Answer)
                                                                });
    public const string SaveFileNameRound = "GameDataRounds.fmf";
    public const string SaveFileNameBonusQuestion = "GameDataBonus.fmf";

    public static void UpdateRoundSaveData(IEnumerable<IQuestioner> rounds)
    {
      if(rounds == null)
      {
        throw new ArgumentNullException("Parameter" + nameof(rounds) + "was null");
      }

      IEnumerable<Round> castedRounds;

      try
      {
        castedRounds = rounds.Cast<Round>();
      }
      catch(Exception)
      {
        MessageBox.Show("An error occurred while saving.\\r\\nChanges to questions were not saved.", "An Error Occurred", MessageBoxButton.OK);
        return;
      }

      UpdateRoundSaveData(castedRounds);
    }

    public static void UpdateRoundSaveData(IEnumerable<Round> rounds)
    {
      if(rounds == null)
      {
        throw new ArgumentNullException("Parameter" + nameof(rounds) + "was null");
      }

      string exePath;
      string filePathRound;

      exePath = AppDomain.CurrentDomain.BaseDirectory;
      filePathRound = exePath + SaveFileNameRound;

      using(StreamWriter stream = new StreamWriter(filePathRound, false))
      {
        Serializer.Serialize(stream, new List<object>(rounds));
      }
    }

    public static void UpdateBonusRoundSaveData(IEnumerable<IQuestioner> bonusQuestions)
    {
      if(bonusQuestions == null)
      {
        throw new ArgumentNullException("Parameter" + nameof(bonusQuestions) + "was null");
      }

      IEnumerable<BonusQuestion> castedBonusQuestions;

      try
      {
        castedBonusQuestions = bonusQuestions.Cast<BonusQuestion>();
      }
      catch(Exception)
      {
        MessageBox.Show("An error occurred while saving.\\r\\nChanges to questions were not saved.", "An Error Occurred", MessageBoxButton.OK);
        return;
      }

      UpdateBonusRoundSaveData(castedBonusQuestions);
    }

    public static void UpdateBonusRoundSaveData(IEnumerable<BonusQuestion> bonusQuestions)
    {
      if(bonusQuestions == null)
      {
        throw new ArgumentNullException("Parameter" + nameof(bonusQuestions) + "was null");
      }

      string exePath;
      string filePathRound;

      exePath = AppDomain.CurrentDomain.BaseDirectory;
      filePathRound = exePath + SaveFileNameBonusQuestion;

      using(StreamWriter stream = new StreamWriter(filePathRound, false))
      {
        Serializer.Serialize(stream, new List<object>(bonusQuestions));
      }
    }

    public static IEnumerable<Round> LoadRoundSaveData()
    {
      string exePath = AppDomain.CurrentDomain.BaseDirectory;
      string filePath = exePath + SaveFileNameRound;

      try
      {
        using(StreamReader stream = new StreamReader(filePath, Encoding.Default))
        {
          Window popup = new Window();
          List<object> res = Serializer.Deserialize(stream) as List<object>;

          var cast = res.Cast<Round>();

          return cast;
        }
      }
      catch(FileNotFoundException)
      {
        UpdateRoundSaveData(new List<Round>());
        return new List<Round>();
      }
      catch(InvalidOperationException ioe)
      {
        if(RequestDataResetOnError(false, ioe.InnerException.Message))
        {
          UpdateRoundSaveData(new List<Round>());
        }
        return new List<Round>();
      }
    }

    public static IEnumerable<BonusQuestion> LoadBonusRoundSaveData()
    {
      string exePath = AppDomain.CurrentDomain.BaseDirectory;
      string filePath = exePath + SaveFileNameBonusQuestion;

      try
      {
        using(StreamReader stream = new StreamReader(filePath, Encoding.Default))
        {
          List<object> res = Serializer.Deserialize(stream) as List<object>;

          var cast = res.Cast<BonusQuestion>();

          return cast;
        }
      }
      catch(FileNotFoundException)
      {
        UpdateBonusRoundSaveData(new List<BonusQuestion>());
        return new List<BonusQuestion>();
      }
      catch(InvalidOperationException ioe)
      {
        if(RequestDataResetOnError(true, ioe.InnerException.Message))
        {
          UpdateRoundSaveData(new List<BonusQuestion>());
        }
        return new List<BonusQuestion>();
      }
    }

    private static bool RequestDataResetOnError(bool isBonusRoundData, string furtherInformation = null)
    {
      string message = string.Format("An error occurred while loading {0} data.\r\nWould you like to format your data?", isBonusRoundData ? "bonus question" : "question");
      string caption = "Error loading data";

      if(MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
      {
        return true;
      }
      else
      {
        string filePath = AppDomain.CurrentDomain.BaseDirectory + (isBonusRoundData ? SaveFileNameBonusQuestion : SaveFileNameRound);
        string errorInfo = string.Format("Erroneous data can be found in file\r\n{0}\r\n\r\nCreating a new {1} will overwrite the errorneous data.", 
                                         filePath, 
                                         isBonusRoundData ? "bonus question" : "question");

        if(furtherInformation != null)
        {
          errorInfo += "\r\n\r\nAdditional Information:\r\n" + furtherInformation;
        }

        MessageBox.Show(errorInfo,
                        "Error loading data",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

        return false;
      }
    }
  }
}
