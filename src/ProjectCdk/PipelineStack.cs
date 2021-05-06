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
        public ProjectPipelineStack(Construct parent, string id, IStackProps props = null) : base(parent, id, props)
        {
            // Defines the artifact representing the sourcecode
            var sourceArtifact = new Artifact_();
            // Defines the artifact representing the cloud assembly 
            // (cloudformation template + all other assets)
            var cloudAssemblyArtifact = new Artifact_();

            ISecret mySecret = Secret.FromSecretNameV2(this, "GitHubPersonalAccessToken", "GitHubPersonalAccessToken");
            // The basic pipeline declaration. This sets the initial structure of our pipeline
            var pipeline = new CdkPipeline(this, "Pipeline", new CdkPipelineProps
            {
                PipelineName = "ProjectPipeline",
                CloudAssemblyArtifact = cloudAssemblyArtifact,
                
                // Generates the source artifact from the GitHub repo we specify in the Owner/Repo props speified below
                SourceAction = new GitHubSourceAction(new GitHubSourceActionProps
                {
                    ActionName = "GitHub",// Any Git-based source control
                    Output = sourceArtifact,// Indicates where the artifact is stored
                    OauthToken = mySecret.SecretValue,//GitHub personal access key to authenticate with GitHub
                    // Replace these with your actual GitHub project name
                    Owner = "helenmgriffin",
                    Repo = "project-cdk",// Designates the repo to draw code from
                    Branch = "master",// repo branch name
                    //A webhook is created in GitHub that triggers the action with POLL
                    Trigger = GitHubTrigger.WEBHOOK //How CodePipeline should be triggered
                }),
                    
                // Builds our source code outlined above into a cloud assembly artifact
                SynthAction = SimpleSynthAction.StandardNpmSynth(new StandardNpmSynthOptions
                {
                    SourceArtifact = sourceArtifact,  // Where to get source code to build
                    CloudAssemblyArtifact = cloudAssemblyArtifact,  // Where to place built source

                    //setup and install our CDKPipelein environment
                    InstallCommand = "npm install -g aws-cdk " +
                    "&& wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb " +
                    "&& dpkg -i packages-microsoft-prod.deb && apt-get update " +
                    "&& apt-get install -y apt-transport-https && apt-get update " +
                    "&& apt-get install -y dotnet-sdk-3.1",
                    BuildCommand = "dotnet build src", // Language-specific build cmd
                }),

            });

            //create an instance of the stage 
            var deploy = new ProjectPipelineStage(this, "Deploy");
            //then add that stage to our pipeline
            var deployStage = pipeline.AddApplicationStage(deploy);
           
        }
    }
}
