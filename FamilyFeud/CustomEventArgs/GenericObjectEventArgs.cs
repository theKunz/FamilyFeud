using System;


namespace FamilyFeud.CustomEventArgs
{
  public class EventArgs<T> : EventArgs
  {
    public EventArgs(T input)
    {
      Obj = input;
    }

    public T Obj { get; }

  }
}
