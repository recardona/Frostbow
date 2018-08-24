using System;
using System.Collections.Generic;

namespace BoltFreezer.Interfaces
{
    public interface IObject
    {
        // Objects have a name.
        string Name { get; set; }

        // Objects have a sub-type.
        string SubType { get; set; }

        // Objects have a type.
        List<string> Types { get; set; }

        // Objects can be cloned.
        Object Clone();
    }
}
