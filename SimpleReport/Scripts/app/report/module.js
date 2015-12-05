angular.module('report', ['shared', 'ui.bootstrap', 'repository', 'subscriptions']).config(['datepickerConfig', 'datepickerPopupConfig', function (datepickerConfig, datepickerPopupConfig) {
    datepickerConfig.startingDay = 1;

    datepickerPopupConfig.startingDay = 1;
}]);;

