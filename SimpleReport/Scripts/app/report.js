angular.module('report', ['shared', 'ui.bootstrap', 'repository']);

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

                    $scope.dateFormat = 'yyyy-MM-dd';
                });

                $scope.viewModel = viewModel;

                $scope.triggerOnScreen = triggerOnScreen;
                $scope.periodChanged = periodChanged;
                $scope.dateChanged = dateChanged;
            };
            $scope.init();

            function dateChanged(parameter) {
                var date = $filter('date')(parameter.Value, $scope.dateFormat);
                parameter.Value = date;
            }

            function periodChanged(parameter) {
                //9999 is custom..
                if (parameter.EnumValue === '9999') {
                    if (!parameter.from)
                        parameter.from = new Date();
                    if (!parameter.to)
                        parameter.to = new Date();
                    
                    var from = $filter('date')(parameter.from, $scope.dateFormat);
                    var to = $filter('date')(parameter.to, $scope.dateFormat);

                    parameter.Value = parameter.EnumValue + ':' + from + '_' + to;

                    var text = parameter.Choices['9999'].split(':')[0] + ': ' + from + ' - ' + to;
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
    ])
    .directive('subscriptionEditor', function () {
        return {
            templateUrl: 'scripts/app/templates/subscriptionEditor.html',
            scope: { reportId: '=', reportParameters: '=', subscriptionId: '=' },
            controller: ['$scope', '$http', 'scheduleRepository', 'subscriptionRepository', function ($scope, $http, scheduleRepository, subscriptionRepository) {

                function init() {
                    //if there is no current subscriptionid, then we are creating a new one.  works for now, might change when we have the list.
                    if (!$scope.subscriptionId) {
                        $scope.subscription = { To: '', Cc: '', Bcc: '', ScheduleId: null }
                    }

                    fetchData();

                    $scope.save = save;
                }
                init();

                function save() {

                    if ($scope.form.$invalid) {
                        toastr.warning('Please fill in required fields');
                        return;
                    }

                    if (!($scope.subscription.To || $scope.subscription.Cc || $scope.subscription.Bcc)) {
                        toastr.warning('To,CC or Bcc needs to be provided.');
                        return;
                    }

                    $scope.reportParameters.forEach(function (param) {
                        if (param.Mandatory && !param.Value) {
                            toastr.warning('You need to provide values for all mandatory parameters.');
                            return;
                        }
                    });

                    var parsedParameters = { reportId: $scope.reportId };
                    $scope.reportParameters.forEach(function (param) {
                        parsedParameters[param.Key] = param.Value;
                    });

                    var url = 'Home/ExecuteReport?' + serialize(parsedParameters);

                    var data = angular.extend({ ReportId: $scope.reportId, ReportUrl: url }, $scope.subscription);
                    subscriptionRepository.save(data).success(function(data) {
                        toastr.success('Subscription saved');
                        $scope.subscription.Id = data.Id;
                    }).error(function() {
                        toastr.error('Something went wrong during save, please try again later or contact support');
                    });
                }

                function serialize(obj) {
                    var str = [];
                    for (var p in obj)
                        if (obj.hasOwnProperty(p)) {
                            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
                        }
                    return str.join("&");
                }

                function fetchData() {
                    scheduleRepository.getAll().success(function (data) {
                        $scope.schedules = data;
                    }).error(function () {
                        toastr.error('Something went wrong when loading schedules, please try again later or contact support');
                    });
                }
            }
            ]
        };
    });