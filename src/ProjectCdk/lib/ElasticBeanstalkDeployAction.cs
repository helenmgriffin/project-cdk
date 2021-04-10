using Amazon.CDK;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.IAM;

namespace ProjectCdk
{
    public class ElasticBeanStalkDeployActionProps: CommonAwsActionProps
    {
        public string ApplicationName { get; set; }
        public string EnvironmentName { get; set; }
        public Artifact_ Input { get; set; }
        public string Account { get; set; }
    }

    public class ElasticBeanStalkDeployAction : IAction
    {
        public IActionProperties ActionProperties { get; set; }
        private readonly ElasticBeanStalkDeployActionProps Props;

        public ElasticBeanStalkDeployAction(ElasticBeanStalkDeployActionProps props)
        {
            this.ActionProperties = new ActionProperties()
            {
                ActionName = "Deploy",
                Provider = "ElasticBeanstalk",
                Category = ActionCategory.DEPLOY,
                ArtifactBounds = new ActionArtifactBounds()
                {
                    MinInputs = 1,
                    MaxInputs = 1,
                    MinOutputs = 0,
                    MaxOutputs = 0
                },
                Inputs = new[] { props.Input }
            };

            this.Props = props;
        }

        public IActionConfig Bind(Construct scope, IStage stage, IActionBindOptions options)
        {
            options.Bucket.GrantRead(options.Role);

            options.Role.AddToPrincipalPolicy(new PolicyStatement(new PolicyStatementProps()
            {
                Resources = new[] { "*" },
                Actions = new[]
                {
                    "elasticbeanstalk:*",
                    "autoscaling:*",
                    "elasticloadbalancing:*",
                    "rds:*",
                    "s3:*",
                    "cloudwatch:*",
                    "cloudformation:*"
                },
            }));

            return new ActionConfig()
            {
                Configuration = new
                {
                    ApplicationName = this.Props.ApplicationName,
                    EnvironmentName = this.Props.EnvironmentName,
                }
            };
        }

        public Rule OnStateChange(string name, IRuleTarget target = null, IRuleProps options = null)
        {
            throw new System.NotImplementedException();
        }
    }

}
