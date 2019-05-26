using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFeud.CustomEventArgs
{
  public class ArithmeticEventArgs : EventArgs
  {
    private int mDelta;
    private Operand mOperation;

    public enum Operand
    {
      Add,
      Subtract,
      Multiply,
      Divide,
      Modulus,
      BitwiseLeft,
      BitwiseRight
    }

    public ArithmeticEventArgs(int delta, Operand operation)
    {
      mOperation = operation;
      mDelta = delta;
    }

    public Operand Operation
    {
      get
      {
        return mOperation;
      }
    }

    public int Delta
    {
      get
      {
        return mDelta;
      }
    }
  }
}
