using System;
using System.Collections.Generic;
using System.Linq;
using ReportCenterUnity.Runtime.ReportCenterSystem.model.core;

namespace ReportCenterSystem
{
    public class ReportEntriesPresenter
    {
        private IReportEntries _entries;
        private IEnumerable<string> _roles = Array.Empty<string>();
        private IEnumerable<string> _categories = Array.Empty<string>();
        private List<string> _generatedRoles = new();
        private List<string> _generatedCategories = new();
        private List<object> _aggregateSearchables = new();

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

        public IEnumerable<IGrouping<object, ReportEntryBase>> GenGroupedReports(ReportingModifiers reportingModifiers)
        {
            // TODO extract Roles and Categories before filtering
            // TODO use MessageHash or Id to not show duplicates
            
            
            
            EntriesFiltererFunc filtererFunc = reportingModifiers.EntriesFilter;
            var groupedEntries = RawList?
                .Distinct(  )
                .Where(filtererFunc.Invoke)
                .OrderBy(x => x, reportingModifiers.ReportEntryComparer)
                .OrderBy(x => x.ObjType, reportingModifiers.TypeComparer)
                .GroupBy(x => x.Group)
                .OrderBy(x => x.Key);
            
            _generatedRoles.Clear();
            _generatedCategories.Clear();
            _aggregateSearchables.Clear();

            if (groupedEntries != null)
            {
                foreach (var group in groupedEntries)
                {
                    foreach (ReportEntryBase entry in group)
                    {
                        foreach (var role in entry.UserRole)
                        {
                            _generatedRoles.Add(role);
                        }

                        foreach (var category in entry.Category)
                        {
                            _generatedCategories.Add(category);
                        }

                        foreach (var searchable in entry.SearchKeys)
                        {
                            _aggregateSearchables.Add(searchable);
                        }
                    }
                }
            }

            _generatedRoles = _generatedRoles.Distinct().ToList();
            _generatedCategories = _generatedCategories.Distinct().ToList();
            _aggregateSearchables = _aggregateSearchables.Distinct().ToList();
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

        public virtual List<string> GeneratedRoles => _generatedRoles;

        public virtual List<string> GeneratedCategories => _generatedCategories;

        public virtual List<object> GeneratedAggregateSearchables => _aggregateSearchables;
        
        public List<string> Roles => StaticRoles.Concat(GeneratedRoles).ToList();

        public List<string> Categories => StaticCategories.Concat(GeneratedCategories).ToList();
        
    }
}