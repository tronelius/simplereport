angular.module('designer', ['shared']);

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
                $scope.reportOwnerAccessLists = data.ReportOwnerAccessLists;
                $scope.settings = data.Settings;
                $scope.accessEditorViewModel = data.AccessEditorViewModel;

            }).
            error(function (data) {
                toastr.error("Couldn't get list of reports from server.","Error");
            });
    };
    $scope.init();

    //Globals and help functions
    var regExParameterMatch = /(@\w+)/g;
    var parameterPositionHash = [];
    function updateCollection(collection, updatedEntity, deleted) {
        var index = collection.indexOf(_.find(collection, function (entity) { return entity.Id === updatedEntity.Id }));

        if (deleted) {
            collection.splice(index, 1);
        } else {    
            if (index !== -1) {
                collection.splice(index, 1, updatedEntity);
            } else {
                collection.push(updatedEntity);
            }
        }
        $scope.$apply();
    }

    //Reports
    $scope.reportDataChanged = function () {
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
            var existingparam = _.findWhere($scope.report.Parameters, { SqlKey: match[0] });
            if (existingparam === undefined) {
                if (currentPosition >= match.index && currentPosition <= match.index + match[0].length) {
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
            foundMatches.push(match[0]);
        }

        //transform from and to-parameters to a period-parameter.
        var periodIsPresentAndValid = false;
        var periodFrom = _.findWhere($scope.report.Parameters, { SqlKey: '@from' });
        var periodTo = _.findWhere($scope.report.Parameters, { SqlKey: '@to' });
        var periodFromAndTo = _.findWhere($scope.report.Parameters, { SqlKey: '@from_@to' });
        if (periodFrom !== undefined && periodTo !== undefined) {
            $scope.report.Parameters.splice($scope.report.Parameters.indexOf(periodTo), 1);
            periodIsPresentAndValid = true;
            if (periodFromAndTo === undefined) {
                periodFrom.SqlKey = '@from_@to';
                periodFrom.InputType = 3;
            } else { //combined parameter already exists.
                $scope.report.Parameters.splice($scope.report.Parameters.indexOf(periodFrom), 1);
            }
        }

        //Delete params no longer in the SQL
        var i = $scope.report.Parameters.length;
        while (i--) {
            if (_.indexOf(foundMatches, $scope.report.Parameters[i].SqlKey) === -1) {
                if ($scope.report.Parameters[i].SqlKey === '@from_@to') {
                   if (!periodIsPresentAndValid) {
                       $scope.report.Parameters.splice(i, 1);
                   }
                } else {
                    $scope.report.Parameters.splice(i, 1);
                }
            }
        }

        

        $scope.reportDataChanged();
    };
    $scope.addNewReport = function() {
        $scope.report = { Id: null, Parameters:[],TemplateEditorAccessStyle : 0 };
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
    $scope.deleteReport = function () {
        $.ajax({
            type: 'post',
            url: 'api/Designer/DeleteReport',
            data: JSON.stringify($scope.report),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            if (data.Success) {
                toastr.success("Report is deleted", "Deleted");
                updateCollection($scope.reportList, $scope.report, true);
                $scope.report = null;
                $scope.$apply();
            } else {
                toastr.error(data.FullMessage, "Error");
            }
        }).error(function (data) {
            toastr.error("Server error when deleting report.", "Error");
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
    $scope.deleteConnection = function () {
        $.ajax({
            type: 'post',
            url: 'api/Designer/DeleteConnection',
            data: JSON.stringify($scope.connection),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            if (data.Success) {
                toastr.success("Connection is deleted", "Deleted");
                updateCollection($scope.connections, $scope.connection, true);
                $scope.connection = null;
                $scope.$apply();
            } else {
                toastr.error(data.FullMessage, "Error");
            }
        }).error(function (data) {
            toastr.error("Server error when deleting connection.", "Error");
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
    $scope.deleteDropdownParameter = function () {
        $.ajax({
            type: 'post',
            url: 'api/Designer/DeleteLookupReport',
            data: JSON.stringify($scope.lookupreport),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            if (data.Success) {
                toastr.success("Lookup report is deleted", "Deleted");
                updateCollection($scope.lookupReports, $scope.lookupreport, true);
                $scope.lookupreport = null;
                $scope.$apply();
            } else {
                toastr.error(data.FullMessage, "Error");
            }
        }).error(function (data) {
            toastr.error("Server error when deleting lookup report.", "Error");
        });
    };

    //Access Lists
    $scope.accessListChanged = function (){};
    $scope.addNewAccessList = function() {
        $scope.access = { Id: null };
    };
    $scope.saveAccessList = function () {
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
            updateCollection($scope.reportOwnerAccessLists, $scope.access);
        }).error(function (data) {
            toastr.error("Server error when saving access control list.", "Error");
        });
    };
    $scope.deleteAccessList = function() {
        $.ajax({
            type: 'post',
            url: 'api/Designer/DeleteAccessList',
            data: JSON.stringify($scope.access),
            processData: false,
            contentType: 'application/json; charset=utf-8'
        }).success(function (data) {
            if (data.Success) {
                toastr.success("Access control list is deleted", "Deleted");
                updateCollection($scope.accessLists, $scope.access, true); 
                updateCollection($scope.reportOwnerAccessLists, $scope.access, true);
                $scope.access = null;
                $scope.$apply();
            } else {
                toastr.error(data.FullMessage, "Error");
            }
        }).error(function (data) {
            toastr.error("Server error when deleting access control list.", "Error");
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