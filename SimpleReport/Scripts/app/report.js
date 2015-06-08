angular.module('report', ['shared']);

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
    ]);