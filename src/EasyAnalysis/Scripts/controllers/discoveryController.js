controllers.controller('discoverController', ['$scope', '$location', 'threadService', '$routeParams',
        function ($scope, $location, threadService, $routeParams) {
            $scope.state = 'init';

            $scope.repository = $routeParams.repository;

            $scope.URIText_keypress = function (e) {
                if (e.which === 13) {
                    $scope.state = 'search';

                    threadService.query($scope.URIText)
                                 .success(function (identifier) {
                                     $location.url('/detail/' + $scope.repository + '/' + identifier);
                                 });
                }
            }

            $scope.goto = function (module) {
                $location.url('/' + module + '/' + $scope.repository);
            }

            $scope.navigateTo = function (id) {
                $location.url('/detail/' + $scope.repository + '/' + id);
            }

            threadService.todo($scope.repository).success(function (data) {
                $scope.todo = data;
            });
        }]);