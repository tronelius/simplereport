angular.module('designer', []);

angular.module('designer').controller('designerController', ['$scope', '$http', function ($scope, $http) {
    $scope.activeTab = 'report';
    $scope.parameterPositionHash = [];

    $scope.init = function() {
        $scope.activeTab = 'report';
        $http.get('api/Designer/GetViewModel').
            success(function (data) {
                $scope.inputTypes = data.InputTypes;
                $scope.reportList = data.Reports;
                $scope.connections = data.Connections;
                $scope.lookupReports = data.LookupReports;
                $scope.accessLists = data.AccessLists;
            }).
            error(function (data) {
                toastr.error("Couldn't get list of reports from server.","Error");
            });
    };

    var regExParameterMatch = /(@\w+)/g;
    var parameterPositionHash = [];

    //Can it be the same loop?
    $scope.reportDataChanged = function () {
        console.debug('report changed');
        $scope.latestSql = $scope.report.Sql;
        parameterPositionHash = [];
        while (match = regExParameterMatch.exec($scope.latestSql)) {
            var existingparam = _.findWhere($scope.report.Parameters, { SqlKey: match[0] });
            if (existingparam !== undefined) {
                parameterPositionHash[match.index] = existingparam.SqlKey;
            }
        }
    };

    $scope.analyzeSQL = function () {
        var currentSQL = $scope.report.Sql;
        var match;
        var currentPosition = $('#sqlarea').prop('selectionStart');
        var foundMatches = [];
        while (match = regExParameterMatch.exec(currentSQL)) {
            //console.debug(match[0]);
            var existingparam = _.findWhere($scope.report.Parameters, { SqlKey:match[0]});
            if (existingparam === undefined) {
                if (currentPosition >= match.index && currentPosition <= match.index + match[0].length) {
                    //console.debug('updated existing');
                    //find out what the changed parameter used to be and get a reference to that
                    var nameOfExisting = parameterPositionHash[match.index];
                    existingparam = _.findWhere($scope.report.Parameters, { SqlKey: nameOfExisting });
                    if (existingparam !== undefined) {
                        existingparam.SqlKey = match[0];
                    } else {
                        $scope.addNewParameter(match[0]);
                    }
                } else {
                    $scope.addNewParameter(match[0]);
                }
            }
            //else {
                //console.debug('existing untouched');
            //}
            foundMatches.push(match[0]);
        }

        //Delete params no longer in the SQL
        var i = $scope.report.Parameters.length;
        while (i--) {
            if (_.indexOf(foundMatches, $scope.report.Parameters[i].SqlKey) === -1) {
                $scope.report.Parameters.splice(i, 1);
                //console.debug('deleted orphaned');
            }
        }
        $scope.reportDataChanged();
    };

    $scope.addNewParameter = function (keyOfParameter) {
        //console.debug('new parameter');
        $scope.report.Parameters.push({ SqlKey: keyOfParameter, Value: '', InputType: 0, Mandatory: false, Label: '', HelpText: '' });
    };

    $scope.save = function () {
        $.ajax({
            type: 'post',
            url: '../api/Designer/SaveReport',
            data: JSON.stringify($scope.report),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            toastr.success("Report Saved","Saved");
        }).error(function (data) {
            toastr.error("Server error when saveing report, please try again later.","Error");
        });
    };
    $scope.init();
}]);