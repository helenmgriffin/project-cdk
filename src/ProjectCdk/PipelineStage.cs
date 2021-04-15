using Amazon.CDK;
using Amazon.CDK.Pipelines;

namespace ProjectCdk
{
    public class ProjectPipelineStage : Stage
    {
        //public readonly CfnOutput HCViewerUrl;
        //public readonly CfnOutput HCEndpoint;

        public ProjectPipelineStage(Construct scope, string id, StageProps props = null) : base(scope , id, props)
        {
            //this declares a new Stage(component of a pipeline), and in that stage instantiate our application stack.(ProjectCdkStack)
            //var service = new DynamoDBStack(this, "DynamoDBStack");

            //CommonProps commonProps = new CommonProps();
            //commonProps.updateEndpoint = "https://q3worczk9d.execute-api.eu-west-1.amazonaws.com/prod/";
            //commonProps.putEndpoint = "https://7tmjsi9uk4.execute-api.eu-west-1.amazonaws.com/prod/";
            //commonProps.getByIDEndpoint = "https://74sug7mg0d.execute-api.eu-west-1.amazonaws.com/prod/";
            //commonProps.getEndpoint = "https://p5fpyxi8ha.execute-api.eu-west-1.amazonaws.com/prod/";
            //commonProps.Env = props.Env;

            var service = new HelpDeskFargateStack(this, "HelpDeskEc2Stack");//, getEndpoint = ds.getEndpoint, getByIDEndpoint = ds.getByIDEndpoint, putEndpoint = ds.putEndpoint, updateEndpoint = ds.updateEndpoint });

            //this.HCEndpoint = service.HCEndpoint;
            //this.HCViewerUrl = service.HCViewerUrl;
        }
    }
}
