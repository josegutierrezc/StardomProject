var pageController = new PageController();

function PageController() {
};

$(document).ready(function () {
    $.ajax({
        url: 'Home/GetStatistics',
        traditional: true,
        data: { DataName: 'carreservationsperagent' },
        method: 'POST',
        success: function (data) {
            if (data.Success) {
                var lineData = {
                    labels: data.Labels,
                    datasets: [
                        {
                            label: data.SecondYear,
                            backgroundColor: 'rgba(26,179,148,0.5)',
                            borderColor: "rgba(26,179,148,0.7)",
                            pointBackgroundColor: "rgba(26,179,148,1)",
                            pointBorderColor: "#fff",
                            data: data.SecondYearData
                        }, {
                            label: data.FirstYear,
                            backgroundColor: 'rgba(220, 220, 220, 0.5)',
                            pointBorderColor: "#fff",
                            data: data.FirstYearData
                        }
                    ]
                };

                var lineOptions = {
                    responsive: true
                };

                var ctx = $('#lineChart');
                new Chart(ctx, { type: 'line', data: lineData, options: lineOptions });
            }
            else {
                toastr.clear();
                toastr.error(data.ErrorDescription, "Oops!")
            }
        }
    });

    var cansee = $('#CanSeePaymentsChart').val();
    if (cansee == "False") return;

    $.ajax({
        url: 'Home/GetStatistics',
        traditional: true,
        data: { DataName: 'paymentsbydate' },
        method: 'POST',
        success: function (data) {
            if (data.Success) {
                var lineData = {
                    labels: data.Labels,
                    datasets: [
                        {
                            label: data.FromYear,
                            backgroundColor: 'rgba(26,179,148,0.5)',
                            borderColor: "rgba(26,179,148,0.7)",
                            pointBackgroundColor: "rgba(26,179,148,1)",
                            pointBorderColor: "#fff",
                            data: data.FirstYearData
                        }, {
                            label: data.ComparedWithYear,
                            backgroundColor: 'rgba(220, 220, 220, 0.5)',
                            pointBorderColor: "#fff",
                            data: data.SecondYearData
                        }
                    ]
                };

                var lineOptions = {
                    responsive: true
                };

                var ctx = $('#paymentChart');
                new Chart(ctx, { type: 'line', data: lineData, options: lineOptions });
            }
            else {
                toastr.clear();
                toastr.error(data.ErrorDescription, "Oops!")
            }
        }
    });

})

