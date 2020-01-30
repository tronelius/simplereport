angular.module('search').controller('searchController', ['$scope', 'viewModel', function ($scope, viewModel) {

    $scope.searchValue = null;

    $scope.init = function () {
        $scope.search = search;
        $scope.viewModel = viewModel;
        $scope.isVisible = isVisible;
        $scope.showReport = showReport;
        $scope.visibleGroups = visibleGroups;
        $scope.reportsMissingGroup = reportsMissingGroup;

    };
    $scope.init();

    function search(value) {
        let groups = viewModel.ReportGroups;
        for (let i = 0; i < groups.length; i++) {
            let group = groups[i];
            for (let j = 0; j < group.length; j++) {

                let report = group[j];
                let match = report.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1;
                report.MatchSearchResult = match;
            }
        }
    }

    function isVisible(group) {
        for (let i = 0; i < group.length; i++) {
            let report = group[i];
            if (report.MatchSearchResult) {
                return true;
            }
        }

        return false;
    }

    function hasName(group) {
        return group.length > 0 && group[0].Group !== null;
    }

    function missingName(group) {
        return group.length === 0 || group[0].Group === null;
    }

    function showReport(id) {
        var url = window.location.origin + "/Home/Report?reportId=" + id;
        window.location.href = url;
    }

    function visibleGroups() {
        if (viewModel && viewModel.ReportGroups) {
            return viewModel.ReportGroups.filter(hasName);
        }

        return [];
    }

    function reportsMissingGroup() {
        let reports = [];

        if (viewModel && viewModel.ReportGroups) {
            let groups = viewModel.ReportGroups.filter(missingName);
            for (let i = 0; i < groups.length; i++) {
                let group = groups[i];
                for (let j = 0; j < group.length; j++) {
                    let report = group[j];
                    reports.push(report);
                }
            }
        }

        return reports;
    }

}]);
