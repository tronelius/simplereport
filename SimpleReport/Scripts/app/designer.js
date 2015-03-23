angular.module('designer', []);

angular.module('designer').controller('designerController', ['$scope', '$http', function ($scope, $http) {
    $scope.report;
    $scope.connection;
    $scope.lookupreport;

    $scope.reportList;
    $scope.connectionList;
    $scope.lookupreportList;
    $scope.inputTypes;

    $scope.activeTab = 'report';
    


    $scope.showReportTab = function() {
        $scope.activeTab = 'report';
        $http.get('api/Designer/reports').
            success(function (data) {
                $scope.inputTypes = data.inputTypes;
                $scope.reportList = data.reportList;
            }).
            error(function (data) {
                toastr['error']("Server error, please try again later.");
            });
    };
    $scope.showReportTab();
}]);