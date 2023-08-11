using Extenso.Reflection;
using Humanizer;
using NUglify.Helpers;

namespace Mantle.Web.Configuration;

public interface ISettings
{
    string Name { get; }

    /// <summary>
    /// True if these settings are global (for all sites) and only the admin user can modify.
    /// False if each tenant can have their own customized settings.
    /// </summary>
    bool IsTenantRestricted { get; }

    string EditorTemplatePath { get; }
}

public abstract class BaseSettings : ISettings
{
    #region ISettings Members

    public abstract string Name { get; }

    public virtual bool IsTenantRestricted => false;

    public abstract string EditorTemplatePath { get; }

    #endregion ISettings Members

    public virtual IHtmlContent RenderKOScript()
    {
        var settingsProperties = GetSettingsProperties();

        var sb = new StringBuilder(1024);

        sb.AppendLine(RenderKOUpdateModelFunction(settingsProperties));
        sb.AppendLine(RenderKOCleanUpFunction(settingsProperties));
        sb.Append(RenderKOOnBeforeSaveFunction(settingsProperties));

        return new HtmlString(sb.ToString());
    }

    public IEnumerable<SettingsPropertyInfo> GetSettingsProperties() =>
        GetType().GetProperties()
        .Select(x => new SettingsPropertyInfo(x.Name, x.Name.Camelize(), x.PropertyType, x.GetCustomAttribute<SettingsPropertyAttribute>()))
        .Where(x => x.Attribute != null);

    public virtual string RenderKOUpdateModelFunction(IEnumerable<SettingsPropertyInfo> settingsProperties)
    {
        var sb = new StringBuilder(1024);

        sb.AppendLine("function updateModel(viewModel, data) {");

        foreach (var property in settingsProperties)
        {
            sb.AppendLine(RenderObservableDeclaration(property));
        }

        bool isResourceSettings = (this is IResourceSettings);
        if (isResourceSettings)
        {
            sb.AppendLine("\tviewModel.resources = ko.observableArray([]);");
        }

        sb.AppendLine("\tif (data) {");
        foreach (var property in settingsProperties)
        {
            sb.AppendLine(RenderObservableAssignment(property));
        }

        if (isResourceSettings)
        {
            sb.AppendLine("\t\tif (data.Resources) { viewModel.setResources(data.Resources); }");
        }

        sb.AppendLine("\t}");
        sb.Append("};");

        return sb.ToString();
    }

    public virtual string RenderKOCleanUpFunction(IEnumerable<SettingsPropertyInfo> settingsProperties)
    {
        var sb = new StringBuilder(512);

        sb.AppendLine("function cleanUp(viewModel) {");
        foreach (var property in settingsProperties)
        {
            if (!string.IsNullOrWhiteSpace(property.Attribute.CleanUp))
            {
                sb.AppendLine($"\t{property.Attribute.CleanUp}");
            }
            else
            {
                sb.AppendLine($"\tdelete viewModel.{property.KOName};");
            }
        }

        if (this is IResourceSettings)
        {
            sb.AppendLine("\tdelete viewModel.resources;");
        }

        sb.Append("};");

        return sb.ToString();
    }

    public virtual string RenderKOOnBeforeSaveFunction(IEnumerable<SettingsPropertyInfo> settingsProperties)
    {
        bool isResourceSettings = (this is IResourceSettings);
        var sb = new StringBuilder(512);

        sb.AppendLine("function onBeforeSave(viewModel) {");
        sb.AppendLine("\tconst data = {");

        int propertyCount = settingsProperties.Count();
        for (int i = 0; i < propertyCount; i++)
        {
            var property = settingsProperties.ElementAt(i);
            bool isLast = (i == propertyCount - 1) && !isResourceSettings;
            sb.AppendLine(RenderSaveValue(property, isLast));
        }

        if (isResourceSettings)
        {
            sb.AppendLine("\t\tResources: viewModel.resources()");
        }

        sb.AppendLine("\t};");
        sb.AppendLine("\tviewModel.value(ko.mapping.toJSON(data));");
        sb.Append("};");

        return sb.ToString();
    }

    protected virtual string RenderObservableDeclaration(SettingsPropertyInfo property)
    {
        if (!string.IsNullOrWhiteSpace(property.Attribute.Declaration))
        {
            return $"\t{property.Attribute.Declaration}";
        }

        if (property.Type == typeof(string) || property.Type.IsEnum)
        {
            return $"\tviewModel.{property.KOName} = ko.observable(\"{property.Attribute.DefaultValue ?? string.Empty}\");";
        }
        else
        {
            string value;
            if (property.Attribute.DefaultValue is not null)
            {
                if (property.Attribute.DefaultValue is bool)
                {
                    value = property.Attribute.DefaultValue.ToString().ToLowerInvariant();
                }
                else
                {
                    value = property.Attribute.DefaultValue.ToString();
                }
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

            return $"\tviewModel.{property.KOName} = ko.observable({value});";
        }
    }

    protected virtual string RenderObservableAssignment(SettingsPropertyInfo property)
    {
        if (!string.IsNullOrWhiteSpace(property.Attribute.Assignment))
        {
            return $"\t\t{property.Attribute.Assignment}";
        }

        if (property.Type == typeof(bool))
        {
            return $"\t\tif (data.{property.Name} && (typeof data.{property.Name} === 'boolean')) {{ viewModel.{property.KOName}(data.{property.Name}); }}";
        }
        else
        {
            return $"\t\tif (data.{property.Name}) {{ viewModel.{property.KOName}(data.{property.Name}); }}";
        }
    }

    protected virtual string RenderSaveValue(SettingsPropertyInfo property, bool isLast) =>
        !string.IsNullOrWhiteSpace(property.Attribute.Save)
        ? $"\t\t{property.Attribute.Save}{(isLast ? string.Empty : ",")}"
        : $"\t\t{property.Name}: viewModel.{property.KOName}(){(isLast ? string.Empty : ",")}";

    public record SettingsPropertyInfo(string Name, string KOName, Type Type, SettingsPropertyAttribute Attribute);
}