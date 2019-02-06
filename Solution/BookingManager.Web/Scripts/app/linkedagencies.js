var pageController = new PageController();

function PageController() {
};

PageController.prototype.addLinkedAgency = function () {
    var name = $('#NewName').val();
    var contact = $('#NewContact').val();
    var phone = $('#NewPhone').val();
    var email = $('#NewEmail').val();

    $.ajax({
        url: 'Agencies/Add',
        traditional: true,
        data: { Name: name, ContactName: contact, Phone: phone, Email: email  },
        method: 'POST',
        success: function (data) {
            $('#divAgencies').html(data);
            var error = $('#ErrorDescription').val();
            if (error == "") {
                toastr.success("La agencia fue adicionada satisfactoriamente.", "Perfecto!");
            }
            else {
                toastr.error("Ha ocurrido un error adicionando esta Agencia. " + error, "Oops!");
            }
        }
    });
};

PageController.prototype.updateLinkedAgency = function (Index, LinkedAgencyId) {
    var name = $('#LinkedAgencies_' + Index + '__Name').val();
    var contact = $('#LinkedAgencies_' + Index + '__ContactName').val();
    var phone = $('#LinkedAgencies_' + Index + '__Phone').val();
    var email = $('#LinkedAgencies_' + Index + '__Email').val();

    $.ajax({
        url: 'Agencies/Update',
        traditional: true,
        data: { Id: LinkedAgencyId, Name: name, ContactName: contact, Phone: phone, Email: email },
        method: 'POST',
        success: function (data) {
            $('#divAgencies').html(data);
            var error = $('#ErrorDescription').val();
            if (error == "") {
                toastr.success("La agencia fue actualizada satisfactoriamente.", "Perfecto!");
            }
            else {
                toastr.error("Ha ocurrido un error modificando esta Agencia. " + error, "Oops!");
            }
        }
    });
};

