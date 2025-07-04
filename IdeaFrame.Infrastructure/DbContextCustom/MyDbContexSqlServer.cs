﻿using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Infrastructure.DbContextCustom
{
    public class MyDbContexSqlServer : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<FileSystemItem> FileSystemItems { get; set; }
        
        public DbSet<MindMapNode>MindMapNodes { get; set; }

        public DbSet<MindMapBranch> MindMapBranches { get; set; }
        public MyDbContexSqlServer(DbContextOptions options) : base(options)
        {
        }

        public void ClearDatabaseBeforeTests()
        {
            this.Database.EnsureDeleted();
            this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            setUpRefershTokenTable(modelBuilder);

            setUpNodeTable(modelBuilder);

            setUpBranchTable(modelBuilder);

        }

        private static void setUpBranchTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MindMapBranch>()
                .ToTable("Branches")
                .HasOne(x => x.Source)
                .WithMany()
                .HasForeignKey(x => x.SourceId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasPrincipalKey(x => x.Id);
            modelBuilder.Entity<MindMapBranch>()
                .HasOne(x => x.Target)
                .WithMany()
                .HasForeignKey(x => x.TargetId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasPrincipalKey(x => x.Id);
        }

        private static void setUpNodeTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileSystemItem>()
                .ToTable("FileSystemItems")
                .HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasPrincipalKey(x => x.Id);
            modelBuilder.Entity<FileSystemItem>()
                .HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MindMapNode>()
                .ToTable("MindMapNodes")
                .HasKey(x => x.Id);
            modelBuilder.Entity<MindMapNode>()
                .HasOne(x => x.MindMapFile)
                .WithMany()
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void setUpRefershTokenTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>()
                .ToTable("RefreshTokens")
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<RefreshToken>(rt => rt.UserName)
                .HasPrincipalKey<ApplicationUser>(u => u.UserName);
        }
    }
}
