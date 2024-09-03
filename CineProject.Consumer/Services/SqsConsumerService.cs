using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;

namespace CineProject.Consumer.Services
{
    public class SqsConsumerService : BackgroundService
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<SqsConsumerService> _logger;
        private readonly string _queueUrl;

        public SqsConsumerService(IAmazonSQS sqsClient, ILogger<SqsConsumerService> logger, IConfiguration configuration)
        {
            _sqsClient = sqsClient;
            _logger = logger;
            _queueUrl = configuration.GetSection("AWS")["QueueUrl"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SQS Consumer Service está iniciando.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var receiveRequest = new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20
                };

                var response = await _sqsClient.ReceiveMessageAsync(receiveRequest, stoppingToken);

                if (response.Messages.Count > 0)
                {
                    foreach (var message in response.Messages)
                    {
                        _logger.LogInformation($"Mensagem recebida: {message.Body}");
                        var deleteRequest = new DeleteMessageRequest
                        {
                            QueueUrl = _queueUrl,
                            ReceiptHandle = message.ReceiptHandle
                        };
                        await _sqsClient.DeleteMessageAsync(deleteRequest, stoppingToken);
                    }
                }
            }

            _logger.LogInformation("SQS Consumer Service está parando.");
        }
    }
}
