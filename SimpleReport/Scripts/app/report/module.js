angular.module('report', ['shared', 'ui.bootstrap', 'repository', 'subscriptions']).config(['uibDatepickerConfig', 'uibDatepickerPopupConfig', function (datepickerConfig, datepickerPopupConfig) {
    datepickerConfig.startingDay = 1;

    datepickerPopupConfig.startingDay = 1;
}]);;

