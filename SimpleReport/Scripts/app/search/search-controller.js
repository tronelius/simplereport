angular.module('search').controller('searchController', ['$scope', 'viewModel', '$timeout', function ($scope, viewModel, $timeout) {

    $scope.searchValue = "";
    
    $scope.init = function () {
        $scope.searchResult = [];
        $scope.search = search;
        $scope.viewModel = viewModel;
        $scope.isVisible = isVisible;
        $scope.showReport = showReport;
        $scope.visibleGroups = visibleGroups;
        $scope.reportsMissingGroup = reportsMissingGroup;
        $scope.matchSearchValue = matchSearchValue;
        $scope.inSearchResult = inSearchResult;
        $scope.searchInProgress = searchInProgress;
        $scope.toggleClosed = toggleClosed;
        $scope.expandedGroup = viewModel && viewModel.Report ? viewModel.Report.Group : "";

    };
    $scope.init();

    function search(value) {
        searchValue = value;
        searchResult = [];
        let groups = viewModel.ReportGroups;
        for (let i = 0; i < groups.length; i++) {
            let group = groups[i];
            for (let j = 0; j < group.length; j++) {
                let report = group[j];
                if (matchSearchValue(report)) {
                    searchResult.push(report.Id);
                }
            }
        }
    }

    function matchSearchValue(report) {
        return report.Name.toLowerCase().indexOf(searchValue.toLowerCase()) !== -1;
    }

    function searchInProgress() {
        return $scope.searchValue !== null && $scope.searchValue.length > 0;
    }

    function inSearchResult(report) {
        if (!searchInProgress()) {
            return true;
        }

        return searchResult.indexOf(report.Id) !== -1;
    }

    function isVisible(group) {
        if (!searchInProgress()) {
            return true;
        }

        for (let i = 0; i < group.length; i++) {
            let report = group[i];
            if (inSearchResult(report)) {
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

    function toggleClosed(group) {
        $timeout(function() {
            var open = $scope.expandedGroup === group[0].Group;
            if (open) {
                $scope.expandedGroup = "";
            } else {
                $scope.expandedGroup = group[0].Group;
            }
        });
    }

}]);
