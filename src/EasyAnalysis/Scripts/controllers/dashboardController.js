controllers.controller('dashboardController', ['$scope', '$location', 'threadService', '$routeParams', 'threadProfileService',
    function ($scope, $location, threadService, $routeParams, threadProfileService) {
        $scope.state = 'init';

        $scope.tagCoverage = {
            daily: 0.0,
            weekly: 0.0,
            monthly: 0.0
        };

        $scope.categoryCoverage = {
            daily: 0.0,
            weekly: 0.0,
            monthly: 0.0
        };

        $scope.repository = $routeParams.repository;

        $scope.navigateTo = function (id) {
            $location.url('/detail/' + $scope.repository + '/' + id);
        }

        threadService.getTagCoverage($scope.repository).success(function (data) {
            $scope.tagCoverage = data;
        });

        threadService.getCategoryCoverage($scope.repository).success(function (data) {
            $scope.categoryCoverage = data;
        });

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