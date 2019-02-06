var pageController = new PageController();

function PageController() {
};

PageController.prototype.markClientAsTroubled = function (clientid,mark) {
    $.ajax({
        url: 'MarkAsTroubled',
        traditional: true,
        data: { ClientId: clientid, Mark: mark },
        method: 'POST',
        success: function (result) {
            if (result.Success) {
                location.reload();
            }
            else {
                $('#troubledConfirmationModal').modal('hide');
                toastr.clear();
                toastr.error("Ha ocurrido un error al intentar guardar estos cambios. Por favor desconéctese de su sesión y conéctese nuevamente.", "Oops !!");
            }
        }
    });
};