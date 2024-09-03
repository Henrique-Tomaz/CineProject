using Amazon.SQS.Model;
using Amazon.SQS;
using Microsoft.Extensions.Options;

namespace CineProject.API.Services
{
    public class SqsService
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly string _queueUrl;

        public SqsService(IAmazonSQS sqsClient, IOptions<AWSSettings> settings)
        {
            _sqsClient = sqsClient;
            _queueUrl = settings.Value.QueueUrl;
        }

        public async Task SendMessageAsync(string message)
        {
            var request = new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = message
            };
            await _sqsClient.SendMessageAsync(request);
        }
    }
}
