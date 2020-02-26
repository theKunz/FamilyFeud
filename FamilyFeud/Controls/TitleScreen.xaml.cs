using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for TitleScreen.xaml
  /// </summary>
  public partial class TitleScreen : UserControl
  {
    private const int TransformDistance = 4280;

    public TitleScreen()
    {
      InitializeComponent();

      TopStackPanel.CacheMode = new BitmapCache() { EnableClearType = false, RenderAtScale = 1, SnapsToDevicePixels = false };
      BottomStackPanel.CacheMode = new BitmapCache() { EnableClearType = false, RenderAtScale = 1, SnapsToDevicePixels = false };

      Loaded += StartAnimationOnLoad;
    }

    public void StartAnimationOnLoad(object obj, RoutedEventArgs args)
    {
      Loaded -= StartAnimationOnLoad;

      (ContainerGrid.Resources["textMoveStoryboard"] as Storyboard).Begin();
    }
  }
}
