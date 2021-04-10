using Amazon.CDK;
using Amazon.CDK.AWS.ElasticBeanstalk;

namespace ProjectCdk
{
    public class ElasticbeanstalkEnvironmentStack : Stack
    {
        public ElasticbeanstalkEnvironmentStack(Construct scope, string id, IStackProps props = null) : base(scope, id)
        {
            const string appName = "MyApp";

            var platform = "arn:aws:elasticbeanstalk:eu-west-1::platform/64bit Amazon Linux 2 v2.1.4 running .NET Core";//this.Node.TryGetContext("platform").ToString();

            var app = new CfnApplication(this, "Application", new CfnApplicationProps
            {
                ApplicationName = appName
            });

            new CfnEnvironment(this, "Environment", new CfnEnvironmentProps
            {
                EnvironmentName = "MySampleEnvironment",
                ApplicationName = app.ApplicationName,
                PlatformArn = platform
            });
        }
    }
}