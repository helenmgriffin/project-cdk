using Amazon.CDK;
using Amazon.CDK.Pipelines;

namespace ProjectCdk
{
    public class ProjectPipelineStage : Stage
    {
        public ProjectPipelineStage(Construct scope, string id, StageProps props = null) : base(scope , id, props)
        {
            //this declares a new Stage(component of a pipeline),
            //and in that stage instantiate our application stacks.(DynamoDBStackCDK and HelpDeskFargateStack)
            var ds = new DynamoDBStack(this, "DynamoDBStackCDK");
            new HelpDeskFargateStack(this, "HelpDeskFargateStack", ds.CommonStacProps);
            //new HelpDeskEC2Stack(this, "HelpDeskEC2Stack", ds.CommonStacProps);
            //new HelpDeskEBSStack(this, "HelpDeskEBSStack");
        }
    }
}
