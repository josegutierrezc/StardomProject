var appController = new AppController();

function AppController() {
    this.SpinnerTimer = undefined;
};

AppController.prototype.showSpinner = function () {
    appController.SpinnerTimer = setInterval(function () {
        $('#overlay').removeClass('spinner-hidden');
        $('#overlay').addClass('spinner-visible');
    }, 1000);
}

AppController.prototype.hideSpinner = function () {
    clearInterval(appController.SpinnerTimer);
    $('#overlay').addClass('spinner-hidden');
    $('#overlay').removeClass('spinner-visible');
}