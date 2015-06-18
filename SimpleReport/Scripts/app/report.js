angular.module('report', ['shared', 'ui.bootstrap', 'repository', 'subscriptions']);

angular.module('report')
    .controller('reportController', [
        '$scope', '$http', 'reportViewModel', '$filter', 'queryStringParser', function ($scope, $http, viewModel, $filter, queryStringParser) {

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

                var s = queryStringParser.parse(location.search);
                
                if (s.subscriptionid) {
                    $scope.subscriptionId = s.subscriptionid;
                    $scope.selectedAction = 'subscribe';
                }

                $scope.viewModel = viewModel;

                $scope.triggerOnScreen = triggerOnScreen;
                $scope.periodChanged = periodChanged;
                $scope.dateChanged = dateChanged;
                $scope.onSubscriptionSaved = onSubscriptionSaved;
            };
            $scope.init();

            function onSubscriptionSaved() {
                $scope.selectedAction = 'editSubscriptions';
            }

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
.directive('emails', function() {
    return {
        require: 'ngModel',
        link: function(scope, elm, attrs, ctrl) {
            ctrl.$validators.emails = function(modelValue, viewValue) {
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
    .directive('subscriptionEditor', function () {
        return {
            templateUrl: 'scripts/app/templates/subscriptionEditor.html',
            scope: { reportId: '=', reportParameters: '=', subscriptionId: '=', saveCb: '&' },
            controller: ['$scope', '$http', 'scheduleRepository', 'subscriptionRepository', function ($scope, $http, scheduleRepository, subscriptionRepository) {

                function init() {
                    
                    if (!$scope.subscriptionId) {
                        $scope.subscription = { To: '', Cc: '', Bcc: '', ScheduleId: null }
                    } else {
                        subscriptionRepository.get($scope.reportId, $scope.subscriptionId).success(function (data) {
                            $scope.subscription = data;
                        }).error(function () {
                            toastr.error('Something went wrong, please try again later or contact support');
                        });
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

                    var params = serialize(parsedParameters);

                    var data = angular.extend({ ReportId: $scope.reportId, ReportParams: params }, $scope.subscription);
                    subscriptionRepository.save(data).success(function (data) {
                        if (data.Error) {
                            toastr.error(data.Error);
                            return;
                        }

                        toastr.success('Subscription saved');
                        if (!$scope.subscription.Id)
                            $scope.subscription.Id = data.Id;

                        if ($scope.saveCb)
                            $scope.saveCb();
                    }).error(function () {
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