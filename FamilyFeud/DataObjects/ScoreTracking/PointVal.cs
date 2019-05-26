using System.ComponentModel;

namespace FamilyFeud.DataObjects
{
  public class PointVal : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private int? mVal;

    public int? PointValue
    {
      get
      {
        return mVal;
      }
      set
      {
        mVal = value == null ? 0 : value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointValue"));
      }
    }
  }
}
