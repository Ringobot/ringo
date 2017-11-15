/// simple HTTPS wrapper for JSON request/response

import * as _https from "https";
import * as _url from "url";

// get
export async function get(url: string, headers?) {
    return await request("GET", url);
};

// post
export async function post(url: string, data, headers?) {
    // default content type is form-urlencoded
    if (headers == undefined || headers["Content-Type"] == undefined) {
        headers["Content-Type"] = 'application/x-www-form-urlencoded';
    } 

    return await request("POST", url, data, headers);
};

// request
function request(method: string, url: string, data?, headers?: string[]) {
    let parsedUrl = _url.parse(url, true);

    let httpOptions: _https.RequestOptions = {
        method: method,
        headers: headers,
        hostname: parsedUrl.hostname,
        port: parseInt(parsedUrl.port),
        path: parsedUrl.path
    };

    console.log("httpjson request", url);

    return new Promise<any>(
        function (httpOptions) {
            let req = _https.request(httpOptions, function (res) {
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

            if (data) req.write(data.toString());
            req.end();
        }
    )
};
