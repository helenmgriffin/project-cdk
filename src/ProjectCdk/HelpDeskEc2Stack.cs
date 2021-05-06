using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Ecr.Assets;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using System.Collections.Generic;
using System.IO;

namespace ProjectCdk
{
    /// <summary>
    /// ECS constructs to set up website application environment
    /// Define a ECS service that runs by loading a public image from Docker Hub
    /// Then create a load balancer and add the service to it
    /// </summary>
    public class HelpDeskEC2Stack : Stack
    {
        public readonly CfnOutput externalDNS;

        internal HelpDeskEC2Stack(Construct scope, string id, CommonProps props) : base(scope, id, props)
        {
            IVpc vpc = new Vpc(this, "CollegeProjectVpc", new VpcProps { MaxAzs = 2 });
            // Create an ECS cluster
            Cluster cluster = new Cluster(this, "CollegeProjectCluster", new ClusterProps { Vpc = vpc });

            //Add capacity to it
            cluster.AddCapacity("CollegeProjectVpcCapacity", new AddCapacityOptions
            {
                InstanceType = new InstanceType("t2.micro")
            });

            //To run a task or service with Amazon EC2 launch type, use the Ec2TaskDefinition
            Ec2TaskDefinition helpDeskTaskDef = new Ec2TaskDefinition(this, "CollegeProjectEC2TaskDef");

            ContainerDefinition helpDeskContainer = helpDeskTaskDef.AddContainer("CollegeProjectContainer", new ContainerDefinitionOptions
            {
                //AWS ECR Repo
                Image = ContainerImage.FromEcrRepository(Repository.FromRepositoryName(this, "collegeprojectrepo", "collegeproject"), "latest"),

                //Pass API Endpoints to docker image environment variables
                Environment = new Dictionary<string, string>
                {
                    ["GetTicketsGatewayUrl"] = props.getEndpoint,
                    ["GetTicketByIDGatewayUrl"] = props.getByIDEndpoint,
                    ["PutTicketGatewayUrl"] = props.putEndpoint,
                    ["UpdateTicketGatewayUrl"] = props.updateEndpoint
                },
                MemoryLimitMiB = 512,
                Privileged = true,
            });;

            helpDeskContainer.AddPortMappings(new PortMapping
            {
                ContainerPort = 80
            });

            // Instantiate an Amazon ECS Service
            Ec2Service collegeProjectService = new Ec2Service(this, "CollegeProjectService", new Ec2ServiceProps
            {
                Cluster = cluster,
                TaskDefinition = helpDeskTaskDef
            });

            // Internet facing load balancer for the frontend services
            var externalLB = new ApplicationLoadBalancer(this, "CollegeProjectExternalLB", new Amazon.CDK.AWS.ElasticLoadBalancingV2.ApplicationLoadBalancerProps
            {
                Vpc = vpc,
                InternetFacing = true
            });

            var externalListener = externalLB.AddListener("CollegeProjectPublicListener", new BaseApplicationListenerProps
            {
                Port = 80,
                Open = true
            });

            IApplicationLoadBalancerTarget[] targets = new IApplicationLoadBalancerTarget[1];
            targets[0] = collegeProjectService;

            externalListener.AddTargets("CollegeProject", new AddApplicationTargetsProps
            {
                Port = 80,
                Targets = targets
            });

            this.externalDNS = new CfnOutput(this, "CollegeProjectExternaleDNS", new CfnOutputProps
            {
                Value = externalLB.LoadBalancerDnsName
            });
        }
    }
}
