angular.module('subscriptions', ['shared', 'ui.bootstrap']);

angular.module('subscriptions')
    .controller('subscriptionController', ['$scope', '$http', function ($scope, $http) {

        $scope.init = function () {
            $scope.activeTab = 'all';
        };
        $scope.init();

    }
]);