using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;

namespace ProjectCdk
{
    public class DynamoDBStack: Stack
    {
        public readonly CfnOutput ticketsTableArn;
        public readonly CfnOutput updateEndpoint;
        public readonly CfnOutput getEndpoint;
        public readonly CfnOutput getByIDEndpoint;
        public readonly CfnOutput putEndpoint;

        internal DynamoDBStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {

            var ticketsConstruct = new TicketsConstruct(this, "TicketsConstruct", new TicketsConstructProps {});

            ticketsTableArn = new CfnOutput(this, "DynamoDBTicketsTableArn", new CfnOutputProps
            {
                Value = ticketsConstruct.TicketsTable.TableArn
            });

            //expose endpoints as properties of our stack
            var updateTicketGateway = new LambdaRestApi(this, "UpdateTicketGateway", new LambdaRestApiProps
            {
                Handler = ticketsConstruct.UpdateHandler
            });

            updateEndpoint = new CfnOutput(this, "UpdateTicketGatewayUrl", new CfnOutputProps
            {
                Value = updateTicketGateway.Url
            });

            //expose endpoints as properties of our stack
            var getTicketsGateway = new LambdaRestApi(this, "GetTicketsGateway", new LambdaRestApiProps
            {
                Handler = ticketsConstruct.GetHandler
            });

            getEndpoint = new CfnOutput(this, "GetTicketsGatewayUrl", new CfnOutputProps
            {
                Value = getTicketsGateway.Url
            });


            //expose endpoints as properties of our stack
            var getTicketByIDGateway = new LambdaRestApi(this, "GetTicketByIDGateway", new LambdaRestApiProps
            {
                Handler = ticketsConstruct.GetByIDHandler
            });

            getByIDEndpoint = new CfnOutput(this, "GetTicketByIDGatewayUrl", new CfnOutputProps
            {
                Value = getTicketByIDGateway.Url
            });

            //expose endpoints as properties of our stack
            var putGateway = new LambdaRestApi(this, "PutTicketGateway", new LambdaRestApiProps
            {
                Handler = ticketsConstruct.PutHandler
            });

            putEndpoint = new CfnOutput(this, "PutTicketGatewayUrl", new CfnOutputProps
            {
                Value = putGateway.Url
            });
            //This will create a new DynamoDB table called "Tickets" with the "TicketID" as a Partition Key(PK), 
            //Billing mode is set to to Pay Per Request(no need to provide the write and read capacity. Not Provisioned) 
            //Set to destroy the resource if we decide to delete
            //Table table = new Table(this, "Tickets", new TableProps
            //{
            //    TableName = "Tickets",
            //    PartitionKey = new Attribute { Name = "TicketID", Type = AttributeType.NUMBER },
            //    BillingMode = BillingMode.PAY_PER_REQUEST,
            //    RemovalPolicy = RemovalPolicy.DESTROY
            //});
            //var role = new Role(this, "DBRole", new RoleProps
            //{
            //    AssumedBy = new AccountPrincipal(this.Account)
            //});
            //table.GrantReadWriteData(role);

            

            //// Defines a new lambda resource
            //var getTickets = new Function(this, "GetTickets", new FunctionProps
            //{
            //    Runtime = Runtime.NODEJS_12_X,// execution environment
            //    Code = Code.FromAsset("lambda"),// Code loaded from the "lambda" directory
            //    Handler = "getTickets.handler"// file is "getTickets", function is "handler"
            //});

            /*  [TicketNumber]    INT              NOT NULL,
                [Summary]         VARCHAR (50)     NOT NULL,
                [Description]     VARCHAR (1000)   NOT NULL,
                [CreationDate]    DATETIME         NOT NULL,
                [Creator]         NCHAR (25)       NOT NULL,
                [ClosedDate]      DATETIME         NULL,
                [TicketPriority]  INT              NOT NULL,
                [ClosingComments] VARCHAR (1000)   NULL,
            */

        }
    }
}
