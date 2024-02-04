namespace ReportCenterSystem.interfaces
{
    public abstract class ReportEntryBaseTyped<T> : ReportEntryBase
    {
        public T Type
        {
            get => (T)ObjType;
            set => ObjType = value;
        }
    }
}