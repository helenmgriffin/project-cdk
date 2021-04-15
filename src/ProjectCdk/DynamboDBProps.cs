using Amazon.CDK;

namespace ProjectCdk
{
    public class CommonProps : StackProps
    {
        public string updateEndpoint { get; set; }
        public string getEndpoint { get; set; }
        public string getByIDEndpoint { get; set; }
        public string putEndpoint { get; set; }

    }
}
