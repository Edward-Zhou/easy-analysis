controllers.controller('navController', ['$scope', '$location', 'repositoryService',
    function ($scope, $location, repositoryService) {
        function setCurrentRepository(code)
        {
            var options = [];

            for (var i = 0; i < $scope.repositories.length; i++) {
                if ($scope.repositories[i].code !== code) {
                    options.push($scope.repositories[i]);
                } else {
                    $scope.currentRepository = $scope.repositories[i];

                    $scope.context.repository = $scope.currentRepository.code;
                }
            }

            $scope.options = options;
        }

        function calculateOptions(code) {
            if ($scope.repositories === undefined) {
                repositoryService.list().then(function (response) {
                    $scope.repositories = response.data;

                    setCurrentRepository(code);
                });
            } else {
                setCurrentRepository(code);
            }
        }

        function changeRepository(code)
        {
            $location.url('/dashboard/' + code);
        }

        $scope.context = {
            controller: '',
            repository: ''
        };


        $scope.changeTo = function (code) {
            changeRepository(code);
        }

        $scope.navigateTo = function (module) {
            console.log('navigate to dashboard');

            var resp = $scope.context.repository;

            if(module === 'dashboard')
            {
                $location.url('/dashboard/' + resp);
            }
            else if (module === 'question')
            {
                $location.url('/explorer/' + resp);
            }
        }

        $scope.menuClass = function (controllerName)
        {
            if(controllerName == $scope.context.controller)
            {
                return 'active'
            } else {
                return '';
            }
        }

        $scope.$on('$routeChangeSuccess', function (angularEvent, current, previous) {
            calculateOptions(current.params.repository);

            $scope.context.controller = current.$$route.controller;
            console.log('route change success: ' + current.params.repository + 'controller: ' + current.$$route.controller);
        });

    }
]);