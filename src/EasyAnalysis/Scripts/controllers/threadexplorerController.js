controllers.controller('threadexplorerController', ['$scope', '$location', '$routeParams', 'threadProfileService',
        function ($scope, $location, $routeParams, threadProfileService) {
            $scope.repository = $routeParams.repository;


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

            $scope.filter = {
                tag: {
                    groups: [],
                    selected: [],
                    related: [],
                },
                answered: ''
            };

            // bug fix - issue#10
            // CODE REFACTOR
            if ($scope.repository === 'UWP')
            {
                $scope.filter.tag.groups = [{
                    name: 'Platform', tags: [
                      { value: '', name: 'All' },
                      { value: 'uwp', name: 'uwp' },
                      { value: 'wp8.1', name: 'wp8.1' },
                      { value: 'w8.1', name: 'w8.1' },
                      { value: 'u8.1', name: 'u8.1' },
                      { value: 'wpsl', name: 'wpsl' }]
                },
                {
                    name: 'Language', tags:
                        [{ value: '', name: 'All' },
                        { value: 'c#', name: 'c#' },
                        { value: 'c++', name: 'c++' },
                        { value: 'vb', name: 'vb' },
                        { value: 'javascript', name: 'javascript' }]
                }];
            }

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