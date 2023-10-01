namespace Dsmviz.Datamodel.Dsm.Interfaces
{
    public interface ISortResult
    {
        int GetIndex(int currentIndex);
        int GetNumberOfElements();
        bool IsValid { get; }
    }
}
