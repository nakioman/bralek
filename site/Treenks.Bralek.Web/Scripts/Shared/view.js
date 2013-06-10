function MenuItem(title, siteUrl, id, unreadItems) {
    var self = this;
    this.Title = ko.observable(title);
    this.SiteUrl = ko.observable(siteUrl);
    this.Id = ko.observable(id);
    this.UnreadItems = ko.observable(unreadItems);
    this.FormatedTitle = ko.computed(function () {
        var title = self.SiteUrl();
        if (self.Title() != '') {
            title = self.Title();
        }
        if (parseInt(self.UnreadItems()) > 0) {
            return title + ' <b>(' + self.UnreadItems() + ')</b>';
        }
        return title;
    });
    this.FavIconUrl = ko.computed(function () {
        return 'http://getfavicon.appspot.com/' + self.SiteUrl();
    });
}
var ViewModel = {
    Subscriptions: ko.observableArray([]),
    MapMenuItemsFromJS: function (data) {
        var dataFromServer = ko.utils.parseJson(data);
        var mappedData = ko.utils.arrayMap(dataFromServer, function (item) {
            return new MenuItem(item.Title, item.SiteUrl, item.Id, item.UnreadItems);
        });
        ViewModel.Subscriptions(mappedData);
    },
    MenuItemClick: function (data, event) {
        $('#homeMenu li').each(function () {
            $(this).removeClass('active');
        });
        $(event.currentTarget).parent().addClass('active');
        var id = null;
        if (typeof data.Id !== 'undefined') {
            id = data.Id();
        } else if ($(event.currentTarget).data('id') == -1) {
            id = -1;
        }
        HomeIndex._currentPage = 0;
        HomeIndex._currentFeedId = id;
        $('#main').html('');
        HomeIndex.GetSubscriptions(HomeIndex._currentPage, HomeIndex._currentFeedId);
        HomeIndex.IsSearching = false;
    },
    ItemFromClick: function (data, event) {
        var feedId = $(event.currentTarget).data('feedid');
        $('#subscriptionsItems').find('a').each(function () {
            if ($(this).data('id') == feedId) {
                $(this).click();
            }
        });
    }
};
ViewModel.GetSubscriptionItem = function (itemId) {
    var item = ko.utils.arrayFirst(this.Subscriptions(), function (subscription) {
        return subscription.Id() == itemId;
    });
    return item;
};
ViewModel.AllUnreadItems = ko.computed(function () {
    var total = 0;
    ko.utils.arrayForEach(this.Subscriptions(), function (item) {
        total += parseInt(item.UnreadItems());
    });
    if (total > 0)
        return '(' + total + ')';
    return '';
}, ViewModel);
ViewModel.AfterRenderHomeItems = function (elements) {
    $('img', elements).unveil();
    Common.SetupTitleOverflow();
};

$(function () {
    ko.applyBindings(ViewModel);
});