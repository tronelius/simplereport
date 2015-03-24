angular.module('designer', []);

angular.module('designer').controller('designerController', ['$scope', '$http', function ($scope, $http) {
    $scope.activeTab = 'report';

    $scope.showReportTab = function() {
        $scope.activeTab = 'report';
        $http.get('../api/Designer/reports').
            success(function (data) {
                $scope.inputTypes = data.InputTypes;
                $scope.reportList = data.Reports;
            }).
            error(function (data) {
                toastr.error("Couldn't get list of reports from server.","Error");
            });
    };

    $scope.save = function () {
        $.ajax({
            type: 'post',
            url: '../api/Designer/SaveReport',
            data: JSON.stringify($scope.report),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            toastr.success("Report Saved","Saved");
        }).error(function (data) {
            toastr.error("Server error when saveing report, please try again later.","Error");
        });
        //$http.post('api/Designer/SaveReport')
    };
    $scope.showReportTab();
}]);