using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

[Table("Collect_Collections")]
public partial class CollectCollection
{
    [Key]
    [Column("CollectionID")]
    public Guid CollectionId { get; set; }

    [Column("AccountID")]
    public Guid AccountId { get; set; }

    [StringLength(500)]
    public string CollectionName { get; set; } = null!;

    [ForeignKey("AccountId")]
    [InverseProperty("CollectCollections")]
    public virtual AcctAccount Account { get; set; } = null!;

    [InverseProperty("Collection")]
    public virtual ICollection<CollectCard> CollectCards { get; set; } = new List<CollectCard>();
}
