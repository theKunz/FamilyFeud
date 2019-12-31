using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFeud.Controls
{
  public interface IRoundControl
  {
    bool NextEnabled { get; set; }
    bool PreviousEnabled { get; set; }

    event EventHandler NextClickEvent;
    event EventHandler PreviousClickEvent;

    void RevealAnswer(int answerIndex);
  }
}
