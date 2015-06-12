angular.module('subscriptions', ['shared', 'ui.bootstrap']);

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
         controller: ['$scope', '$http', function ($scope, $http) {

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

                 $http.get("api/schedule/all").success(function (data) {
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
                 $http.post("api/schedule/save", $scope.schedule).success(function (data) {
                     console.log('save', data);
                     if (!$scope.schedule.Id) {
                         $scope.schedule.Id = data.Id;
                         $scope.data.push($scope.schedule);
                         $scope.selectedSchedule = $scope.schedule;
                     }
                 }).error(function () {
                     toastr.error('Something went wrong, please try again later or contact support');
                 });
             }

             function remove() {
                 if (!$scope.schedule.Id) {
                     $scope.schedule = null;
                     return;
                 }

                 $http.post("api/schedule/delete", { Id: $scope.schedule.Id }).success(function (data) {
                     $scope.data = data;
                     $scope.schedule = null;
                 }).error(function () {
                     toastr.error('Something went wrong, please try again later or contact support');
                 });
             }
         }]
     };
 });