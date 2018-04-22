import _azure = require('azure-sb');

var connStr = process.env.SB_CONNECTION_STRING;
var sbService = _azure.createServiceBusService(connStr);

if (!connStr) throw new Error('Env var SB_CONNECTION_STRING not set');

export async function sendMessage(topicName, messageBody) {
  var message = {
    body: (JSON.stringify(messageBody))
  };

  console.log("serviceBus:sendMessage", message.body)

  return new Promise<any>((resolve, reject) => {
    sbService.sendTopicMessage(topicName, message, function (err) {
      if (err) {
        console.error(err);
        reject(err);
        return;
      } else {
        resolve();
        return;
      }
    });
  });
}
