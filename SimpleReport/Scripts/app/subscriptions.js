angular.module('subscriptions', ['shared', 'ui.bootstrap', 'repository']);

angular.module('subscriptions')
    .controller('subscriptionController', ['$scope', '$http', function ($scope, $http) {

        $scope.init = function () {
            $scope.activeTab = 'all';
        };
        $scope.init();

    }
    ])
 .directive('scheduleEditor', function () {
     return {
         templateUrl: 'scripts/app/templates/scheduleEditor.html',
         scope: { },
         controller: ['$scope', '$http', 'scheduleRepository', function ($scope, $http, scheduleRepository) {

             $scope.init = function () {
                 $scope.schedule = null;
                 $scope.selectedSchedule = null;
                 $scope.data = null;

                 fetchData();

                 $scope.selectedScheduleChanged = selectedScheduleChanged;
                 $scope.addNew = addNew;
                 $scope.save = save;
                 $scope.delete = remove;
             };
             $scope.init();

             function fetchData() {
                 $scope.data = null;

                 scheduleRepository.getAll().success(function (data) {
                     $scope.data = data;
                 }).error(function () {
                     toastr.error('Something went wrong, please try again later or contact support');
                 });
             }

             function selectedScheduleChanged() {
                 $scope.schedule = $scope.selectedSchedule;
             }

             function addNew() {
                 $scope.schedule = { Name: '', Cron: '' };
                 $scope.selectedSchedule = null;
             }

             function save() {
                 scheduleRepository.save($scope.schedule).success(function (data) {
                     if (!$scope.schedule.Id) {
                         $scope.schedule.Id = data.Id;
                         $scope.data.push($scope.schedule);
                         $scope.selectedSchedule = $scope.schedule;
                     }

                     toastr.success('Schedule saved');
                 }).error(function () {
                     toastr.error('Something went wrong, please try again later or contact support');
                 });
             }

             function remove() {
                 if (!$scope.schedule.Id) {
                     $scope.schedule = null;
                     return;
                 }

                 scheduleRepository.delete($scope.schedule.Id).success(function (data) {
                     $scope.data = data;
                     $scope.schedule = null;
                     toastr.success('Schedule removed');
                 }).error(function () {
                     toastr.error('Something went wrong, please try again later or contact support');
                 });
             }
         }]
     };
 })
.directive('cronEditor', ['$timeout', function ($timeout) {
    return {
        restrict: 'A',
        require: 'ngModel',
        scope: { value: '=ngModel' },
        link: function (scope, element, attrs, ngModelCtrl) {
            $(function () {
                var options = {
                    onChange: function () {
                        $timeout(function () {
                            ngModelCtrl.$setViewValue(($(element).cron("value")));
                        });
                    },
                    customValues: {
                        "quarter": "0 0 30 6,9 *;0 0 31 3,12 *",//two crons in one..
                    }
                }

                var oldValue;
                scope.$watch('value', function (val) {
                    if (oldValue !== val) {
                        oldValue = val;
                        $(element).cron("value", val);
                    }
                });

                $(element).cron(options);
            });
        }
    }
}]);