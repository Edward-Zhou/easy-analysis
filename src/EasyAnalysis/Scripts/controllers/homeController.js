controllers.controller('homeController', ['$scope', '$location', function ($scope, $location) {
    $scope.navigateTo = function (repository) {
        $location.url('/discover/' + repository);
    }
}]);