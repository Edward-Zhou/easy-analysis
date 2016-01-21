controllers.controller('navController', ['$scope', '$location', 'repositoryService',
    function ($scope, $location, repositoryService) {
        function setCurrentRepository(code)
        {
            var options = [];

            for (var i = 0; i < $scope.repositories.length; i++) {
                if ($scope.repositories[i].code !== code) {
                    options.push($scope.repositories[i]);
                } else {
                    $scope.context.repository = $scope.repositories[i];
                }
            }

            $scope.options = options;
        }

        function promise(fn)
        {
            if ($scope.repositories === undefined) {
                repositoryService.list().then(function (response) {
                    if ($scope.repositories === undefined)
                    {
                        $scope.repositories = response.data;
                    }
                    
                    fn.call();
                });
            } else {
                fn.call();
            }
        }

        function calculateOptions(code) {
            promise(function () { setCurrentRepository(code); });
        }

        function changeRepository(code)
        {
            $location.url('/dashboard/' + code);
        }

        $scope.context = {
            controller: '',
            repository: {
                code: '',
                text: '-- select --'
            }
        };


        $scope.changeTo = function (code) {
            changeRepository(code);
        }

        $scope.navigateTo = function (module) {
            console.log('navigate to dashboard');

            var resp = $scope.context.repository.code;

            if(module === 'dashboard')
            {
                $location.url('/dashboard/' + resp);
            }
            else if (module === 'question')
            {
                $location.url('/explorer/' + resp);
            }
            else if (module === 'report')
            {
                $location.url('/report/' + resp);
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
        });
    }
]);