//Desperate need for refactoring... but later...
angular.module('designer').controller('designerController', ['$scope', '$http', 'subscriptionRepository', 'designerRepository', 'Upload', 'reportParameterHelper', function ($scope, $http, subscriptionRepository, designerRepository, upload, reportParameterHelper) {
    $scope.activeTab = 'report';

    $scope.init = function () {
        $scope.activeTab = 'report';
        $http.get('api/Designer/GetViewModel').
            success(function (data) {
                $scope.inputTypes = data.InputTypes;
                $scope.reportList = data.Reports;
                $scope.connections = data.Connections;
                $scope.lookupReports = data.LookupReports;
                $scope.typeAheadReports = data.TypeAheadReports;
                $scope.accessLists = data.AccessLists;
                $scope.reportOwnerAccessLists = data.ReportOwnerAccessLists;
                $scope.settings = data.Settings;
                $scope.accessEditorViewModel = data.AccessEditorViewModel;
                $scope.SubscriptionEnabled = data.SubscriptionEnabled;
                $scope.reportResultTypes = data.ReportResultTypes;
                $scope.connectionTypes = data.ConnectionTypes;
            }).
            error(function (data) {
                toastr.error("Couldn't get list of reports from server.", "Error");
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
        //$scope.$apply();
    }

    //Reports
    $scope.reportDataChanged = function () {
        if ($scope.SubscriptionEnabled)
            createOriginalParameterBackup();

        if ($scope.SubscriptionEnabled && !$scope.report.hasLoadedSubscriptions) {//we dont want this to run on every keyup.. and yes i know we reload subs on every save, but i think that is correct.
            $scope.report.hasLoadedSubscriptions = true;
            var report = $scope.report;
            report.warnForParameterChanges = true;
            subscriptionRepository.allForReport(report.Id).success(function (data) {
                report.SubscriptionCount = data.length;
                report.warnForParameterChanges = data.length > 0;
            }).error(function () {
                toastr.warning('Could not load subscriptions for the report. Validation will assume there are subscriptions for your safety.');
            });
        }

        $scope.report.Parameters = reportParameterHelper.sortedParameters($scope.report.Parameters, $scope.report.Sql);
        $scope.filterReports();
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

        $scope.report.Parameters = reportParameterHelper.sortedParameters($scope.report.Parameters, $scope.report.Sql);
        $scope.reportDataChanged();
    };

    function parameterChecksum(p) {
        var o = { InputType: p.InputType, LookupReportId: p.LookupReportId };

        if (p.Mandatory && p.Value)
            o.hasDefaultValue = true;

        return o;
    }

    function createOriginalParameterBackup(force) {
        if (!force && $scope.report.parameterChecksums)
            return;

        var backup = {};
        $scope.report.Parameters.forEach(function (p) {
            backup[p.SqlKey] = parameterChecksum(p);
        });

        $scope.report.parameterChecksums = backup;
    }

    function isChecksumsMatching(origin, current) {
        var valid = true;
        //this works based on the fact that currently we dont care if checksum params are added to current, because the only addition is a default value which will obviously work.
        Object.keys(origin).forEach(function (k) {
            if (origin[k] !== current[k]) {
                valid = false;
            }
        });

        return valid;
    }

    function hasMadeInvalidParameterChanges() {
        var invalid = false;
        $scope.report.Parameters.forEach(function (p) {
            var checksum = $scope.report.parameterChecksums[p.SqlKey];
            if (checksum) {
                if (!isChecksumsMatching(checksum, parameterChecksum(p))) {
                    invalid = true;
                }
            } else {
                if (p.Value === undefined || p.Value === '') {
                    invalid = true;
                }
            }
        });

        return invalid;
    }

    $scope.addNewSingleReport = function () {
        $scope.report = { Id: null, Parameters: [], TemplateEditorAccessStyle: 0, SubscriptionAccessStyle: 0, ReportOwnerAccessId: $scope.reportOwnerAccessLists[0].Id, AccessId: $scope.accessLists[0].Id, ReportType: 0, Sql: "" };
    };
    $scope.addNewMultiReport = function () {
        $scope.report = { Id: null, Parameters: [], TemplateEditorAccessStyle: 0, SubscriptionAccessStyle: 0, ReportOwnerAccessId: $scope.reportOwnerAccessLists[0].Id, AccessId: $scope.accessLists[0].Id, ReportType: 1, ConvertToPdf: false, OnScreenFormatAllowed: false, ReportList: [], ReportResultType: "WordResultPlain", TemplateFormat: 2, Sql: "" };
        $scope.filterReports();
    };

    $scope.filteredReports = {};

    $scope.filterReports = function () {
        var reports = $scope.reportList;
        var alreadyAddedReports = $scope.report.ReportList || [];
        var filteredReports = reports.filter(function(report) { return report.ReportType !== 1 && report.ReportResultType !== null && report.ReportResultType !== "ExcelResultPlain" && !alreadyAddedReports.some((r) => { return r.LinkedReportId === report.Id }) });
        filteredReports.sort((a, b) => {
            var nameA = a.Name.toUpperCase(); // ignore upper and lowercase
            var nameB = b.Name.toUpperCase(); // ignore upper and lowercase
            if (nameA < nameB) {
                return -1;
            }
            if (nameA > nameB) {
                return 1;
            }
            
            return 0;
        });
        $scope.filteredReports = filteredReports;
    };

    $scope.addLinkedReport = function (report) {
        var order = $scope.report.ReportList ? $scope.report.ReportList.length : 0;
        var linkedReport = { LinkedReportId: report.Id, ReportId: $scope.report.Id, Name: report.Name, Order: order };

        $scope.report.ReportList.push(linkedReport);
        $scope.filterReports();
    };

    $scope.removeLinkedReport = function (report) {
        var index = $scope.report.ReportList.indexOf(report);
        $scope.report.ReportList.splice(index, 1);
        $scope.report.ReportList.forEach((linkedReport, i) => { // set new Order
            linkedReport.Order = i;
        });
        $scope.filterReports();
    };

    $scope.addNewParameter = function (keyOfParameter) {
        $scope.report.Parameters.push({ SqlKey: keyOfParameter, Value: "", InputType: 0, Mandatory: false, Label: "", HelpText: "" });
    };

    $scope.goToReport = function () {
        if (!$scope.report || !$scope.report.Id) return;
        var redirectUrl = "/home/Report?reportId=" + $scope.report.Id;
        $scope.saveReport(false, () => { window.open(redirectUrl, "rapportfliken"); });
    }

    $scope.saveReport = function (force, callback = null) {
        $scope.showSaveConfirmation = false;

        if ($scope.report.ReportType !== 1 && !$scope.report.ConnectionId)
            return;
       
        if ($scope.SubscriptionEnabled && !force && $scope.report.warnForParameterChanges) {
            if (hasMadeInvalidParameterChanges()) {
                $scope.showSaveConfirmation = true;
                return;
            }
        }

        designerRepository.saveReport($scope.report).then(function (result) {
            toastr.success("Report saved", "Saved");
            $scope.report = result.data;
            $scope.reportDataChanged();
            updateCollection($scope.reportList, $scope.report);
            if (callback) {
                callback();
            }
        }, function (error) {
            toastr.error("Server error when saving report.", "Error");
        });
    };

    $scope.deleteReport = function (force) {
        if (!force) {
            $scope.showDeleteConfirmation = true;
            return;
        }
        $scope.showDeleteConfirmation = false;

        designerRepository.deleteReport($scope.report).then(function (result) {
            if (result.data.Success) {
                toastr.success("Report is deleted", "Deleted");
                updateCollection($scope.reportList, $scope.report, true);
                $scope.report = null;
                $scope.$apply();
            } else {
                toastr.error(result.data.FullMessage, "Error");
            }
        }, function (data) {
            toastr.error("Server error when deleting report.", "Error");
        });
    };

    //Connections
    $scope.connectionChanged = function () { };
    $scope.saveConnection = function () {
        designerRepository.saveConnection($scope.connection).then(function (result) {
            toastr.success("Connection saved", "Saved");
            $scope.connection = result.data;
            updateCollection($scope.connections, $scope.connection);
        }, function (data) {
            toastr.error("Server error when saving connection.", "Error");
        });
    };
    $scope.addNewConnection = function () {
        $scope.connection = { Id: null };
    };
    $scope.verifyConnection = function () {
        designerRepository.verifyConnection($scope.connection).then(function (result) {
            $scope.connection.Verified = result.data.Success;
            if (result.data.Success) {
                toastr.success("Connection verified", "OK!");
            } else {
                toastr.error("Connectionstring is not valid and wont work", "Not OK!");
            }
        }, function (result) {
            toastr.error("Server error when verifing the connection.", "Error");
        });
    };
    $scope.deleteConnection = function () {
        designerRepository.deleteConnection($scope.connection).then(function (result) {
            if (result.data.Success) {
                toastr.success("Connection is deleted", "Deleted");
                updateCollection($scope.connections, $scope.connection, true);
                $scope.connection = null;
            } else {
                toastr.error(result.data.FullMessage, "Error");
            }
        }, function (result) {
            toastr.error("Server error when deleting connection.", "Error");
        });
    };

    //Dropdown parameters
    $scope.verifyLookupSql = function () {
        var re = /((\bid\b).+(\bname\b))|((\bname\b).+(\bid\b))/i;
        var match;
        if ((match = re.exec($scope.lookupreport.Sql)) !== null) {
            $scope.lookupreport.SqlOk = false;
        } else {
            $scope.lookupreport.SqlOk = true;
        }
    }
    $scope.addNewDropdownParameter = function () {
        $scope.lookupreport = { Id: null };
    };
    $scope.saveDropdownParameter = function () {
        designerRepository.saveLookupReport($scope.lookupreport).then(function (result) {
            toastr.success("Dropdown parameter saved", "Saved");
            $scope.lookupreport = result.data;
            updateCollection($scope.lookupReports, $scope.lookupreport);
        }, function (result) {
            toastr.error("Server error when saving dropdown parameter.", "Error");
        });
    };
    $scope.deleteDropdownParameter = function () {
        designerRepository.deleteReport($scope.lookupreport).then(function (result) {
            if (result.data.Success) {
                toastr.success("dropdown parameter is deleted", "Deleted");
                updateCollection($scope.lookupReports, $scope.lookupreport, true);
                $scope.lookupreport = null;
            } else {
                toastr.error(result.FullMessage, "Error");
            }
        }, function (result) {
            toastr.error("Server error when deleting dropdown parameter.", "Error");
        });
    };

    //TypeAheads
    $scope.verifytypeAheadReportSql = function () {
        var re = /((\bid\b).+(\bname\b))|((\bname\b).+(\bid\b))/i;
        var match;
        if ((match = re.exec($scope.typeAheadReport.Sql)) !== null) {
            $scope.typeAheadReport.SqlOk = false;
        } else {
            $scope.typeAheadReport.SqlOk = true;
        }
    }
    $scope.addNewtypeaheadreport = function () {
        $scope.typeAheadReport = { Id: null };
    };
    $scope.saveTypeAhead = function () {
        designerRepository.saveTypeAheadReport($scope.typeAheadReport).then(function (result) {
            toastr.success("Typeahead parameter saved", "Saved");
            $scope.typeAheadReport = result.data;
            updateCollection($scope.typeAheadReports, $scope.typeAheadReport);
        }, function (result) {
            toastr.error("Server error when saving typeahead parameter.", "Error");
        });
    };
    $scope.deleteTypeAhead = function () {
        designerRepository.deleteTypeAheadReport($scope.typeAheadReport).then(function (result) {
            if (result.data.Success) {
                toastr.success("Typeahead parameter is deleted", "Deleted");
                updateCollection($scope.typeAheadReports, $scope.typeAheadReport, true);
                $scope.typeAheadReport = null;
            } else {
                toastr.error(result.FullMessage, "Error");
            }
        }, function (result) {
            toastr.error("Server error when deleting typeahead parameter.", "Error");
        });
    };

    //Access Lists
    $scope.accessListChanged = function () { };
    $scope.addNewAccessList = function () {
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
    $scope.deleteAccessList = function () {
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
    $scope.saveSettings = function () {
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

    $scope.exportModel = function () {
        window.open(designerRepository.exportModelUrl(), '_blank', '');
    };
    $scope.importModel = function (files) {
        if (files && files.length) {
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                upload.upload({
                    url: 'api/Designer/ImportModel',
                    file: file
                }).success(function (data, status, headers, config) {
                    if (!data.error) {
                        if (data.message !== undefined) {
                            toastr.info("Model errors:" + data.message);
                        }
                        toastr.success("New data model imported!");
                    } else {
                        toastr.error(data.error);
                    }
                }).error(function () {
                    toastr.error("Couldn't upload the file, please try again.", "Error");
                });
            }
        }
    }
    $scope.clearModel = function (force) {
        if (!force) {
            $scope.showClearConfirmation = true;
        } else {
            designerRepository.clearModel();
            $scope.showClearConfirmation = false;
        }
    };
}]);