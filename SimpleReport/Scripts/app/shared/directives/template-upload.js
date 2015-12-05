angular.module('shared').directive('templateUpload', function () {
        return {
            templateUrl: 'scripts/app/templates/templateUpload.html',
            scope: { reportid: '=', hasReportTemplate: '=' },
            controller: [
                '$scope', 'Upload', function ($scope, upload) {
                    var baseDownloadUrl = 'Home/DownloadTemplate?ReportId = ';

                    $scope.$watch('hasReportTemplate', function (val) {
                        if (val)
                            $scope.downloadLink = baseDownloadUrl + $scope.reportid;
                        else
                            $scope.downloadLink = null;
                    });

                    if ($scope.hasReportTemplate === 'true')
                        $scope.downloadLink = baseDownloadUrl + $scope.reportid;

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
                                    $scope.progress = progressPercentage;
                                }).success(function (data, status, headers, config) {
                                    if (!data.error) {
                                        $scope.hasReportTemplate = true;
                                        $scope.downloadLink = baseDownloadUrl + $scope.reportid;
                                        toastr.success("Template file uploaded");
                                        $scope.progress = null;
                                    } else {
                                        $scope.progress = 0;
                                        toastr.error(data.error);
                                    }
                                }).error(function () {
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