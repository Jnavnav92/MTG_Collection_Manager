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

    /// <summary>
    /// Collection Name + CollectionID is the primary key, but foreign key in Collect_Cards needs to map to CollectionID so a composite key isn't a good fit.
    /// code will have to maintain uniqueness of this value.
    /// </summary>
    [StringLength(100)]
    public string CollectionName { get; set; } = null!;

    [ForeignKey("AccountId")]
    [InverseProperty("CollectCollections")]
    public virtual AcctAccount Account { get; set; } = null!;

    [InverseProperty("Collection")]
    public virtual ICollection<CollectCard> CollectCards { get; set; } = new List<CollectCard>();
}
