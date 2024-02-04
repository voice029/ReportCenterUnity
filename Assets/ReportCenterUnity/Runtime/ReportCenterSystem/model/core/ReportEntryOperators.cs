using System;
using System.Collections;
using System.Collections.Generic;

namespace ReportCenterUnity.Runtime.ReportCenterSystem.model.core
{
    public delegate bool EntriesFiltererFunc(ReportEntryBase entry, int index);

    public delegate int TypeSorterFunc(object obj1, object obj2);
        
    public delegate int ReportEntrySorterFunc(ReportEntryBase obj1, ReportEntryBase obj2);

    public delegate bool DistinctEntryComparerFunc(ReportEntryBase objA, ReportEntryBase objB);
        
    public delegate int DistinctEntryHashFunc(ReportEntryBase obj);
    
    public static class ReportEntryOperators
    {
        public static int StandardReportEntryComparer(ReportEntryBase a, ReportEntryBase b)
        {
            int compare = a.SortOrder.CompareTo(b.SortOrder);
            if (compare == 0)
            {
                compare = String.Compare(a.ObjType?.ToString(), b.ObjType?.ToString(), StringComparison.Ordinal);
            }

            if (compare == 0)
            {
                compare = String.Compare(a.Message, b.Message, StringComparison.Ordinal);
            }

            return compare;
        }

        public static bool StandardDuplicationIdEquals(ReportEntryBase x, ReportEntryBase y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            // previous condition helps the "either or" style null check
            if (x == null || y == null)
            {
                return false;
            }

            return !String.IsNullOrWhiteSpace(x.Message)
                   && !String.IsNullOrWhiteSpace(y.Message)
                   && x.MessageHash == y.MessageHash;
        }

        public static int StandardTypeCompare(object obj1, object obj2)
        {
            int hashCode1 = obj1.GetHashCode();
            int hashCode2 = obj2.GetHashCode();
            return hashCode1 == hashCode2 ? 0 :
                hashCode1 < hashCode2 ? -1 :
                1;
        }

        public static int StandardTypeCompareHash(ReportEntryBase obj) => obj.MessageHash;
        
        private class CustomEqualityComparer : IEqualityComparer<ReportEntryBase>
        {
            private DistinctEntryComparerFunc _entryComparer;
            private DistinctEntryHashFunc _entryHash;

            public CustomEqualityComparer(DistinctEntryComparerFunc entryComparer, DistinctEntryHashFunc entryHash)
            {
                _entryComparer = entryComparer;
                _entryHash = entryHash;
            }

            public bool Equals(ReportEntryBase x, ReportEntryBase y)
            {
                return _entryComparer(x, y);
            }

            public int GetHashCode(ReportEntryBase obj)
            {
                return _entryHash(obj);
            }
        }

        public static IEqualityComparer<ReportEntryBase> CreateDistinctEqualityCompare(DistinctEntryComparerFunc entryComparer, DistinctEntryHashFunc entryHash)
        {
            return new CustomEqualityComparer(entryComparer, entryHash);
        }

        private class ReportEntryBaseComparer : IComparer<ReportEntryBase>
        {
            public ReportEntrySorterFunc reportEntrySorter;
            public int Compare(ReportEntryBase x, ReportEntryBase y)
            {
                return reportEntrySorter(x, y);
            }
        }
        
        public static IComparer<ReportEntryBase> CreateReportEntrySorter(ReportEntrySorterFunc reportEntrySorter)
        {
            return new ReportEntryBaseComparer
            {
                reportEntrySorter = reportEntrySorter
            };
        }

        public static IComparer<object> CreateTypeComparer(TypeSorterFunc typeComparer)
        {
            return Comparer<object>.Create(typeComparer.Invoke);
        }
    }
}