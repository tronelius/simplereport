angular.module('subscriptions').directive('cronEditor', ['$timeout', function ($timeout) {
    return {
        restrict: 'A',
        require: 'ngModel',
        scope: { value: '=ngModel' },
        link: function (scope, element, attrs, ngModelCtrl) {
            $(function () {
                var options = {
                    onChange: function () {
                        $timeout(function () {
                            ngModelCtrl.$setViewValue(($(element).cron("value")));
                        });
                    },
                    customValues: {
                        "quarter": "0 0 30 6,9 *;0 0 31 3,12 *",//two crons in one..
                    }
                }

                var oldValue;
                scope.$watch('value', function (val) {
                    if (oldValue !== val) {
                        oldValue = val;
                        $(element).cron("value", val);
                    }
                });

                $(element).cron(options);
            });
        }
    }
}])