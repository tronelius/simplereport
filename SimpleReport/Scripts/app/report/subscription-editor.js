angular.module('report').directive('subscriptionEditor', function () {
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