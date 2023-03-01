using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class DeckType
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    private string _name = "";
    /// <summary>
    /// 
    /// </summary>
    public string Name
    {
        get { return _name; }
        set { _name = value.WsNormalized(); }
    }

    private string? _description;
    /// <summary>
    /// 
    /// </summary>
    public string? Description
    {
        get { return _description; }
        set { _description = value.TrimmedOrNullIfEmpty(); }
    }

    private string _previewUrl = "";
    /// <summary>
    /// 
    /// </summary>
    public string PreviewUrl
    {
        get { return _previewUrl; }
        set { _previewUrl = value.EmptyIfNullOrTrimmed(); }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Height { get; set; }
    
    private Collection<CardDefinition> _cards = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<CardDefinition> Cards
    {
        get { return _cards; }
        set { _cards = value ?? new Collection<CardDefinition>(); }
    }

    private Collection<SheetDefinition> _sheets = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<SheetDefinition> Sheets
    {
        get { return _sheets; }
        set { _sheets = value ?? new Collection<SheetDefinition>(); }
    }
    
    private Collection<PlanningMeeting> _meetings = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<PlanningMeeting> Meetings
    {
        get { return _meetings; }
        set { _meetings = value ?? new Collection<PlanningMeeting>(); }
    }

    internal static void OnBuildEntity(EntityTypeBuilder<DeckType> builder)
    {
        _ = builder.HasKey(nameof(Id));
        // _ = builder.HasOne(i => i.Team).WithMany(t => t.Milestones).HasForeignKey(nameof (TeamId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        // _ = builder.HasOne(i => i.Epic).WithMany(t => t.Milestones).HasForeignKey(nameof (TeamId)).OnDelete(DeleteBehavior.Restrict);
    }
}
