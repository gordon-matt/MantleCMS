define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');
    const koMap = require('knockout-mapping');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    const apiUrl = "/odata/mantle/web/SettingsApi";

    ko.mapping = koMap;

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;

            this.id = ko.observable(emptyGuid);
            this.name = ko.observable("");
            this.type = ko.observable("");
            this.value = ko.observable(null);
        }

        attached = async () => {
            currentSection = $("#grid-section");

            this.gridPageSize = $("#GridPageSize").val();

            GridHelper.initKendoGrid(
                "Grid",
                apiUrl,
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Mantle.Web/Settings.Model.Name'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }],
                this.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${apiUrl}(${id})`);
            this.id(data.Id);
            this.name(data.Name);
            this.type(data.Type);
            this.value(data.Value);

            await fetch(`/admin/configuration/settings/get-editor-ui/${this.type().replaceAll('.', '-')}`)
                .then(response => response.json())
                .then((data) => {
                    // Clean up from previously injected html/scripts
                    if (typeof cleanUp == 'function') {
                        cleanUp(this);
                    }

                    // Remove Old Scripts
                    const oldScripts = $('script[data-settings-script="true"]');

                    if (oldScripts.length > 0) {
                        for (const oldScript of oldScripts) {
                            $(oldScript).remove();
                        }
                    }

                    const elementToBind = $("#form-section")[0];
                    ko.cleanNode(elementToBind);

                    const result = $(data.content);

                    // Add new HTML
                    const content = $(result.filter('#settings-content')[0]);
                    const details = $('<div>').append(content.clone()).html();
                    $("#settings-details").html(details);

                    // Add new Scripts
                    const scripts = result.filter('script');

                    for (const script of scripts) {
                        $(script).attr("data-settings-script", "true"); //for some reason, .data("settings-script", "true") doesn't work here
                        $(script).appendTo('body');
                    }

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof updateModel == 'function') {
                        const data = ko.toJS(ko.mapping.fromJSON(this.value()));
                        updateModel(this, data);
                        ko.applyBindings(this, elementToBind);
                    }

                    //this.validator.resetForm();
                    switchSection($("#form-section"));
                })
                .catch(error => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.GetRecordError'));
                    console.error('Error: ', error);
                });
        };

        save = async () => {
            // ensure the function exists before calling it...
            if (typeof onBeforeSave == 'function') {
                onBeforeSave(this);
            }

            await ODataHelper.putOData(`${apiUrl}(${this.id()})`, {
                Id: this.id(),
                Name: this.name(),
                Type: this.type(),
                Value: this.value()
            });
        };

        cancel = () => {
            switchSection($("#grid-section"));
        };

        setResources = (resources) => {
            if (!this.resources) {
                console.error('Could not find an observable array named "resources".');
                return;
            }

            if (!resources || !Array.isArray(resources)) {
                console.warn('No resources available.');
                return;
            }

            for (const resource of resources) {
                const item = new RequiredResourceCollectionModel();
                item.init(resource);
                viewModel.resources.push(item);
            }
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});

class RequiredResourceModel {
    constructor(model) {
        this.Type = model.Type;
        this.Order = model.Order;
        this.Path = ko.observable(model.Path);
    }
}

class RequiredResourceCollectionModel {
    constructor() {
        this.Name = null;
        this.Resources = ko.observableArray([]);
    }

    init = (resource) => {
        this.Name = resource.Name;

        for (const model of resource.Resources) {
            this.Resources.push(new RequiredResourceModel(model));
        };
    };
}