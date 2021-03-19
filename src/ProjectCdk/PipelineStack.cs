using Amazon.CDK;
using Amazon.CDK.AWS.CodeCommit;

namespace ProjectCdk
{
    public class ProjectPipelineStack: Stack
    {
        public ProjectPipelineStack(Construct parent, string id, IStackProps props = null) : base(parent, id, props)
        {
            // Creates a CodeCommit repository called 'ProjectRepo'
            var repo = new Repository(this, "ProjectRepo", new RepositoryProps
            {
                RepositoryName = "ProjectRepo"
            });

            // Pipeline code goes here
        }
    }
}
