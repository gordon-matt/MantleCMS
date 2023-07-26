'use strict'

const localStorageRootKey = "BlogContent_";
const localStorageUsersKey = localStorageRootKey + "Users_";

const BlogEntryModel = function () {
    const self = this;

    self.headline = ko.observable(null);
    self.userId = ko.observable(null);
    self.userName = ko.observable(null);
    self.dateCreatedUtc = ko.observable(null);
    self.teaserImageUrl = ko.observable(null);
    self.shortDescription = ko.observable(null);
    self.fullDescription = ko.observable(null);
    self.useExternalLink = ko.observable(false);
    self.externalLink = ko.observable(null);

    self.showDetails = function () {
        viewModel.selected(self);
        switchSection($('#details-section'));
    };
};

const ViewModel = function () {
    const self = this;
    self.entries = ko.observableArray([]);
    self.selected = ko.observable(new BlogEntryModel());

    self.total = ko.observable(0);
    self.pageIndex = ko.observable(1);
    self.pageSize = settings.itemsPerPage;

    self.pageCount = function () {
        return Math.ceil(self.total() / self.pageSize);
    };

    self.showMain = function () {
        switchSection($('#main-section'));
    };

    self.showPrevious = function () {
        const selectedIndex = $.inArray(self.selected(), self.entries());
        if (selectedIndex == 0) {
            if (self.pageIndex() > 1) {
                $("li[data-lp=" + (self.pageIndex() - 1) + "] > a").click();
                setTimeout(function () {
                    const previous = self.entries()[self.entries().length - 1];
                    self.selected(previous);
                }, 500);
            }
        }
        else {
            const previous = self.entries()[selectedIndex - 1];
            self.selected(previous);
        }
    };

    self.showNext = function () {
        const selectedIndex = $.inArray(self.selected(), self.entries());
        if (selectedIndex == (self.entries().length - 1)) {
            if (self.pageIndex() < self.pageCount()) {
                $("li[data-lp=" + (self.pageIndex() + 1) + "] > a").click();
                setTimeout(function () {
                    const next = self.entries()[0];
                    self.selected(next);
                }, 500);
            }
        }
        else {
            const next = self.entries()[selectedIndex + 1];
            self.selected(next);
        }
    };

    self.canShowPrevious = function () {
        const selectedIndex = $.inArray(self.selected(), self.entries());
        if (self.pageIndex() == 1 && selectedIndex == 0) {
            return false;
        }
        return true;
    };

    self.canShowNext = function () {
        const selectedIndex = $.inArray(self.selected(), self.entries());
        if (self.pageIndex() == self.pageCount() && selectedIndex == (self.entries().length - 1)) {
            return false;
        }
        return true;
    };
};

breeze.config.initializeAdapterInstances({ dataService: "OData" });
const manager = new breeze.EntityManager('/odata/mantle/cms');

const pagerInitialized = false;
const userIds = [];

let viewModel;
$(document).ready(function () {
    //TODO: Maybe clear this only after a certain amount of time (not very page load)?

    const keys = getLocalStorageKeys();
    for (const i = 0; i < keys.length; i++) {
        const key = keys[i];
        if (key.startsWith(localStorageUsersKey)) {
            localStorage.removeItem(key);
        }
    }

    viewModel = new ViewModel();
    getBlogs();
    ko.applyBindings(viewModel);
});

function getBlogs() {
    userIds = [];

    const query = new breeze.EntityQuery()
        .from("BlogPostApi")
        .orderBy("DateCreatedUtc desc")
        .skip((viewModel.pageIndex() - 1) * viewModel.pageSize)
        .take(viewModel.pageSize)
        .inlineCount();

    manager.executeQuery(query).then(function (data) {
        viewModel.entries([]);
        viewModel.selected(new BlogEntryModel());
        viewModel.total(data.inlineCount);

        $(data.httpResponse.data.results).each(function () {
            const current = this;
            const entry = new BlogEntryModel();
            entry.headline(current.Headline);
            entry.userId(current.UserId);

            const date = moment(current.DateCreatedUtc);
            entry.dateCreatedUtc(date.format(settings.dateFormat));

            entry.teaserImageUrl(current.TeaserImageUrl);
            entry.shortDescription(current.ShortDescription);
            entry.fullDescription(current.FullDescription);
            entry.useExternalLink(current.UseExternalLink);
            entry.externalLink(current.ExternalLink);
            viewModel.entries.push(entry);
            userIds.push(current.UserId);
        });

        if (!pagerInitialized) {
            $('#pager').bootpag({
                total: viewModel.pageCount(),
                page: viewModel.pageIndex(),
                maxVisible: 5,
                leaps: true,
                firstLastUse: true,
            }).on("page", function (event, num) {
                viewModel.pageIndex(num);
                getBlogs();
            });
            pagerInitialized = true;
        }
        getUserNames();
    }).fail(function (e) {
        alert(e);
    });
};

function getUserNames() {
    let query = new breeze.EntityQuery().from("PublicUserApi");

    let predicate = null;
    let haveAnyNew = false;
    $(userIds).each(function (index, userId) {
        if (!localStorage.getItem(localStorageUsersKey + userId)) {
            haveAnyNew = true;
            if (predicate == null) {
                predicate = breeze.Predicate.create('Id', '==', userId);
            }
            else {
                predicate = predicate.or(breeze.Predicate.create('Id', '==', userId));
            }
        }
    });

    query = query
        .where(predicate)
        .select("Id, UserName");

    if (haveAnyNew) {
        manager.executeQuery(query).then(function (data) {
            $(data.httpResponse.data.results).each(function (index, item) {
                localStorage.setItem(localStorageUsersKey + item.Id, item.UserName);
            });
            $(viewModel.entries()).each(function (index, item) {
                item.userName(localStorage.getItem(localStorageUsersKey + item.userId()));
            });
        }).fail(function (e) {
            alert(e);
        });
    }
    else {
        $(viewModel.entries()).each(function (index, item) {
            item.userName(localStorage.getItem(localStorageUsersKey + item.userId()));
        });
    }
};