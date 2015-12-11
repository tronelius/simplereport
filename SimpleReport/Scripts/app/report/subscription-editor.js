angular.module('report').directive('subscriptionEditor', function () {
        return {
            templateUrl: 'scripts/app/templates/subscriptionEditor.html',
            scope: { reportId: '=', reportParameters: '=', subscriptionId: '=', saveCb: '&' },
            controller: ['$scope', '$http', 'reportViewModel','scheduleRepository', 'subscriptionRepository', 'reportUrlHelper', function ($scope, $http, viewModel, scheduleRepository, subscriptionRepository, reportUrlHelper) {

                function init() {

                    $scope.$watch('subscriptionId', function() {
                        if (!$scope.subscriptionId) {
                            $scope.subscription = { To: '', Cc: '', Bcc: '', ScheduleId: null, MailSubject: viewModel.Report.MailSubject, MailText: viewModel.Report.MailText }
                        } else {
                            subscriptionRepository.get($scope.reportId, $scope.subscriptionId).success(function (data) {
                                $scope.subscription = data;
                            }).error(function () {
                                toastr.error('Something went wrong, please try again later or contact support');
                        });
                        }
                    });

                    fetchData();
                    $scope.save = save;
                }
                init();

                function hasInvalidParameters() {
                    return $scope.reportParameters.some(function(param) {
                        if (param.Mandatory && !param.Value) {
                            return true;
                        }
                        return false;
                    });
                }

                //function hasInvalidParametersForSubscription() {
                //    return $scope.reportParameters.some(function (param) {
                //        if (param.Mandatory && !param.Value && !(param.InputType === 6 || param.InputType === 7)) {
                //            return true;
                //        }
                //        return false;
                //    });
                //}

                function save() {

                    if ($scope.form.$invalid) {
                        toastr.warning('Please fill in required fields');
                        return;
                    }

                    if (!($scope.subscription.To || $scope.subscription.Cc || $scope.subscription.Bcc)) {
                        toastr.warning('To,CC or Bcc needs to be provided.');
                        return;
                    }

                    if (hasInvalidParameters()) {
                        toastr.warning('You need to provide values for all mandatory parameters.');
                        return;
                    }

                    var params = reportUrlHelper.toUrlByIdAndParams($scope.reportId, $scope.reportParameters);

                    var data = angular.extend($scope.subscription, { ReportId: $scope.reportId, ReportParams: params });
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