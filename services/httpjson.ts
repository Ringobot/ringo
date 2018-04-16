/// simple HTTPS wrapper for JSON request/response

import * as _https from "https";
import * as _url from "url";
import { RequestOptions } from "https";

// get
export async function get(url: string, headers?:any) {
    return await request("GET", url, undefined, headers);
};

// post
export async function post(url: string, data, headers?:any) {
    // default content type is form-urlencoded
    if (headers == undefined) headers = {"Content-Type": "application/x-www-form-urlencoded"};
    
    if (headers["Content-Type"] == undefined) {
        headers["Content-Type"] = 'application/x-www-form-urlencoded';
    }

    return await request("POST", url, data, headers);
};

export async function put(url: string, data, headers?:any) {
    // default content type is form-urlencoded
    if (headers == undefined) headers = {"Content-Type": "application/x-www-form-urlencoded"};
    
    if (headers["Content-Type"] == undefined) {
        headers["Content-Type"] = 'application/x-www-form-urlencoded';
    }

    return await request("PUT", url, data, headers);
};

// request
export function request(method: string, url: string, data?, headers?: any) {
    let parsedUrl = _url.parse(url, true);

    let options: RequestOptions = {
        method: method,
        headers: headers,
        hostname: parsedUrl.hostname,
        port: parseInt(parsedUrl.port),
        path: parsedUrl.path
    };

    console.log("httpjson request", url);

    return new Promise<any>((resolve, reject) => {
        let req = _https.request(options, function (res) {
            let output = '';
            res.setEncoding('utf8');

            res.on('data', function (chunk) {
                output += chunk;
            });

            res.on('end', function () {
                if (res.statusCode < 200 || res.statusCode > 299) {
                    reject(output);
                    return;
                }

                var obj = JSON.parse(output);
                resolve(obj);
                return;
            });
        });

        req.on('error', function (err) {
            reject(err);
            return;
        });

        if (data) req.write(data.toString());
        req.end();
    });
};
