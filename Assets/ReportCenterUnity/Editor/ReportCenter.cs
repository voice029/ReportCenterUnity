using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportCenterSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public class ReportCenter : EditorWindow
{
  private ReportEntriesPresenter _reportPresenter = new(ReportEntries.Instance);
#region Public Members

  [MenuItem("Window/ReportCenter")]
  public static void ShowExample()
  {
      ReportCenter wnd = GetWindow<ReportCenter>();
      wnd.titleContent = new GUIContent("Report Center");
  }
  
#endregion

#region Private Members

  [SerializeField]
  private VisualTreeAsset m_VisualTreeAsset = default;

  [SerializeField]
  private VisualTreeAsset m_ReportItem;
  private ListView m_reportListView;
  
  [SerializeField]
  private VisualTreeAsset m_GroupReportItem;
  
  [SerializeField]
  private DropdownField m_RoleDropdown;

  [SerializeField]
  private DropdownField m_CategoryDropdown;

  
  private VisualElement MakeReportItem() {
    return m_ReportItem.Instantiate();
  }
  
  public void CreateGUI()
  {
    // Each editor window contains a root VisualElement object
    VisualElement root = rootVisualElement;
    root.Add(new Button(() => {
      new ReportEntry
      {
        Category = new[] { (new Random().Next() % 2).ToString() }.ToList(),
        UserRole = new[] { (new Random().Next() % 4).ToString() }.ToList(),
        SearchKeys = new object[] {(new Random().Next() % 100)}.ToList(),
        Type = ReportType.Validation,
        Group = (new Random().Next() % 3).ToString(),
        Message = (new Random().Next()).ToString()
      }.Report();
    }) {
      text = "Add"
    });
    
    // Instantiate UXML
    VisualElement fromUxml = m_VisualTreeAsset.Instantiate();
    root.Add(fromUxml);

    m_RoleDropdown = fromUxml.Q<DropdownField>("role");
    m_CategoryDropdown = fromUxml.Q<DropdownField>("category");
    
    m_reportListView = fromUxml.Q<ListView>("reportList");
    m_reportListView.makeItem = () => m_GroupReportItem.Instantiate();
    m_reportListView.bindItem = GroupedReportItemBinding;
  }

  private void GroupedReportItemBinding(VisualElement element, int i)
  {
    var reportData = new ReportData();
    IGrouping<object, ReportEntryBase> grouping = (IGrouping<object, ReportEntryBase>)m_reportListView.itemsSource[i];

    element.Q<Label>("groupName").text = grouping.Key?.ToString() ?? "";
    
    var listView = element.Q<VisualElement>("groupedReportList");
    listView.Clear();
    var listViewItemsSource = grouping.ToList();

    foreach (var report in listViewItemsSource)
    {
      var reportUi = m_ReportItem.Instantiate();
      reportUi.Q<Label>("message").text = report.Message;
      VisualElement actionArea = element.Q<VisualElement>("actions");
      
      foreach (ReportActionEntry action in report.Actions ?? Array.Empty<ReportActionEntry>()) {
        actionArea.Add(new Button(() => { action.TriggerAction(reportData); }) { text = action.Name });
      }
      listView.Add(reportUi);
    }

    HashSet<string> displaySearchable = new();
    foreach (object obj in _reportPresenter.GeneratedAggregateSearchables)
    {
      displaySearchable.Add(obj.GetType().Name);
      displaySearchable.Add(obj.ToString());
    }
    
    StringBuilder builder = new();
    foreach (object displayItem in displaySearchable)
    {
      builder.Append(displayItem);
      builder.Append(", ");
    }

    element.Q<Label>("keySearch").text = builder.ToString();

    m_RoleDropdown = element.Q<DropdownField>("role");
    m_RoleDropdown.RegisterValueChangedCallback((aEvent) =>
    {
      m_CurrentRole = aEvent.newValue;
    });
    
    m_CategoryDropdown = element.Q<DropdownField>("category");
    m_CategoryDropdown.RegisterValueChangedCallback(aEvent =>
    {
      m_CurrentCategory = aEvent.newValue;
    });
  }
  
  [SerializeField]
  private string m_CurrentRole;
  
  [SerializeField]
  private string m_CurrentCategory;

  private void Update()
  {
    GroupedUpdate();
    
    m_reportListView.RefreshItems();
  }

  private void GroupedUpdate()
  {
    
    m_RoleDropdown = rootVisualElement.Q<DropdownField>("role");
    m_RoleDropdown.RegisterValueChangedCallback((aEvent) =>
    {
      m_CurrentRole = aEvent.newValue;
    });
    
    
    m_CategoryDropdown = rootVisualElement.Q<DropdownField>("category");
    m_CategoryDropdown.RegisterValueChangedCallback(aEvent =>
    {
      m_CurrentCategory = aEvent.newValue;
    });

    var searchBar = rootVisualElement.Q<ObjectField>("objectSearch");
    var searchValue = searchBar.value;

    var uniSearchBar = rootVisualElement.Q<ToolbarPopupSearchField>("search");
    var uniSearchValue = uniSearchBar.value;

    var roleSearch = m_CurrentRole;
    var cateSearch = m_CurrentCategory;
    
    bool initFilterRet = searchValue is null
                         && String.IsNullOrEmpty(uniSearchValue)
                         && String.IsNullOrEmpty(m_CurrentRole)
                         && String.IsNullOrEmpty(m_CurrentCategory);
    
    bool FilterObjects(ReportEntryBase entry, int index)
    {
      if (initFilterRet)
        return true;
      
      bool filterRet = false;
      
      if (searchValue is not null)
      {
        filterRet |= entry.SearchKeys.Contains(searchValue);
      }

      if (!String.IsNullOrEmpty(uniSearchValue))
      {
        filterRet |= entry.MatchAnyProp(uniSearchValue);
      }

      if (!String.IsNullOrEmpty(roleSearch))
      {
        filterRet |= entry.Category.Contains(roleSearch);
      }
      
      if (!String.IsNullOrEmpty(cateSearch))
      {
        filterRet |= entry.Category.Contains(cateSearch);
      }
      
      return filterRet;
    }

    var reportMod = ReportingModifiers.Default();
    reportMod.EntriesFilter = FilterObjects;
    
    var groupedEntries = _reportPresenter.GenGroupedReports(reportMod);

    if (m_RoleDropdown is not null)
    {
      List<string> roles = _reportPresenter.Roles; // m_Roles.ToList();
      int currentIndex = roles.FindIndex(x => x.Equals(m_CurrentRole));
      m_RoleDropdown.choices = roles;
      if (currentIndex > -1)
      {
        m_RoleDropdown.index = currentIndex;
      }
    }

    if (m_CategoryDropdown is not null)
    {
      List<string> categories = _reportPresenter.Categories; // m_Categories.ToList();
      int currentIndex = categories.FindIndex(x => x.Equals(m_CurrentRole));
      m_CategoryDropdown.choices = categories;
      if (currentIndex > -1)
      {
        m_CategoryDropdown.index = currentIndex;
      }
    }
    
    m_reportListView.itemsSource = groupedEntries.ToList();
  }

  #endregion
}
