using Extenso.Reflection;
using Humanizer;
using System.Reflection;

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
        var blockProperties = GetType().GetProperties()
            .Select(x => new
            {
                Name = x.Name,
                KOName = x.Name.Camelize(),
                Type = x.PropertyType,
                Attribute = x.GetCustomAttribute<BlockPropertyAttribute>(),
            })
            .Where(x => x.Attribute != null);

        var sb = new StringBuilder(1024);

        sb.AppendLine("const contentBlockModel = (function () {");
        sb.AppendLine("\tconst f = {};");
        sb.AppendLine("\tf.updateModel = function (blockModel) {");

        foreach (var property in blockProperties)
        {
            if (property.Type == typeof(string) || property.Type.IsEnum)
            {
                sb.AppendLine($"\t\tblockModel.{property.KOName} = ko.observable(\"{property.Attribute.DefaultValue ?? string.Empty}\");");
            }
            else
            {
                string value;
                if (property.Attribute.DefaultValue is not null)
                {
                    value = property.Attribute.DefaultValue.ToString();
                }
                else if (property.Type.GetDefaultValue() is not null)
                {
                    var val = property.Type.GetDefaultValue();
                    if (val is bool)
                    {
                        value = val.ToString().ToLowerInvariant();
                    }
                    else
                    {
                        value = val.ToString();
                    }
                }
                else
                {
                    value = "null";
                }

                //string value = property.Attribute.DefaultValue ?? property.Type.GetDefaultValue() ?? "null";
                sb.AppendLine($"\t\tblockModel.{property.KOName} = ko.observable({value});");
            }
        }

        sb.AppendLine($"\t\tconst data = ko.mapping.fromJSON(blockModel.blockValues());");
        sb.AppendLine("\t\tif (data) {");
        foreach (var property in blockProperties)
        {
            if (property.Type == typeof(bool))
            {
                sb.AppendLine($"\t\t\tif (data.{property.Name} && (typeof data.{property.Name} === 'boolean')) {{ blockModel.{property.KOName}(data.{property.Name}()); }}");
            }
            else
            {
                sb.AppendLine($"\t\t\tif (data.{property.Name}) {{ blockModel.{property.KOName}(data.{property.Name}()); }}");
            }
        }
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t};");

        sb.AppendLine("\tf.cleanUp = function (blockModel) {");
        foreach (var property in blockProperties)
        {
            sb.AppendLine($"\t\tdelete blockModel.{property.KOName};");
        }
        sb.AppendLine("\t};");

        sb.AppendLine("\tf.onBeforeSave = function (blockModel) {");
        sb.AppendLine("\t\tconst data = {");

        int propertyCount = blockProperties.Count();
        for (int i = 0; i < propertyCount; i++)
        {
            var property = blockProperties.ElementAt(i);
            bool isLast = (i == propertyCount - 1);
            sb.AppendLine($"\t\t\t{property.Name}: blockModel.{property.KOName}(){(isLast ? string.Empty : ",")}");
        }

        sb.AppendLine("\t\t};");
        sb.AppendLine("\t\tblockModel.blockValues(ko.mapping.toJSON(data));");
        sb.AppendLine("\t};");
        sb.AppendLine("\treturn f;");
        sb.Append("})();");

        return new HtmlString(sb.ToString());
    }
}