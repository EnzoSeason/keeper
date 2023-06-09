# keeper

A financial reporting tool

## Architecture

![archi-1](./img/archi-1.png)
[Source](https://miro.com/app/board/uXjVMWXddkA=/)

There are 2 services:

- Configuring Service:

  It manages the reporting configuration. The reporting configuration indicates how the report is created from the data source.

- Reporting Service:

  It collects the transactions and build the report from them.
  
Both of 2 services write into the same database on the same instance. Technically, they can access all the collections used by them. However, the access permission of each service is controlled by its own `appsetting.json`.

## Configuring Service

### API

It's a CRUD API.

- Create a config: It must have a username and UID.

- Read a config

- Update a config: Only the reporting configuration can be updated. UID is immutable.

## Reporting Service

### API

It follows CQRS architecture pattern.

- **Add transactions command**

- **Upload a CSV file of transaction command**

- **Build a report command**: It receives the command that asks the App to read the data from database, create the report, and save the results into database

- **Get a report query**: It returns the reporting result.

## Usage (In Progress)

To run the project in Development mode on the local machine

```code
docker compose up
```

## Development

Run the project locally. Please lanuch **MongoDB** on your local machine.

## ITest

To run the integration tests locally, we need to, first, move to the `service` repo.

### Reporting

```code
docker compose -f docker-compose-test.yml -f docker-compose-test.override.yml up --build reporting-api-integration-test --exit-code-from reporting-api-integration-test
```
