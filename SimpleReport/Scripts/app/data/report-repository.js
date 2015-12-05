angular.module('repository').factory('reportRepository', ['$http', function ($http) {

    var urlBase = 'api/report/';
    var repo = {};

    repo.getIdToNameMappings = function() {
        return $http.get(urlBase + 'idToNameMappings');
    }

    return repo;
}]);