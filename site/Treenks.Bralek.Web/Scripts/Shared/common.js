var Common = {
    _subscriptionItems: null,
    _markItemsAsReadAction: null,
    FormInlineValidationSetup: function (formId) {
        var form = $('#' + formId);
        $.validator.unobtrusive.parse(form);
        form.makeValidationInline();
    },
    SetupHomeMenu: function () {
        $('#subscriptionsMainItem').click(function () {
            var icon = $('#subscriptionsMainItem i');
            if (icon.hasClass('icon-caret-down')) {
                icon.removeClass('icon-caret-down');
                icon.addClass('icon-caret-right');
            } else {
                icon.removeClass('icon-caret-right');
                icon.addClass('icon-caret-down');
            }
        });
    },
    SetupSignOutAnchor: function () {
        $('#signoutAnchor').click(function () {
            $('#logoffForm').submit();
        });
    },
    SetupAlerts: function () {
        $('#homeAlerts a').click(function () {
            var height = $('#homeAlerts').height();
            $("#mainSection").animate({ 'margin-top': '-=' + height + 'px' }, 200);
            $('#homeAlerts').fadeOut(200, function () {
                $("#mainSection").animate({ 'margin-top': '+=' + height + 'px' }, 0);
            });
        });

        if ($('#homeAlerts').text().trim() != 'x') {
            $('#homeAlerts').show();
        }
    },
    SetupTitleOverflow: function () {
        $('.word-wrap').bind('mouseenter', function () {
            var $this = $(this).find('small');
            if (this.offsetWidth < this.scrollWidth && !$this.attr('title'))
                $this.attr('title', $this.text());
        });
    },
    SetupPopup: function () {
        $('#popup .popup-close').click(function () {
            $('#popup').modal('hide');
        });

        $('#popup').on('hidden', function () {
            $(this).data('modal').$element.removeData();
        });
    },
    SetupSubscriptionItemsViewModels: function () {
        $.getJSON(Common._subscriptionItems).done(function (data) {
            ViewModel.MapMenuItemsFromJS(data);
            $('#allItems a').click();
        }).fail(function () {
            $('#alertErrorLoadingItems').show('slow');
        });
    },
    SetupMarkItemsAsReadMenu: function () {
        var markAsRead = function (timeFrameOption) {
            $.post(Common._markItemsAsReadAction, Common.AddAntiForgeryTokenToData({ timeFrame: timeFrameOption }))
                .done(function () {
                    Common.SetupSubscriptionItemsViewModels();
                    HomeIndex.GetSubscriptions(0, HomeIndex._currentFeedId);
                }).fail(function () {
                    $('#alertErrorMarkasReadItems').show('slow');
                });
        };
        $('#markReadOlderOneDay').click(function () {
            markAsRead("DAY");
        });
        $('#markReadOlderOneWeek').click(function () {
            markAsRead("WEEK");
        });
        $('#markReadOlderOneMonth').click(function () {
            markAsRead("MONTH");
        });
        $('#markReadOlderAll').click(function () {
            markAsRead("ALL");
        });
    },
    AddAntiForgeryTokenToData: function (data) {
        data.__RequestVerificationToken = $('#antiForgeryToken input[name=__RequestVerificationToken]').val();
        return data;
    },
    Init: function (subscriptionItemActionUrl, markItemsAsReadAction) {
        Common._subscriptionItems = subscriptionItemActionUrl;
        Common._markItemsAsReadAction = markItemsAsReadAction;
        Common.SetupSubscriptionItemsViewModels();
        Common.SetupSignOutAnchor();
        Common.SetupHomeMenu();
        Common.SetupAlerts();
        Common.SetupPopup();
        Common.SetupMarkItemsAsReadMenu();
        Common.SetupAjaxErrorAlertsCloseButtons();
    },
    SearchFeedItems: function () {
        if (!HomeIndex._isLoadingPage) {
            $('#searchFromPage').val(0);
        }
        HomeIndex._isLoadingPage = true;
        $('#homeMenu li').each(function () {
            $(this).removeClass('active');
        });
        if ($('#searchFromPage').val() == 0) {
            $('#main').html('');
        }
        HomeIndex.SetupLoadingElement(true);
        HomeIndex.IsSearching = true;
    },
    UpdateSearchFeedItes: function (data) {
        HomeIndex.SetupLoadingElement(false);
        try {
            $('#main').append(data);
        } catch (e) {
            console.log(e.message);
        }
        HomeIndex.SetupPublisDates();
        HomeIndex.SetupAnchorsAndRemoveStyles();
        ko.applyBindings(ViewModel, $('#main')[0]);
        HomeIndex.SetupBookmarkAction();
        HomeIndex._isLoadingPage = false;
    },
    GetSubscriptionsFromSearch: function () {
        HomeIndex._isLoadingPage = true;
        var page = parseInt($('#searchFromPage').val()) + 1;
        $('#searchFromPage').val(page);
        $('#frmSearchForm').submit();
    },
    SetupAjaxErrorAlertsCloseButtons: function () {
        $('#alertErrorLoadingItems a').click(function () {
            $('#alertErrorLoadingItems').hide('slow');
        });
        $('#alertErrorMarkasReadItems a').click(function () {
            $('#alertErrorMarkasReadItems').hide('slow');
        });
    }
};