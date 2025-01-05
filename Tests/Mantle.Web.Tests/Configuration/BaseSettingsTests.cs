using Extenso.AspNetCore.Mvc.Html;
using Mantle.Web.Configuration;
using System.Text;

namespace Mantle.Web.Tests.Configuration;

public class BaseSettingsTests
{
    [Fact]
    public void RenderKOScript_DateTimeSettings()
    {
        var settings = new DateTimeSettings();

        string expected =
@"function updateModel(viewModel, data) {
	viewModel.defaultTimeZoneId = ko.observable("""");
	viewModel.allowUsersToSetTimeZone = ko.observable(false);
	if (data) {
		if (data.DefaultTimeZoneId) { viewModel.defaultTimeZoneId(data.DefaultTimeZoneId); }
		if (data.AllowUsersToSetTimeZone && (typeof data.AllowUsersToSetTimeZone === 'boolean')) { viewModel.allowUsersToSetTimeZone(data.AllowUsersToSetTimeZone); }
	}
};
function cleanUp(viewModel) {
	delete viewModel.defaultTimeZoneId;
	delete viewModel.allowUsersToSetTimeZone;
};
function onBeforeSave(viewModel) {
	const data = {
		DefaultTimeZoneId: viewModel.defaultTimeZoneId(),
		AllowUsersToSetTimeZone: viewModel.allowUsersToSetTimeZone()
	};
	viewModel.value(ko.mapping.toJSON(data));
};";

        string actual = settings.RenderKOScript().GetString();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RenderKOScript_SiteSettings()
    {
        var settings = new SiteSettings();

        string expected =
@"function updateModel(viewModel, data) {
	viewModel.siteName = ko.observable(""My Site"");
	viewModel.defaultFrontendLayoutPath = ko.observable(""~/Views/Shared/_Layout.cshtml"");
	viewModel.adminLayoutPath = ko.observable(""~/Areas/Admin/Views/Shared/_Layout.cshtml"");
	viewModel.defaultGridPageSize = ko.observable(10);
	viewModel.defaultTheme = ko.observable(""Default"");
	viewModel.allowUserToSelectTheme = ko.observable(false);
	viewModel.defaultLanguage = ko.observable("""");
	viewModel.defaultMetaKeywords = ko.observable("""");
	viewModel.defaultMetaDescription = ko.observable("""");
	viewModel.homePageTitle = ko.observable(""Home Page"");
	viewModel.resources = ko.observableArray([]);
	if (data) {
		if (data.SiteName) { viewModel.siteName(data.SiteName); }
		if (data.DefaultFrontendLayoutPath) { viewModel.defaultFrontendLayoutPath(data.DefaultFrontendLayoutPath); }
		if (data.AdminLayoutPath) { viewModel.adminLayoutPath(data.AdminLayoutPath); }
		if (data.DefaultGridPageSize) { viewModel.defaultGridPageSize(data.DefaultGridPageSize); }
		if (data.DefaultTheme) { viewModel.defaultTheme(data.DefaultTheme); }
		if (data.AllowUserToSelectTheme && (typeof data.AllowUserToSelectTheme === 'boolean')) { viewModel.allowUserToSelectTheme(data.AllowUserToSelectTheme); }
		if (data.DefaultLanguage) { viewModel.defaultLanguage(data.DefaultLanguage); }
		if (data.DefaultMetaKeywords) { viewModel.defaultMetaKeywords(data.DefaultMetaKeywords); }
		if (data.DefaultMetaDescription) { viewModel.defaultMetaDescription(data.DefaultMetaDescription); }
		if (data.HomePageTitle) { viewModel.homePageTitle(data.HomePageTitle); }
		if (data.Resources) { viewModel.setResources(data.Resources); }
	}
};
function cleanUp(viewModel) {
	delete viewModel.siteName;
	delete viewModel.defaultFrontendLayoutPath;
	delete viewModel.adminLayoutPath;
	delete viewModel.defaultGridPageSize;
	delete viewModel.defaultTheme;
	delete viewModel.allowUserToSelectTheme;
	delete viewModel.defaultLanguage;
	delete viewModel.defaultMetaKeywords;
	delete viewModel.defaultMetaDescription;
	delete viewModel.homePageTitle;
	delete viewModel.resources;
};
function onBeforeSave(viewModel) {
	const data = {
		SiteName: viewModel.siteName(),
		DefaultFrontendLayoutPath: viewModel.defaultFrontendLayoutPath(),
		AdminLayoutPath: viewModel.adminLayoutPath(),
		DefaultGridPageSize: viewModel.defaultGridPageSize(),
		DefaultTheme: viewModel.defaultTheme(),
		AllowUserToSelectTheme: viewModel.allowUserToSelectTheme(),
		DefaultLanguage: viewModel.defaultLanguage(),
		DefaultMetaKeywords: viewModel.defaultMetaKeywords(),
		DefaultMetaDescription: viewModel.defaultMetaDescription(),
		HomePageTitle: viewModel.homePageTitle(),
		Resources: viewModel.resources()
	};
	viewModel.value(ko.mapping.toJSON(data));
};";

        string actual = settings.RenderKOScript().GetString();

        Assert.Equal(expected, actual);
    }
}