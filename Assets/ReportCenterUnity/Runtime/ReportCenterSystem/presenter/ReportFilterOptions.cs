using System.Collections.Generic;

namespace ReportCenterSystem
{
    public struct ReportFilterOptions
    {
        public HashSet<string> GeneratedRoles;
        public HashSet<string> GeneratedCategories;
        public HashSet<object> AggregateSearchables;
        public HashSet<string> GeneratedContacts;
    }
}