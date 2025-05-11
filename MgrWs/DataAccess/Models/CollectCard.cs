using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

[Table("Collect_Cards")]
public partial class CollectCard
{
    [Key]
    [Column("CardID")]
    public Guid CardId { get; set; }

    [Column("CollectionID")]
    public Guid CollectionId { get; set; }

    [Column("ScryfallCardID")]
    public Guid ScryfallCardId { get; set; }

    [ForeignKey("CollectionId")]
    [InverseProperty("CollectCards")]
    public virtual CollectCollection Collection { get; set; } = null!;
}
