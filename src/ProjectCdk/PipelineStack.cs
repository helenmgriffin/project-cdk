using Amazon.CDK;
using Amazon.CDK.AWS.CodeCommit;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.Pipelines;
using System.Collections.Generic;

namespace ProjectCdk
{
    public class ProjectPipelineStack : Stack
    {
        public ProjectPipelineStack(Construct parent, string id, IStackProps props = null) : base(parent, id, props)
        {
            // Defines the artifact representing the sourcecode
            var sourceArtifact = new Artifact_();
            // Defines the artifact representing the cloud assembly 
            // (cloudformation template + all other assets)
            var cloudAssemblyArtifact = new Artifact_();
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
                    OauthToken = SecretValue.PlainText("47ecb2c60673dcb4b25c501dc4aaa5d82327a0b5"),//.PlainText("a9535df8d5185be0c2644a5247d35c97c601d9d5"), //("GitHubPersonalAccessToken"), //("GitHubPersonalAccessToken", "1"), 
                    Trigger = GitHubTrigger.WEBHOOK,
                    // Replace these with your actual GitHub project name
                    Owner = "helenmgriffin",
                    Repo = "project-cdk",
                    Branch = "master"
                }),

                // Builds our source code outlined above into a could assembly artifact
                SynthAction = SimpleSynthAction.StandardNpmSynth(new StandardNpmSynthOptions
                {
                    SourceArtifact = sourceArtifact,  // Where to get source code to build
                    CloudAssemblyArtifact = cloudAssemblyArtifact,  // Where to place built source

                    InstallCommand = "npm install -g aws-cdk && wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && dpkg -i packages-microsoft-prod.deb && apt-get update && apt-get install -y apt-transport-https && apt-get update && apt-get install -y dotnet-sdk-3.1",
                    BuildCommand = "dotnet build src", // Language-specific build cmd
                })
            }); 

            //create an instance of the stage 
            var deploy = new ProjectPipelineStage(this, "Deploy");
            //then add that stage to our pipeline
            var deployStage = pipeline.AddApplicationStage(deploy);

            deployStage.AddActions(new ShellScriptAction(new ShellScriptActionProps
            {
                ActionName = "TestViewerEndpoint",
                UseOutputs = new Dictionary<string, StackOutput> {
                    { "ENDPOINT_URL", pipeline.StackOutput(deploy.HCViewerUrl) }
                },
                Commands = new string[] { "curl -Ssf $ENDPOINT_URL" }
            }));
            deployStage.AddActions(new ShellScriptAction(new ShellScriptActionProps
            {
                ActionName = "TestAPIGatewayEndpoint",
                UseOutputs = new Dictionary<string, StackOutput> {
                    { "ENDPOINT_URL", pipeline.StackOutput(deploy.HCEndpoint)  }
                },
                Commands = new string[] {
                    "curl -Ssf $ENDPOINT_URL/",
                    "curl -Ssf $ENDPOINT_URL/hello",
                    "curl -Ssf $ENDPOINT_URL/test"
                }
            }));
        }
    }
}
