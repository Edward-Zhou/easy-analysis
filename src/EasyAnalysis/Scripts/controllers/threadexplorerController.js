﻿controllers.controller('threadexplorerController', ['$scope', '$location', '$routeParams', 'threadProfileService',
        function ($scope, $location, $routeParams, threadProfileService) {
            $scope.repository = $routeParams.repository;

            function applyFilterChange(selection)
            {
                var tags = [];

                if(selection !== undefined)
                {
                    for(var ele in selection)
                    {
                        if (selection[ele] !== '')
                        {
                            tags.push(selection[ele]);
                        }
                    }
                }

                for (var i = 0; i < $scope.filter.tag.selected.length; i++)
                {
                    tags.push($scope.filter.tag.selected[i]);
                }

                var start = '2015-10-1', end = '2015-10-31';

                threadProfileService.relatedTags(start, end, tags, $scope.filter.answered)
                                           .then(function (response) {
                                               $scope.filter.tag.related = response.data;
                                           });

                threadProfileService.list(start, end, tags, $scope.filter.answered).then(function (response) {
                    $scope.threadProfiles = response.data;
                });
            }

            $scope.selection = {};

            $scope.threadProfiles = [];

            $scope.filter = {
                tag: {
                    groups: [
                        {
                            name: 'Platform', tags: [
                              { value: '', name: 'All'}, 
                              { value: 'uwp', name: 'uwp'}, 
                              { value: 'wp8.1', name:'wp8.1'}, 
                              { value: 'w8.1', name: 'w8.1' },
                              { value: 'u8.1', name: 'u8.1' },
                              { value: 'wpsl', name: 'wpsl' }]
                        },
                        {
                            name: 'Language', tags:
                              [{ value: '', name: 'All'},
                               { value: 'c#', name: 'c#' },
                               { value: 'c++', name: 'c++' },
                               { value: 'vb', name: 'vb' },
                               { value: 'javascript', name: 'javascript' }]
                        }
                    ],
                    selected: [],
                    related: []
                }
            };

            $scope.selection_value_changed = function (name, selection) {
                applyFilterChange(selection);
            }

            $scope.add_tag_to_filter = function (tag) {
                $scope.filter.tag.selected.push(tag);
                applyFilterChange($scope.selection);
            }

            $scope.remove_tag_from_filter = function (tag) {
                var newSelected = [];

                for (var i = 0; i < $scope.filter.tag.selected.length; i++)
                {
                    if(tag !== $scope.filter.tag.selected[i])
                    {
                        newSelected.push($scope.filter.tag.selected[i]);
                    }
                }

                $scope.filter.tag.selected = newSelected;

                applyFilterChange($scope.selection);
            }
        }]);