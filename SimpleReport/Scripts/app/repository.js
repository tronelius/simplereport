angular.module('repository', []);

angular.module('repository').factory('scheduleRepository', ['$http', function ($http) {

    var urlBase = 'api/schedule/';
    var repo = {};

    repo.getAll = function () {
        return $http.get(urlBase + 'all');
    };

    repo.save = function (schedule) {
        return $http.post(urlBase + 'save', schedule);
    };

    repo.delete = function (id) {
        return $http.post(urlBase + 'delete', { Id: id });
    };

    return repo;
}])
.factory('subscriptionRepository', ['$http', function ($http) {

    var urlBase = 'api/subscription/';
    var repo = {};

    //not implemented
    //repo.getAll = function () {
    //    return $http.get(urlBase + 'all');
    //};

    repo.save = function (subscription) {
        return $http.post(urlBase + 'save', subscription);
    };

    //not implemented
    //repo.delete = function (id) {
    //    return $http.post(urlBase + 'delete', { Id: id });
    //};

    return repo;
}]);