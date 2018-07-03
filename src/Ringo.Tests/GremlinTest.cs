using System;
using System.Collections.Generic;
using System.Text;

namespace Ringo.Tests
{
    class GremlinTest
    {

        /*
         * g.V().has('spotifyid', '2d0hyoQ5ynDBnkvAbJKORj').repeat(out('related')).times(2).by(out().count())
         * 
         * g.V().has('spotifyid', '2d0hyoQ5ynDBnkvAbJKORj').repeat(out('related')).times(2).in('likes').where(otherV().hasId('default-user'))
         * 
         * g.V().has('spotifyid', '2d0hyoQ5ynDBnkvAbJKORj').as('self').repeat(out('related')).times(2).where(without('self')).groupCount().order(local).by(values, decr)
         * 
         * gremlin> g.V().has('person','name','alice').as('her'). //1\
               out('bought').aggregate('self'). //2\
               in('bought').where(neq('her')). //3\
               out('bought').where(without('self')). //4\
               groupCount().
               order(local).
                 by(values, decr)

        g.V().has('spotifyid', '2d0hyoQ5ynDBnkvAbJKORj').as('self').repeat(out('related')).times(2).where(without('self')).groupCount().order(local).by(values, decr)
        
        g.V('soundgarden:c0858bf513ca31c91e6ad7adcb8c956b').as('self').repeat(both('related')).times(4).where(without('self')).groupCount().order(local).by(values, decr)

        g.V('soundgarden:c0858bf513ca31c91e6ad7adcb8c956b').as('self').repeat(both('related')).times(2).where(without('self')).values('spotifyid', 'name').groupCount().order(local).by(values, decr).limit(5)
         
        g.V('soundgarden:c0858bf513ca31c91e6ad7adcb8c956b').as('self').repeat(both('related')).times(2).where(without('self')).inE()

        g.V('soundgarden:c0858bf513ca31c91e6ad7adcb8c956b').as('self').repeat(both('related')).times(2).where(without('self')).in('likes').where(new('default-user'))

        g.V('soundgarden:c0858bf513ca31c91e6ad7adcb8c956b').as('self').repeat(both('related')).times(2).where(without('self')).in('likes').where(without('U9ZFDN1RN:TA0VBN61L'))

        g.V('soundgarden:c0858bf513ca31c91e6ad7adcb8c956b').as('self').repeat(both('related')).times(2).where(without('self')).in('likes').where(without('id', 'U9ZFDN1RN:TA0VBN61L'))

         */
    }
}
