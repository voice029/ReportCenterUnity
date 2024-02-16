using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReportCenterUnity.Runtime.ReportCenterSystem.model.core;

namespace ReportCenterSystem
{
    public class ReportEntriesPresenter
    {
        private IReportEntries _entries;
        private IEnumerable<string> _roles = Array.Empty<string>();
        private IEnumerable<string> _categories = Array.Empty<string>();
        private HashSet<string> _generatedRoles = new();
        private HashSet<string> _generatedCategories = new();
        private HashSet<object> _aggregateSearchables = new();

        public ReportEntriesPresenter() { }

        public ReportEntriesPresenter(IReportEntries entries)
        {
            SetReportEntries(entries);
        }

        public void SetReportEntries(IReportEntries entries)
        {
            _entries = entries;
        }

        public List<ReportEntryBase> RawList => RawEnumerable.ToList();

        public IEnumerable<ReportEntryBase> RawEnumerable => _entries.Reports;

        public async Task<IEnumerable<IGrouping<object, ReportEntryBase>>> GenGroupReportsAysnc(
            ReportingModifiers reportingModifiers)
        {
            return await Task.FromResult(GenGroupedReports(reportingModifiers));
        }
        
        public IEnumerable<IGrouping<object, ReportEntryBase>> GenGroupedReports(ReportingModifiers reportingModifiers)
        {
            
            _generatedRoles.Clear();
            _generatedCategories.Clear();
            _aggregateSearchables.Clear();
            
            var entriesFromRawList = RawList;
            foreach (var entry in entriesFromRawList)
            {
                foreach (var category in entry.Category)
                {
                    _generatedCategories.Add(category);
                }
                
                foreach (var role in entry.UserRole)
                {
                    _generatedRoles.Add(role);
                }
                
                foreach (var searchable in entry.SearchKeys)
                {
                    _aggregateSearchables.Add(searchable);
                }
            }
            
            EntriesFiltererFunc filtererFunc = reportingModifiers.EntriesFilter;
            var groupedEntries = entriesFromRawList
                .Distinct(reportingModifiers.DistinctReportEntryBaseEqualityComparer)
                .Where(filtererFunc.Invoke)
                .OrderBy(x => x, reportingModifiers.ReportEntryComparer)
                .OrderBy(x => x.ObjType, reportingModifiers.TypeComparer)
                .GroupBy(x => x.Group)
                .OrderBy(x => x.Key);
            
            return groupedEntries;
        }

        public virtual IEnumerable<string> StaticRoles
        {
            get => _roles;
            set => _roles = value;
        }

        public virtual IEnumerable<string> StaticCategories
        {
            get => _categories;
            set => _categories = value;
        }

        public virtual HashSet<string> GeneratedRoles => _generatedRoles;

        public virtual HashSet<string> GeneratedCategories => _generatedCategories;

        public virtual HashSet<object> GeneratedAggregateSearchables => _aggregateSearchables;
        
        public HashSet<string> Roles =>  StaticRoles.Concat(GeneratedRoles).ToHashSet();

        public HashSet<string> Categories => StaticCategories.Concat(GeneratedCategories).ToHashSet();
        
    }
}