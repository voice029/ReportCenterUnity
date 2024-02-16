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
  private const string SHOW_ALL = "[All]";
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
    
    var randRole = new TextField("Role");
    root.Add( randRole);

    var randCat = new TextField("Cate");
    root.Add(randCat);

    var randSearch = new TextField("Searchable");
    root.Add(randSearch);

    var randGroup = new TextField("Group");
    root.Add(randGroup);

    var randMessage = new TextField("Message");
    root.Add(randMessage);
    
    root.Add(new Button(() => {
      new ReportEntry()
      {
        UserRole = new[] {randRole.value}.ToList(),
        Category = new[] {randCat.value}.ToList(),
        SearchKeys = new object[] {randSearch.value}.ToList(),
        Type = ReportType.Validation,
        Group = randGroup.value,
        Message = randMessage.value,
      }.Report();
      // new ReportEntry
      // {
      //   Category = new[] { (new Random().Next() % 2).ToString() }.ToList(),
      //   UserRole = new[] { (new Random().Next() % 4).ToString() }.ToList(),
      //   SearchKeys = new object[] {(new Random().Next() % 100)}.ToList(),
      //   Type = ReportType.Validation,
      //   Group = (new Random().Next() % 3).ToString(),
      //   Message = (new Random().Next()).ToString()
      // }.Report();
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

  private async void GroupedUpdate()
  {
    
    var searchBar = rootVisualElement.Q<ObjectField>("objectSearch");
    var searchValue = searchBar.value;

    var uniSearchBar = rootVisualElement.Q<ToolbarPopupSearchField>("search");
    var uniSearchValue = uniSearchBar.value;

    var roleSearch = m_CurrentRole;
    var cateSearch = m_CurrentCategory;
    
    bool FilterObjects(ReportEntryBase entry, int index)
    {

      
      bool filterRet = true;
      
      if (searchValue is not null)
      {
        filterRet &= entry.SearchKeys.Contains(searchValue);
      }

      if (!String.IsNullOrEmpty(uniSearchValue))
      {
        filterRet &= entry.MatchAnyProp(uniSearchValue);
      }

      if (!String.IsNullOrEmpty(roleSearch) && roleSearch != SHOW_ALL)
      {
        filterRet &= entry.UserRole.Contains(roleSearch);
      }
      
      if (!String.IsNullOrEmpty(cateSearch) && cateSearch != SHOW_ALL)
      {
        filterRet &= entry.Category.Contains(cateSearch);
      }
      
      return filterRet;
    }

    ReportingModifiers reportMod = ReportingModifiers.DefaultDup();
    reportMod.EntriesFilter = FilterObjects;

    var groupedEntries = await _reportPresenter.GenGroupReportsAysnc(reportMod);
    // var groupedEntries = _reportPresenter.GenGroupedReports(reportMod);

    if (m_RoleDropdown is not null)
    {
      var roles = _reportPresenter.Roles; // m_Roles.ToList();
      var dropDownList = m_RoleDropdown.choices;
      dropDownList.Clear();
      dropDownList.Add(SHOW_ALL);
      bool found = false;
      foreach (var role in roles)
      {
        if (m_CurrentRole?.Equals(role) == true)
        {
          m_RoleDropdown.index = dropDownList.Count;
          found = true;
        }
        dropDownList.Add(role);
      }

      if (!found)
      {
        m_RoleDropdown.index = 0;
      }
    }

    if (m_CategoryDropdown is not null)
    {
      var categories = _reportPresenter.Categories; // m_Categories.ToList();
      var categoryDropdown= m_CategoryDropdown.choices;
      categoryDropdown.Clear();
      categoryDropdown.Add(SHOW_ALL);
      bool found = false;
      foreach (var category in categories)
      {
        if (m_CurrentCategory?.Equals(category) == true)
        {
          found = true;
          m_CategoryDropdown.index = categoryDropdown.Count;
        }
        categoryDropdown.Add(category);
      }
      if (!found)
      {
        m_CategoryDropdown.index = 0;
      }
    }
    
    m_reportListView.itemsSource = groupedEntries.ToList();
  }

  #endregion
}
