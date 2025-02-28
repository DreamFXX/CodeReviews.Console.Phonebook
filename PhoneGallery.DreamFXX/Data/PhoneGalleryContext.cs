﻿using Microsoft.EntityFrameworkCore;
using PhoneGallery.DreamFXX.Models;
namespace PhoneGallery.DreamFXX.Data;

public class PhoneGalleryContext : DbContext
{
    public PhoneGalleryContext(DbContextOptions<PhoneGalleryContext> options) : base(options)
    {
    }

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Family" },
            new Category { Id = 2, Name = "Girls" },
            new Category { Id = 3, Name = "Friends" }
        );
    }
}
