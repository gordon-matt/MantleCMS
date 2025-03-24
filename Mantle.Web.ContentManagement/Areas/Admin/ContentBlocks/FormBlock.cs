namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

public class FormBlock : ContentBlockBase
{
    //[AllowHtml]
    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HtmlTemplate)]
    public string HtmlTemplate { get; set; }

    //[AllowHtml]
    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.ThankYouMessage)]
    public string ThankYouMessage { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.RedirectUrl)]
    public string RedirectUrl { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.EmailAddress)]
    [LocalizedHelpText(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HelpText.EmailAddress)]
    public string EmailAddress { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.UseAjax)]
    public bool UseAjax { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.FormUrl)]
    [LocalizedHelpText(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HelpText.FormUrl)]
    public string FormUrl { get; set; }

    #region ContentBlockBase Overrides

    public override string Name => "Form Content Block";

    public override string DisplayTemplatePath => "/Areas/Admin/ContentBlocks/Views/Shared/DisplayTemplates/FormBlock.cshtml";

    public override string EditorTemplatePath => "/Areas/Admin/ContentBlocks/Views/Shared/EditorTemplates/FormBlock.cshtml";

    public override string RenderKOUpdateModelFunction(IEnumerable<BlockPropertyInfo> blockProperties)
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
        sb.AppendLine("blockModel.tinyMCEConfig = mantleDefaultTinyMCEConfig;");
        sb.Append("\t};");

        return sb.ToString();
    }

    public override string RenderKOOnBeforeSaveFunction(IEnumerable<BlockPropertyInfo> blockProperties)
    {
        var sb = new StringBuilder(512);

        sb.AppendLine("\tf.onBeforeSave = function (blockModel) {");

        sb.AppendLine(
@"		let thankYouMessage = blockModel.thankYouMessage();
if (blockModel.useAjax()) {
	thankYouMessage = stripHTML(thankYouMessage);
}");

        sb.AppendLine("\t\tconst data = {");

        int propertyCount = blockProperties.Count();
        for (int i = 0; i < propertyCount; i++)
        {
            var property = blockProperties.ElementAt(i);
            bool isLast = i == propertyCount - 1;

            if (property.Name == "ThankYouMessage")
            {
                sb.AppendLine($"\t\t\tThankYouMessage: thankYouMessage{(isLast ? string.Empty : ",")}");
            }
            else
            {
                sb.AppendLine(RenderSaveValue(property, isLast));
            }
        }

        sb.AppendLine("\t\t};");
        sb.AppendLine("\t\tblockModel.blockValues(ko.mapping.toJSON(data));");
        sb.Append("\t};");

        return sb.ToString();
    }

    #endregion ContentBlockBase Overrides
}