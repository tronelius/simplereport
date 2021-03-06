﻿angular.module('report').controller('reportController', ['$scope', '$http', 'reportViewModel', '$filter', 'queryStringParser', 'reportUrlHelper', '$window', 'reportParameterHelper', function ($scope, $http, viewModel, $filter, queryStringParser, reportUrlHelper, $window, reportParameterHelper) {

    $scope.init = function () {
        $scope.dateFormat = 'yyyy-MM-dd HH:mm';

        viewModel.Report.Parameters = reportParameterHelper.sortedParameters(viewModel.Report.Parameters, viewModel.Report.Sql);

        viewModel.Report.Parameters.forEach(function (param) {
            if (param.InputType === 6) { //listaSyncedDate
                if(param.Value && param.Value !== 'SyncedDate')
                    param.DisplayValue = new Date(param.Value);

                if(param.Value === '')
                    param.Value = 'SyncedDate';

            }
            if (param.InputType === 7) { //SyncedRunningDate
                if (param.Value && param.Value !== 'SyncedRunningDate')
                    param.DisplayValue = new Date(param.Value);

                if (param.Value === '')
                    param.Value = 'SyncedRunningDate';
            }

            //periods of type custom comes on the format Enum:from_to
            if (param.InputType === 3) { //period
                if (param.Value && ~param.Value.indexOf(':')) {
                    var temp = param.Value.split(':');
                    var enumValue = temp[0];

                    if (temp.length > 1) {
                        var values = temp[1].split('_');
                        param.from = values[0];
                        param.to = values[1];
                    }

                    param.EnumValue = enumValue;
                } else {
                    param.EnumValue = param.Value;
                }
            }

            if (param.InputType === 2) { //date
                if (param.Value)
                    param.DisplayValue = new Date(param.Value);
            }
        });

        var s = queryStringParser.parse(location.search);
        $scope.autorefreshmode = false;

        if (s.subscriptionid) {
            $scope.subscriptionId = s.subscriptionid;
            $scope.selectedAction = 'subscribe';
        } else if (s.selectedAction) {
            $scope.selectedAction = s.selectedAction;
            $scope.autorefreshmode = false;
            if (s.selectedAction === 'autorefresh') {
                $scope.autorefreshmode = true;
            }
        }

        $scope.viewModel = viewModel;

        $scope.triggerOnScreen = triggerOnScreen;
        $scope.triggerBookmark = triggerBookmark;
        $scope.triggerSubscribe = triggerSubscribe;
        $scope.triggerAutoRefresh = triggerAutoRefresh;
        $scope.periodChanged = periodChanged;
        $scope.dateChanged = dateChanged;
        $scope.onSubscriptionSaved = onSubscriptionSaved;
        $scope.hasValidParameters = hasValidParameters;
        $scope.hasvalidParametersForSubscription = hasvalidParametersForSubscription;
        $scope.onSendOnceSaved = onSendOnceSaved;
    };
    $scope.init();

    function onSubscriptionSaved() {
        $scope.selectedAction = 'editSubscriptions';
    }

    function onSendOnceSaved() {
        $scope.selectedAction = null;
    }

    function dateChanged(parameter) {
        var date = $filter('date')(parameter.DisplayValue, $scope.dateFormat);
        parameter.Value = date;
    }

    function periodChanged(parameter) {
        //9999 is custom..
        if (parameter.EnumValue === '9999') {
            if (!parameter.from)
                parameter.from = new Date();
            if (!parameter.to)
                parameter.to = new Date();

            var from = $filter('date')(parameter.from, $scope.dateFormat);
            var to = $filter('date')(parameter.to, $scope.dateFormat);

            parameter.Value = parameter.EnumValue + ':' + from + '_' + to;

            parameter.Choices[parameter.Choices.length - 1].Value = 'Custom: ' + from + ' - ' + to;
        } else
            parameter.Value = parameter.EnumValue;
    }

    function triggerOnScreen() {
        if ($scope.selectedAction !== 'onScreen')
            $scope.selectedAction = 'onScreen';
        else
            $scope.$broadcast('refreshOnScreen');
    }

    function triggerAutoRefresh() {
        var url = reportUrlHelper.toUrl($scope.viewModel.Report);
        url += '&selectedAction=autorefresh';

        $window.location.href = '/Home/Report?' + url;
    }

    function triggerBookmark() {
        var url = reportUrlHelper.toUrl($scope.viewModel.Report);
        url += '&selectedAction=onScreen';

        $window.location.href = '/Home/Report?' + url;
    }

    function triggerSubscribe() {
        $scope.viewModel.Report.Parameters.forEach(function(param) {
            if (param.InputType === 6) //SyncedDate
                param.Value = 'SyncedDate';
            if (param.InputType === 7) //SyncedRunningDate
                param.Value = 'SyncedRunningDate';
        });
        $scope.selectedAction = 'subscribe';
        $scope.subscriptionId = null;
    }

    function hasValidParameters() {
        var valid = true;
        $scope.viewModel.Report.Parameters.forEach(function (param) {
            if (param.Mandatory && (!param.Value || (param.Value === 'SyncedDate' || param.Value === 'SyncedRunningDate'))) {
                valid = false;
            }
        });

        return valid;
    }

    function hasvalidParametersForSubscription() {
        var valid = true;
        $scope.viewModel.Report.Parameters.forEach(function (param) {
            if (param.InputType !== 7 && param.InputType !== 6) { //Exclude syncparameters
                if (param.Mandatory && !param.Value) {
                    valid = false;
                }
            }
        });
        return valid;
    }

    $scope.getTypeAheadData = function(reportid, typeaheadid, search) {
        return $http.post('Home/GetTypeAheadData?reportid=' + reportid + '&typeaheadid=' + typeaheadid + '&search='+search).then(function (response) {
            return response.data;
        });
    };

    $scope.onSelect = function (par, item, model, label) {
        par.Value = item.Id;
        par.typeaheadLabel = item.Name;
    };
}
])