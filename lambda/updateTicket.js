var doc = require('aws-sdk');
var dynamo = new doc.DynamoDB();

exports.handler = async function (event, context) {
    var getParams = {
        TableName: process.env.TABLE_NAME,
        Key: {
            "TicketID": { "S": event.ticketID }
        },
        UpdateExpression: "set Summary = :val1, Description=:val2",
        "ExpressionAttributeValues": {
            ":val1": { "S": event.summary },
            ":val2": { "S": event.description }
        },
        ReturnValues: "UPDATED_NEW"
    };


    await dynamo.updateItem(getParams, function (err, data) {
        if (err) console.log(err, err.stack); // an error occurred
        else {
            context.succeed(data);
        }
    }).promise();

};