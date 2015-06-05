angular.module('report', ['ngFileUpload']);

angular.module('report')
    .controller('reportController', [
        '$scope', '$http', 'reportViewModel', '$timeout', function ($scope, $http, viewModel, $timeout) {

            $scope.init = function () {
                //TODO: we should probably get the data once we have converted the view.
                //$http.get('api/Designer/GetViewModel').
                //    success(function (data) {
                //        $scope.inputTypes = data.InputTypes;
                //        $scope.reportList = data.Reports;
                //        $scope.connections = data.Connections;
                //        $scope.lookupReports = data.LookupReports;
                //        $scope.accessLists = data.AccessLists;
                //        $scope.settings = data.Settings;
                //    }).
                //    error(function (data) {
                //        toastr.error("Couldn't get list of reports from server.","Error");
                //    });

                $scope.viewModel = viewModel;

                $timeout(registerJqueryStuff, 500);

            };
            $scope.init();

            function registerJqueryStuff() {
                $(".numeric").numeric();
                $(".datepicker").datepicker();
            }

        }
    ])
    .directive('templateUpload', function () {
        return {
            templateUrl: 'scripts/app/templates/templateUpload.html',
            scope: {reportid: '@'},
            controller: ['$scope', 'Upload', function ($scope, upload) {
                $scope.downloadLink = 'Home/DownloadTemplate?ReportId=' + $scope.reportid;
              
                $scope.upload = function (files) {
                    if (files && files.length) {
                        for (var i = 0; i < files.length; i++) {
                            var file = files[i];
                            upload.upload({
                                url: 'Home/UploadTemplate',
                                fields: {
                                    'reportId': $scope.reportid
                                },
                                file: file
                            }).progress(function (evt) {
                                var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                                $scope.log = 'progress: ' + progressPercentage + '% ' + evt.config.file.name + '\n' + $scope.log;
                                $scope.progress = progressPercentage;
                            }).success(function (data, status, headers, config) {
                                $scope.log = 'file ' + config.file.name + 'uploaded. Response: ' + JSON.stringify(data) + '\n' + $scope.log;
                            }).error(function() {
                                $scope.progress = null;
                                //TODO: error handling. Do we use toastr for errors?
                                alert('damnit');
                            });
                        }
                    }
                };
            }]
        };
    });