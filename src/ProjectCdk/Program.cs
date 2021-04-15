using Amazon.CDK;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ProjectCdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            //Environment env = new Environment();
            //env.Account = "235629185262";//System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT");//app.Account;
            //env.Region = "eu-west-1";// System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION");// app.Region;

            //DynamoDBStack ds = new DynamoDBStack(app, "DynamoDBStack", new StackProps { Env = env });
            //CommonProps commonProps = ds.CommonStacProps;

            //CommonProps commonProps = new CommonProps();
            //commonProps.updateEndpoint = "https://q3worczk9d.execute-api.eu-west-1.amazonaws.com/prod/";
            //commonProps.putEndpoint = "https://7tmjsi9uk4.execute-api.eu-west-1.amazonaws.com/prod/";
            //commonProps.getByIDEndpoint = "https://74sug7mg0d.execute-api.eu-west-1.amazonaws.com/prod/";
            //commonProps.getEndpoint = "https://p5fpyxi8ha.execute-api.eu-west-1.amazonaws.com/prod/";
            //commonProps.Env = env;

            //new HelpDeskFargateStack(app, "HelpDeskEc2Stack", commonProps);//, getEndpoint = ds.getEndpoint, getByIDEndpoint = ds.getByIDEndpoint, putEndpoint = ds.putEndpoint, updateEndpoint = ds.updateEndpoint });
            //new HelpDeskEC2Stack(app, "HelpDeskEC2Stack", commonProps);
            //new HelpDeskEBSStack(app, "HelpDeskEBSStack", new StackProps { Env = env });

            //change the entry point to deploy our pipeline. no longer want the main CDK application to deploy the original app.
            new ProjectPipelineStack(app, "ProjectPipelineStack");

            app.Synth();
        }



       
    }
}
