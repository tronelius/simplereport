angular.module('shared').directive('onScreenReport', function () {
        return {
            templateUrl: 'scripts/app/templates/onScreenReport.html',
            scope: { reportid: '=', parameters: '=', refreshinterval: '=', title: '=', disablespinner:'=' },
            controller: [
                '$scope', '$http', function ($scope, $http) {

                    $scope.$watch('reportid', function (val) {
                        if (val)
                            fetchData();
                        else
                            $scope.data = null;
                    });

                    $scope.$on('refreshOnScreen', fetchData);

                    function fetchData() {
                        $scope.data = null;
                        var parsedParameters = {};
                        $scope.parameters.forEach(function (param) {
                            parsedParameters[param.Key] = param.Value;
                        });

                        $http.get("Home/ExecuteOnScreenReport", { params: angular.extend({ reportId: $scope.reportid }, parsedParameters) }).success(function (data) {
                            $scope.data = data;
                            $scope.updatetime = new Date().toLocaleTimeString();
                            if ($scope.refreshinterval) {
                                setTimeout(fetchData, $scope.refreshinterval * 1000);
                            }
                        }).error(function () {
                            toastr.error('Something went wrong, please try again later or contact support');
                        });
                    }
                }
            ]
        };
    })