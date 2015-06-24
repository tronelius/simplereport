angular.module('subscriptions').controller('subscriptionController', ['$scope', function($scope) {

        $scope.init = function() {
            $scope.activeTab = 'all';
        };
        $scope.init();

    }
]);