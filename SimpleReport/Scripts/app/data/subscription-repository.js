﻿angular.module('repository').factory('subscriptionRepository', ['$http', function($http) {

        var urlBase = 'api/subscription/';
        var repo = {};

        repo.get = function(reportId, id) {
            return $http.get(urlBase + 'get?id=' + id + '&reportId=' + reportId);
        };

        repo.getAll = function() {
            return $http.get(urlBase + 'all');
        };

        repo.list = function(reportId, filter) {
            return $http.get(urlBase + 'list?filter=' + (filter || '') + '&reportId=' + (reportId || ''));
        };

        repo.allForReport = function(reportid) {
            return $http.get(urlBase + 'list?reportId=' + reportid);
        };

        repo.save = function(subscription) {
            return $http.post(urlBase + 'save', { data: subscription, reportId: subscription.ReportId });
        };

        repo.delete = function(reportId, id) {
            return $http.post(urlBase + 'delete', { data: id, reportId: reportId });
        };

        repo.send = function(reportId, id) {
            return $http.post(urlBase + 'send', { data: id, reportId: reportId });
        };

        return repo;
    }
]);