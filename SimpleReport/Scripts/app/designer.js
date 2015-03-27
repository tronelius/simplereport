angular.module('designer', []);

angular.module('designer').controller('designerController', ['$scope', '$http', function ($scope, $http) {
    $scope.activeTab = 'report';

    $scope.showReportTab = function() {
        $scope.activeTab = 'report';
        $http.get('../api/Designer/reports').
            success(function (data) {
                $scope.inputTypes = data.InputTypes;
                $scope.reportList = data.Reports;
            }).
            error(function (data) {
                toastr.error("Couldn't get list of reports from server.","Error");
            });
    };

    $scope.reportChanged = function () {
        console.debug('report changed');
        $scope.latestSql = $scope.report.Sql;
    };

    $scope.analyzeSQL = function () {
        var currentSQL = $scope.report.Sql;
        var re = /(@\w+)/g;
        var match;
        var currentPosition = $('#sqlarea').prop('selectionStart');
        var foundMatches = [];
        while (match = re.exec(currentSQL)) {
            console.debug(match[0]);
            var existingparam = _.findWhere($scope.report.Parameters, { SqlKey:match[0]});
            if (existingparam === undefined) {
                if (currentPosition >= match.index && currentPosition <= match.index + match[0].length) {
                    console.debug('updated existing');
                    //existingparam.SqlKey = match[0];
                } else {
                    console.debug('new');
                    $scope.report.Parameters.push({ SqlKey: match[0], Value: '', InputType: 0, Mandatory: false, Label: '', HelpText: '' });
                }
            } else {
                console.debug('existing untouched');
            }
            foundMatches.push(match[0]);
        }

        //Delete params no longer in the SQL
        var i = $scope.report.Parameters.length;
        while (i--) {
            if (_.indexOf(foundMatches, $scope.report.Parameters[i].SqlKey) === -1) {
                $scope.report.Parameters.splice(i, 1);
                console.debug('deleted orphaned');
            }
        }
        
        $scope.latestSql = currentSQL;
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
    $scope.showReportTab();
}]);