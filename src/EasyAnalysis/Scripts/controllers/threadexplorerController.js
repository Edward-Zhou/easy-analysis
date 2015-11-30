controllers.controller('threadexplorerController', ['$scope', '$location', '$routeParams', 'threadProfileService',
        function ($scope, $location, $routeParams, threadProfileService) {
            $scope.repository = $routeParams.repository;

            //function getToday()
            //{
            //    var today = new Date();
            //    var dd = today.getDate();
            //    var mm = today.getMonth() + 1;
            //    var yyyy = today.getFullYear();

            //    if (dd < 10) {
            //        dd = '0' + dd
            //    }

            //    if (mm < 10) {
            //        mm = '0' + mm
            //    }

            //    today = yyyy + '-' + mm + '-' + dd;
            //}

            var date = new Date();
            var firstDayThisMonth = new Date(date.getFullYear(), date.getMonth(), 1);
            var lastDayThisMonth = new Date(date.getFullYear(), date.getMonth() + 1, 0);

            var firstDayLastMonth = new Date(date.getFullYear(), date.getMonth()-1, 1);
            var lastDayLastMonth = new Date(date.getFullYear(), date.getMonth(), 0);

            var firstDayLastThreeMonth = new Date(date.getFullYear(), date.getMonth() - 2, 1);

            function getDateRange()
            {
                var today = new Date();
                var dd = today.getDate();
                var start = firstDayThisMonth, end = lastDayThisMonth;

                if ($scope.filter.range === 'lm') {
                    start = firstDayLastMonth;
                    end = lastDayLastMonth;
                } else if ($scope.filter.range === 'l3m') {
                    start = firstDayLastThreeMonth;
                    end = lastDayThisMonth;
                }
                
                return [start, end];
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

                threadProfileService.relatedTags($scope.repository, range[0], range[1], $scope.filter.allTags, $scope.filter.answered)
                                           .then(function (response) {
                                               $scope.filter.tag.related = response.data;
                                           });

                threadProfileService.list($scope.repository, range[0], range[1], $scope.filter.allTags, $scope.filter.answered, $scope.page).then(function (response) {
                    $scope.threadProfiles = response.data;
                });
            }

            $scope.selection = {};

            $scope.threadProfiles = [];

            $scope.page = 1;

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
                    related: [],
                },
                answered: ''
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


            function reload()
            {
                var range = getDateRange();

                threadProfileService
                    .list($scope.repository, range[0], range[1], $scope.filter.allTags, $scope.filter.answered, $scope.page)
                    .then(function (response) {
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