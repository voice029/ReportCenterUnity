using System;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Common.Tree;
using JetBrains.Annotations;
using ReportCenterSystem;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

public abstract class ReportEntryBase
{
#region Public Members

  /// <summary>
  /// Used to Filter particular user roles
  /// </summary>
  public List<string> UserRole;
  
  /// <summary>
  /// Use to put an Report Item under a particular category
  /// </summary>
  public List<string> Category;
  
  /// <summary>
  /// Used to group related Item Reports Together
  /// </summary>
  public Object Group;
  
  /// <summary>
  /// Will use a look up table to determine additional visual elements such as error or info symbols.
  /// </summary>
  public object ObjType;

  private string _message;
  /// <summary>
  /// Primary message to display to the user.
  /// </summary>
  public string Message
  {
    get => _message;
    set
    {
      MessageHash = value.GetHashCode();
      _message = value;
    }
  }

  public int MessageHash
  {
    get;
    private set;
  }

  /// <summary>
  /// Sort Order
  /// </summary>
  public int SortOrder;
  
  /// <summary>
  /// Message to display when mouse hovering.
  /// </summary>
  public string Tooltip;

  /// <summary>
  /// Search keys will search using these set of objects also.
  /// </summary>
  public List<Object> SearchKeys;
  
  /// <summary>
  /// When the user copies the message, this information will be copied too though it is not displayed to the user.
  /// </summary>
  public string CopyDetails;
  
  public string Id;

  /// <summary>
  /// Adds an action that can be invoked by the user.
  /// </summary>
  /// <param name="name">The name/display message of the action.</param>
  /// <param name="action">the callback for when the action is invoked.</param>
  public void Add(string name, ReportAction action) {
    Actions.IncrementAdd(new ReportActionEntry {
      Name = name,
      TriggerAction = action
    });
  }

  /// <summary>
  /// Adds an action that can be invoked by the user.
  /// </summary>
  /// <param name="name">The name/display message for the action</param>
  /// <param name="tooltip">The information to show when mouse is hovering</param>
  /// <param name="action">the callback for when the action is invoked.</param>
  public void Add(string name, string tooltip, ReportAction action) {
    Actions.IncrementAdd(new ReportActionEntry {
      Name = name,
      Tooltip = tooltip,
      TriggerAction = action
    });
  }

  /// <summary>
  /// Callbacks meant for the user to invoke
  /// </summary>
  public ReportActionEntry[] Actions;
  
  /// <summary>
  /// Used to determine the behavior of the Report Item.  Such as being updated or removed from the list.
  /// </summary>
  public UpdateStatus Update;

  /// <summary>
  /// Return is a method that us used to add a report to a collection. 
  /// </summary>
  /// <param name="userData">Default is null.  This provides a </param>
  public abstract void Report(object userData = null);
  
  
  public static Object DefaultGroup = new();
  
  
  static ReportEntryBase()
  {
    // ReportEntryPublisher.Instance?.OnInitialize();
  }
  
  #endregion

  public bool MatchAnyProp(string uniSearchValue)
  {
    return UserRole?.Contains(uniSearchValue) == true
           || Category?.Contains(uniSearchValue) == true
           || Group?.ToString().Contains(uniSearchValue) == true
           || Message?.Contains(uniSearchValue) == true
           || Tooltip?.Contains(uniSearchValue) == true
           || SearchKeys?.Any(x => x.ToString().Contains(uniSearchValue)) == true
           || Actions?.Any(x => x.Name?.Contains(uniSearchValue) == true || x.Tooltip?.Contains(uniSearchValue) == true) == true;
  }
}