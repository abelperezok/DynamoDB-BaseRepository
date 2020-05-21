# Data model assumptions

When trying to generalize a concept, there has to be some assumptions. In this case, the table structure follows the general ideas about hierarchical data overloading the partition and sort keys as well as the GSI.

* Generic partition key "PK" string.
* Generic sort key "SK" string.
* Generic attribute "GSI1" string.
* GSI partition key is "GSI1".
* GSI sort key is also "SK".
* GSI projects all attributes.
* Other attributes such as ID, Name, Description, etc.

## Table with sample data

PK (S) | SK (S) | GSI1 | ID | Name | Description
-------|--------|------|----|------|------------
USER#U1 | METADATA#U1 | USER | U1 | Abel	
USER#U2 | METADATA#U2 | USER | U2 | Nestor	
PROJECT#P1 | METADATA#P1 | PROJECT | P1 | Project 1 | desc project 1
PROJECT#P2 | METADATA#P2 | PROJECT | P2 | Project 2 | desc project 2
PROJECT#P3 | METADATA#P3 | PROJECT | P3 | Project 3 | desc project 3
USER#U1  | GAME#G1 | | G1 | Game 1 |
USER#U1  | GAME#G2 | | G2 | Game 2 |

## GSI with sample data

PK (S) GSI1 | SK (S) SK | ID | Name | Description
------------|-----------|----|------|------------
USER | METADATA#U1 | U1 | Abel	  |
USER | METADATA#U2 | U2 | Nestor  |	
PROJECT | METADATA#P1 | P1 | Project 1 | desc project 1
PROJECT | METADATA#P2 | P2 | Project 2 | desc project 2
PROJECT | METADATA#P3 | P3 | Project 3 | desc project 3


## Queries

* Single item given the ID: Table PK = ENTITY#ID, SK = METADATA#ID
  * Get User U1: Table PK = USER#U1, SK = METADATA#U1
  * Get Project P1: Table PK = PROJECT#P1, SK = METADATA#P1
  * Get Game G1 for User U1: Table PK = USER#U1, SK = GAME#G1

* Multiple items of one type: GSI PK = ENTITY
  * Get all users: GSI PK = USER
  * Get all projects: GSI PK = PROJECT

* Item collection by parent ID: Table PK = PARENT_ENTITY#ID, SK = ENTITY#ID
  * Get all games by user U1 : Table PK = USER#U1, SK begins_with GAME

## Creating the table - AWS CLI

Use ```aws dynamodb create-table``` command to create a table.

```shell
$ aws dynamodb create-table \
--table-name dynamodb_test_table \
--attribute-definitions \
    AttributeName=PK,AttributeType=S \
    AttributeName=SK,AttributeType=S \
    AttributeName=GSI1,AttributeType=S \
--key-schema \
    AttributeName=PK,KeyType=HASH \
    AttributeName=SK,KeyType=RANGE \
--provisioned-throughput \
    ReadCapacityUnits=1,WriteCapacityUnits=1 \
--global-secondary-indexes \
IndexName=GSI1,KeySchema=["{AttributeName=GSI1,KeyType=HASH},{AttributeName=SK,KeyType=RANGE}"],\
Projection="{ProjectionType=ALL}",\
ProvisionedThroughput="{ReadCapacityUnits=1,WriteCapacityUnits=1}"
```

Optionally, wait for the table to be active.

```shell
$ aws dynamodb wait table-exists --table-name dynamodb_test_table
```

## Creating the table - CloudFormation

```yaml
  # DynamoDb table
  DynamoDbTestTable:
    Type: AWS::DynamoDB::Table
    Properties: 
      KeySchema: 
        - AttributeName: PK
          KeyType: HASH
        - AttributeName: SK
          KeyType: RANGE
      AttributeDefinitions:
        - AttributeName: PK
          AttributeType: S
        - AttributeName: SK
          AttributeType: S
        - AttributeName: GSI1
          AttributeType: S
      ProvisionedThroughput:
        ReadCapacityUnits: 1
        WriteCapacityUnits: 1
      GlobalSecondaryIndexes:
        - IndexName: GSI1
          KeySchema: 
            - AttributeName: GSI1
              KeyType: HASH
            - AttributeName: SK
              KeyType: RANGE
          Projection: 
            ProjectionType: ALL
          ProvisionedThroughput: 
            ReadCapacityUnits: 1
            WriteCapacityUnits: 1
```
