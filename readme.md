## Microservice Architecture example on using Azure Functions
This repo contains reference examples of implementing a few common serverless architectures using azure functions.

## Durable
Demonstrates the execution of stateful asynchronous workloads using the [Durable Functions framework](https://github.com/Azure/azure-functions-durable-extension).

## Messaging
This function shows an asynchronous message passing pattern. Messages fed in via the Http entry point are published to a servicebus topic which triggers the next function to pick up the message and add it to a storage queue. This ultimately triggers another function which inserts the message into CosmosDB.

## MicroserviceApi
This function is a simple CRUD microservice backed by an azure storage table.

## SPA
The SPA function show how you can use Azure Function proxies to serve a Single Page App and its backing API from a single application.

## Terraform
Contains a terraform definition of all infrastructure necessary to run the above applications.