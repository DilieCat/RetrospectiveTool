
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Retrospective_Core.Models;

namespace Retrospective_EFSQLRetrospectiveDbImpl {
  
    public class RetroSpectiveDbContext : IdentityDbContext<RetroUser, RetroRole, int> {

        public RetroSpectiveDbContext(DbContextOptions<RetroSpectiveDbContext> options) : base(options) { }
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			base.OnModelCreating(modelBuilder);

	        modelBuilder.Entity<Retrospective>()
		        .HasMany(i => i.RetroColumns)
		        .WithOne(c => c.Retrospective)
		        .IsRequired()
		        .OnDelete(DeleteBehavior.Cascade);

	        modelBuilder.Entity<RetroColumn>()
		        .HasMany(i => i.RetroCards)
		        .WithOne(c => c.RetroColumn)
		        .IsRequired()
		        .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<RetroColumn>()
				.HasMany(i => i.RetroFamilies)
				.WithOne(c => c.RetroColumn)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<RetroFamily>()
				.HasOne(i => i.RetroColumn)
				.WithMany(f => f.RetroFamilies)
				.HasForeignKey(i => i.RetroColumnId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<RetroCard>(e => {
				e.HasOne(i => i.RetroFamily)
				.WithMany(i => i.RetroCards)
				.IsRequired(false)
				.HasForeignKey(i => i.RetroFamilyId)
				.OnDelete(DeleteBehavior.NoAction);


				e.HasOne(i => i.RetroColumn)
				.WithMany(i => i.RetroCards)
				.HasForeignKey(i => i.RetroColumnId)
				.OnDelete(DeleteBehavior.Cascade);
			});



			
        }

        public virtual DbSet<Retrospective> Retrospectives { get; set; }

        public virtual DbSet<RetroColumn> RetroColumns { get; set; }

        public virtual DbSet<RetroCard> RetroCards { get; set; }

		public virtual DbSet<RetroFamily> RetroFamilies { get; set; }
    }
}
