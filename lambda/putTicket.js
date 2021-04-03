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
    var getParams = {
        TableName: process.env.TABLE_NAME,
        Item: {
            "TicketGUID": {
                S: body.TicketGuid
            },
            "TicketNumber": {
                N: body.TicketNumber
            },
            "Summary": {
                S: body.Summary
            },
            "Description": {
                S: body.Description
            },
            "CreationDate": {
                S: body.CreationDate
            },
            "Creator": {
                S: body.Creator
            },
            "TicketPriority": {
                S: body.TicketPriority
            }
        }
    };

    let response;
    await dynamo.putItem(getParams, function (err, data) {
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