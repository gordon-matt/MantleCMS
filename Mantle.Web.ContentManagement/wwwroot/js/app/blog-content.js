'use strict'

const localStorageRootKey = "BlogContent_";
const localStorageUsersKey = localStorageRootKey + "Users_";

class BlogEntryModel {
    constructor() {
        this.headline = ko.observable(null);
        this.userId = ko.observable(null);
        this.userName = ko.observable(null);
        this.dateCreatedUtc = ko.observable(null);
        this.teaserImageUrl = ko.observable(null);
        this.shortDescription = ko.observable(null);
        this.fullDescription = ko.observable(null);
        this.useExternalLink = ko.observable(false);
        this.externalLink = ko.observable(null);
    }

    showDetails = () => {
        viewModel.selected(this);
        switchSection($('#details-section'));
    };
}

class ViewModel {
    constructor() {
        this.entries = ko.observableArray([]);
        this.selected = ko.observable(new BlogEntryModel());

        this.total = ko.observable(0);
        this.pageIndex = ko.observable(1);
        this.pageSize = settings.itemsPerPage;
    }

    pageCount = () => {
        return Math.ceil(this.total() / this.pageSize);
    };

    showMain = () => {
        switchSection($('#main-section'));
    };

    showPrevious = () => {
        const selectedIndex = $.inArray(this.selected(), this.entries());
        if (selectedIndex == 0) {
            if (this.pageIndex() > 1) {
                $("li[data-lp=" + (this.pageIndex() - 1) + "] > a").click();
                setTimeout(function () {
                    const previous = this.entries()[this.entries().length - 1];
                    this.selected(previous);
                }, 500);
            }
        }
        else {
            const previous = this.entries()[selectedIndex - 1];
            this.selected(previous);
        }
    };

    showNext = () => {
        const selectedIndex = $.inArray(this.selected(), this.entries());
        if (selectedIndex == (this.entries().length - 1)) {
            if (this.pageIndex() < this.pageCount()) {
                $("li[data-lp=" + (this.pageIndex() + 1) + "] > a").click();
                setTimeout(function () {
                    const next = this.entries()[0];
                    this.selected(next);
                }, 500);
            }
        }
        else {
            const next = this.entries()[selectedIndex + 1];
            this.selected(next);
        }
    };

    canShowPrevious = () => {
        const selectedIndex = $.inArray(this.selected(), this.entries());
        if (this.pageIndex() == 1 && selectedIndex == 0) {
            return false;
        }
        return true;
    };

    canShowNext = () => {
        const selectedIndex = $.inArray(this.selected(), this.entries());
        if (this.pageIndex() == this.pageCount() && selectedIndex == (this.entries().length - 1)) {
            return false;
        }
        return true;
    };
}

breeze.config.initializeAdapterInstances({ dataService: "OData" });
const manager = new breeze.EntityManager('/odata/mantle/cms');

const pagerInitialized = false;
const userIds = [];

let viewModel;
$(document).ready(function () {
    //TODO: Maybe clear this only after a certain amount of time (not every page load)?

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

        data.httpResponse.data.results.forEach(item => {
            const entry = new BlogEntryModel();
            entry.headline(item.Headline);
            entry.userId(item.UserId);

            const date = moment(item.DateCreatedUtc);
            entry.dateCreatedUtc(date.format(settings.dateFormat));

            entry.teaserImageUrl(item.TeaserImageUrl);
            entry.shortDescription(item.ShortDescription);
            entry.fullDescription(item.FullDescription);
            entry.useExternalLink(item.UseExternalLink);
            entry.externalLink(item.ExternalLink);
            viewModel.entries.push(entry);
            userIds.push(item.UserId);
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

    userIds.forEach(userId => {
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
            data.httpResponse.data.results.forEach(item => {
                localStorage.setItem(localStorageUsersKey + item.Id, item.UserName);
            });

            viewModel.entries().forEach(item => {
                item.userName(localStorage.getItem(localStorageUsersKey + item.userId()));
            });
        }).fail(function (e) {
            alert(e);
        });
    }
    else {
        viewModel.entries().forEach(item => {
            item.userName(localStorage.getItem(localStorageUsersKey + item.userId()));
        });
    }
};