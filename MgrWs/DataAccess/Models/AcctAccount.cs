using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

[Table("Acct_Accounts")]
[Index("EmailAddress", Name = "AK_Email", IsUnique = true)]
public partial class AcctAccount
{
    [Key]
    [Column("AccountID")]
    public Guid AccountId { get; set; }

    [StringLength(500)]
    public string EmailAddress { get; set; } = null!;

    [Column("PWHash")]
    [StringLength(60)]
    [Unicode(false)]
    public string Pwhash { get; set; } = null!;

    public bool? AccountVerified { get; set; }

    public Guid? AuthorizationToken { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<CollectCollection> CollectCollections { get; set; } = new List<CollectCollection>();
}
