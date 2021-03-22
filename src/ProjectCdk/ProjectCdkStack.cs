using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.APIGateway;
using Eladb.DynamoTableViewer;

namespace ProjectCdk
{
    public class ProjectCdkStack : Stack
    {
        public readonly CfnOutput HCViewerUrl;
        public readonly CfnOutput HCEndpoint;
        internal ProjectCdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Defines a new lambda resource
            var hello = new Function(this, "HelloHandler", new FunctionProps
            {
                Runtime = Runtime.NODEJS_10_X,// execution environment
                Code = Code.FromAsset("lambda"),// Code loaded from the "lambda" directory
                Handler = "hello.handler"// file is "hello", function is "handler"
            });

            var helloWithCounter = new HitCounter(this, "HelloHitCounter", new HitCounterProps
            {
                Downstream = hello
            });

            //expose endpoints as properties of our stack
            var gateway = new LambdaRestApi(this, "Endpoint", new LambdaRestApiProps
            {
                Handler = helloWithCounter.Handler
            });

            this.HCEndpoint = new CfnOutput(this, "GatewayUrl", new CfnOutputProps
            {
                Value = gateway.Url
            });

            // Defines a new TableViewer resource
            var tv = new TableViewer(this, "ViewerHitCount", new TableViewerProps
            {
                Title = "Hello Hits",
                Table = helloWithCounter.MyTable
            });

            this.HCViewerUrl = new CfnOutput(this, "TableViewerUrl", new CfnOutputProps
            {
                Value = tv.Endpoint
            });
        }
    }
}
