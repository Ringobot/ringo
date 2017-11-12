//@ts-check
var baseUrl = 'https://api.spotify.com/v1';
var $http = require('./httpjson.js');

// returns playlists[]
module.exports.getPlaylists = function (username, offset, callback) {

    getAuthToken(function(error, token) {
        if (error) {callback(error); return; }

        $http.get(baseUrl + '/users/' + encodeURIComponent(username.toLowerCase()) + '/playlists?limit=50&offset=' + offset, {
            headers: { 'Authorization': 'Bearer ' + token }
        }, function(error, response) {
            if (error) { callback(error); return; }

            console.log('got playlists');
            callback(null, response);
        });
    });
};

var authToken = { token:null, expires:null };

var getAuthToken = function (callback) {
    var now = new Date();
    
    if (process.env.SpotifyApiClientId == undefined || process.env.SpotifyApiClientSecret == undefined) {
        callback(new Error("spotify.js requires environment vars \"SpotifyApiClientId\" and \"SpotifyApiClientSecret\"."));
        return;
    }

    if (authToken.expires == null || authToken.expires.getTime() < now.getTime()) {
        var authEncoded = new Buffer(process.env.SpotifyApiClientId + ':' + process.env.SpotifyApiClientSecret).toString('base64');

        // get a new token
        $http.post('https://accounts.spotify.com/api/token', "grant_type=client_credentials", 
            { headers: { 'Authorization': 'Basic ' + authEncoded } },
            function(error, response) {
            if (error) {
                callback(error);
                return;
            }

            console.log('got auth token');
            var expires = new Date(now.getTime() + response.expires_in * 1000);
            authToken = { token: response.access_token, expires: expires };
            callback(null, authToken.token);
            return;

        });
        return;
    }
    
    callback(null, authToken.token);
};
