import cdk = require('@aws-cdk/core');
import elasticbeanstalk = require('@aws-cdk/aws-elasticbeanstalk');
import s3assets = require('@aws-cdk/aws-s3-assets');

export class ElbtestStack extends cdk.Stack {
    constructor(scope: cdk.App, id: string, props?: cdk.StackProps) {
        super(scope, id, props);

        // Construct an S3 asset from the ZIP located from directory up.
        const elbZipArchive = new s3assets.Asset(this, 'MyElbAppZip', {
            path: `${__dirname}/../app.zip`,
        });

        const appName = 'MyApp';
        const app = new elasticbeanstalk.CfnApplication(this, 'Application', {
            applicationName: appName,
        });

        // Example of some options which can be configured
        const optionSettingProperties: elasticbeanstalk.CfnEnvironment.OptionSettingProperty[] = [
            {
                namespace: 'aws:autoscaling:launchconfiguration',
                optionName: 'InstanceType',
                value: 't3.small',
            },
            {
                namespace: 'aws:autoscaling:launchconfiguration',
                optionName: 'IamInstanceProfile',
                // Here you could reference an instance profile by ARN (e.g. myIamInstanceProfile.attrArn)
                // For the default setup, leave this as is (it is assumed this role exists)
                // https://stackoverflow.com/a/55033663/6894670
                value: 'aws-elasticbeanstalk-ec2-role',
            },
            {
                namespace: 'aws:elasticbeanstalk:container:nodejs',
                optionName: 'NodeVersion',
                value: '10.16.3',
            },
        ];

        // Create an app version from the S3 asset defined above
        // The S3 "putObject" will occur first before CF generates the template
        const appVersionProps = new elasticbeanstalk.CfnApplicationVersion(this, 'AppVersion', {
            applicationName: appName,
            sourceBundle: {
                s3Bucket: elbZipArchive.s3BucketName,
                s3Key: elbZipArchive.s3ObjectKey,
            },
        });

        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        const elbEnv = new elasticbeanstalk.CfnEnvironment(this, "Environment", {
            environmentName: 'MySampleEnvironment',
            applicationName: app.applicationName || appName,
            solutionStackName: '64bit Amazon Linux 2018.03 v4.11.0 running Node.js',
            optionSettings: optionSettingProperties,
            // This line is critical - reference the label created in this same stack
            versionLabel: appVersionProps.ref,
        });
        // Also very important - make sure that `app` exists before creating an app version
        appVersionProps.addDependsOn(app);
    }
}