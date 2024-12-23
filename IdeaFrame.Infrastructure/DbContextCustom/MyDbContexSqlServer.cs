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
        DbSet<RefreshToken> RefreshTokens { get; set; }
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
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshTokens");
        }



    }
}
