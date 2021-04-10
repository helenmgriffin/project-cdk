using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.SecretsManager;
//using Talnakh.SqlServerSeeder;

namespace ProjectCdk
{
    class Stack2Props : StackProps
    {
        public IVpc Vpc { get; set; }
    }

    /**
 * Stack1 creates the VPC
 */
    public class DatabaseCdkStack : Stack
    {
        public IVpc vpc { get; }
        internal DatabaseCdkStack(Construct scope, string id, IStackProps props) : base(scope, id, props)
        {
            vpc = Vpc.FromLookup(this, "VPC", new VpcLookupOptions
            {
                // This imports the default VPC but you can also
                // specify a 'vpcName' or 'tags'.
                IsDefault = true
            });
            // SQL Server

            var sg = new SecurityGroup(this, "NorthwindDatabaseSecurityGroup", new SecurityGroupProps
            {
                Vpc = vpc,

                SecurityGroupName = "Northwind-DB-SG",
                AllowAllOutbound = false
            });

            // !!!!!!!!!! replace IP according to the instructions above
            sg.AddIngressRule(Peer.Ipv4("172.31.0.0/16"), Port.Tcp(1433)); // SQL Server
            sg.AddIngressRule(Peer.Ipv4("91.223.87.110/32"), Port.Tcp(1433)); // SQL Server
            // !!!!!!!!!!

            var sqlInstance = new DatabaseInstance(this, "NorthwindSQLServer", new DatabaseInstanceProps
            {
                Vpc = vpc,

                InstanceIdentifier = "northwind-sqlserver",
                Engine = DatabaseInstanceEngine.SqlServerEx(new SqlServerExInstanceEngineProps { Version = SqlServerEngineVersion.VER_14 }), // SQL Server Express
                Credentials = Credentials.FromGeneratedSecret("adminuser"),
                //MasterUsername = "adminuser",
                //MasterUserPassword = new SecretValue("Admin12345?"),
                //IamAuthentication = true,

                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.SMALL), // t3.small
                SecurityGroups = new ISecurityGroup[] { sg },
                MultiAz = false,
                VpcSubnets = new SubnetSelection() { SubnetType = SubnetType.PUBLIC }, // public subnet

                DeletionProtection = false, // you need to be able to delete database
                DeleteAutomatedBackups = true,
                BackupRetention = Duration.Days(0),
                RemovalPolicy = RemovalPolicy.DESTROY // you need to be able to delete database
                
            });

            //var seeder = new SqlServerSeeder(this, "SqlSeeder", new SqlServerSeederProps
            //{
            //    Vpc = vpc,
            //    Database = sqlInstance,
            //    Port = 1433,
            //    CreateScriptPath = "./SQL/v1.0.0.sql",//script to be executed on resource creation
            //    DeleteScriptPath = "./SQL/cleanup.sql"
            //});
            //var role = new Role(this, "DBRole", new RoleProps
            //{
            //    AssumedBy = new AccountPrincipal(this.Account)
            //});
            //sqlInstance.GrantConnect(role); // Grant the role connection access to the DB.

            new CfnOutput(this, "SQLServerSecretFullArn", new CfnOutputProps
            {
                Value = sqlInstance.Secret.SecretFullArn
            });
            new CfnOutput(this, "SQLServerSecretValue", new CfnOutputProps
            {
                Value = sqlInstance.Secret.SecretValue.ToJSON().ToString()
            });
            new CfnOutput(this, "SQLServerEndpointAddress", new CfnOutputProps
            {
                Value = sqlInstance.DbInstanceEndpointAddress
            });
        }
    }

    /**
 * Stack2 consumes the VPC
 */
    class Stack2 : Stack
    {
        public Stack2(App scope, string id, Stack2Props props) : base(scope, id, props)
        {

            // Pass the VPC to a construct that needs it
            //new ConstructThatTakesAVpc(this, "Construct", new ConstructThatTakesAVpcProps
            //{
            //    Vpc = props.Vpc
            //});
        }
    }

    //Stack1 stack1 = new Stack1(app, "Stack1");
    //Stack2 stack2 = new Stack2(app, "Stack2", new Stack2Props
    //{
    //    Vpc = stack1.Vpc
    //});
}
