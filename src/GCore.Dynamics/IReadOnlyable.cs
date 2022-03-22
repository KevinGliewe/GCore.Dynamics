namespace GCore.Dynamics;

public interface IReadOnlyable
{
    bool IsReadOnly { get; }
    void SetReadOnly(bool onlyread, bool recursive = true);
}