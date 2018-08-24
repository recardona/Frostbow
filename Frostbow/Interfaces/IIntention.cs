﻿using System;

namespace BoltFreezer.Interfaces
{
    public interface IIntention
    {
        // Intentions are associated with characters.
        string Character { get; set; }

        // Intentions are specified with predicates.
        IPredicate Predicate { get; set; }

        // Intentions can be cloned.
        Object Clone();
    }
}
