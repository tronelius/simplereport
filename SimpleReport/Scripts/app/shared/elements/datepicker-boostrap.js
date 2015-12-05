angular.module('shared').directive('datepickerBootstrap', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            $(function () {
                element.datepicker({ 'format': 'yyyy-mm-dd' }).on('changeDate', function (e) {
                    scope.$apply(function () {
                        ngModelCtrl.$setViewValue(e.currentTarget.value);
                    });
                });
            });
        }
    }
})