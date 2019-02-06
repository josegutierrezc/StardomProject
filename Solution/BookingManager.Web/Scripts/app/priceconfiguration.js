var pageController = new PageController();

function PageController() {
};

$(document).ready(function () {
    $('.table').footable();
});

PageController.prototype.onSelectionChanged = function () {
    var touroperatorid = $('#TourOperatorId').val();
    var seasonid = $('#SeasonId').val();

    if (touroperatorid == "") touroperatorid = -1;
    if (seasonid == "") seasonid = -1;

    $.ajax({
        url: 'CarPriceConfiguration/Get',
        traditional: true,
        data: { TourOperatorId: touroperatorid, SeasonId: seasonid },
        method: 'POST',
        success: function (data) {
            $('#divPriceConfiguration').html(data);
            $('.table').footable();
        }
    });
};

PageController.prototype.update = function () {
    var form = $('#formUpdate');
    var formdata = form.serialize();

    $.ajax({
        url: 'CarPriceConfiguration/Update',
        traditional: true,
        data: formdata,
        method: 'POST',
        success: function (data) {
            if (data.Success) {
                toastr.success("La configuración de precios fue actualizada satisfactoriamente", "Actualizada!")
                return;
            }
            else {
                toastr.clear();
                toastr.error('Ocurrió un error intentando actualizar esta configuración de precios. Por favor, cierre su sesión e intente conectarse nuevamente.', 'Oops !!!')
            }
        }
    });
}