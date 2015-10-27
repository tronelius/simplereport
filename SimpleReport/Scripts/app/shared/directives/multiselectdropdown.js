angular.module('shared').directive('multiselectDropdown', [function () {
    return {
        scope: { choices: '=msdChoices', value: '=msdValue' },
        controller: [
            '$scope', '$element', '$timeout', function ($scope, $element, $timeout) {

                var element = $($element); // Get the element as a jQuery element

                var options = $scope.choices.map(function (c) {
                    return { label: c.Value, title: c.Value, value: c.Key, selected: false };
                });

                var selected = [];

                element.multiselect({
                    onChange: function (option, checked, select) {
                        var opt = option[0];

                        if (checked)
                            selected.push(opt.value);
                        else
                            selected = selected.filter(function(s) {
                                return s !== opt.value;
                            });

                        $timeout(function() {
                            $scope.value = selected.join(',');
                        });
                    },
                    enableFiltering: true
                });
                element.multiselect('dataprovider', options);

            }
        ]
    };
}]);