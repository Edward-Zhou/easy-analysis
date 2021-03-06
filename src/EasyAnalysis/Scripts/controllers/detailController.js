﻿controllers.controller('detailController', ['$scope', '$http' ,'threadService', 'repositoryService', '$location', '$routeParams',
    function ($scope, $http, threadService, repositoryService, $location, $routeParams) {
        // model state init
        $scope.identifier = $routeParams.identifier;
        $scope.repository = $routeParams.repository;

        $scope.state = 'load';

        $scope.show_legacy_category = $scope.repository !== 'OFFICE';

        // dynamic fields init
        repositoryService.getFields($scope.repository)
                         .success(function (data) {

                             $scope.groups = data;

                             threadService.getFiledValues($scope.repository, $scope.identifier)
                                          .success(function (data) {
                                              $scope.model_ex = data;
                                          });
                         });

        $scope.fieldValueChange = function(name, value)
        {
            threadService.setField($scope.repository, $scope.identifier, name, value);
        }

        var loadDetail = function (data) {
            $scope.data = data;

            // load detail data
            threadService.detail($scope.identifier)
                         .success(function (data) {
                             $scope.item = data;

                             $scope.typeSelection = $scope.item.typeId;

                             $scope.state = 'done';
                         });
        }

        if ($scope.repository === 'SQL')
        {
            threadService.cascadeOptions($scope.identifier)
             .success(loadDetail);
        } else {
            threadService.types($scope.repository)
            .success(loadDetail);
        }

        $scope.onSelectionChange = function (sel) {
            threadService.classify($scope.identifier, sel === '' ? '-1' : sel);
        }

        $scope.back = function () {
            $location.url('/discover/' + $scope.repository);
        }

        // region - tag related functions

        $scope.loadTags = function (query) {
            console.log('q = ' + query);
            return $http.get('/api/tag/search?q=' + query);
        }

        $scope.onTagAdded = function (tag) {
            threadService.addTag($scope.identifier, tag.text);
        }

        $scope.onTagRemoved = function (tag) {
            threadService.removeTag($scope.identifier, tag.text);
        }    

        // endregion
    }]);