﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Messaging.Forums.Data.Entities;

public class ForumGroup : TenantEntity<int>
{
    private ICollection<Forum> forums;

    public string Name { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public virtual ICollection<Forum> Forums
    {
        get => forums ??= []; protected set => forums = value;
    }
}

public class ForumGroupMap : IEntityTypeConfiguration<ForumGroup>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<ForumGroup> builder)
    {
        builder.ToTable(Constants.Tables.Groups, MantleWebConstants.DatabaseSchemas.Plugins);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.DisplayOrder).IsRequired();
        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.UpdatedOnUtc).IsRequired();
    }

    public bool IsEnabled => PluginManager.IsPluginInstalled(Constants.PluginSystemName);
}