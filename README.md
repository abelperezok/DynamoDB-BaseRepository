# DynamoDB base repository

C# .NET Core implementation of the repository pattern using DynamoDB as data store using hierarchical data modelling strategy overloading the partition and sort key as well secondary index.

This implementation aims to solve the most common data persistence use cases ranging from single entities to more complex data models.

Key features:
* Pre-packaged CRUD and batch operations.
* Generic design for flexibility of data types.
* Easily extensible allowing the addition of specific functionality.

## Detailed content

[Data model assumptions](docs/data-model-assumptions.md)

[Usage - single entities](docs/usage-single-entities.md)

[Methods reference](docs/methods-reference.md)

[Example: CRUD operations](docs/example-crud-operations.md)

[Example: Batch operations](docs/example-batch-operations.md)




