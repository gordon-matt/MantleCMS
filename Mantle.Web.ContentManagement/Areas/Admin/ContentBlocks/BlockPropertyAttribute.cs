namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

[AttributeUsage(AttributeTargets.Property)]
public class BlockPropertyAttribute : Attribute
{
    public static readonly BlockPropertyAttribute Default = new();

    public BlockPropertyAttribute() : this(default)
    {
    }

    public BlockPropertyAttribute(object defaultValue)
    {
        DefaultValue = defaultValue;
    }

    public object DefaultValue { get; set; }

    public override bool IsDefaultAttribute() => Equals(Default);
}