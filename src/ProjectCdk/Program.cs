using Amazon.CDK;

namespace ProjectCdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new ProjectCdkStack(app, "ProjectCdkStack");

            app.Synth();
        }
    }
}
