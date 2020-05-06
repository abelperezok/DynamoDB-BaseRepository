using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Docker.DotNet;
using Docker.DotNet.Models;
using Xunit;

namespace DynamoDbRepository.Tests
{
    public class DynamoDBDockerFixture : IAsyncLifetime
    {
        private DockerClient _dockerClient;
        private string _containerId;
        private const string dynamodbLocalImage = "amazon/dynamodb-local";
        public string TableName = "test_table";
        public string ServiceUrl = "http://localhost:8000";
        AmazonDynamoDBClient _dynamoDbClient;

        public DynamoDBDockerFixture()
        {
            _dynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig { ServiceURL = ServiceUrl });
            _dockerClient = new DockerClientConfiguration(new Uri(DockerApiUri())).CreateClient();
        }
        
        #region [    IAsyncLifetime interface    ]

        public async Task InitializeAsync()
        {
            await PullImage();
            await StartContainer();

            var createTableResponse = await CreateTableAsync(TableName);
            Console.WriteLine($"{createTableResponse.TableDescription.TableStatus}");

            await WaitUntilTableIsActive(TableName);
        }
        public async Task DisposeAsync()
        {
            if (_containerId != null)
            {
                await DisposeContainer();
            }
        }

        #endregion


        #region [    Docker container handling    ]

        private string DockerApiUri()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
            {
                return "npipe://./pipe/docker_engine";
            }

            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            if (isLinux)
            {
                return "unix:///var/run/docker.sock";
            }

            throw new Exception("Was unable to determine what OS this is running on, does not appear to be Windows or Linux!?");
        }

        private async Task PullImage()
        {
            var imageCreateParams = new ImagesCreateParameters
            {
                FromImage = dynamodbLocalImage,
                Tag = "latest"
            };
            await _dockerClient.Images.CreateImageAsync(imageCreateParams, null, new Progress<JSONMessage>());            
        }

        private async Task StartContainer()
        {
            var containerParams = new CreateContainerParameters
            {
                Image = dynamodbLocalImage,
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { "8000", default(EmptyStruct) }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "8000", new List<PortBinding>{ new PortBinding { HostPort="8000" } } }
                    },
                    PublishAllPorts = true
                },
            };
            var createContainerResponse = await _dockerClient.Containers.CreateContainerAsync(containerParams);
            _containerId = createContainerResponse.ID;
            Console.WriteLine($"Container {createContainerResponse.ID} created");
            foreach (var warning in createContainerResponse.Warnings)
            {
                Console.WriteLine(warning);
            }
            var containerStarted = await _dockerClient.Containers.StartContainerAsync(_containerId, null);
            Console.WriteLine($"Container {containerStarted} started");
        }

        private async Task DisposeContainer()
        {
            await _dockerClient.Containers.KillContainerAsync(_containerId, new ContainerKillParameters());
            Console.WriteLine($"Container {_containerId} killed");

            await _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters());
            Console.WriteLine($"Container {_containerId} removed");
        }

        #endregion


        #region [    DynamoDB table creation    ]

        private async Task<CreateTableResponse> CreateTableAsync(string tableName)
        {
            var createTableReq = new CreateTableRequest
            {
                TableName = tableName,
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement {AttributeName = "PK", KeyType = KeyType.HASH },
                    new KeySchemaElement {AttributeName = "SK", KeyType = KeyType.RANGE }
                },
                AttributeDefinitions = new List<AttributeDefinition>{
                    new AttributeDefinition { AttributeName = "PK", AttributeType = ScalarAttributeType.S },
                    new AttributeDefinition { AttributeName = "SK", AttributeType = ScalarAttributeType.S },
                    new AttributeDefinition { AttributeName = "GSI1", AttributeType = ScalarAttributeType.S }
                },
                ProvisionedThroughput = new ProvisionedThroughput(5, 5),
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                {
                    new GlobalSecondaryIndex
                    {
                        IndexName = "GSI1",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement {AttributeName = "GSI1", KeyType = KeyType.HASH },
                            new KeySchemaElement {AttributeName = "SK", KeyType = KeyType.RANGE }
                        },
                        Projection = new Projection
                        {
                            ProjectionType = ProjectionType.ALL
                        },
                        ProvisionedThroughput = new ProvisionedThroughput(5, 5)
                    }
                }
            };

            return await _dynamoDbClient.CreateTableAsync(createTableReq);
        }

        private async Task WaitUntilTableIsActive(string tableName)
        {
            var currentStatus = TableStatus.CREATING;
            do
            {
                Console.WriteLine($"Checking if the Table is ready ... Currently is {currentStatus}");
                var describeTable = await _dynamoDbClient.DescribeTableAsync(tableName);
                currentStatus = describeTable.Table.TableStatus;
                await Task.Delay(3000);
            }
            while (currentStatus != TableStatus.ACTIVE);
            Console.WriteLine("Table ready !");
        }

        #endregion

    }
}