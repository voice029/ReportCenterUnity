using System;
using System.Collections.Generic;
using ReportCenterUnity.Runtime.ReportCenterSystem.model.core;

//TODO C# standard for naming private static fields and private fields
namespace ReportCenterSystem
{
    public class ReportingModifiers : ICloneable
    {
        #region Public Members
        public EntriesFiltererFunc EntriesFilter { get; set; }

        private ReportEntrySorterFunc _entriesSorter;
        public ReportEntrySorterFunc EntriesSorter
        {
            get => _entriesSorter;
            set
            {
                _entriesSorter = value;
                ReplaceReportEntryComparer(_entriesSorter);
            }
        }
        
        public IComparer<ReportEntryBase> ReportEntryComparer
        {
            get;
            
            // private because we want to updated the comparer
            private set;
        }

        private TypeSorterFunc _typeSorter;
        public TypeSorterFunc TypeSorter
        {
            get => _typeSorter;
            set
            {
                _typeSorter = value;
                ReplaceTypeComparer(_typeSorter);
            }
        }

        public IComparer<object> TypeComparer
        {
            get;
            
            // private because we want to updated the comparer
            private set;
        }

        private DistinctEntryHashFunc _distinctEntryHasher;
        public DistinctEntryHashFunc DistinctEntryHasher
        {
            get => _distinctEntryHasher;
            set
            {
                _distinctEntryHasher = value;
                ReplaceDistinctReportEqualityComparer(_distinctEntryEquals, _distinctEntryHasher);
            }
        }
        
        private DistinctEntryComparerFunc _distinctEntryEquals;
        public DistinctEntryComparerFunc DistinctEntryEquals
        {
            get => _distinctEntryEquals;
            set
            {
                _distinctEntryEquals = value;
                ReplaceDistinctReportEqualityComparer(_distinctEntryEquals, _distinctEntryHasher);
            }
        }

        public IEqualityComparer<ReportEntryBase> DistinctReportEntryBaseEqualityComparer
        {
            get;
            
            // private because we want to updated the comparer
            private set;
        }
        
        public static ReportingModifiers DefaultDup()
        {
            return (ReportingModifiers)_sStandardModifier.Clone();
        }
        
        public object Clone()
        {
            ReportingModifiers clone = new ReportingModifiers();
            clone.EntriesFilter = EntriesFilter;
            clone.EntriesSorter = EntriesSorter;
            clone.ReportEntryComparer = ReportEntryComparer;
            clone.TypeSorter = TypeSorter;
            clone.TypeComparer = TypeComparer;
            
            clone.DistinctEntryEquals = DistinctEntryEquals;
            clone.DistinctEntryHasher = DistinctEntryHasher;
            clone.DistinctEntryEquals = DistinctEntryEquals;
            return clone;
        }
        
        #endregion

        #region Private Members

        private static readonly ReportingModifiers _sStandardModifier;

        static ReportingModifiers()
        {
            _sStandardModifier = new ReportingModifiers();
            _sStandardModifier.EntriesFilter = (_, _) => true;
            _sStandardModifier.EntriesSorter = ReportEntryOperators.StandardReportEntryComparer;
            _sStandardModifier.ReportEntryComparer =
                ReportEntryOperators.CreateReportEntrySorter(_sStandardModifier.EntriesSorter);
            _sStandardModifier.TypeSorter = ReportEntryOperators.StandardTypeCompare;
            _sStandardModifier.TypeComparer = ReportEntryOperators.CreateTypeComparer(_sStandardModifier.TypeSorter);
            _sStandardModifier.DistinctEntryEquals = ReportEntryOperators.StandardDuplicationIdEquals;
            _sStandardModifier.DistinctEntryHasher = ReportEntryOperators.StandardTypeCompareHash;
            _sStandardModifier.DistinctReportEntryBaseEqualityComparer = 
                ReportEntryOperators.CreateDistinctEqualityCompare(_sStandardModifier.DistinctEntryEquals, _sStandardModifier.DistinctEntryHasher);
        }
        
        private void ReplaceDistinctReportEqualityComparer(DistinctEntryComparerFunc distinctEntryComparerFunc, DistinctEntryHashFunc distinctEntryHashFunc)
        {
            DistinctReportEntryBaseEqualityComparer =
                ReportEntryOperators.CreateDistinctEqualityCompare(distinctEntryComparerFunc, distinctEntryHashFunc);
        }
        
        private void ReplaceReportEntryComparer(ReportEntrySorterFunc reportEntrySorterFunc)
        {
            ReportEntryComparer = ReportEntryOperators.CreateReportEntrySorter(reportEntrySorterFunc);
        }
        
        private void ReplaceTypeComparer(TypeSorterFunc typeSorterFunc)
        {
            TypeComparer = ReportEntryOperators.CreateTypeComparer(typeSorterFunc);
        }
        
        #endregion
    }
}