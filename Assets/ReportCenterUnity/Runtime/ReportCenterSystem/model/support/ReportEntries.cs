using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportCenterSystem
{
    public class ReportEntries : IReportEntries
    {
        public delegate void Added(ReportEntryBase reportEntry);
        public Added OnAdded;
        
        /// <summary>
        /// Get a Enumerator for all the ReportItems
        /// </summary>
        public IEnumerable<ReportEntryBase> Reports => _reportEntries;

        public static ReportEntries Instance { get; } = new ReportEntries();
        
        public void AssignReportEnumerator(IEnumerable<ReportEntryBase> newReportItemEnumerator)
        {
            _reportEntries = newReportItemEnumerator;
        }

        public void Add(ReportEntryBase entry)
        {
            if (_reportEntries is ICollection<ReportEntryBase> collection)
            {
                collection.Add(entry);
                ReportEntryPublisher.Instance.OnAddedTo(this, entry);
                OnAdded?.Invoke(entry);
                return;
            }
            throw new Exception($"Cannot Add a ReportEntry with current Enumerable {_reportEntries.GetType()}");
        }
        
        private IEnumerable<ReportEntryBase> _reportEntries = new List<ReportEntryBase>();
    }
}