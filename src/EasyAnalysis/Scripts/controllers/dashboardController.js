controllers.controller('dashboardController',
    ['$scope', '$location', '$routeParams', 'threadProfileService',
    function ($scope, $location, $routeParams, threadProfileService) {
        $scope.repository = $routeParams.repository;

        $scope.navigateTo = function (id) {
            $location.url('/detail/' + $scope.repository + '/' + id);
        }

        var range = Utility.getDateRange('l30d');

        $scope.words = [];

        threadProfileService.relatedTags($scope.repository, range[0], range[1], [], '', 150)
            .then(function (response) {
                var tags = response.data;

                var words = [];

                for (var i = 0; i < tags.length; i++) {
                    words.push({
                        text: tags[i].name,
                        weight: tags[i].freq
                    });
                }

                $scope.words = words;
            }, function errorCallback(response) { });
    }
]);