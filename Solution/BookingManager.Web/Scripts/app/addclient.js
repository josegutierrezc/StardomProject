var pageController = new PageController();

function PageController() {
};

$(document).ready(function () {
    $.fn.datepicker.defaults.format = "dd/mm/yyyy";
    $.fn.datepicker.defaults.autoclose = true;
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy'
    });
});

function submitForm(saveandreturn) {
    var form = $('#addForm');
    var formdata = form.serialize();
    $.ajax({
        url: 'Add',
        traditional: true,
        data: formdata,
        method: 'POST',
        success: function (data) {
            if (data.Success) {
                if (saveandreturn)
                    window.location.href = "/Clients?AssignToReservationId=-1";
                else
                    window.location.href = "/CarReservations/Add?ClientId=" + data.Data;
            }
            else {
                toastr.clear();
                toastr.error(data.ErrorDescription, "Oops!")
            }
        }
    });
};

PageController.prototype.saveAndReturn = function () {
    submitForm(true);
};

PageController.prototype.saveAndCreateNewCarReservation = function () {
    submitForm(false);
};