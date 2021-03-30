var doc = require('aws-sdk');
var dynamo = new doc.DynamoDB();

exports.handler = async function (event, context) {

    var getParams = {
        TableName: process.env.TABLE_NAME,
        Item: {
            "TicketID": {
                S: event.ticketID
            },
            "Summary": {
                S: event.summary
            },
            "Description": {
                S: event.description
            }
        }
    };

    await dynamo.putItem(getParams, function (err, data) {
        if (err) console.log(err, err.stack); // an error occurred
        else {
            context.succeed(data);
        }
    }).promise();
};