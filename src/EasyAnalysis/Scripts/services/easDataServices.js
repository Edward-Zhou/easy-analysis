// [production]: eas-api.azurewebsites.net
// [dev]: localhost:58116

var web_api_config = {
   host: 'app-svr.cloudapp.net'
   //host: 'localhost:58116' //Local Test
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
        types: function (id) {
            return $http.get('api/thread/' + id + '/types');
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
        getTags: function (id) {
            return $http.get('api/thread/' + id + '/tags');
        },
        getTagCoverage: function (resp) {
            return $http.get(
                'http://' + web_api_config.host + '/api/Thread?repository=' + resp)
        }
    }
}]);

app.factory('threadProfileService', ['$http', function ($http) {
    return {
        relatedTags: function (resp, start, end, tags, answered) {
            return $http.get(
                'http://' + web_api_config.host + '/api/ThreadProfiles/relatedtags?repository=' + resp + '&start=' + start +
                '&end=' + end +
                '&answered=' + answered +
                '&tags=' + encodeURIComponent(tags.join('|')))
        },

        list: function (resp, start, end, tags, answered, page) {
            return $http.get('http://' + web_api_config.host + '/api/ThreadProfiles?repository=' + resp + '&page=' + page + '&length=10&start=' + start +
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
            return $http.get('http://' + web_api_config.host
                               + '/api/UserProfile?repository=' + encodeURIComponent(resp)
                               + '&month=&display_name=' + encodeURIComponent(name));
        },

        list: function (resp, month, length) {
            return $http.get('http://' + web_api_config.host
                                       + '/api/UserProfile?repository=' + encodeURIComponent(resp)
                                       + '&month=' + month
                                       + '&length=' + length);
        },

        newsearch: function (resp, name, months) {
            var uri = 'http://' + web_api_config.host
                + '/api/UserProfile?repository=' + encodeURIComponent(resp)
                + '&display_name=' + encodeURIComponent(name);
            for (var i = 0; i < months.length; i++) {
                uri += '&month[' + i + ']=' + months[i];
            }

            return $http.get(uri);
        },

        newlist: function (resp, length, months) {
            var uri = 'http://' + web_api_config.host
                + '/api/UserProfile?repository=' + encodeURIComponent(resp)
                + '&length=' + encodeURIComponent(length);
            for (var i = 0; i < months.length; i++) {
                uri += '&month[' + i + ']=' + months[i];
            }

            return $http.get(uri);
        }
    }
}]);
