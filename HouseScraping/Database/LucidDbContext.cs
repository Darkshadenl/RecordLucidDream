using System;
using Microsoft.EntityFrameworkCore;
using Interfaces;
using HouseScraping.Models;

namespace HouseScraping.Database;

public class LucidDbContext : DbContext
{
    public DbSet<AudioRecordingInfo> AudioRecordings { get; set; }
    public string DbPath { get; }

    public LucidDbContext(DbContextOptions<LucidDbContext> options) : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "LucidDatabase.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AudioRecordingInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FilePath).IsRequired();
            entity.Property(e => e.RecordedAt).IsRequired();
            entity.Property(e => e.IsProcessed).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}