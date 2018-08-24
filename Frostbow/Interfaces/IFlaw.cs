using BoltFreezer.Enums;

namespace BoltFreezer.Interfaces
{
    public interface IFlaw
    {
        // Read Only
        FlawType Ftype { get; }
    }
}
