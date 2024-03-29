﻿define(function (require) {
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

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;

        self.id = ko.observable(emptyGuid);
        self.name = ko.observable("");
        self.type = ko.observable("");
        self.value = ko.observable(null);

        self.attached = async function () {
            currentSection = $("#grid-section");

            self.gridPageSize = $("#GridPageSize").val();

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
                self.gridPageSize,
                { field: "Name", dir: "asc" });
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${apiUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.type(data.Type);
            self.value(data.Value);

            await fetch(`/admin/configuration/settings/get-editor-ui/${self.type().replaceAll('.', '-')}`)
                .then(response => response.json())
                .then((data) => {
                    // Clean up from previously injected html/scripts
                    if (typeof cleanUp == 'function') {
                        cleanUp(self);
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
                        $(script).attr("data-settings-script", "true");//for some reason, .data("settings-script", "true") doesn't work here
                        $(script).appendTo('body');
                    }

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof updateModel == 'function') {
                        const data = ko.toJS(ko.mapping.fromJSON(self.value()));
                        updateModel(self, data);
                        ko.applyBindings(self, elementToBind);
                    }

                    //self.validator.resetForm();
                    switchSection($("#form-section"));
                })
                .catch(error => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.GetRecordError'));
                    console.error('Error: ', error);
                });
        };
        self.save = async function () {
            // ensure the function exists before calling it...
            if (typeof onBeforeSave == 'function') {
                onBeforeSave(self);
            }

            await ODataHelper.putOData(`${apiUrl}(${self.id()})`, {
                Id: self.id(),
                Name: self.name(),
                Type: self.type(),
                Value: self.value()
            });
        };
        self.cancel = function () {
            switchSection($("#grid-section"));
        };

        self.setResources = function (resources) {
            if (!self.resources) {
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
    };

    const viewModel = new ViewModel();
    return viewModel;
});

const RequiredResourceModel = function (model) {
    const self = this;

    self.Type = model.Type;
    self.Order = model.Order;
    self.Path = ko.observable(model.Path);
};

const RequiredResourceCollectionModel = function () {
    const self = this;

    self.Name = null;
    self.Resources = ko.observableArray([]);

    self.init = function (resource) {
        self.Name = resource.Name;

        for (const model of resource.Resources) {
            self.Resources.push(new RequiredResourceModel(model));
        };
    };
};