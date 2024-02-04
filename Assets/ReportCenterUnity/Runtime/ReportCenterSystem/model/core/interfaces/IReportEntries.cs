using System.Collections.Generic;

public interface IReportEntries
{
    public void Add(ReportEntryBase entry);

    public void AssignReportEnumerator(IEnumerable<ReportEntryBase> newReportItemEnumerator);

    public IEnumerable<ReportEntryBase> Reports { get; }
}