using Amazon.CDK;
using Amazon.CDK.Pipelines;

namespace ProjectCdk
{
    public class ProjectPipelineStage : Stage
    {
        public ProjectPipelineStage(Construct scope, string id, StageProps props = null) : base(scope , id, props)
        {
            //this declares a new Stage(component of a pipeline), and in that stage instantiate our application stack.(ProjectCdkStack)
            var service = new ProjectCdkStack(this, "WebService");
        }
    }
}
