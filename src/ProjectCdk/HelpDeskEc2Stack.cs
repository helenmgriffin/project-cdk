using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Ecr.Assets;
using Amazon.CDK.AWS.ECS;
//using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
//using Amazon.CDK.AWS.ECS.Patterns;
//using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using System.IO;

namespace ProjectCdk
{
    /// <summary>
    /// ECS constructs to set up website application environment
    /// Define a ECS service that runs by loading a public image from Docker Hub
    /// Then create a load balancer and add the service to it
    /// </summary>
    public class HelpDeskEc2Stack : Stack
    {
        public readonly CfnOutput externalDNS;

        internal HelpDeskEc2Stack(Construct scope, string id, StackProps props) : base(scope, id, props)
        {
            IVpc vpc = new Vpc(this, "HelpDeskVpc", new VpcProps { MaxAzs = 2 });
            // Create an ECS cluster
            Cluster cluster = new Cluster(this, "HelpDeskVpcCluster", new ClusterProps { Vpc = vpc });

            //var asset = new DockerImageAsset(this, "CollegeProjectImage", new DockerImageAssetProps
            //{
            //    Directory = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "CollegeProject"))
            //});

            //// Create a load-balanced Fargate service and make it public
            //ApplicationLoadBalancedFargateService fargateService = new ApplicationLoadBalancedFargateService(this, "HelpDeskFargateService",
            //    new ApplicationLoadBalancedFargateServiceProps
            //    {
            //        Cluster = cluster,          // Required
            //        DesiredCount = 1,           // Default is 1
            //        TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
            //        {
            //            Image = ContainerImage.FromDockerImageAsset(asset),//ContainerImage.FromRegistry("helenmgriffin/collegeproject:latest")
            //        },
            //        MemoryLimitMiB = 512,      // Default is 256
            //        PublicLoadBalancer = true    // Default is false
            //    }
            //);;

            //this.externalDNS = new CfnOutput(this, "HelpDeskExternaleDNS",
            //    new CfnOutputProps
            //    {
            //        Value = fargateService.LoadBalancer.LoadBalancerDnsName
            //    }
            //);
            // Add capacity to it
            cluster.AddCapacity("HelpDeskVpcCapacity", new AddCapacityOptions
            {
                InstanceType = new InstanceType("t2.micro")
            });

            //To run a task or service with Amazon EC2 launch type, use the Ec2TaskDefinition
            Ec2TaskDefinition helpDeskTaskDef = new Ec2TaskDefinition(this, "HelpDeskEc2TaskDef");

            var helpDeskContainer = helpDeskTaskDef.AddContainer("HelpDeskContainer", new ContainerDefinitionOptions
            {
                // Use an image from DockerHub
                Image = ContainerImage.FromRegistry("helenmgriffin/collegeproject"),
                MemoryLimitMiB = 512,
                Privileged = true
                //Environment = new Dictionary<string, string>
                //{
                //    ["GetEndpoint"] = props.getEndpoint.Value.ToString(),
                //    ["GetByIDEndpoint"] = props.getByIDEndpoint.Value.ToString(),
                //    ["PutEndpoint"] = props.putEndpoint.Value.ToString(),
                //    ["UpdateEndpoint"] = props.updateEndpoint.Value.ToString()
                //}
            }); ;
            
            helpDeskContainer.AddPortMappings(new PortMapping
            {
                ContainerPort = 3000
            });

            // Instantiate an Amazon ECS Service
            Ec2Service helpDeskService = new Ec2Service(this, "HelpDeskService", new Ec2ServiceProps
            {
                Cluster = cluster,
                TaskDefinition = helpDeskTaskDef
            });

            // Internet facing load balancer for the frontend services
            var externalLB = new ApplicationLoadBalancer(this, "ExternalLB", new ApplicationLoadBalancerProps
            {
                Vpc = vpc,
                InternetFacing = true
            });

            var externalListener = externalLB.AddListener("PublicListener", new BaseApplicationListenerProps
            {
                Port = 80,
                Open = true
            });

            IApplicationLoadBalancerTarget[] targets = new IApplicationLoadBalancerTarget[1];
            targets[0] = helpDeskService;

            externalListener.AddTargets("HelpDesk", new AddApplicationTargetsProps
            {
                Port = 80,
                Targets = targets
            });

            this.externalDNS = new CfnOutput(this, "HelpDeskExternaleDNS", new CfnOutputProps
            {
                Value = externalLB.LoadBalancerDnsName
            });
        }
    }
}
