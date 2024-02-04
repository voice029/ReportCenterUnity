using ReportCenterSystem.interfaces;

namespace ReportCenterSystem
{
    public class ReportEntryTyped<T> : ReportEntryBaseTyped<T>
    {
        public override void Report(object userData = null)
        {
            ReportEntries.Instance.Add(this);
        }
    }
}