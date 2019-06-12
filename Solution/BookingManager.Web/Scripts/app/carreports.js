var pageController = new PageController();

function PageController() {
};

PageController.prototype.onReportSelected = function () {
    var reportid = $('#SelectedReportId').val();
    $.ajax({
        url: 'CarReports/Get',
        traditional: true,
        data: { ReportId: reportid },
        method: 'POST',
        success: function (data) {
            $('#divReportFilters').html(data);
        }
    });
};

function downloadReport(formdata) {
    $.ajax({
        url: 'CarReports/DownloadReport',
        traditional: true,
        data: formdata,
        method: 'GET',
        success: function (response) {
            window.open(response);
        }
    });
};

PageController.prototype.onDownloadReport = function () {
    var form = $('#formReport');
    var formdata = form.serialize();

    $.ajax({
        url: 'CarReports/ValidateReportParameters',
        traditional: true,
        data: formdata,
        method: 'POST',
        success: function (result) {
            if (result.Success) {
                var reportId = $('#SelectedReport_ReportId').val();
                var format = $('#SelectedReport_Format').val();
                var fromDate = $('#SelectedReport_FromDate').val();
                var toDate = $('#SelectedReport_ToDate').val();
                var touroperatorid = $('#SelectedReport_TourOperatorId').val();
                var paymentstatusid = $('#SelectedReport_PaymentStatusId').val();
                var reservationStatus = $('#SelectedReport_CarReservationStatus').val();
                window.open("CarReports/DownloadReport?ReportId=" + reportId + "&Format=" + format + "&FromDate=" + fromDate + "&ToDate=" + toDate + "&TourOperatorId=" + touroperatorid + "&PaymentStatusId=" + paymentstatusid + "&CarReservationStatus=" + reservationStatus, "_blank");
            }
            else {
                toastr.error(result.ErrorDescription, "Oops!");
                return;
            }
        }
    });
};

