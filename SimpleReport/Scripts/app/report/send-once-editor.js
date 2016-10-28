angular.module('report').directive('sendOnceEditor', function () {
        return {
            templateUrl: 'scripts/app/templates/sendOnceEditor.html',
            scope: { reportId: '=', reportParameters: '=', saveCb:'&'},
            controller: ['$scope', '$http', 'reportViewModel','scheduleRepository', 'subscriptionRepository', 'reportUrlHelper','$window', function ($scope, $http, viewModel, scheduleRepository, subscriptionRepository, reportUrlHelper, $window) {

                function init() {
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

                function save() {
                    if ($scope.form.$invalid) {
                        toastr.warning('Please fill in required fields');
                        return;
                    }

                    if (hasInvalidParameters()) {
                        toastr.warning('You need to provide values for all mandatory parameters.');
                        return;
                    }

                    if (!($scope.subscription.To || $scope.subscription.Cc || $scope.subscription.Bcc)) {
                        toastr.warning('To,CC or Bcc needs to be provided.');
                        return;
                    }

                    
                    var params = reportUrlHelper.toUrlByIdAndParams($scope.reportId, $scope.reportParameters);
                    var data = angular.extend($scope.subscription, { ReportId: $scope.reportId, ReportParams: params });
                    data.SubscriptionType = 1;
                        subscriptionRepository.save(data).success(function (data) {
                            if (data.Error) {
                                toastr.error(data.Error);
                                return;
                            }

                            toastr.success('Subscription saved');

                            if ($scope.saveCb)
                                $scope.saveCb();
                        }).error(function () {
                            toastr.error('Something went wrong during save, please try again later or contact support');
                        });
                }
            }
            ]
        };
    });