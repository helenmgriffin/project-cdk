var doc = require('aws-sdk');
var dynamo = new doc.DynamoDB.DocumentClient();

exports.handler = async function (event, context) {

    var getParams = {
        TableName: process.env.TABLE_NAME,
    };

    let response;

    await dynamo.scan(getParams, function (err, data) {
        if (err) console.log(err);
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
        }
    }).promise();


    console.log("response: " + JSON.stringify(response))
    return response;
};