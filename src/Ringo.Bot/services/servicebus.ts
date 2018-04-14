import _azure = require('azure-sb');

var connStr = process.env.SB_CONNECTION_STRING;
var topicName = 'graph';
 
console.log('Connecting to ' + connStr + ' queue ' + topicName);
var sbService = _azure.createServiceBusService(connStr);

export function sendMessages(entityRelationship) {
  var message = {
    body: (JSON.stringify(entityRelationship))
    }
  //message.body = JSON.stringify(body);
  console.log(message.body)
    sbService.sendTopicMessage(topicName, message, function (err) {
    if (err) {
      console.log('Failed Tx: ', err);
    } else {
      console.log('Sent ' + message);
    }
    });
}