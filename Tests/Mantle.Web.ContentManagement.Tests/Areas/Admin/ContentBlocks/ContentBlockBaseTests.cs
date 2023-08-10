using Extenso.AspNetCore.Mvc.Html;
using Mantle.Web.ContentManagement.Areas.Admin.Localization;

namespace Mantle.Web.ContentManagement.Tests.Areas.Admin.ContentBlocks;

public class ContentBlockBaseTests
{
    [Fact]
    public void RenderKOScript_LanguageSwitchBlock()
    {
        var block = new LanguageSwitchBlock();

        string expected =
@"let contentBlockModel = (function () {
	const f = {};
	f.updateModel = function (blockModel) {
		blockModel.style = ko.observable("""");
		blockModel.includeInvariant = ko.observable(false);
		blockModel.invariantText = ko.observable(""[ Invariant ]"");
		const data = ko.mapping.fromJSON(blockModel.blockValues());
		if (data) {
			if (data.Style) { blockModel.style(data.Style()); }
			if (data.IncludeInvariant && (typeof data.IncludeInvariant === 'boolean')) { blockModel.includeInvariant(data.IncludeInvariant()); }
			if (data.InvariantText) { blockModel.invariantText(data.InvariantText()); }
		}
	};
	f.cleanUp = function (blockModel) {
		delete blockModel.style;
		delete blockModel.includeInvariant;
		delete blockModel.invariantText;
	};
	f.onBeforeSave = function (blockModel) {
		const data = {
			Style: blockModel.style(),
			IncludeInvariant: blockModel.includeInvariant(),
			InvariantText: blockModel.invariantText()
		};
		blockModel.blockValues(ko.mapping.toJSON(data));
	};
	return f;
})();";

        string actual = block.RenderKOScript().GetString();

        Assert.Equal(expected, actual);
    }
}