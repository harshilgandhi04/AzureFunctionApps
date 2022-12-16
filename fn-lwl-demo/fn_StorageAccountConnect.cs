using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using System;

namespace fn_lwl_demo
{
    public static class fn_StorageAccountConnect
    {
        [FunctionName("fn_StorageAccountConnect")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("Reading input string");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                string fileContent = data.FileContent;
                string fileName = data.FileName;
                string containerName = data.ContainerName;

                //create a BlobContainerClient
                BlobContainerClient containerClient = new BlobContainerClient(System.Environment.GetEnvironmentVariable("saweu01_STORAGE"), containerName);

                //Create container if not exists
                containerClient.CreateIfNotExists();

                log.LogInformation("Container Created");

                //Create blob inside container
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                await blobClient.UploadAsync(BinaryData.FromString(fileContent), overwrite: true);

                log.LogInformation("Blob Uploaded");

                return new OkObjectResult("Blob Uploaded Successfully");
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult("Azure Function fn_StorageAccountConnect Failed: " + e);
            }
        }
    }
}
