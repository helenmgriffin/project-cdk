var doc = require('aws-sdk');
var dynamo = new doc.DynamoDB();

exports.handler = async function (event, context) {
    let body;
    if (event.body !== null && event.body !== undefined) {
        body = JSON.parse(event.body)
    }
    else
        body = event;

    console.log(body);
    var ClosedDate = "";
    if (body.ClosedDate !== null)
        ClosedDate = body.ClosedDate;
    var getParams = {
        TableName: process.env.TABLE_NAME,
        Key: {
            "TicketGUID": { "S": body.TicketGuid }
        },
        UpdateExpression: "set ClosedDate=:closedDate, ClosingComments=:closingComments",
        "ExpressionAttributeValues": {
            ":closedDate": { "S": ClosedDate },
            ":closingComments": { "S": body.ClosingComments }
        },
        ReturnValues: "UPDATED_NEW"
    };

    let response;
    await dynamo.updateItem(getParams, function (err, data) {
        if (err) console.log(err, err.stack); // an error occurred
        else {
            var items = data['Items'];
            // The output from a Lambda proxy integration must be 
            // in the following JSON object. The 'headers' property 
            // is for custom response headers in addition to standard 
            // ones. The 'body' property  must be a JSON string. For 
            // base64-encoded payload, you must also set the 'isBase64Encoded'
            // property to 'true'.
            response = {
                statusCode: 200,
                body: JSON.stringify(items),
                isBase64Encoded: true
            };
            //context.succeed(data);
        }
    }).promise();

    console.log("response: " + JSON.stringify(response))
    return response;
};