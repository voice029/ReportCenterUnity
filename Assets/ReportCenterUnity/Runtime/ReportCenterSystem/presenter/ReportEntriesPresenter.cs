using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReportCenterUnity.Runtime.ReportCenterSystem.model.core;

namespace ReportCenterSystem
{
    public class ReportEntriesPresenter
    {
        
        private ReportFilterOptions _filterOptions = new ()
        {
            GeneratedRoles = new HashSet<string>(),
            GeneratedCategories = new HashSet<string>(),
            AggregateSearchables = new HashSet<object>()
        };
        
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
        
        public async Task<IEnumerable<IGrouping<object, ReportEntryBase>>> MakeGroupReportsAsync(ReportingModifiers reportingModifiers)
        {
            return await Task.Run( () => ProduceGroupedReports(RawList, reportingModifiers, ref _filterOptions));
        }
        
        public static IEnumerable<IGrouping<object, ReportEntryBase>> ProduceGroupedReports(
            List<ReportEntryBase> entriesFromRawList, ReportingModifiers reportingModifiers,
            ref ReportFilterOptions filterOptions)
        {
            var generatedRoles = filterOptions.GeneratedRoles;
            var generatedCategories = filterOptions.GeneratedCategories;
            var aggregateSearchables = filterOptions.AggregateSearchables;

            foreach (var entry in entriesFromRawList)
            {
                foreach (var category in entry.Category)
                {
                    generatedCategories.Add(category);
                }
                
                foreach (var role in entry.UserRole)
                {
                    generatedRoles.Add(role);
                }
                
                foreach (var searchable in entry.SearchKeys)
                {
                    aggregateSearchables.Add(searchable);
                }
            }
            
            EntriesFiltererFunc filtererFunc = reportingModifiers.EntriesFilter;
            var groupedEntries = entriesFromRawList
                .Distinct(reportingModifiers.DistinctReportEntryBaseEqualityComparer)
                .Where(filtererFunc.Invoke)
                .OrderBy(x => x, reportingModifiers.ReportEntryComparer)
                .ThenBy(x => x.ObjType, reportingModifiers.TypeComparer)
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

        public virtual HashSet<string> GeneratedRoles => _filterOptions.GeneratedRoles;

        public virtual HashSet<string> GeneratedCategories => _filterOptions.GeneratedCategories;

        public virtual HashSet<object> GeneratedAggregateSearchables => _filterOptions.AggregateSearchables;
        
        public HashSet<string> Roles =>  StaticRoles.Concat(GeneratedRoles).ToHashSet();

        public HashSet<string> Categories => StaticCategories.Concat(GeneratedCategories).ToHashSet();
        
        #region Private Members
        private IReportEntries _entries;
        private IEnumerable<string> _roles = Array.Empty<string>();
        private IEnumerable<string> _categories = Array.Empty<string>();
        #endregion
    }
}