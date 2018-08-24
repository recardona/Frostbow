using BoltFreezer.Enums;

namespace BoltFreezer.Interfaces
{
    public interface ISelection
    {
        SelectionType EType { get; }

        float Evaluate(IPlan plan);

        string ToString();
    }
}
