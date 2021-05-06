using Amazon.CDK;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ProjectCdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            //change the entry point to deploy our pipeline. no longer want the main CDK application to deploy the original app.
            new ProjectPipelineStack(app, "ProjectPipelineStack");

            app.Synth();
        }



       
    }
}
