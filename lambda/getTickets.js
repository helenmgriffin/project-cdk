var doc = require('aws-sdk');
var dynamo = new doc.DynamoDB();

exports.handler = async function (event, context) {
    var getParams = {
        TableName: process.env.TABLE_NAME,
    };

    await dynamo.scan(getParams, function (err, data) {
        if (err) console.log(err, err.stack); // an error occurred
        else {
            context.succeed(data);
        }
    }).promise();
};