angular.module('repository').factory('designerRepository', ['$http', function ($http) {

    var urlBase = 'api/designer/';
    var repo = {};

    repo.exportModelUrl = function() {
        return urlBase + 'exportModel';
    };

    repo.clearModel = function() {
        return $http.post(urlBase + 'clearModel');
    };

    repo.saveReport = function(report) {
        return $http.post(urlBase + 'SaveReport', report);
    };

    repo.deleteReport = function(report) {
        return $http.post(urlBase + 'DeleteReport', report);
    };

    repo.saveConnection = function(connection) {
        return $http.post(urlBase + 'SaveConnection', connection);
    };

    repo.deleteConnection = function (connection) {
        return $http.post(urlBase + 'DeleteConnection', connection);
    };

    repo.verifyConnection = function (connection) {
        return $http.post(urlBase + 'VerifyConnection', connection);
    };

    repo.saveLookupReport = function (lookupReport) {
        return $http.post(urlBase + 'SaveLookupReport', lookupReport);
    };

    repo.deleteLookupReport = function (lookupReport) {
        return $http.post(urlBase + 'DeleteLookupReport', lookupReport);
    };

    repo.saveTypeAheadReport = function (typeaheadreport) {
        return $http.post(urlBase + 'SaveTypeAheadReport', typeaheadreport);
    };

    repo.deleteTypeAheadReport = function (typeaheadreport) {
        return $http.post(urlBase + 'DeleteTypeAheadReport', typeaheadreport);
    };

    return repo;
}]);