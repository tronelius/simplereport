angular.module('subscriptions', ['shared', 'ui.bootstrap', 'repository', 'schedule']).config(['uibDatepickerConfig', 'uibDatepickerPopupConfig', function (datepickerConfig, datepickerPopupConfig) {
    datepickerConfig.startingDay = 1;

    datepickerPopupConfig.startingDay = 1;
}]);