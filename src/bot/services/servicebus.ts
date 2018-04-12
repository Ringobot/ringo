import _azure = require('azure-sb');

var connStr = process.env.SB_CONNECTION_STRING;
var topicName = 'graph';
 
console.log('Connecting to ' + connStr + ' queue ' + topicName);
var sbService = _azure.createServiceBusService(connStr);

var idx = 0;
export function sendMessages() {
  var msg = 'Message # ' + (++idx);
  sbService.sendTopicMessage(topicName, msg, function (err) {
   if (err) {
     console.log('Failed Tx: ', err);
   } else {
     console.log('Sent ' + msg);
   }
  });
}