using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class ProcessBlob
    {
        private readonly ILogger<ProcessBlob> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public ProcessBlob(ILogger<ProcessBlob> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }

        [Function("ProcessBlob")]
        public async Task Run(
             [BlobTrigger("process-input/{name}", Connection = "AzureWebJobsStorage")] Stream stream,
            string name
        )
        {
            // Create a BlobClient to the output container
            var outputContainerClient = _blobServiceClient.GetBlobContainerClient("process-output");
            var outputBlobClient = outputContainerClient.GetBlobClient(name);

            // Read the content of the input blob
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();

            // Log information
            _logger.LogInformation($"C# Blob trigger function processed blob\n Name: {name} \n Data Length: {content.Length}");

            // Write the content to the output blob
            using var outputStream = await outputBlobClient.OpenWriteAsync(true);
            using var streamWriter = new StreamWriter(outputStream);
            await streamWriter.WriteAsync(content);
        }
    }
}
