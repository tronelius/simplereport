angular.module('schedule').directive('scheduleEditor', function () {
     return {
         templateUrl: 'scripts/app/templates/scheduleEditor.html',
         scope: {},
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
                 if (!$scope.schedule.Name || !$scope.schedule.Cron) {
                     toastr.warn('A schedule needs both a name and an interval');
                 }

                 scheduleRepository.save($scope.schedule).success(function (data) {
                     if (data.Error) {
                         toastr.error(data.Error);
                         return;
                     }

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
                     if (!data.error) {
                         $scope.data = data;
                         $scope.schedule = null;
                         toastr.success('Schedule removed');
                     } else {
                         toastr.error(data.error);
                     }
                 }).error(function () {
                     toastr.error('Something went wrong, please try again later or contact support');
                 });
             }
         }]
     };
 })