controllers.controller('detailController', ['$scope', '$http' ,'threadService', 'repositoryService', '$location', '$routeParams',
    function ($scope, $http, threadService, repositoryService, $location, $routeParams) {
        // model state init
        $scope.identifier = $routeParams.identifier;
        $scope.repository = $routeParams.repository;

        $scope.state = 'load';

        $scope.model = {
            categorySelect: '-1',
            typeSelect: '-1'
        };

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

        threadService.types($scope.repository)
            .success(function (data) {
                $scope.data = data;

                // load detail data
                threadService.detail($scope.identifier)
                             .success(function (data) {
                                 $scope.item = data;

                                 var typeId = $scope.item.TypeId;

                                 var vm = calculateSelection(typeId);

                                 $scope.model.categorySelect = vm.categorySelect;

                                 // temp workaround to update the UI, refactor the code
                                 // in the future
                                 setTimeout(function () {
                                     $scope.$apply(function () {
                                         $scope.model.typeSelect = vm.typeSelect;
                                     });
                                 }, 0);

                                 $scope.state = 'done';
                             });
            });


        $scope.typeSelectChange = function () {
            threadService.classify($scope.identifier, $scope.model.typeSelect);
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

        function calculateSelection(id) {
            var vm = {
                categorySelect: '-1',
                typeSelect: '-1'
            };

            for (var i = 0; i < $scope.data.categories.length; i++) {
                var group = $scope.data.typeGroups[i];

                for (var j = 0; j < group.length; j++) {
                    if (group[j].id == id) {
                        vm.categorySelect = i.toString();
                        vm.typeSelect = id.toString();
                    }
                }
            }

            return vm;
        }
    }]);