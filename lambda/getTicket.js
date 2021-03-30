var doc = require('aws-sdk');
var dynamo = new doc.DynamoDB();

exports.handler = async function (event, context) {
    var condition = {};
    condition["TicketID"] = {
        ComparisonOperator: 'EQ',
        AttributeValueList: [{ N: event.ticketID }]
    }

    var getParams = {
        TableName: 'Tickets',
/*        ProjectionExpression: 'TicketID,Summary',*/
        KeyConditions: condition
    };

    await dynamo.query(getParams, function (err, data) {
        if (err) console.log(err, err.stack); // an error occurred
        else {
            context.succeed(data);
        }
    }).promise();
};