var pageController = new PageController();

function PageController() {
};

$(document).ready(function () {
    $('.table').footable();
});

PageController.prototype.onAgentSelected = function (username) {
    $.ajax({
        url: '/Agents/GetDetails',
        traditional: true,
        data: { Username: username },
        method: 'GET',
        success: function (data) {
            $('#divAgentDetails').html(data);
        }
    });
}

PageController.prototype.onPrivilegeChanged = function (username, privilegename, checked) {
    $.ajax({
        url: '/Agents/UpdatePrivilege',
        traditional: true,
        data: { Username: username, PrivilegeName: privilegename, IsSelected: checked },
        method: 'POST',
        success: function (data) {
            if (data.Success) {
                toastr.success("El permiso se ha modificado satisfactoriamente", "Perfecto!");
            }
            else {
                toastr.error("Ha ocurrido un error al intentar modificar los permisos de este agente.", "Oops!!")
            }
        }
    });
}