angular.module('repository').factory('scheduleRepository', ['$http', function($http) {

        var urlBase = 'api/schedule/';
        var repo = {};

        repo.getAll = function() {
            return $http.get(urlBase + 'all');
        };

        repo.save = function(schedule) {
            return $http.post(urlBase + 'save', schedule);
        };

        repo.delete = function(id) {
            return $http.post(urlBase + 'delete', id );
        };

        return repo;
    }
]);