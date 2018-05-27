import _graph = require('../../services/graphstorage')
import { expect } from 'chai';
import 'mocha';

describe('graphstorage', () => {

    it('vertexEdgeVertices returns not null', async () => {
      const userId = 'defaultUser'
      let data = await _graph.vertexEdgeVertices(userId, 'likes', 'artist')
      expect(data).to.not.be.null;
    });

    it('execute returns not null', async () => {
        const userId = 'defaultUser'
        let data = await _graph.execute('g.V().limit(limit)', {limit: 1})
        expect(data).to.not.be.null;
      });
  
  });
  