using FamilyFeud.Controls;
using FamilyFeud.DataObjects;
using FamilyFeud.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace FamilyFeud
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private GameWindow gameWindow;
    private XmlSerializer mSerializer;
    private ObservableCollection<IQuestioner> dummyDataQ;
    private ObservableCollection<IQuestioner> dummyDataB;

    public MainWindow()
    {
      InitializeComponent();

      Loaded += (s, args) =>
      {
        BackgroundRoot.Height = 1080.0 - SystemParameters.CaptionHeight * 3 + 3;
      };

      dummyDataQ = new ObservableCollection<IQuestioner>()
      {
        new Round("Who am I?", new ObservableCollection<Answer>()
        {
          new Answer("I am one", 1),
          new Answer("I am two", 2),
          new Answer("I am three", 3),
          new Answer("I am four", 4),
          new Answer("I am five", 5),
          new Answer("I am six", 6),
        }),
        new Round("Who are you?", new ObservableCollection<Answer>()
        {
          new Answer("You are one", 1),
          new Answer("You are two", 2),
          new Answer("You are three", 3),
          new Answer("You are four", 4),
          new Answer("You are five", 5),
          new Answer("You are six", 6),
        }),
        new Round("Who are they?", new ObservableCollection<Answer>()
        {
          new Answer("They are one", 1),
          new Answer("They are two", 2),
          new Answer("They are three", 3),
          new Answer("They are four", 4),
          new Answer("They are five", 5),
          new Answer("They are six", 6),
        })
      };

      dummyDataB = new ObservableCollection<IQuestioner>()
      {
        new BonusQuestion("BQ1", "BQA1", 10),
        new BonusQuestion("BQ2", "BQA2", 10),
        new BonusQuestion("BQ3", "BQA3", 10),
        new BonusQuestion("BQ4", "BQA4", 10),
        new BonusQuestion("BQ5", "BQA5", 10),
        new BonusQuestion("BQ6", "BQA6", 10),
        new BonusQuestion("BQ7", "BQA7", 10),
        new BonusQuestion("BQ8", "BQA8", 10),
        new BonusQuestion("BQ9", "BQA9", 10),
        new BonusQuestion("BQ10", "BQA10", 10),
        new BonusQuestion("BQ11", "BQA11", 10),
        new BonusQuestion("BQ12", "BQA12", 10),
        new BonusQuestion("BQ13", "BQA13", 10)
      };

      LoadSaveData();
      QuestionList.ItemSource = dummyDataQ;
      BonusQuestionList.ItemSource = dummyDataB;
    }

    private void MainMenu_Closed(object sender, EventArgs e)
    {
      gameWindow?.Close();
      App.Current.Shutdown();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnBeginRandomGame_Click(object sender, RoutedEventArgs e)
    {
      gameWindow?.Close();
      gameWindow = new GameWindow();
      gameWindow.Show();
    }

    private void btnBeginCustomGame_Click(object sender, RoutedEventArgs e)
    {
      GameBuilder gb = new GameBuilder(dummyDataQ.Cast<Round>(), dummyDataB.Cast<BonusQuestion>());
      gb.GameBuildingCompleted += GameBuildingCompleted;
      gb.Show();
    }

    private void GameBuildingCompleted(object sender, GameBuildingCompletedEventArgs args)
    {
      Game game = new Game();
      ObservableCollection<Round> rounds = new ObservableCollection<Round>();
      BonusRound bonusRound = new BonusRound();

      // Create a deep copy to prevent editing of rounds and Bonus Questions from messing up existing games.
      // If you disable editing while a game is occurring, you can remove this.
      foreach(Round round in args.SelectedRounds)
      {
        rounds.Add(round.Copy());
      }

      foreach(BonusQuestion bonusQuestion in args.SelectedBonusQuestions)
      {
        bonusRound.BonusQuestions.Add(bonusQuestion.Copy());
      }

      gameWindow?.Close();

      gameWindow = new GameWindow(new Game(rounds, bonusRound));
      gameWindow.Show();
    }

    public void LoadSaveData()
    {
      string exePath;
      string filePathRound;
      string filePathBonus;

      exePath = System.AppDomain.CurrentDomain.BaseDirectory;
      filePathRound = exePath + "GameDataRounds.xml";
      filePathBonus = exePath + "GameDataBonus.xml";

      mSerializer = new XmlSerializer(typeof(List<object>),
                                      new Type[]
                                      {
                                        typeof(Game),
                                        typeof(Round),
                                        typeof(BonusRound),
                                        typeof(Question),
                                        typeof(Answer)
                                      });

      // Ensures that the file exists
      using(StreamWriter stream = new StreamWriter(filePathRound, true))
      {
        stream.Write("");
      }

      using (StreamWriter stream = new StreamWriter(filePathBonus, true))
      {
        stream.Write("");
      }

      // -------------Temporary-------------------------
      using (StreamWriter stream = new StreamWriter(filePathRound, false))
      {
        mSerializer.Serialize(stream, new List<object>(dummyDataQ));
      }
      using(StreamWriter stream = new StreamWriter(filePathBonus, false))
      {
        mSerializer.Serialize(stream, new List<object>(dummyDataB));
      }
      //------------------------------------------------

      using (StreamReader stream = new StreamReader(filePathRound, Encoding.Default))
      {
        Window popup = new Window();
        List<object> res = mSerializer.Deserialize(stream) as List<object>;
        string returnStr = "";

        res.ForEach((object o) => returnStr += (o as Round).ToString());

        popup.Content = returnStr;
        popup.Show();
      }

      using (StreamReader stream = new StreamReader(filePathBonus, Encoding.Default))
      {
        Window popup = new Window();
        List<object> res = mSerializer.Deserialize(stream) as List<object>;
        string returnStr = "";

        res.ForEach((object o) => returnStr += (o as BonusQuestion).ToString());
        
        popup.Content = returnStr;
        popup.Show();
      }
    }
  }
}
