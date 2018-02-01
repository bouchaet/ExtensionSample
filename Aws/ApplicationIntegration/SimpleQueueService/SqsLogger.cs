using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SQS;
using Amazon.SQS.Model;
using Entities;
using System;

namespace Aws.ApplicationIntegration.SimpleQueueService
{
    public static class SqsLogger
    {
        private static readonly AmazonSQSClient SqsClient;
        private const string ProfileName = "log_profile";
        private static readonly RegionEndpoint LoggerRegion = RegionEndpoint.USEast2;

        static SqsLogger()
        {
            Logger.Singleton.Info += (source, info) => Write($"INFO: {info}");
            Logger.Singleton.Warning += (source, warn) => Write($"WARNING: {warn}");

            RefreshProfile();
            SqsClient = GetClient();
        }

        private static AmazonSQSClient GetClient()
        {
            try
            {
                AWSConfigs.RegionEndpoint = LoggerRegion;
                var chain = new CredentialProfileStoreChain();
                return !chain.TryGetAWSCredentials(ProfileName, out AWSCredentials credentials)
                    ? null
                    : new AmazonSQSClient(credentials);
             }
             catch(Exception ex)
             {
                 Logger.WriteError($"Exception while building AWS SQS client: {ex}");
                 return null; 
             }
        }

        private static void RefreshProfile()
        {
            try
            {
                var netSdkFile = new NetSDKCredentialsFile();
                netSdkFile.TryGetProfile(ProfileName, out CredentialProfile loggerProfile);

                if (loggerProfile == null)
                    loggerProfile = new CredentialProfile(
                        ProfileName,
                        new CredentialProfileOptions());

                loggerProfile.Options.AccessKey =
                    Environment.GetEnvironmentVariable("es.sqs.useraccesskey");
                loggerProfile.Options.SecretKey =
                    Environment.GetEnvironmentVariable("es.sqs.usersecretkey");
                loggerProfile.Region = LoggerRegion;

                netSdkFile.RegisterProfile(loggerProfile);                
            }
            catch (System.Exception e)
            {
                Entities.Logger.WriteError($"Exception while building AWS profile: {e}");
            }

        }

        private static void Write(string s)
        {
            var request = new SendMessageRequest
            {
                MessageBody = s,
                QueueUrl = Environment.GetEnvironmentVariable("es.sqs.log")
            };

            SqsClient?.SendMessageAsync(request);
        }

        public static void Init()
        {
            Logger.WriteInfo("Initializing SQS logger...");
        }
    }
}