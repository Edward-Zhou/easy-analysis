// [production]: eas-api.azurewebsites.net
// [dev]: localhost:58116

var service_config = {
    web_api: 'app-svr.cloudapp.net'
    // web_api: 'localhost:58116' //Local Test
}

app.factory('threadService', ['$http', function ($http) {
    return {
        query: function (uri) {
            // TODO: CODE_REFACTOR
            var req = {
                method: 'POST',
                url: '/api/thread',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                data: '=' + uri
            }

            return $http(req);
        },
        get: function (id) {
            return $http.get('api/thread/' + id);
        },
        classify: function (threadId, typeId) {
            return $http.post('api/thread/' + threadId + '/classify/' + typeId);
        },
        todo: function (repository) {
            return $http.get('api/thread/' + repository + '/todo');
        },
        detail: function (id) {
            return $http.get('api/thread/' + id + '/detail');
        },
        addTag: function (id, tag) {
            // TODO: CODE_REFACTOR
            var req = {
                method: 'POST',
                url: 'api/thread/' + id + '/tag/',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                data: '=' + encodeURIComponent(tag)
            }

            return $http(req);
        },
        removeTag: function (id, tag) {
            var req = {
                method: 'DELETE',
                url: 'api/thread/' + id + '/tag/',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                data: '=' + encodeURIComponent(tag)
            }

            return $http(req);
        },
        getTags: function (id) {
            return $http.get('api/thread/' + id + '/tags');
        },
        setField: function (repository, id, name, value) {
            var req = {
                method: 'POST',
                url: '/api/thread/' + repository + '/' + id + '/field/' + encodeURIComponent(name),
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                data: '=' + encodeURIComponent(value)
            };

            return $http(req);
        },
        getFiledValues: function (repository, id) {
            return $http.get('/api/thread/' + repository + '/' + id + '/field');
        },
        // TO BE REMOVE
        types: function (id) {
            return $http.get('api/thread/' + id + '/types');
        },
        getTagCoverage: function (resp) {
            return $http.get(
                'http://' + service_config.web_api + '/api/Thread?repository=' + resp + '&datatype=0')
        },
        getCategoryCoverage: function (resp) {
            return $http.get(
                'http://' + service_config.web_api + '/api/Thread?repository=' + resp + '&datatype=1')
        },
    }
}]);

app.factory('threadProfileService', ['$http', function ($http) {
    return {
        relatedTags: function (resp, start, end, tags, answered) {
            return $http.get(
                'http://' + service_config.web_api + '/api/ThreadProfiles/relatedtags?repository=' + resp + '&start=' + start +
                '&end=' + end +
                '&answered=' + answered +
                '&tags=' + encodeURIComponent(tags.join('|')))
        },

        list: function (resp, start, end, tags, answered, page) {
            return $http.get('http://' + service_config.web_api + '/api/ThreadProfiles?repository=' + resp + '&page=' + page + '&length=10&start=' + start +
                '&end=' + end +
                '&answered=' + answered +
                '&tags=' + encodeURIComponent(tags.join('|')));
        }
    }
}]);

app.factory('userProfileService', ['$http', function ($http) {
    return {
        query: function (uri) {
            // TODO: CODE_REFACTOR
            var req = {
                method: 'POST',
                url: '/api/UserProfile',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                data: '=' + uri
            }

            return $http(req);
        },
        details: function (repository, display_name) {
            return $http.get('api/UserProfile/' + repository + '/todo');
        },

        search: function (resp, name) {
            return $http.get('http://' + service_config.web_api
                               + '/api/UserProfile?repository=' + encodeURIComponent(resp)
                               + '&month=&display_name=' + encodeURIComponent(name));
        },

        list: function (resp, month, length) {
            return $http.get('http://' + service_config.web_api
                                       + '/api/UserProfile?repository=' + encodeURIComponent(resp)
                                       + '&month=' + month
                                       + '&length=' + length);
        },

        newsearch: function (resp, name, months) {
            var uri = 'http://' + service_config.web_api
                + '/api/UserProfile?repository=' + encodeURIComponent(resp)
                + '&display_name=' + encodeURIComponent(name);
            for (var i = 0; i < months.length; i++) {
                uri += '&month[' + i + ']=' + months[i];
            }

            return $http.get(uri);
        },

        newlist: function (resp, length, months) {
            var uri = 'http://' + service_config.web_api
                + '/api/UserProfile?repository=' + encodeURIComponent(resp)
                + '&length=' + encodeURIComponent(length);
            for (var i = 0; i < months.length; i++) {
                uri += '&month[' + i + ']=' + months[i];
            }

            return $http.get(uri);
        }
    }
}]);


app.factory('repositoryService', ['$http', function ($http) {
    return {
        getFields: function (resp) {
            return $http.get('/api/Repository/fields?name=' + encodeURIComponent(resp));
        }
    }
}]);