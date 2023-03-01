using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class SheetDefinition
{
    /// <summary>
    /// 
    /// </summary>
    public int SheetNumber { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string URL { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    public int MaxValue { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid DeckTypeId { get; set; }

    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8618
    public DeckType DeckType { get; set; }
#pragma warning restore CS8618

    internal static void OnBuildEntity(EntityTypeBuilder<SheetDefinition> builder)
    {
        _ = builder.HasKey(nameof(SheetNumber), nameof(DeckTypeId));
        _ = builder.HasOne(i => i.DeckType).WithMany(t => t.Sheets).HasForeignKey(nameof (DeckTypeId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
}
