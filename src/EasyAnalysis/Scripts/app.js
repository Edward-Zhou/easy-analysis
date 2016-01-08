var controllers = angular.module('controllers', []);

var app = angular.module('_app_', ['ngRoute', 'controllers', 'ngTagsInput', 'angular-jqcloud']);

app.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/', {
            templateUrl: 'partials/home.html',
            controller: 'homeController'
        }).when('/discover/:repository', {
            templateUrl: 'partials/discover.html',
            controller: 'discoverController'
        }).when('/detail/:repository/:identifier', {
            templateUrl: 'partials/detail.html',
            controller: 'detailController'
        }).when('/embed/:repository/:identifier', {
            templateUrl: 'partials/embed.html',
            controller: 'detailController'
        }).when('/askers/:repository', {
            templateUrl: 'partials/askers.html',
            controller: 'askersController'
        }).when('/explorer/:repository', {
            templateUrl: 'partials/threadexplorer.html',
            controller: 'threadexplorerController'
        }).when('/dupdetection/:repository', {
            templateUrl: 'partials/dupdetection.html',
            controller: 'dupdetectionController'
        });
  }]);


app.directive('cascadeDropdown', function () {
    return {
        restrict: 'E',
        scope: {
            data: '=',
            selected: '=',
            onSelectionChange: '&'
        },
        templateUrl: 'partials/cascade_dropdwon.html',
        link: function (scope, element, attrs) {
            
            function calculateSelection(id) {
                var vm = {
                    category: '',
                    selection: ''
                };


                if (typeof id === 'undefined')
                {
                    return vm;
                }

                for (var i = 0; i < scope.data.categories.length; i++) {
                    var group = scope.data.typeGroups[i];

                    for (var j = 0; j < group.length; j++) {
                        if (group[j].id == id) {
                            vm.category = i.toString();
                            vm.selection = id.toString();
                        }
                    }
                }

                return vm;
            }

            scope.currentOptions = [];

            if (scope.onSelectionChange)
            {
                scope.onSecondSelectionChange = scope.onSelectionChange;
            }      

            scope.onFirstLevelSelectionChange = function (newVal) {
                console.log('first level selection change: ' + newVal);

                scope.selection = '';

                if (newVal === '')
                {
                    scope.currentOptions = [];
                } else {
                    scope.currentOptions = scope.data.typeGroups[newVal];
                }
            }

            scope.$watch('selected', function (newValue, oldValue) {
                var vm = calculateSelection(newValue);

                console.log('category: ' + vm.category + ', type:  ' + vm.selection);

                if (vm.selection !== '')
                {
                    if (vm.category !== scope.category)
                    {
                        scope.category = vm.category;

                        scope.currentOptions = scope.data.typeGroups[scope.category];
                    }

                    setTimeout(function () {
                        scope.$apply(function () {
                            var options = scope.currentOptions;

                            scope.selection = vm.selection;
                        });
                    }, 0);
                }
            });
        }
    };
})