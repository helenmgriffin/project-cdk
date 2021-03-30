using Amazon.CDK;
using Newtonsoft.Json.Linq;

namespace ProjectCdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            Environment env = new Environment();
            env.Account = "235629185262";//System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT");//app.Account;
            env.Region = "eu-west-1";// System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION");// app.Region;

            new DynamoDBStack(app, "DynamoDBStack", new StackProps { Env = env });

            app.Synth();

            //change the entry point to deploy our pipeline. no longer want the main CDK application to deploy the original app.
            //var app = new App();
            //new ProjectPipelineStack(app, "ProjectPipelineStack");

            //app.Synth();
            //var app = new App();
            //new ProjectCdkStack(app, "ProjectCdkStack");

            //app.Synth();
        }



       
    }
}
