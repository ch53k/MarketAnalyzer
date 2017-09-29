angular.module('marketAnalyzer').component('appMessageBox', {
    templateUrl: 'Modules/App/partials/app.messageBox.partial.html',
    controller: AppMessageBox,
    bindings: {
        resolve: '<',
        close: '&',
        dismiss: '&'
    }
});

/* @ngInject */
function AppMessageBox() {
    var $ctrl = this;

    //#region Controller Variables
    $ctrl.title = 'Confirm';
    $ctrl.message = '';
    $ctrl.okButton = 'Yes';
    $ctrl.cancelButton = 'No';
    $ctrl.showCancelButton = true;
    //#endregion

    //#region Init
    $ctrl.$onInit = function () {
        $ctrl.message = $ctrl.resolve.message;

        if ($ctrl.resolve.title) {
            $ctrl.title = $ctrl.resolve.title;
        }

        if ($ctrl.resolve.okButton) {
            $ctrl.okButton = $ctrl.resolve.okButton;
        }

        if ($ctrl.resolve.cancelButton) {
            $ctrl.cancelButton = $ctrl.resolve.cancelButton;
        }

        if ($ctrl.resolve.showCancelButton!=undefined) {
            $ctrl.showCancelButton = $ctrl.resolve.showCancelButton;
        }
    };
    //#endregion

    //#region Buttons
    $ctrl.buttonOkClick = function () {
        $ctrl.close();
    };

    $ctrl.buttonCancelClick = function () {
        $ctrl.dismiss();
    };
    //#endregion
}