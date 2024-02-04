
public struct ReportActionEntry {
  public string Name;
  public string Tooltip;
  public ReportAction TriggerAction;

  public ReportActionEntry(string name, ReportAction triggerAction) {
    Name = name;
    Tooltip = null;
    TriggerAction = triggerAction;
  }

  public ReportActionEntry(string name, string tooltip, ReportAction triggerAction) {
    Name = name;
    Tooltip = tooltip;
    TriggerAction = triggerAction;
  }
}

public enum ReportStatus {
  StillValid,
  NowInvalid
}

public struct ReportData {}
public delegate void ReportAction(ReportData actionData) ;
public delegate ReportStatus UpdateStatus(ReportData actionData);
