controllers.controller('dashboardController',
    ['$scope', '$location', '$routeParams', 'threadProfileService',
    function ($scope, $location, $routeParams, threadProfileService) {
        $scope.repository = $routeParams.repository;

        function calculateWeight(freq)
        {
            if(freq >= 10)
            {
                return 10;
            } else {
                return freq;
            }
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
                        link: '#/explorer/' + $scope.repository + '?tags=' + tags[i].name,
                        weight: calculateWeight(tags[i].freq)
                    });
                }

                $scope.words = words;
            }, function errorCallback(response) { });
    }
]);