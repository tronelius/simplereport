angular.module('shared').directive('onScreenReport', function () {
        return {
            templateUrl: 'scripts/app/templates/onScreenReport.html',
            scope: { reportid: '=', parameters: '=', refreshinterval: '=', title: '=', disablespinner:'=' },
            controller: [
                '$scope', '$http', function ($scope, $http) {
                    $scope.data = null;
                    $scope.$watch('reportid', function (val) {
                        if (val)
                            fetchData();
                        else
                            $scope.data = null;
                    });

                    $scope.$on('refreshOnScreen', fetchData);

                    function fetchData() {
                        var parsedParameters = {};
                        $scope.parameters.forEach(function (param) {
                            if (param.InputType === 5 && param.Value.startsWith(",")) { // multichoice and starts with ","
                                param.Value = param.Value.substr(1);
                            }
                            parsedParameters[param.Key] = param.Value;
                        });

                        $http.get("Home/ExecuteOnScreenReport", { params: angular.extend({ reportId: $scope.reportid }, parsedParameters) }).success(function (data) {
                            $scope.data = data;
                            $scope.updatetime = new Date().toLocaleTimeString();
                            if ($scope.refreshinterval) {
                                setTimeout(fetchData, $scope.refreshinterval * 1000);
                            }
                        }).error(function () {
                            $scope.data = null;
                            toastr.error('Something went wrong, please try again later or contact support');
                        });
                    }
                }
            ]
        };
    })