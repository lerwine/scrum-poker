using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class Participant
{
    private readonly FKOptionalNavProperty<CardDefinition> _drawnCard = new(e => e.Id);
    /// <summary>
    /// 
    /// </summary>
    public Guid? DrawnCardId
    {
        get => _drawnCard.ForeignKey;
        set => _drawnCard.ForeignKey = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public CardDefinition? DrawnCard
    {
        get => _drawnCard.Model;
        set => _drawnCard.Model = value;
    }

    private readonly FKNavProperty<CardColor> _cardColor = new(e => e.Id);
    /// <summary>
    /// 
    /// </summary>
    public Guid CardColorId
    {
        get => _cardColor.ForeignKey;
        set => _cardColor.ForeignKey = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public CardColor? CardColor
    {
        get => _cardColor.Model;
        set => _cardColor.Model = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public int PointsAssigned { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? ScrumCapacity { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime LastActivity { get; set; }
    
    private readonly FKNavProperty<PlanningMeeting> _meeting = new(e => e.Id);
    /// <summary>
    /// 
    /// </summary>
    public Guid MeetingId
    {
        get => _meeting.ForeignKey;
        set => _meeting.ForeignKey = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public PlanningMeeting? Meeting
    {
        get => _meeting.Model;
        set => _meeting.Model = value;
    }

    private readonly FKNavProperty<UserProfile> _user = new(e => e.Id);
    /// <summary>
    /// 
    /// </summary>
    public Guid UserId
    {
        get => _user.ForeignKey;
        set => _user.ForeignKey = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public UserProfile? User
    {
        get => _user.Model;
        set => _user.Model = value;
    }
    
    internal static void OnBuildEntity(EntityTypeBuilder<Participant> builder)
    {
        _ = builder.HasOne(p => p.Meeting).WithMany(d => d.Participants).HasForeignKey(nameof(MeetingId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.User).WithMany(d => d.Participation).HasForeignKey(nameof(UserId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.CardColor).WithMany(d => d.Participants).HasForeignKey(nameof(CardColorId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.DrawnCard).WithMany(d => d.Participants).HasForeignKey(nameof(DrawnCardId)).OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasKey(nameof(MeetingId), nameof(UserId));
    }
}
