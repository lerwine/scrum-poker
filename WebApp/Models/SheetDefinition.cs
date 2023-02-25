using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class SheetDefinition
{
    public int SheetNumber { get; set; }
    
    public string URL { get; set; } = "";

    public int MaxValue { get; set; }

    public Guid DeckTypeId { get; set; }

#pragma warning disable CS8618
    public DeckType DeckType { get; set; }
#pragma warning restore CS8618

    internal static void OnBuildEntity(EntityTypeBuilder<SheetDefinition> builder)
    {
        _ = builder.HasKey(nameof(SheetNumber), nameof(DeckTypeId));
        _ = builder.HasOne(i => i.DeckType).WithMany(t => t.Sheets).HasForeignKey(nameof (DeckTypeId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
}
