﻿using EasyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyAnalysis.Repository
{
    public class DefaultDbConext : DbContext
    {
        public DefaultDbConext() : base ("DefaultConnection") 
        {
        }

        public DbSet<ThreadModel> Threads { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<DropDownField> DropDownFields { get; set; }

        public DbSet<ScopeModel> Scopes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Configurations
                .Add(new TagTypeConfigration())
                .Add(new ThreadTypeConfigration())
                .Add(new DropDownFieldOptionTypeConfigration())
                .Add(new DropDownFieldTypeConfigration())
                .Add(new ScopeTypeConfigration());

            base.OnModelCreating(modelBuilder);
        }
    }
}