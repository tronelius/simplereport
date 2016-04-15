angular.module('shared').directive('templateUpload', function () {
    return {
        restrict: "A",
        templateUrl: 'scripts/app/templates/templateUpload.html',
        scope: {
            reportid: '=',
            hasReportTemplate: '=',
            templateFormat: '=',
            reportResultType: '=',
            reportResultTypes: '=',
            showreportResultTypes: '=',
            convertToPdf: '='
        },
        controller: [
            '$scope', 'Upload', '$http', function ($scope, upload, $http) {
                var baseDownloadUrl = 'Home/DownloadTemplate?ReportId=';
                $scope.showDeleteConfirmation = false;
                $scope.$watch('hasReportTemplate', function (val) {
                    if (val)
                        $scope.downloadLink = baseDownloadUrl + $scope.reportid;
                    else
                        $scope.downloadLink = null;
                });

                if ($scope.hasReportTemplate === 'true')
                    $scope.downloadLink = baseDownloadUrl + $scope.reportid;

                $scope.updateMetadata = function () {
                    $http.post("Home/UpdateTemplateMetadata", { reportId: $scope.reportid, convertToPdf: $scope.convertToPdf }).success(function (data) {
                        toastr.success("Metadata updated");
                    }).error(function () {
                        toastr.error('Something went wrong, please try again later or contact support');
                    });
                }

                $scope.deleteTemplate = function (really) {
                    if (really) {
                        $http.post("Home/DeleteTemplate", { reportId: $scope.reportid }).success(function (data) {
                            toastr.success("Template deleted");
                            $scope.downloadLink = null;
                            $scope.hasReportTemplate = false;
                            $scope.templateFormat = 0;
                        }).error(function () {
                            toastr.error('Something went wrong, please try again later or contact support');
                        });
                        $scope.showDeleteConfirmation = false;
                    } else {
                        $scope.showDeleteConfirmation = true;
                    }
                }

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
                                    $scope.reportResultType = data.ReportResultType;
                                    $scope.downloadLink = baseDownloadUrl + $scope.reportid;
                                    toastr.success("Template file uploaded");
                                    $scope.progress = null;
                                    $scope.templateFormat = data.TemplateFormat;
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