angular.module('report').controller('reportController', ['$scope', '$http', 'reportViewModel', '$filter', 'queryStringParser', 'reportUrlHelper', function ($scope, $http, viewModel, $filter, queryStringParser, reportUrlHelper) {

    $scope.init = function () {
        viewModel.Report.Parameters.forEach(function (param) {
            if (param.InputType === 5) //SyncedDate
                param.Value = 'SyncedDate';
            if (param.InputType === 6) //SyncedRunningDate
                param.Value = 'SyncedRunningDate';

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

            $scope.dateFormat = 'yyyy-MM-dd';
        });

        var s = queryStringParser.parse(location.search);

        if (s.subscriptionid) {
            $scope.subscriptionId = s.subscriptionid;
            $scope.selectedAction = 'subscribe';
        } else if (s.selectedAction) {
            $scope.selectedAction = s.selectedAction;
        }

        $scope.viewModel = viewModel;

        $scope.triggerOnScreen = triggerOnScreen;
        $scope.triggerBookmark = triggerBookmark;
        $scope.triggerSubscribe = triggerSubscribe;
        $scope.periodChanged = periodChanged;
        $scope.dateChanged = dateChanged;
        $scope.onSubscriptionSaved = onSubscriptionSaved;
        $scope.hasValidParameters = hasValidParameters;
    };
    $scope.init();

    function onSubscriptionSaved() {
        $scope.selectedAction = 'editSubscriptions';
    }

    function dateChanged(parameter) {
        var date = $filter('date')(parameter.Value, $scope.dateFormat);
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

            var text = parameter.Choices['9999'].split(':')[0] + ': ' + from + ' - ' + to;
            parameter.Choices['9999'] = text;
        } else
            parameter.Value = parameter.EnumValue;
    }

    function triggerOnScreen() {
        if ($scope.selectedAction !== 'onScreen')
            $scope.selectedAction = 'onScreen';
        else
            $scope.$broadcast('refreshOnScreen');
    }

    function triggerBookmark() {
        var url = reportUrlHelper.toUrl($scope.viewModel.Report);
        url += '&selectedAction=onScreen';

        location.href = 'Home/Report?' + url;
    }

    function triggerSubscribe() {
        $scope.selectedAction = 'subscribe';
        $scope.subscriptionId = null;
    }

    function hasValidParameters() {
        var valid = true;
        $scope.viewModel.Report.Parameters.forEach(function (param) {
            if (param.Mandatory && !param.Value) {
                valid = false;
            }
        });

        return valid;
    }
}
])