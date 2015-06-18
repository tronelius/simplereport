angular.module('shared', ['ngFileUpload']);

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
                                        toastr.success("Template file uploaded");
                                        $scope.progress = null;
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
                        $scope.data = null;
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
.directive('datepickerBootstrap', function () {
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
})
.factory('queryStringParser', function() {
    var parser = {};

    parser.parse = function (str) {
        if (typeof str !== 'string') {
            return {};
        }

        str = str.trim().replace(/^(\?|#|&)/, '');

        if (!str) {
            return {};
        }

        return str.split('&').reduce(function (ret, param) {
            var parts = param.replace(/\+/g, ' ').split('=');
            var key = parts[0];
            var val = parts[1];

            key = decodeURIComponent(key);
            // missing `=` should be `null`:
            // http://w3.org/TR/2012/WD-url-20120524/#collect-url-parameters
            val = val === undefined ? null : decodeURIComponent(val);

            if (!ret.hasOwnProperty(key)) {
                ret[key] = val;
            } else if (Array.isArray(ret[key])) {
                ret[key].push(val);
            } else {
                ret[key] = [ret[key], val];
            }

            return ret;
        }, {});
    }

    return parser;
});