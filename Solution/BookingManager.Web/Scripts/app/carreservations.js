var pageController = new PageController();

function PageController() {
};

function getActiveReservations(pagesize, pagenumber, searchfor, filter) {
    $.ajax({
        url: 'CarReservations/Actives',
        traditional: true,
        data: { PageSize: pagesize, PageNumber: pagenumber, SearchFor: searchfor, Filter: filter },
        method: 'GET',
        success: function (data) {
            $('#divActiveReservations').html(data);
            $('.table').footable();
        }
    });
};

function getHistoryReservations(pagesize, pagenumber, searchfor, filter) {
    $.ajax({
        url: 'CarReservations/History',
        traditional: true,
        data: { PageSize: pagesize, PageNumber: pagenumber, SearchFor: searchfor, Filter: filter },
        method: 'GET',
        success: function (data) {
            $('#divHistoryReservations').html(data);
            $('.table').footable();
        }
    });
};

PageController.prototype.onActivePageSizeChanged = function (pagesize) {
    var pagenumber = $('#PageNumber').val();
    var searchfor = $('#SearchFor').val();
    var filter = $('#Filter').val();
    getActiveReservations(pagesize, pagenumber, searchfor, filter);
};

PageController.prototype.onHistoryPageSizeChanged = function (pagesize) {
    var pagenumber = $('#HistoryPageNumber').val();
    var searchfor = $('#HistorySearchFor').val();
    var filter = $('#HistoryFilter').val();
    getHistoryReservations(pagesize, pagenumber, searchfor, filter);
};

PageController.prototype.onActivePageSelected = function (pagenumber) {
    var pagesize = $('#PageSize').val();
    var searchfor = $('#SearchFor').val();
    var filter = $('#Filter').val();
    getActiveReservations(pagesize, pagenumber, searchfor, filter);
};

PageController.prototype.onHistoryPageSelected = function (pagenumber) {
    var pagesize = $('#HistoryPageSize').val();
    var searchfor = $('#HistorySearchFor').val();
    var filter = $('#HistoryFilter').val();
    getHistoryReservations(pagesize, pagenumber, searchfor, filter);
};

PageController.prototype.onActiveSearch = function () {
    var searchfor = $('#SearchFor').val();
    var pagesize = $('#PageSize').val();
    var pagenumber = $('#PageNumber').val();
    var filter = $('#Filter').val();
    getActiveReservations(pagesize, pagenumber, searchfor, filter);
};

PageController.prototype.onHistorySearch = function () {
    var searchfor = $('#HistorySearchFor').val();
    var pagesize = $('#HistoryPageSize').val();
    var pagenumber = $('#HistoryPageNumber').val();
    var filter = $('#HistoryFilter').val();
    getHistoryReservations(pagesize, pagenumber, searchfor, filter);
};

PageController.prototype.onPreviousPageSelected = function () {
    var searchfor = $('#SearchFor').val();
    var pagesize = parseInt($('#PageSize').val());
    var pagenumber = parseInt($('#PageNumber').val());
    var filter = $('#Filter').val();
    if (pagenumber == 1) return;

    getActiveReservations(pagesize, pagenumber - 1, searchfor, filter);
};

PageController.prototype.onHistoryPreviousPageSelected = function () {
    var searchfor = $('#HistorySearchFor').val();
    var pagesize = parseInt($('#HistoryPageSize').val());
    var pagenumber = parseInt($('#HistoryPageNumber').val());
    var filter = $('#HistoryFilter').val();
    if (pagenumber == 1) return;

    getHistoryReservations(pagesize, pagenumber - 1, searchfor, filter);
};

PageController.prototype.onNextPageSelected = function () {
    var searchfor = $('#SearchFor').val();
    var pagesize = parseInt($('#PageSize').val());
    var pagenumber = parseInt($('#PageNumber').val());
    var totalpages = parseInt($('#TotalPages').val());
    var filter = $('#Filter').val();

    if (pagenumber == totalpages) return;

    getActiveReservations(pagesize, pagenumber + 1, searchfor, filter);
};

PageController.prototype.onHistoryNextPageSelected = function () {
    var searchfor = $('#HistorySearchFor').val();
    var pagesize = parseInt($('#HistoryPageSize').val());
    var pagenumber = parseInt($('#HistoryPageNumber').val());
    var totalpages = parseInt($('#HistoryTotalPages').val());
    var filter = $('#HistoryFilter').val();

    if (pagenumber == totalpages) return;

    getHistoryReservations(pagesize, pagenumber + 1, searchfor, filter);
};

PageController.prototype.onFilterChanged = function () {
    var searchfor = $('#SearchFor').val();
    var pagesize = parseInt($('#PageSize').val());
    var pagenumber = 1;
    var filter = $('#Filter').val();

    getActiveReservations(pagesize, pagenumber, searchfor, filter);
};

PageController.prototype.onHistoryFilterChanged = function () {
    var searchfor = $('#HistorySearchFor').val();
    var pagesize = parseInt($('#HistoryPageSize').val());
    var pagenumber = 1;
    var filter = $('#HistoryFilter').val();

    getHistoryReservations(pagesize, pagenumber, searchfor, filter);
};

$(document).ready(function () {
    $('.table').footable();
});