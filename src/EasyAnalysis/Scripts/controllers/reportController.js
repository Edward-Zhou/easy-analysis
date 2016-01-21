controllers.controller('reportController',
    ['$scope', '$routeParams', 'reportService',
    function ($scope, $routeParams, reportService) {
        $scope.repository = $routeParams.repository;

        var range = Utility.getDateRange('l30d');

        reportService
            .run('categoryReport', { repository: $scope.repository, start: range[0], end: range[1] })
            .then(function (response) {
                $scope.categories = response.data;
            });
    }
    ]);