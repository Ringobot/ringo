"use strict";
/// simple HTTPS wrapper for JSON request/response
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const _https = require("https");
const _url = require("url");
// get
function get(url, headers) {
    return __awaiter(this, void 0, void 0, function* () {
        return yield request("GET", url);
    });
}
exports.get = get;
;
// post
function post(url, data, headers) {
    return __awaiter(this, void 0, void 0, function* () {
        // default content type is form-urlencoded
        if (headers == undefined || headers["Content-Type"] == undefined) {
            headers["Content-Type"] = 'application/x-www-form-urlencoded';
        }
        return yield request("POST", url, data, headers);
    });
}
exports.post = post;
;
// request
function request(method, url, data, headers) {
    let parsedUrl = _url.parse(url, true);
    let options = {
        method: method,
        //headers: headers,
        hostname: parsedUrl.hostname,
        port: parseInt(parsedUrl.port),
        path: parsedUrl.path
    };
    console.log("httpjson request", url);
    return new Promise(resolve => function (options) {
        let req = _https.request(options, function (res) {
            let output = '';
            res.setEncoding('utf8');
            res.on('data', function (chunk) {
                output += chunk;
            });
            res.on('end', function () {
                if (res.statusCode != 200) {
                    throw new Error(output);
                }
                var obj = JSON.parse(output);
                return obj;
            });
        });
        req.on('error', function (err) {
            throw err;
        });
        if (data)
            req.write(data.toString());
        req.end();
    });
}
;
