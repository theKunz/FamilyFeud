using FamilyFeud.DataObjects;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using DrawingPoint = System.Drawing.Point;

namespace FamilyFeud.Helpers
{
  public static class ImagePrinter
  {
    public static void DownloadBonusRoundImage(IEnumerable<BonusQuestion> questions)
    {
      if(questions == null || questions.Count() == 0)
      {
        return;
      }

      SaveFileDialog fileDialog = new SaveFileDialog();
      fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
      fileDialog.FileName = "Bonus_Round_Entry_Page";
      fileDialog.DefaultExt = ".bmp";
      fileDialog.Filter = "Bitmap Image|*.bmp";

      bool? fileChosen = fileDialog.ShowDialog();

      if(!fileChosen.HasValue || !fileChosen.Value)
      {
        return;
      }

      DrawingPoint[] imageLocs = new System.Drawing.Point[10] 
      {
        new DrawingPoint(115, 283), 
        new DrawingPoint(115, 438), 
        new DrawingPoint(115, 593), 
        new DrawingPoint(115, 748), 
        new DrawingPoint(115, 903), 
        new DrawingPoint(1034, 283),
        new DrawingPoint(1034, 438),
        new DrawingPoint(1034, 593),
        new DrawingPoint(1034, 748),
        new DrawingPoint(1034, 903),
      };
      
      Bitmap outBmp = (Bitmap)Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "/Images/BonusQuestionPrintout.bmp");

      using(Graphics graphics = Graphics.FromImage(outBmp))
      {
        using(Font arialFont = new Font("Arial", 15, System.Drawing.FontStyle.Bold))
        {
          string[] outQuestions = questions.Select(p => p.Question.QuestionText).ToArray();

          for(int i = 0; i < outQuestions.Count(); i++)
          {
            graphics.DrawString(outQuestions[i], arialFont, Brushes.Black, imageLocs[i].X, imageLocs[i].Y);
          }
        }
      }

      try
      {
        outBmp.Save(fileDialog.FileName);
      }
      catch(AccessViolationException)
      {
        MessageBox.Show("You are not permitted to write files at that location", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }
      catch(Exception)
      {
        MessageBox.Show("An error occured, the file may not have been saved.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    public static void DownloadBonusRoundImage(BonusRound bonusRound)
    {
      if(bonusRound != null)
      {
        DownloadBonusRoundImage(bonusRound.BonusQuestions);
      }
    }
  }
}
