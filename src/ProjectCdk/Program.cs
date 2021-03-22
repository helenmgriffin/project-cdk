using Amazon.CDK;


namespace ProjectCdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            //var secret = Helper.GetSecret();

            //change the entry point to deploy our pipeline. no longer want the main CDK application to deploy the original app.
            var app = new App();
            new ProjectPipelineStack(app, "ProjectPipelineStack");

            app.Synth();
            //var app = new App();
            //new ProjectCdkStack(app, "ProjectCdkStack");

            //app.Synth();
        }



       
    }
}
