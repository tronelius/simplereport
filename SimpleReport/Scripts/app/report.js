angular.module('report', ['shared', 'ui.bootstrap']);

angular.module('report')
    .controller('reportController', [
        '$scope', '$http', 'reportViewModel', '$filter', function ($scope, $http, viewModel, $filter) {

            $scope.init = function () {
                viewModel.Report.Parameters.forEach(function (param) {
                    //periods of type custom comes on the format Enum:from_to
                    if (param.InputType === 3) { //period
                        if (param.Value && ~param.Value.indexOf(':')) {
                            var temp = param.Value.split(':');
                            var enumValue = temp[0];

                            if (temp.length > 1) {
                                var values = temp[1].split('_');
                                param.from = values[0];
                                param.to = values[1];
                            }

                            param.EnumValue = enumValue;
                        } else {
                            param.EnumValue = param.Value;
                        }
                    }
                });

                $scope.viewModel = viewModel;

                $scope.triggerOnScreen = triggerOnScreen;
                $scope.periodChanged = periodChanged;
            };
            $scope.init();

            function periodChanged(parameter) {
                //9999 is custom..
                if (parameter.EnumValue === '9999') {
                    if (!parameter.from)
                        parameter.from = new Date();
                    if (!parameter.to)
                        parameter.to = new Date();

                    parameter.Value = parameter.EnumValue + ':' + parameter.from + '_' + parameter.to;
                    
                    var text = parameter.Choices['9999'].split(':')[0];
                    if (parameter.from && parameter.to) {
                        var format = 'yyyy-MM-dd';
                        var from = $filter('date')(parameter.from, format);
                        var to = $filter('date')(parameter.from, format);
                        text += ': ' + from + ' - ' + to;
                    }
                    parameter.Choices['9999'] = text;
                } else
                    parameter.Value = parameter.EnumValue;
            }

            function triggerOnScreen() {
                if ($scope.selectedAction !== 'onScreen')
                    $scope.selectedAction = 'onScreen';
                else
                    $scope.$broadcast('refreshOnScreen');
            }
        }
    ]);