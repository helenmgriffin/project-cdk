using Amazon.CDK;

namespace ProjectCdk
{
    public class DynamboDBProps : StackProps
    {
        public CfnOutput updateEndpoint { get; set; }
        public CfnOutput getEndpoint { get; set; }
        public CfnOutput getByIDEndpoint { get; set; }
        public CfnOutput putEndpoint { get; set; }

    }
}
