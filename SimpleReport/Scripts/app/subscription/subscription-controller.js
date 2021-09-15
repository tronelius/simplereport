angular.module('subscriptions').controller('subscriptionController', ['$scope','$http', function($scope, $http) {

        $scope.init = function() {
            $scope.activeTab = 'all';
            $http.get('api/Subscription/GetSettings').
            success(function (data) {
                $scope.settings = data;
            }).
            error(function (data) {
                toastr.error("Couldn't get settings from server.", "Error");
            });
        };
        $scope.init();
//Settings
    $scope.saveSettings = function () {
        $.ajax({
            type: 'post',
            url: 'api/Subscription/SaveSettings',
            data: JSON.stringify($scope.settings),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            toastr.success("Settings saved", "Saved");
        }).error(function (data) {
            toastr.error("Server error when saving settings.", "Error");
        });
    };
    }
]);