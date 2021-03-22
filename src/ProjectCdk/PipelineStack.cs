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
            // Creates a CodeCommit repository called 'ProjectRepo'
            //var repo = new Repository(this, "ProjectRepo", new RepositoryProps
            //{
            //    RepositoryName = "project-cdk"
            //});

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
                    OauthToken = SecretValue.PlainText("a4d9a00381b1bbb5c24873a61f3bfeff4ff1d67b"),//.PlainText("a9535df8d5185be0c2644a5247d35c97c601d9d5"), //("GitHubPersonalAccessToken"), //("GitHubPersonalAccessToken", "1"), 
                    Trigger = GitHubTrigger.WEBHOOK,
                    // Replace these with your actual GitHub project name
                    Owner = "helenmgriffin",
                    Repo = "project-cdk",
                    Branch = "master"
                }),

                // Generates the source artifact from the repo we created in the last step
                //SourceAction = new CodeCommitSourceAction(new CodeCommitSourceActionProps
                //{
                //    ActionName = "CodeCommit", // Any Git-based source control
                //    Output = sourceArtifact, // Indicates where the artifact is stored
                //    Repository = repo // Designates the repo to draw code from
                //}),

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
