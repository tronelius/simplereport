angular.module('shared')
.directive('numeric', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            $(function () {
                element.numeric();
            });
        }
    }
})