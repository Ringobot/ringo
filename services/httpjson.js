//@ts-check
var $http = require("http");
var $https = require("https");
var $url = require('url');

module.exports.request = request;

module.exports.get = function (url, options, callback) {
if (!options) options = {};
    options.method = 'GET';
    request(url, null, options, callback);
};

module.exports.post = function (url, data, options, callback) {
if (!options) options = {};

options.method = 'POST';
// default content type is form-urlencoded
if (options.headers['Content-Type'] == undefined) {
    options.headers['Content-Type'] = 'application/x-www-form-urlencoded';
}
request(url, data, options, callback);
};


var request = function (url, data, options, callback) {
    var parsedUrl = $url.parse(url, true);
    var http = parsedUrl.protocol == "https:" ? $https : $http;

    options.hostname = parsedUrl.hostname;
    options.port = parsedUrl.port;
    options.path = parsedUrl.path;

    console.log("httpjson request", url);

    var req = http.request(options, function(res) {
        var output = '';
        res.setEncoding('utf8');

        res.on('data', function(chunk) {
            output += chunk;
        });

        res.on('end', function() {
            if (res.statusCode != 200) {
                callback(new Error(output));
                return;
            }
            var obj = JSON.parse(output);
            callback(null, obj);
        });
    });

    req.on('error', function(err) {
        throw err;
    });

    if (data) req.write(data.toString());
    req.end();
};
