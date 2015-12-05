angular.module('shared').directive('emails', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$validators.emails = function (modelValue, viewValue) {
                    if (ctrl.$isEmpty(modelValue)) {
                        // consider empty models to be valid
                        return true;
                    }

                    var split = modelValue.split(';');

                    var valid = true;
                    split.forEach(function (email) {
                        var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
                        if (!re.test(email))
                            valid = false;
                    });

                    return valid;
                };
            }
        };
    })