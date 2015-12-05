angular.module('repository').factory('designerRepository', ['$http', function ($http) {

    var urlBase = 'api/designer/';
    var repo = {};

    repo.exportModelUrl = function() {
        return urlBase + 'exportModel';
    }

    repo.clearModel = function () {
        return $http.post(urlBase + 'clearModel');
    }
    return repo;
}]);