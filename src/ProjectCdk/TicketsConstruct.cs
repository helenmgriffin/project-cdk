using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using System.Collections.Generic;

namespace ProjectCdk
{
    public class TicketsConstructProps
    {
        // The function for which we want to count url hits
        public IFunction Downstream { get; set; }
    }
    public class TicketsConstruct: Construct
    {
        public Function UpdateHandler { get; }
        public Function GetHandler { get; }
        public Function GetByIDHandler { get; }
        public Function PutHandler { get; }

        public Table TicketsTable { get; }

        /// <summary>
        /// Create a DynamoDB table
        /// Create Get/Update Lambda Functions to access the DynamoDB
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="id"></param>
        /// <param name="props"></param>
        public TicketsConstruct(Construct scope, string id, TicketsConstructProps props) : base(scope, id)
        {
            /// This will create a new DynamoDB table called "Tickets" with the "TicketID" as a Partition Key(PK), 
            //  Billing mode is set to to Pay Per Request(no need to provide the write and read capacity. Not Provisioned) 
            //  Set to destroy the resource if we decide to delete
            var table = new Table(this, "Tickets", new TableProps
            {
                TableName = "Tickets",
                PartitionKey = new Attribute
                {
                    Name = "TicketID",
                    Type = AttributeType.STRING
                },
                BillingMode = BillingMode.PAY_PER_REQUEST,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            TicketsTable = table;

            Dictionary<string, string> environment = new Dictionary<string, string>
            {
                ["TABLE_NAME"] = table.TableName
            };
            Runtime runtime = Runtime.NODEJS_12_X;
            Code code = Code.FromAsset("lambda");

            UpdateHandler = new Function(this, "UpdateTicket", new FunctionProps
            {
                Runtime = runtime,
                Code = code,
                Handler = "updateTicket.handler",
                Environment = environment
            });

            // Defines a new lambda resource
            GetHandler = new Function(this, "GetTickets", new FunctionProps
            {
                Runtime = runtime,// execution environment
                Code = code,// Code loaded from the "lambda" directory
                Handler = "getTickets.handler",// file is "getTickets", function is "handler"
                Environment = environment
            });

            // Defines a new lambda resource
            GetByIDHandler = new Function(this, "GetTicket", new FunctionProps
            {
                Runtime = runtime,// execution environment
                Code = code,// Code loaded from the "lambda" directory
                Handler = "getTicket.handler",// file is "getTickets", function is "handler"
                Environment = environment
            });

            // Defines a new lambda resource
            PutHandler = new Function(this, "PutTicket", new FunctionProps
            {
                Runtime = runtime,// execution environment
                Code = code,// Code loaded from the "lambda" directory
                Handler = "putTicket.handler",// file is "getTickets", function is "handler"
                Environment = environment
            });

            // Grant the lambda role read/write permissions to our table
            table.GrantReadWriteData(UpdateHandler);
            // Grant the lambda role read/write permissions to our table
            table.GrantReadWriteData(GetHandler);
            // Grant the lambda role read/write permissions to our table
            table.GrantReadWriteData(GetByIDHandler);
            // Grant the lambda role read/write permissions to our table
            table.GrantReadWriteData(PutHandler);
        }
    }
}
