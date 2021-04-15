using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;

namespace ProjectCdk
{
    /// <summary>
    /// ECS constructs to set up website application environment
    /// Define a ECS service that runs by loading a public image from Docker Hub
    /// Then create a load balancer and add the service to it
    /// </summary>
    public class HelpDeskFargateStack : Stack
    {
        public readonly CfnOutput externalDNS;

        internal HelpDeskFargateStack(Construct scope, string id, CommonProps props = null) : base(scope, id, props)
        {
            //Existing VPC
            IVpc vpc = Vpc.FromLookup(this, "VPC", new VpcLookupOptions
            {
                // This imports the default VPC but you can also
                // specify a 'vpcName' or 'tags'.
                IsDefault = true,
            });
            // Create an ECS cluster
            Cluster cluster = new Cluster(this, "CollegeProjectCluster", new ClusterProps { Vpc = vpc });
            
            // Create a load-balanced Fargate service and make it public
            ApplicationLoadBalancedFargateService fargateService = new ApplicationLoadBalancedFargateService(this, "CollegeProjectService",
                new ApplicationLoadBalancedFargateServiceProps
                {
                    Cluster = cluster,          // Required
                    DesiredCount = 1,           // Default is 1
                    TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                    {
                        //AWS ECR Repo
                        Image = ContainerImage.FromEcrRepository(Repository.FromRepositoryName(this, "collegeprojectrepo", "collegeproject"), "latest"),
                        //Dockerhub Repo
                        //Image = ContainerImage.FromRegistry("helenmgriffin/collegeproject:latest")
                        //Environment = new Dictionary<string, string>
                        //{
                        //    ["GetEndpointUrl"] = props.getEndpoint,
                        //    ["GetByIDEndpointUrl"] = props.getByIDEndpoint,
                        //    ["CreateEndpointUrl"] = props.putEndpoint,
                        //    ["UpdateEndpointUrl"] = props.updateEndpoint
                        //}
                    },
                    MemoryLimitMiB = 512,      // Default is 256
                    PublicLoadBalancer = true,    // Default is false
                    AssignPublicIp = true,
                }
            );

            this.externalDNS = new CfnOutput(this, "CollegeProjecExternaleDNS",
                new CfnOutputProps
                {
                    Value = fargateService.LoadBalancer.LoadBalancerDnsName
                }
            );
        }
    }
}
