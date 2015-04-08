angular.module('designer', []);

angular.module('designer').controller('designerController', ['$scope', '$http', function ($scope, $http) {
    $scope.activeTab = 'report';

    $scope.init = function() {
        $scope.activeTab = 'report';
        $http.get('api/Designer/GetViewModel').
            success(function (data) {
                $scope.inputTypes = data.InputTypes;
                $scope.reportList = data.Reports;
                $scope.connections = data.Connections;
                $scope.lookupReports = data.LookupReports;
                $scope.accessLists = data.AccessLists;
                $scope.settings = data.Settings;
            }).
            error(function (data) {
                toastr.error("Couldn't get list of reports from server.","Error");
            });
    };
    $scope.init();

    //Globals and help functions
    var regExParameterMatch = /(@\w+)/g;
    var parameterPositionHash = [];
    function updateCollection(collection, updatedEntity) {
        var index = collection.indexOf(_.find(collection, function (entity) { return entity.Id === updatedEntity.Id }));
        if (index !== -1) {
            collection.splice(index, 1, updatedEntity);
        } else {
            collection.push(updatedEntity);
        }
    }

    //Reports
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
            var existingparam = _.findWhere($scope.report.Parameters, { SqlKey: match[0] });
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
    $scope.addNewReport = function() {
        $scope.report = { Id: null };
    };
    $scope.addNewParameter = function (keyOfParameter) {
        //console.debug('new parameter');
        $scope.report.Parameters.push({ SqlKey: keyOfParameter, Value: "", InputType: 0, Mandatory: false, Label: "", HelpText: "" });
    };
    $scope.saveReport = function () {
        $.ajax({
            type: 'post',
            url: 'api/Designer/SaveReport',
            data: JSON.stringify($scope.report),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            toastr.success("Report saved", "Saved");
            $scope.report = data;
            updateCollection($scope.reportList, $scope.report);
        }).error(function (data) {
            toastr.error("Server error when saving report.", "Error");
        });
    };

    //Connections
    $scope.connectionChanged = function (){};
    $scope.saveConnection = function () {
        $.ajax({
            type: 'post',
            url: 'api/Designer/SaveConnection',
            data: JSON.stringify($scope.connection),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            toastr.success("Connection saved", "Saved");
            $scope.connection = data;
            updateCollection($scope.connections, $scope.connection);
        }).error(function (data) {
            toastr.error("Server error when saving connection.", "Error");
        });
    };
    $scope.addNewConnection = function() {
        $scope.connection = { Id: null };
    };
    $scope.verifyConnection = function() {
        $.ajax({
            type: 'post',
            url: 'api/Designer/VerifyConnection',
            data: JSON.stringify($scope.connection),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            $scope.connection.Verified = data.Success;
            $scope.$apply();
            if (data.Success) {
                toastr.success("Connection verified", "OK!");
            } else {
                toastr.error("Connectionstring is not valid, and vill not work", "Not OK!");
            }
        }).error(function (data) {
            toastr.error("Server error when verifing the connection.", "Error");
        });
    };

    //Dropdown parameters
    $scope.lookupReportChanged = function (){};
    $scope.verifyLookupSql = function () {
        var re = /((\bid\b).+(\bname\b))|((\bname\b).+(\bid\b))/i;
        var match;
        if ((match = re.exec($scope.lookupreport.Sql)) !== null) {
            $scope.lookupreport.SqlOk = false;
        } else {
            $scope.lookupreport.SqlOk = true;
        }
    }
    $scope.addNewDropdownParameter = function() {
        $scope.lookupreport = { Id: null };
    };
    $scope.saveDropdownParameter = function () {
        $.ajax({
            type: 'post',
            url: 'api/Designer/SaveLookupReport',
            data: JSON.stringify($scope.lookupreport),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            toastr.success("Dropdown parameter saved", "Saved");
            $scope.lookupreport = data;
            updateCollection($scope.lookupReports, $scope.lookupreport);
        }).error(function (data) {
            toastr.error("Server error when saving dropdown parameter.", "Error");
        });
    };

    //Access Lists
    $scope.accessListChanged = function (){};
    $scope.addNewAccessList = function() {
        $scope.access = { Id: null };
    };
    $scope.saveAccesList = function () {
        $.ajax({
            type: 'post',
            url: 'api/Designer/SaveAccessList',
            data: JSON.stringify($scope.access),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            toastr.success("Access control list saved", "Saved");
            $scope.access = data;
            updateCollection($scope.accessLists, $scope.access);
        }).error(function (data) {
            toastr.error("Server error when saving access control list.", "Error");
        });
    };

    //Settings
    $scope.saveSettings = function() {
        $.ajax({
            type: 'post',
            url: 'api/Designer/SaveSettings',
            data: JSON.stringify($scope.settings),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            toastr.success("Settings saved", "Saved");
        }).error(function (data) {
            toastr.error("Server error when saving settings.", "Error");
        });
    };

}]);