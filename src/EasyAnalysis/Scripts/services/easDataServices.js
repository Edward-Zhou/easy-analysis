// [production]: eas-api.azurewebsites.net
// [dev]: localhost:58116

var web_api_config = {
    host: 'eas-api.azurewebsites.net'
    // host: 'localhost:58116'
}

app.factory('threadProfileService', ['$http', function ($http) {
    return {
        relatedTags: function (start, end, tags, answered) {
            return $http.get(
                'http://' + web_api_config.host + '/api/ThreadProfiles/relatedtags?repository=uwp&start=' + start +
                '&end=' + end +
                '&answered=' + answered +
                '&tags=' + encodeURIComponent(tags.join('|')))
        },

        list: function (start, end, tags, answered, page) {
            return $http.get('http://' + web_api_config.host + '/api/ThreadProfiles?repository=uwp&page=' + page + '&length=10&start=' + start +
                '&end=' + end +
                '&answered=' + answered +
                '&tags=' + encodeURIComponent(tags.join('|')));
        }
    }
}]);

app.factory('userProfileService', ['$http', function ($http) {
    return {
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
