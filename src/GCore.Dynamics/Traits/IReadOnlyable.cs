namespace GCore.Dynamics.Traits;

public interface IReadOnlyable
{
    bool IsReadOnly { get; }
    void SetReadOnly(bool onlyread, bool recursive = true);
}