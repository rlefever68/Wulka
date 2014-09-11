namespace Wulka.Interfaces
{
    public interface IDirty
    {
        bool IsDirty { get; set; }
        void ClearIsDirty();
        void SetIsDirty();
    }
}
