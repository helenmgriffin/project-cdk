﻿using Amazon.CDK;
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

            var service = new HelpDeskFargateStack(this, "HelpDeskEc2Stack");//, getEndpoint = ds.getEndpoint, getByIDEndpoint = ds.getByIDEndpoint, putEndpoint = ds.putEndpoint, updateEndpoint = ds.updateEndpoint });

            //this.HCEndpoint = service.HCEndpoint;
            //this.HCViewerUrl = service.HCViewerUrl;
        }
    }
}
