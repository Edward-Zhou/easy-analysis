controllers.controller('threadexplorerController',
    ['$scope', '$location', '$routeParams', 'threadProfileService',
        function ($scope, $location, $routeParams, threadProfileService) {
            $scope.repository = $routeParams.repository;

            $scope.filter = {
                tag: {
                    groups: [],
                    selected: [],
                    related: [],
                },
                answered: ''
            };

            if ($routeParams.hasOwnProperty('tags'))
            {
                $scope.filter.tag.selected = decodeURIComponent($routeParams.tags).split(',');
            }

            function getDateRange()
            {
                var name = $scope.filter.range;

                name = name === undefined || name === '' ? 'tm' : name;

                return Utility.getDateRange(name);
            }

            function applyFilterChange(selection)
            {
                // reset the page index
                $scope.page = 1;

                var tags = [];

                if (selection !== undefined) {
                    for (var ele in selection) {
                        if (selection[ele] !== '') {
                            tags.push(selection[ele]);
                        }
                    }
                }

                for (var i = 0; i < $scope.filter.tag.selected.length; i++) {
                    tags.push($scope.filter.tag.selected[i]);
                }

                if (tags.length > 0)
                {
                    $location.search('tags', encodeURIComponent(tags.join(',')));
                }

                var range = getDateRange();

                $scope.filter.allTags = tags;

                var done1 = false;

                var done2 = false;

                $scope.done = false;

                $scope.error = false;

                threadProfileService
                    .relatedTags($scope.repository, range[0], range[1], $scope.filter.allTags, $scope.filter.answered)
                    .then(function (response) {
                        done1 = true;

                        $scope.done = done1 && done2;

                        $scope.filter.tag.related = response.data;
                    },
                    function errorCallback(response) {
                        $scope.error = true;
                    });

                threadProfileService
                    .list($scope.repository, range[0], range[1], $scope.filter.allTags, $scope.filter.answered, $scope.page)
                    .then(function (response) {
                        done2 = true;

                        $scope.done = done1 && done2;

                        $scope.threadProfiles = response.data;
                    }, function errorCallback(response) {
                        $scope.error = true;
                    });
            }

            $scope.selection = {};

            $scope.threadProfiles = [];

            $scope.page = 1;

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

            $scope.remove_question = function (id) {
                threadProfileService.remove(id, $scope.repository)
                                    .then(function () {
                                        reload();
                                    })
            }

            function reload()
            {
                var range = getDateRange();

                $scope.done = false;

                threadProfileService
                    .list($scope.repository, range[0], range[1], $scope.filter.allTags, $scope.filter.answered, $scope.page)
                    .then(function (response) {
                        $scope.done = true;

                        $scope.threadProfiles = response.data;
                    });
            }

            $scope.next_page = function () {
                $scope.page = $scope.page + 1;

                reload();
            }

            $scope.prev_page = function () {
                $scope.page = $scope.page - 1;

                reload();
            }

            applyFilterChange($scope.selection);
        }]);