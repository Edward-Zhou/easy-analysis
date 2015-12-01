var controllers = angular.module('controllers', []);

var app = angular.module('_app_', ['ngRoute', 'controllers', 'angucomplete-alt']);

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