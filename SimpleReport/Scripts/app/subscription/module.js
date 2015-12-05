angular.module('subscriptions', ['shared', 'ui.bootstrap', 'repository', 'schedule']).config(['datepickerConfig', 'datepickerPopupConfig', function (datepickerConfig, datepickerPopupConfig) {
    datepickerConfig.startingDay = 1;

    datepickerPopupConfig.startingDay = 1;
}]);