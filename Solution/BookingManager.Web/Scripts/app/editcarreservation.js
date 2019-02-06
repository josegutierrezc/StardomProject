var pageController = new PageController();

function PageController() {
};

$(document).ready(function () {
    $.fn.datepicker.defaults.format = "dd/mm/yyyy";
    $.fn.datepicker.defaults.autoclose = true;
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy'
    });
    $("input[name='Discount']").TouchSpin({
        min: -1000000000,
        max: 1000000000,
        stepinterval: 50,
        maxboostedstep: 10000000,
        prefix: '$'
    });
    $('#Discount').on('change', pageController.onDiscountChange);
});

function submitForm() {
    var form = $('#formEdit');
    var formdata = form.serialize();

    $.ajax({
        url: 'Edit',
        traditional: true,
        data: formdata,
        method: 'POST',
        success: function (data) {
            if (data.Success) {
                if (data.CarReservationId == -1) {
                    window.location.href = "/CarReservations";
                }
                else {
                    toastr.success("La reserva fue guardada satisfactoriamente", "Actualizada!")
                    return;
                }
            }
            else {
                toastr.clear();
                toastr.error('Ocurrió un error intentando modificar esta reserva. ' + data.ErrorDescription, 'Oops !!!')
            }
        }
    });
}

PageController.prototype.saveAndStay = function () {
    $('#SaveAndStay').val('true');
    submitForm();
};

PageController.prototype.saveAndReturn = function () {
    $('#SaveAndStay').val('false');
    submitForm();
};

PageController.prototype.cancel = function (carreservationid) {
    $.ajax({
        url: 'Cancel',
        traditional: true,
        data: { CarReservationId: carreservationid },
        method: 'POST',
        success: function (data) {
            if (data.Success) {
                window.location.reload();
            }
            else {
                toastr.clear();
                toastr.error('Ocurrió un error intentando cancelar esta reserva. Por favor, cierre su sesión e intente conectarse nuevamente.', 'Oops !!!')
            }
        }
    });
};

PageController.prototype.onCancel = function () {
    $('#confirmCancellationModal').modal('show');
}

PageController.prototype.onRecover = function (carreservationid) {
    $.ajax({
        url: 'Recover',
        traditional: true,
        data: { CarReservationId: carreservationid },
        method: 'POST',
        success: function (data) {
            if (data.Success) {
                window.location.reload();
            }
            else {
                toastr.clear();
                toastr.error('Ocurrió un error intentando recuperar esta reserva. Por favor, cierre su sesión e intente conectarse nuevamente.', 'Oops !!!')
            }
        }
    });    
}

function updatePaymentBalance() {
    var totalPaid = $('#TotalPaid').val();
    var finalPrice = $('#FinalPrice').val();
    var paymentBalance = finalPrice - totalPaid;
    var formattedValue = '$' + parseFloat(paymentBalance, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
    $('#PaymentBalance').text(formattedValue);
};

PageController.prototype.onDiscountChange = function () {
    pageController.updatePrice();
};

PageController.prototype.onProvinceChanged = function () {
    var agencynumber = $('#AgencyNumber').val();
    var provinceid = $('#ProvinceId').val();

    $.ajax({
        url: 'RentCarPlaces',
        traditional: true,
        data: { AgencyNumber: agencynumber, ProvinceId: provinceid },
        method: 'GET',
        success: function (result) {
            $('#RentCarPlaceId').empty();
            $.each(result.Data, function (key, value) {
                $('#RentCarPlaceId').append($("<option></option>")
                    .attr("value", value.Id).text(value.Name));
            });
        }
    });
};

PageController.prototype.onCarProviderChanged = function () {
    var agencynumber = $('#AgencyNumber').val();
    var carproviderid = $('#CarProviderId').val();

    $.ajax({
        url: 'CarCategories',
        traditional: true,
        data: { AgencyNumber: agencynumber, CarProviderId: carproviderid },
        method: 'GET',
        success: function (result) {
            $('#CarCategoryId').empty();
            $.each(result.Data, function (key, value) {
                $('#CarCategoryId').append($("<option></option>")
                    .attr("value", value.Id).text(value.Name));
            });
        }
    });

    $.ajax({
        url: 'TourOperators',
        traditional: true,
        data: { AgencyNumber: agencynumber, CarProviderId: carproviderid },
        method: 'GET',
        success: function (result) {
            $('#TourOperatorId').empty();
            $.each(result.Data, function (key, value) {
                $('#TourOperatorId').append($("<option></option>")
                    .attr("value", value.Id).text(value.Name));
            });
        }
    });

    pageController.updatePrice();
};

PageController.prototype.updatePrice = function () {
    var agencynumber = $('#AgencyNumber').val();
    var carcategoryid = $('#CarCategoryId').val();
    var touroperatorid = $('#TourOperatorId').val();
    var fromdate = $('#FromDate').val();
    var todate = $('#ToDate').val();
    var discount = $('#Discount').val();
    var currentcostprice = $('#CostPrice').val();
    var currentsaleprice = $('#SalePrice').val();

    $.ajax({
        url: 'UpdatePrices',
        traditional: true,
        data: { AgencyNumber: agencynumber, CarCategoryId: carcategoryid, TourOperatorId: touroperatorid, FromDate: fromdate, ToDate: todate, Discount: discount, CurrentCostPrice: currentcostprice, CurrentSalePrice: currentsaleprice },
        method: 'GET',
        success: function (result) {
            $('#Discount').off('change', pageController.onDiscountChange);
            $('#divPriceConfiguration').html(result);
            $("input[name='Discount']").TouchSpin({
                min: -1000000000,
                max: 1000000000,
                stepinterval: 50,
                maxboostedstep: 10000000,
                prefix: '$'
            });
            $('#Discount').on('change', pageController.onDiscountChange);

            var pricefound = $('#PriceConfigurationFound').val();
            if (pricefound == "False") { toastr.warning('No se encontró una configuración de precio para esta reserva. Por favor tenga cuidado al modificar sus datos, los descuentos y pagos pueden modificarse sin que afecten la reserva.', 'Atención !!!'); }

            updatePaymentBalance();
        }
    });
};

PageController.prototype.addPayment = function () {
    var agencynumber = $('#AgencyNumber').val();
    var reservationid = $('#Id').val();
    var paymentconceptid = $('#NewPayment_ConceptId').val();
    var paymentmethodid = $('#NewPayment_MethodId').val();
    var amount = $('#NewPayment_Amount').val();
    var isreimb = $('#NewPayment_IsReimbursement').val() == "0" ? false : true;

    if (reservationid == -1) {
        toastr.clear();
        toastr.error("Necesita salvar esta reserva antes de poder adicionar pagos.", "Oops !!");
        return;
    }
    if (amount <= 0) {
        toastr.clear();
        toastr.error("La cantidad a pagar no puede ser igual o menor que cero.", "Oops !!");
        return;
    }

    $.ajax({
        url: 'AddPayment',
        traditional: true,
        data: { AgencyNumber: agencynumber, CarReservationId: reservationid, PaymentConceptId: paymentconceptid, PaymentMethodId: paymentmethodid, Amount: amount, IsReimbursement: isreimb },
        method: 'POST',
        success: function (result) {
            if (result.Success) {
                $('#divPayments').html(result.Data);
                toastr.clear();
                toastr.success("El pago fue satisfactoriamente adicionado.", "Perfecto !");
                updatePaymentBalance();
            }
            else
            {
                toastr.clear();
                toastr.error("Ha ocurrido un error al intentar adicionar este pago. Por favor desconéctese de su sesión y conéctese nuevamente.", "Oops !!");
            }
        }
    });
};

PageController.prototype.removePayment = function (paymentid) {
    var agencynumber = $('#AgencyNumber').val();
    var reservationid = $('#Id').val();
    
    if (reservationid == -1) {
        toastr.clear();
        toastr.error("Necesita salvar esta reserva antes de poder adicionar pagos.", "Oops !!");
        return;
    }
    
    $.ajax({
        url: 'RemovePayment',
        traditional: true,
        data: { AgencyNumber: agencynumber, CarReservationId: reservationid, PaymentId: paymentid },
        method: 'POST',
        success: function (result) {
            if (result.Success) {
                $('#divPayments').html(result.Data);
                toastr.clear();
                toastr.success("El pago fue satisfactoriamente removido.", "Perfecto !");
                updatePaymentBalance();
            }
            else {
                toastr.clear();
                toastr.error("Ha ocurrido un error al intentar remover este pago. Por favor desconéctese de su sesión y conéctese nuevamente.", "Oops !!");
            }
        }
    });
};

PageController.prototype.viewOrChangeClient = function () {
    $('#viewOrChangeClientModal').show('modal');
};

PageController.prototype.downloadVoucher = function () {
    var carreservationid = $('#Id').val();
    $.ajax({
        url: 'DownloadVoucher',
        traditional: true,
        data: { CarReservationId: carreservationid, Format: "pdf" },
        method: 'GET',
        success: function (result) {
            
        }
    });
}