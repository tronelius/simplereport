﻿angular.module('shared', ['ngFileUpload']);

angular.module('shared')
    .directive('templateUpload', function() {
        return {
            templateUrl: 'scripts/app/templates/templateUpload.html',
            scope: { reportid: '=', hasReportTemplate: '=' },
            controller: [
                '$scope', 'Upload', function($scope, upload) {
                    var baseDownloadUrl = 'Home/DownloadTemplate?ReportId = ';

                    $scope.$watch('hasReportTemplate', function(val) {
                        if (val)
                            $scope.downloadLink = baseDownloadUrl + $scope.reportid;
                        else
                            $scope.downloadLink = null;
                    });

                    if ($scope.hasReportTemplate === 'true')
                        $scope.downloadLink = baseDownloadUrl + $scope.reportid;

                    $scope.upload = function(files) {
                        if (files && files.length) {
                            for (var i = 0; i < files.length; i++) {
                                var file = files[i];
                                upload.upload({
                                    url: 'Home/UploadTemplate',
                                    fields: {
                                        'reportId': $scope.reportid
                                    },
                                    file: file
                                }).progress(function(evt) {
                                    var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                                    $scope.progress = progressPercentage;
                                }).success(function(data, status, headers, config) {
                                    if (!data.error) {
                                        $scope.hasReportTemplate = true;
                                        $scope.downloadLink = baseDownloadUrl + $scope.reportid;
                                    } else {
                                        $scope.progress = 0;
                                        toastr.error(data.error);
                                    }
                                }).error(function() {
                                    $scope.progress = null;
                                    toastr.error("Couldn't upload the file, please try again.", "Error");
                                });
                            }
                        }
                    };
                }
            ]
        };
    })
    .directive('onScreenReport', function() {
        return {
            templateUrl: 'scripts/app/templates/onScreenReport.html',
            scope: { reportid: '=', parameters: '=' },
            controller: [
                '$scope', '$http', function($scope, $http) {

                    $scope.$watch('reportid', function(val) {
                        if (val)
                            fetchData();
                        else
                            $scope.data = null;
                    });

                    $scope.$on('refreshOnScreen', fetchData);

                    function fetchData() {
                        var parsedParameters = {};
                        $scope.parameters.forEach(function(param) {
                            parsedParameters[param.Key] = param.Value;
                        });

                        $http.get("Home/ExecuteOnScreenReport", { params: angular.extend({ reportId: $scope.reportid }, parsedParameters) }).success(function(data) {
                            $scope.data = data;
                        }).error(function() {
                            toastr.error('Something went wrong, please try again later or contact support');
                        });
                    }
                }
            ]
        };
    })
.directive('datepicker', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            $(function () {
                element.datepicker({'format': 'yyyy-mm-dd'}).on('changeDate', function (e) {
                    scope.$apply(function () {
                        ngModelCtrl.$setViewValue(e.currentTarget.value);
                    });
                });
            });
        }
    }
})
.directive('numeric', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            $(function () {
                element.numeric();
            });
        }
    }
});