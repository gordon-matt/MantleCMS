﻿namespace Mantle.Data.Entity;

public interface IMantleEntityTypeConfiguration
{
    bool IsEnabled { get; }
}

public abstract class MantleEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>, IMantleEntityTypeConfiguration
    where TEntity : class
{
    public abstract void Configure(EntityTypeBuilder<TEntity> builder);

    public virtual bool IsEnabled => true;
}