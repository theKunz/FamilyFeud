using System;


namespace FamilyFeud.CustomEventArgs
{
  public class EventArgs<T> : EventArgs
  {
    private T privObj; 
    public EventArgs(T input)
    {
      privObj = input;
    }

    public T Obj => privObj;

  }
}
