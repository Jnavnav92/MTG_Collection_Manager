using System;
using System.Collections.Generic;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data;

public partial class CollectionMgrContext : DbContext
{
    private readonly string connectionString;
    public CollectionMgrContext(string connString) : base()
    {
        connectionString = connString;
    }

    public CollectionMgrContext(DbContextOptions<CollectionMgrContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcctAccount> AcctAccounts { get; set; }

    public virtual DbSet<CollectCard> CollectCards { get; set; }

    public virtual DbSet<CollectCollection> CollectCollections { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(connectionString);

            base.OnConfiguring(optionsBuilder);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcctAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK_tblAccounts");

            entity.Property(e => e.AccountId).ValueGeneratedNever();
            entity.Property(e => e.AccountVerified).HasDefaultValue(false);
            entity.Property(e => e.Pwhash).IsFixedLength();
        });

        modelBuilder.Entity<CollectCard>(entity =>
        {
            entity.Property(e => e.CardId).ValueGeneratedNever();

            entity.HasOne(d => d.Collection).WithMany(p => p.CollectCards)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cards_Collection_CollectionID");
        });

        modelBuilder.Entity<CollectCollection>(entity =>
        {
            entity.Property(e => e.CollectionId).ValueGeneratedNever();

            entity.HasOne(d => d.Account).WithMany(p => p.CollectCollections)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Collections_Accounts_AccountID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
