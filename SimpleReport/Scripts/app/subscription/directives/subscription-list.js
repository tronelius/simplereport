angular.module('subscriptions').directive('subscriptionList', function () {
    return {
        templateUrl: 'scripts/app/templates/subscriptionList.html',
        scope: { showReportName: '@', filter: '@', reportId: '=' },
        controller: [
            '$scope', '$http', 'subscriptionRepository', 'reportRepository', '$q', '$window', function ($scope, $http, subscriptionRepository, reportRepository, $q, $window) {

                $scope.init = function () {
                    $scope.showReportName = $scope.showReportName === 'true';
                    fetchData();

                    $scope.sendSubscription = sendSubscription;
                    $scope.editSubscription = editSubscription;
                    $scope.deleteSubscription = deleteSubscription;
                    $scope.confirmDeleteSubscription = confirmDeleteSubscription;
                    $scope.subscriptionIdToDelete = 0;
                    $scope.showDeleteConfirmation = false;
                };
                $scope.init();

                function fetchData() {
                    var promises = {};
                    if ($scope.showReportName)
                        promises.reports = reportRepository.getIdToNameMappings();

                    if (!$scope.reportId)
                        promises.subscriptions = subscriptionRepository.list($scope.reportId, $scope.filter);
                    else
                        promises.subscriptions = subscriptionRepository.allForReport($scope.reportId);

                    $q.all(promises).then(function (data) {
                        if (data.reports) {//add the reportnames to subs based on reportid.
                            data.subscriptions.data.forEach(function (s) {
                                data.reports.data.forEach(function (r) {
                                    if (r.Id === s.ReportId)
                                        s.ReportName = r.Name;
                                });
                            });
                        }

                        $scope.subscriptions = data.subscriptions.data;
                    }).catch(function () {
                        toastr.error('Something went wrong, please try again later or contact support');
                    });
                }

                function sendSubscription(id) {
                    subscriptionRepository.send($scope.reportId, id);
                    toastr.success("Subscription is schedule for resend, it can take up to 1 minute before send is complete.");
                }

                function editSubscription(sub) {
                    $window.location.href ='/Home/Report?' + sub.ReportParams + '&subscriptionid=' + sub.Id;
                }

                function confirmDeleteSubscription(id) {
                    $scope.subscriptionIdToDelete = id;
                    $scope.showDeleteConfirmation = true;
                }

                function deleteSubscription(id) {
                    $scope.showDeleteConfirmation = false;
                    subscriptionRepository.delete($scope.reportId, id).success(function () {
                        $scope.subscriptions = $scope.subscriptions.filter(function (s) {
                            return s.Id !== id;
                        });
                        toastr.success("Subscription deleted");
                    }).error(function () {
                        toastr.error('Something went wrong, please try again later or contact support');
                    });
                }
            }
        ]
    }
});