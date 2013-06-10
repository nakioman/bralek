var HomeIndex = {
    _subscriptionAction: null,
    _currentPage: null,
    _pixelsBeforeBottomToPreloadPage: 0,
    _currentFeedId: null,
    _postMarkAsReadAction: null,
    _IsEmpty: false,
    _orderByOldest: null,
    _showAllItems: null,
    _isLoadingPage: false,
    _bookmarkAction: null,
    IsSearching: false,
    _subscriptionMenuItemsUpdatedAction: null,
    GetSubscriptions: function (pageNumber, feedId) {
        HomeIndex._isLoadingPage = true;
        HomeIndex._IsEmpty = false;
        HomeIndex.SetupLoadingElement(true);
        $.get(HomeIndex._subscriptionAction, {
            page: pageNumber,
            subscriptionId: feedId,
            orderByOldest: HomeIndex._orderByOldest,
            showAllPosts: HomeIndex._showAllItems
        }).done(function (data) {
            HomeIndex.SetupLoadingElement(false);
            try {
                $('#main').append(data);
            } catch (e) {
                console.log(e.message);
            }
            HomeIndex.SetupPublisDates();
            HomeIndex.SetupAnchorsAndRemoveStyles();
            HomeIndex.SetupSubscriptionItems(true);
            HomeIndex._currentPage = pageNumber;
            HomeIndex._isLoadingPage = false;
            ko.applyBindings(ViewModel, $('#main')[0]);
            HomeIndex.SetupSubscriptionsLoadNewItems();
            HomeIndex.SetupBookmarkAction();
        }).fail(function () {
            HomeIndex.SetupLoadingElement(false);
            $('#alertErrorLoadingItems').show('slow');
            HomeIndex._isLoadingPage = false;
        });
    },
    SetupLoadingElement: function (show) {
        if (show) {
            $('#main').append('<div class="row-fluid" id="loading"><div class="span12 text-center"><i class="icon-spinner icon-spin icon-large"></i></div></div>');
        } else {
            $('#loading').remove();
        }
    },
    SetupPublisDates: function () {
        $('.publishdate').each(function () {
            var utcServerDate = $(this).data('publishdateutc');
            var date = new Date(utcServerDate + ' UTC');
            $(this).text(date.toLocaleDateString() + ' ' + date.toLocaleTimeString());
        });
    },
    SetupAnchorsAndRemoveStyles: function () {
        $('.subscriptionItem .span12').find('a').each(function () {
            if (!$(this).hasClass('from') && !$(this).hasClass('bookmark') && $(this).parent('h3').length == 0) {
                $(this).attr('target', '_blank');
            }
        });
        $('.itemContent').each(function () {
            $(this).find('style').each(function () {
                $(this).remove();
            });
        });
    },
    SetupInfiniteScroll: function () {
        HomeIndex.SetupSubscriptionItems();
        if (HomeIndex._ShouldLoadNextPage()) {
            if (HomeIndex.IsSearching) {
                Common.GetSubscriptionsFromSearch();
            } else {
                HomeIndex.GetSubscriptions(HomeIndex._currentPage + 1, HomeIndex._currentFeedId);
            }

        }
    },
    _ShouldLoadNextPage: function () {
        var bottom = $(window).scrollTop() + $(window).height();
        var atBottom = $('#main').height() - bottom;
        return (atBottom < HomeIndex._pixelsBeforeBottomToPreloadPage && !HomeIndex._IsEmpty && !HomeIndex._isLoadingPage);
    },
    SetupSubscriptionItems: function (firstItem) {
        $('.subscriptionItem').each(function () {
            $(this).removeClass('subscriptionItem-focus');
        });
        var itemInViewPort = $('.subscriptionItem:in-viewport').length;
        if (itemInViewPort > 1 && !firstItem) {
            var secondItem = $($('.subscriptionItem:in-viewport')[1]);
            secondItem.addClass('subscriptionItem-focus');
            HomeIndex.MarkItemAsRead(secondItem);
        }
        else {
            var item = $('.subscriptionItem:in-viewport:first');
            item.addClass('subscriptionItem-focus');;
            HomeIndex.MarkItemAsRead(item);
        }
    },
    MarkItemAsRead: function (subscriptionItem) {
        var itemId = subscriptionItem.data('id');
        var feedId = subscriptionItem.data('feedid');
        var alreadyRead = subscriptionItem.data('alreadyread');
        if (!alreadyRead) {
            if (!isNaN(itemId)) {
                $.ajax({
                    url: HomeIndex._postMarkAsReadAction,
                    type: "POST",
                    data: Common.AddAntiForgeryTokenToData({ id: itemId }),
                }).done(function (data) {
                    if (data == "MARKED") {
                        $(ViewModel.Subscriptions()).each(function () {
                            if (this.Id() == feedId) {
                                subscriptionItem.data('alreadyread', true);
                                var unreadItems = this.UnreadItems() - 1;
                                this.UnreadItems(unreadItems);
                            }
                        });
                    }
                }).fail(function () {
                    $('#alertErrorMarkasReadItems').show('slow');
                });
            }
        }
    },
    Init: function (subscriptionActionLink, postMarkAsReadActionLink, orderByOldestFirst, showAllItems,
        subscriptionMenuItemsUpdatedAction, bookmarkAction) {
        HomeIndex._bookmarkAction = bookmarkAction;
        HomeIndex._subscriptionMenuItemsUpdatedAction = subscriptionMenuItemsUpdatedAction;
        HomeIndex._showAllItems = showAllItems;
        HomeIndex._orderByOldest = orderByOldestFirst;
        HomeIndex._subscriptionAction = subscriptionActionLink;
        HomeIndex._postMarkAsReadAction = postMarkAsReadActionLink;
        HomeIndex.SetupOrderFilterLink();
        HomeIndex.SetupShowAllItemsFilterLink();
        $(window).scroll(HomeIndex.SetupInfiniteScroll);
        HomeIndex.SetupFeedsRefreshInterval();
    },
    SetupFeedsRefreshInterval: function () {
        var tenMinutesInMiliSeconds = 600000;
        setInterval(function () {
            $.getJSON(HomeIndex._subscriptionMenuItemsUpdatedAction).done(function (data) {
                var items = $.parseJSON(data);
                $(items).each(function () {
                    var itemToUpdate = ViewModel.GetSubscriptionItem(this.Id);
                    itemToUpdate.UnreadItems(this.UnreadItems);
                    if (HomeIndex._currentFeedId == this.Id || HomeIndex._currentFeedId == null) {
                        $('#newItemsAvailable').show('slow');
                    }
                });
            }).fail(function () {
                $('#alertErrorLoadingItems').show('slow');
            });
        }, tenMinutesInMiliSeconds);
    },
    SetupSubscriptionsLoadNewItems: function () {
        $('#newItemsAvailable .close').click(function () {
            $('#newItemsAvailable').hide('slow');
        });
        $('#newItemsAvailable small a').click(function () {
            $('#main').html('');
            HomeIndex.GetSubscriptions(0, HomeIndex._currentFeedId);
        });
    },
    MarkFeedAsEmpty: function () {
        HomeIndex._IsEmpty = true;
    },
    SetupOrderFilter: function () {
        $('#oldestFirstFilter a i').removeClass();
        if (HomeIndex._orderByOldest) {
            $('#oldestFirstFilter a i').addClass('icon-check');
        } else {
            $('#oldestFirstFilter a i').addClass('icon-check-empty');
        }
    },
    SetupOrderFilterLink: function () {
        HomeIndex.SetupOrderFilter();
        $('#oldestFirstFilter a').click(function () {
            HomeIndex._orderByOldest = !HomeIndex._orderByOldest;
            HomeIndex.SetupOrderFilter();
            $('#main').html('');
            HomeIndex.GetSubscriptions(0, HomeIndex._currentFeedId);
        });
    },
    SetupShowAllItemsFilter: function () {
        $('#showAllItemsFilter a i').removeClass();
        if (HomeIndex._showAllItems) {
            $('#showAllItemsFilter a i').addClass('icon-check');
        } else {
            $('#showAllItemsFilter a i').addClass('icon-check-empty');
        }
    },
    SetupShowAllItemsFilterLink: function () {
        HomeIndex.SetupShowAllItemsFilter();
        $('#showAllItemsFilter a').click(function () {
            HomeIndex._showAllItems = !HomeIndex._showAllItems;
            HomeIndex.SetupShowAllItemsFilter();
            $('#main').html('');
            HomeIndex.GetSubscriptions(0, HomeIndex._currentFeedId);
        });
    },
    SetupBookmarkAction: function () {
        $('.bookmark').unbind('click');
        $('.bookmark').click(function (event) {
            var addBookmark = false;
            if ($(this).parents('.subscriptionItem').find('.bookmark i').hasClass('icon-bookmark')) {
                $(this).parents('.subscriptionItem').find('.bookmark i').removeClass();
                $(this).parents('.subscriptionItem').find('.bookmark i').addClass('icon-bookmark-empty');
            } else {
                $(this).parents('.subscriptionItem').find('.bookmark i').removeClass();
                $(this).parents('.subscriptionItem').find('.bookmark i').addClass('icon-bookmark');
                addBookmark = true;
            }
            var itemId = $(this).parents('.subscriptionItem').data('id');
            $.post(HomeIndex._bookmarkAction, Common.AddAntiForgeryTokenToData({ add: addBookmark, id: itemId }));
            event.preventDefault();
        });
    }
};