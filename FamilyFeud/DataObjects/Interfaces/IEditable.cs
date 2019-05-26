using System;

namespace FamilyFeud.DataObjects
{
  interface IEditable
  {
    Action<object> EditAction { get; set; }
  }
}
