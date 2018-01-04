import * as httpj from "../../services/httpjson";
import { expect } from 'chai';
import 'mocha';
import './helper';

describe('httpjson.request', () => {

  it('returns response from GET /posts', async () => {
    let response = await httpj.request("get", "https://jsonplaceholder.typicode.com/posts");
    expect(response).to.not.be.null;
  });

});

describe('httpjson.get', () => {

  it('returns response from GET /posts', async () => {
    let response = await httpj.get("https://jsonplaceholder.typicode.com/posts");
    expect(response).to.not.be.null;
  });

});

describe('httpjson.post', () => {

  it('returns response from POST /posts', async () => {
    const data = { userId: 1, id: 1, title: "title", body: "Body" };
    let response = await httpj.post("https://jsonplaceholder.typicode.com/posts", data);
    expect(response).to.not.be.null;
  });

});
