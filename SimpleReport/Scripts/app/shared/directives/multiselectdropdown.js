angular.module('shared').directive('multiselectDropdown', [function () {
    return {
        scope: { choices: '=msdChoices', value: '=msdValue', required: '=msdRequired', name:'=msdName' },
        controller: [
            '$scope', '$element', '$timeout', function($scope, $element, $timeout) {

                var element = $($element); // Get the element as a jQuery element
                var valueArr = [];
                if ($scope.value !== null && $scope.value !== "") {
                    valueArr = $scope.value.split(',');
                }
                var options = $scope.choices.map(function (c) {
                    return { label: c.Value, title: c.Value, value: c.Key, selected: (valueArr.indexOf(c.Key) > -1) };
                });

                var selected = valueArr;
               
                element.multiselect({
                    onChange: function(option, checked, select) {
                        var opt = option[0];

                        if (checked)
                            selected.push(opt.value);
                        else
                            selected = selected.filter(function(s) {
                                return s !== opt.value;
                            });

                        if ($scope.required && selected.length === 0) { //beware of ugly code!
                            $(option.parent().parent().children()[1]).addClass('required');
                        } else {
                            $(option.parent().parent().children()[1]).removeClass('required');
                        }

                        $timeout(function() {
                            $scope.value = selected.join(',');
                        });
                    },
                    maxHeight:400,
                    enableFiltering: true,
                    enableCaseInsensitiveFiltering: true,
                    enableFullValueFiltering: false,
                    buttonContainer: $scope.required && selected.length === 0 ? '<div class="required" />' : '<div />',
                    buttonClass: 'form-control overflow'
            });
                element.multiselect('dataprovider', options);

            }
           
        ]
    };
}]);