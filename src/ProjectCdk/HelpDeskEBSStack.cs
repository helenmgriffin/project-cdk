using Amazon.CDK;
using Amazon.CDK.AWS.ElasticBeanstalk;
using Amazon.CDK.AWS.S3.Assets;
using System.IO;
using static Amazon.CDK.AWS.ElasticBeanstalk.CfnApplicationVersion;

namespace ProjectCdk
{
    public class HelpDeskEBSStack : Stack
    {
        public readonly CfnOutput externalDNS;

        internal HelpDeskEBSStack(Construct scope, string id, StackProps props = null) : base(scope, id, props)
        {
            // Construct an S3 asset from the ZIP located from directory up.
            Asset elbZipArchive = new Asset(this, "HelpDeskAppZip", new AssetProps
            {
                //Path = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "publish"), "CollegeProject")
                Path = Path.Join("C:\\CollegeProjectWebSite", "CollegeProjectWebSite.zip")
            });

            var appName = "CollegeProject";
            CfnApplication app = new CfnApplication(this, "CollegeProject", new CfnApplicationProps
            {
                ApplicationName = appName
            });

            // Example of some options which can be configured
            CfnEnvironment.OptionSettingProperty[] optionSettingProperties = new CfnEnvironment.OptionSettingProperty[2];
            
            CfnEnvironment.OptionSettingProperty option1 = new CfnEnvironment.OptionSettingProperty();
            option1.Namespace = "aws:autoscaling:launchconfiguration";
            option1.OptionName = "InstanceType";
            option1.Value = "t2.micro";

            optionSettingProperties[0] = option1;

            CfnEnvironment.OptionSettingProperty option2 = new CfnEnvironment.OptionSettingProperty();
            option2.Namespace = "aws:autoscaling:launchconfiguration";
            option2.OptionName = "IamInstanceProfile";
            option2.Value = "aws-elasticbeanstalk-ec2-role";

            optionSettingProperties[1] = option2;

            // Create an app version from the S3 asset defined above
            // The S3 "putObject" will occur first before CF generates the template
            var appVersionProps = new CfnApplicationVersion(this, "AppVersion", new CfnApplicationVersionProps
            {
                ApplicationName = appName,
                SourceBundle = new SourceBundleProperty
                {
                    S3Bucket = elbZipArchive.S3BucketName,
                    S3Key = elbZipArchive.S3ObjectKey
                },
            });

            CfnEnvironment env = new CfnEnvironment(this, "CollegeProjectEnvironment", new CfnEnvironmentProps
            {
                EnvironmentName = "CollegeProjectEnvironment",
                ApplicationName = app.ApplicationName,
                SolutionStackName = "64bit Amazon Linux 2 v2.1.4 running .NET Core",
                OptionSettings = optionSettingProperties,
                VersionLabel = appVersionProps.Ref,
                //PlatformArn = platform
            });

            // Also very important - make sure that `app` exists before creating an app version
            appVersionProps.AddDependsOn(app);

            this.externalDNS = new CfnOutput(this, "HelpDeskExternaleDNS", new CfnOutputProps
            {
                Value = env.AttrEndpointUrl
            });
        }
    }
}
