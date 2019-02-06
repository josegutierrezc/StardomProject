var pageController = new PageController();

function PageController() {
};

$(document).ready(function () {
    $('.table').footable();
});

function getClients(pagesize, pagenumber, searchfor) {
    var assignTo = $('#AssignToReservationId').val();
    $.ajax({
        url: 'Clients/Get',
        traditional: true,
        data: { AssignToReservationId: assignTo, PageSize: pagesize, PageNumber: pagenumber, SearchFor: searchfor },
        method: 'GET',
        success: function (data) {
            $('#divClients').html(data);
            $('.table').footable();
        }
    });
};

PageController.prototype.onActivePageSizeChanged = function (pagesize) {
    var pagenumber = $('#PageNumber').val();
    var searchfor = $('#SearchFor').val();
    getClients(pagesize, pagenumber, searchfor);
};

PageController.prototype.onActivePageSelected = function (pagenumber) {
    var pagesize = $('#PageSize').val();
    var searchfor = $('#SearchFor').val();
    getClients(pagesize, pagenumber, searchfor);
};

PageController.prototype.onActiveSearch = function () {
    var searchfor = $('#SearchFor').val();
    var pagesize = $('#PageSize').val();
    var pagenumber = $('#PageNumber').val();
    getClients(pagesize, pagenumber, searchfor);
};

PageController.prototype.onFirstPageSelected = function () {
    var searchfor = $('#SearchFor').val();
    var pagesize = parseInt($('#PageSize').val());
    var pagenumber = parseInt($('#PageNumber').val());
    if (pagenumber == 1) return;

    getClients(pagesize, 1, searchfor);
};

PageController.prototype.onPreviousPageSelected = function () {
    var searchfor = $('#SearchFor').val();
    var pagesize = parseInt($('#PageSize').val());
    var pagenumber = parseInt($('#PageNumber').val());
    if (pagenumber == 1) return;

    getClients(pagesize, pagenumber - 1, searchfor);
};

PageController.prototype.onNextPageSelected = function () {
    var searchfor = $('#SearchFor').val();
    var pagesize = parseInt($('#PageSize').val());
    var pagenumber = parseInt($('#PageNumber').val());
    var totalpages = parseInt($('#TotalPages').val());

    if (pagenumber == totalpages) return;

    getClients(pagesize, pagenumber + 1, searchfor);
};

PageController.prototype.onLastPageSelected = function () {
    var searchfor = $('#SearchFor').val();
    var pagesize = parseInt($('#PageSize').val());
    var pagenumber = parseInt($('#PageNumber').val());
    var totalpages = parseInt($('#TotalPages').val());

    if (pagenumber == totalpages) return;

    getClients(pagesize, totalpages, searchfor);
};

PageController.prototype.onClientSelected = function (clientid) {
    var assignTo = $('#AssignToReservationId').val();
    $.ajax({
        url: '/Clients/GetDetails',
        traditional: true,
        data: { AssignToReservationId: assignTo, ClientId: clientid },
        method: 'GET',
        success: function (data) {
            $('#divClientDetails').html(data);
        }
    });
}

PageController.prototype.onClientRemoved = function (clientid) {
    var rowId = "#row" + clientid;
    $.ajax({
        url: '/Clients/Remove',
        traditional: true,
        data: { ClientId: clientid },
        method: 'POST',
        success: function (result) {
            if (result.Success) {
                $(rowId).remove();
                $('.table').footable();
                toastr.success("El cliente fue satisfactoriamente eliminado.", "Perfecto!");
            }
            else {
                toastr.error("Ocurrió un error al intentar eliminar este cliente. Por favor intentelo nuevamente.", "Oops!")
            }
        }
    });
}