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
                Handler = ticketsConstruct.UpdateHandler,
                DefaultCorsPreflightOptions = new CorsOptions {  AllowOrigins = Cors.ALL_ORIGINS, AllowMethods = new string[]{ "POST" } }
            });;

            //var items1 = updateTicketGateway.Root.AddResource("items");
            //items1.AddMethod("POST", null, new MethodOptions { ApiKeyRequired = true});// POST /items

            updateEndpoint = new CfnOutput(this, "UpdateTicketGatewayUrl", new CfnOutputProps
            {
                Value = updateTicketGateway.Url
            });

            //expose endpoints as properties of our stack
            var getTicketsGateway = new LambdaRestApi(this, "GetTicketsGateway", new LambdaRestApiProps
            {
                Handler = ticketsConstruct.GetHandler,
                DefaultCorsPreflightOptions = new CorsOptions { AllowOrigins = Cors.ALL_ORIGINS, AllowMethods = new string[] { "GET" } }
            });

            //var items2 = getTicketsGateway.Root.AddResource("items");
            //items2.AddMethod("GET");// GET /items

            getEndpoint = new CfnOutput(this, "GetTicketsGatewayUrl", new CfnOutputProps
            {
                Value = getTicketsGateway.Url
            });

            //expose endpoints as properties of our stack
            var getTicketByIDGateway = new LambdaRestApi(this, "GetTicketByIDGateway", new LambdaRestApiProps
            {
                Handler = ticketsConstruct.GetByIDHandler,
                DefaultCorsPreflightOptions = new CorsOptions { AllowOrigins = Cors.ALL_ORIGINS, AllowMethods = new string[] { "GET" } }
            });

            //var items3 = getTicketByIDGateway.Root.AddResource("items");
            //items3.AddMethod("GET");// GET /items

            getByIDEndpoint = new CfnOutput(this, "GetTicketByIDGatewayUrl", new CfnOutputProps
            {
                Value = getTicketByIDGateway.Url
            });

            //expose endpoints as properties of our stack
            var putGateway = new LambdaRestApi(this, "PutTicketGateway", new LambdaRestApiProps
            {
                Handler = ticketsConstruct.PutHandler,
                DefaultCorsPreflightOptions = new CorsOptions { AllowOrigins = Cors.ALL_ORIGINS, AllowMethods = new string[] { "PUT" } }
            });

            //var items4 = putGateway.Root.AddResource("items");
            //items4.AddMethod("Put");// PUT /items

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

            



        }
    }
}
