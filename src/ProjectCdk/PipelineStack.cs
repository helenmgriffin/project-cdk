using Amazon.CDK;
using Amazon.CDK.AWS.CodeCommit;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.AWS.KMS;
using Amazon.CDK.AWS.SecretsManager;
using Amazon.CDK.Pipelines;
using System.Collections.Generic;

namespace ProjectCdk
{
    public class ProjectPipelineStack : Stack
    {
        [System.Obsolete]
        public ProjectPipelineStack(Construct parent, string id, IStackProps props = null) : base(parent, id, props)
        {

            // Defines the artifact representing the sourcecode
            var sourceArtifact = new Artifact_();
            // Defines the artifact representing the cloud assembly 
            // (cloudformation template + all other assets)
            var cloudAssemblyArtifact = new Artifact_();

            ISecret mySecret = Secret.FromSecretNameV2(this, "GitHubPersonalAccessToken", "GitHubPersonalAccessToken");
            //SecretValue oauth = SecretValue.SecretsManager("arn:aws:secretsmanager:eu-west-1:235629185262:secret:GitHubPersonalAccessToken-Be69at");
            // The basic pipeline declaration. This sets the initial structure
            // of our pipeline
            var pipeline = new CdkPipeline(this, "Pipeline", new CdkPipelineProps
            {
                PipelineName = "ProjectPipeline",
                CloudAssemblyArtifact = cloudAssemblyArtifact,

                SourceAction = new GitHubSourceAction(new GitHubSourceActionProps
                {
                    ActionName = "GitHub",
                    Output = sourceArtifact,
                    OauthToken = mySecret.SecretValue,
                    // Replace these with your actual GitHub project name
                    Owner = "helenmgriffin",
                    Repo = "project-cdk",
                    Branch = "master",
                    Trigger = GitHubTrigger.WEBHOOK
                }),
                    
                // Builds our source code outlined above into a cloud assembly artifact
                SynthAction = SimpleSynthAction.StandardNpmSynth(new StandardNpmSynthOptions
                {
                    SourceArtifact = sourceArtifact,  // Where to get source code to build
                    CloudAssemblyArtifact = cloudAssemblyArtifact,  // Where to place built source

                    InstallCommand = "npm install -g aws-cdk && wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && dpkg -i packages-microsoft-prod.deb && apt-get update && apt-get install -y apt-transport-https && apt-get update && apt-get install -y dotnet-sdk-3.1",
                    BuildCommand = "dotnet build src", // Language-specific build cmd
                }),

            }) ;

            //create an instance of the stage 
            var deploy = new ProjectPipelineStage(this, "Deploy");
            //then add that stage to our pipeline
            var deployStage = pipeline.AddApplicationStage(deploy);
            //var deployStage = pipeline.AddStage("Deploy");

            //deployStage.AddActions(new ElasticBeanStalkDeployAction(new ElasticBeanStalkDeployActionProps()
            //{
            //    Input = new Artifact_("SourceArtifact"),
            //    ActionName = "Deploy",
            //    ApplicationName = "CollegeProject",
            //    EnvironmentName = "CollegeProject-MVCEBEnvironment"
            //}));

            //deployStage.AddActions(new ShellScriptAction(new ShellScriptActionProps
            //{
            //    ActionName = "TestViewerEndpoint",
            //    UseOutputs = new Dictionary<string, StackOutput> {
            //        { "ENDPOINT_URL", pipeline.StackOutput(deploy.HCViewerUrl) }
            //    },
            //    Commands = new string[] { "curl -Ssf $ENDPOINT_URL" }
            //}));

            //deployStage.AddActions(new ShellScriptAction(new ShellScriptActionProps
            //{
            //    ActionName = "TestAPIGatewayEndpoint",
            //    UseOutputs = new Dictionary<string, StackOutput> {
            //        { "ENDPOINT_URL", pipeline.StackOutput(deploy.HCEndpoint)  }
            //    },
            //    Commands = new string[] {
            //        "curl -Ssf $ENDPOINT_URL/",
            //        "curl -Ssf $ENDPOINT_URL/hello",
            //        "curl -Ssf $ENDPOINT_URL/test"
            //    }
            //}));
        }
    }
}
