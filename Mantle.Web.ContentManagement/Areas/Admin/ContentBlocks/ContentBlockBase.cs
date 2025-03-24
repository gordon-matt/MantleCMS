using System.Reflection;
using Extenso.Reflection;
using Humanizer;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

public abstract class ContentBlockBase : BaseEntity<Guid>, IContentBlock
{
    #region IContentBlock Members

    public string Title { get; set; }

    public int Order { get; set; }

    public bool Enabled { get; set; }

    public abstract string Name { get; }

    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.Model.ZoneId)]
    public Guid ZoneId { get; set; }

    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.Model.CustomTemplatePath)]
    public string CustomTemplatePath { get; set; }

    public Guid? PageId { get; set; }

    public bool Localized { get; set; }

    public string CultureCode { get; set; }

    public Guid? RefId { get; set; }

    public abstract string DisplayTemplatePath { get; }

    public abstract string EditorTemplatePath { get; }

    #endregion IContentBlock Members

    public virtual IHtmlContent RenderKOScript()
    {
        var blockProperties = GetBlockProperties();

        var sb = new StringBuilder(1024);

        sb.AppendLine("contentBlockModel = (function () {");
        sb.AppendLine("\tconst f = {};");

        sb.AppendLine(RenderKOUpdateModelFunction(blockProperties));
        sb.AppendLine(RenderKOCleanUpFunction(blockProperties));
        sb.AppendLine(RenderKOOnBeforeSaveFunction(blockProperties));

        sb.AppendLine("\treturn f;");
        sb.Append("})();");

        return new HtmlString(sb.ToString());
    }

    public IEnumerable<BlockPropertyInfo> GetBlockProperties() =>
        GetType().GetProperties()
        .Select(x => new BlockPropertyInfo(x.Name, x.Name.Camelize(), x.PropertyType, x.GetCustomAttribute<BlockPropertyAttribute>()))
        .Where(x => x.Attribute != null);

    public virtual string RenderKOUpdateModelFunction(IEnumerable<BlockPropertyInfo> blockProperties)
    {
        var sb = new StringBuilder(1024);

        sb.AppendLine("\tf.updateModel = function (blockModel) {");

        foreach (var property in blockProperties)
        {
            sb.AppendLine(RenderObservableDeclaration(property));
        }

        sb.AppendLine($"\t\tconst data = ko.mapping.fromJSON(blockModel.blockValues());");
        sb.AppendLine("\t\tif (data) {");
        foreach (var property in blockProperties)
        {
            sb.AppendLine(RenderObservableAssignment(property));
        }
        sb.AppendLine("\t\t}");
        sb.Append("\t};");

        return sb.ToString();
    }

    public virtual string RenderKOCleanUpFunction(IEnumerable<BlockPropertyInfo> blockProperties)
    {
        var sb = new StringBuilder(512);

        sb.AppendLine("\tf.cleanUp = function (blockModel) {");
        foreach (var property in blockProperties)
        {
            sb.AppendLine($"\t\tdelete blockModel.{property.KOName};");
        }
        sb.Append("\t};");

        return sb.ToString();
    }

    public virtual string RenderKOOnBeforeSaveFunction(IEnumerable<BlockPropertyInfo> blockProperties)
    {
        var sb = new StringBuilder(512);

        sb.AppendLine("\tf.onBeforeSave = function (blockModel) {");
        sb.AppendLine("\t\tconst data = {");

        int propertyCount = blockProperties.Count();
        for (int i = 0; i < propertyCount; i++)
        {
            var property = blockProperties.ElementAt(i);
            bool isLast = i == propertyCount - 1;
            sb.AppendLine(RenderSaveValue(property, isLast));
        }

        sb.AppendLine("\t\t};");
        sb.AppendLine("\t\tblockModel.blockValues(ko.mapping.toJSON(data));");
        sb.Append("\t};");

        return sb.ToString();
    }

    protected virtual string RenderObservableDeclaration(BlockPropertyInfo property)
    {
        if (property.Type == typeof(string) || property.Type.IsEnum)
        {
            return $"\t\tblockModel.{property.KOName} = ko.observable(\"{property.Attribute.DefaultValue ?? string.Empty}\");";
        }
        else
        {
            string value;
            if (property.Attribute.DefaultValue is not null)
            {
                value = property.Attribute.DefaultValue is bool
                    ? property.Attribute.DefaultValue.ToString().ToLowerInvariant()
                    : property.Attribute.DefaultValue.ToString();
            }
            else if (property.Type.GetDefaultValue() is not null)
            {
                object val = property.Type.GetDefaultValue();
                value = val is bool ? val.ToString().ToLowerInvariant() : val.ToString();
            }
            else
            {
                value = "null";
            }

            return $"\t\tblockModel.{property.KOName} = ko.observable({value});";
        }
    }

    protected virtual string RenderObservableAssignment(BlockPropertyInfo property) => property.Type == typeof(bool)
        ? $"\t\t\tif (data.{property.Name} && (typeof data.{property.Name} === 'boolean')) {{ blockModel.{property.KOName}(data.{property.Name}()); }}"
        : $"\t\t\tif (data.{property.Name}) {{ blockModel.{property.KOName}(data.{property.Name}()); }}";

    protected virtual string RenderSaveValue(BlockPropertyInfo property, bool isLast) =>
        $"\t\t\t{property.Name}: blockModel.{property.KOName}(){(isLast ? string.Empty : ",")}";

    public record BlockPropertyInfo(string Name, string KOName, Type Type, BlockPropertyAttribute Attribute);
}