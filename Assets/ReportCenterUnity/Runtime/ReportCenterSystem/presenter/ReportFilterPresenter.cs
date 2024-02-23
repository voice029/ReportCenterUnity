using System;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;

namespace ReportCenterSystem
{
    public class ReportFilterPresenter
    {
        public ReportFilterOptions FilterOptions { get; set; }
        
        // TODO added SelectionMethods like "Add Roles" 
        // TODO Maybe helpers that decouples ReportFilterPresenter from ReportEntriesPresenter
        
        public void GenFromFilterOptions(ReportFilterOptions options, bool clearPrevious = true)
        {
            if (clearPrevious)
            {
                ClearSelectables();
            }
            _selectableRoles.AddRange(options.GeneratedRoles);
            _selectableCategories.AddRange(options.GeneratedCategories);
            _selectableSearchables.AddRange(options.AggregateSearchables.ToArray());
            _selectableContacts.AddRange( options.GeneratedContacts);
        }

        private void ClearSelectables()
        {
            _selectableRoles.Clear();
            _selectableCategories.Clear();
            _selectableSearchables.Clear();
            _selectableContacts.Clear();
        }

        public string SelectedRole
        {
            get => _selectedRoles[0];
            set => _selectedRoles[0] = value;
        }

        public string SelectedCategory
        {
            get => _selectedCategories[0];
            set => _selectedRoles[0] = value;
        }

        public string SelectedSearchable
        {
            get => _selectedSearchables[0];
            set => _selectedSearchables[0] = value;
        }

        public string SelectedContact
        {
            get => _selectedContacts[0];
            set => _selectedContacts[0] = value;
        }

        private List<string> _selectedRoles = new(new []{NULL_STRING});
        public IReadOnlyList<string> SelectedRoles => _selectedRoles;
        private List<string> _selectableRoles = new();
        public IReadOnlyList<string> SelectableRoles => _selectableRoles;

        private List<string> _selectedCategories = new( new []{NULL_STRING});
        public IReadOnlyList<string> SelectedCategories => _selectedCategories;
        private List<string> _selectableCategories = new();
        public IReadOnlyList<string> SelectableCategories => _selectableCategories;

        private List<string> _selectedSearchables = new( new []{NULL_STRING});
        public IReadOnlyList<string> SelectedSearchables => _selectedSearchables;
        private List<object> _selectableSearchables = new();
        public IReadOnlyList<object> SelectableSearchables => _selectableSearchables;

        private List<string> _selectedContacts = new( new []{NULL_STRING});
        public IReadOnlyList<string> SelectedContacts => _selectedContacts;
        private List<string> _selectableContacts = new();
        public IReadOnlyList<object> SelectableContacts => _selectableContacts;

        
        public int RoleIndexPosition => SelectedRole.IndexOf(SelectedRole, StringComparison.Ordinal);
        public int CategoryIndexPosition => SelectedRole.IndexOf(SelectedCategory, StringComparison.Ordinal);
        public int SearchableIndexPosition => SelectedRole.IndexOf(SelectedSearchable, StringComparison.Ordinal);
        public int ContactIndexPosition => SelectedRole.IndexOf(SelectedContact, StringComparison.Ordinal);

        public IReadOnlyList<int> RoleIndicesPositions => GenMatchingIndices(_selectedRoles);
        
        public IReadOnlyList<int> CategoryIndicesPositions => GenMatchingIndices(_selectableCategories);

        public IReadOnlyList<int> SearchableIndicesPositions => GenMatchingIndices(_selectedSearchables);

        public IReadOnlyList<int> ContactIndicesPositions => GenMatchingIndices(_selectedContacts);

        private const string NULL_STRING = null;

        private static List<int> GenMatchingIndices(List<string> list, List<int> indices = null)
        {
            indices ??= new List<int>();
            foreach (string selectedRole in list)
            {
                int pos = list.IndexOf(selectedRole);
                if (pos >= 0)
                {
                    indices.Add(pos);
                }
            }

            return indices;
        }
    }
}