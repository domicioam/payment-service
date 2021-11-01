# Architecture decisions

Contents

- [Architecture decisions](#architecture-decisions)
  - [Introduction](#introduction)
  - [Non-functional requirements](#non-functional-requirements)
  - [Decisions](#decisions)
  - [Conclusion](#conclusion)

## Introduction

The intent of this page is to document the initial decisions made in this project regards software architecture and guide other developers when trying to grasp the project.

## Non-functional requirements

Together with functional requirements, non-functional requirements are one of the main drivers to guide software architecture and here some of them are listed:

- Services must be multi-platform and be able to host in Windows and Linux servers as necessary;
- Handle multiple currencies and locals;
- Handle multiple transactions at the same time at all different transaction stages;
- Every transaction must complete in less than 200ms;
- When under heavy load, the application must scale to meet the performance criteria above;
- Credit card information must not be visible in memory;
- Requests to the service must be idempotent;
- Traceability: must be able to track every step of the transaction via logging in the different services involved in the whole process;
- Log must be in one place to avoid having to handle multiple log files when looking for errors;
- Client authentication and authorization;
- System must be available ~99,9% for 24h time period;

## Decisions

### Programming language

C# and .NET 5 were chosen for this project because they are multi-platform and provide IDEs in both Windows, Mac and GNU/Linux leaving us free to develop and host our services in any of these operational systems;

### IDE

This is up to the personal preference of the developer. The options available are Visual Studio, Visual Studio Code and IntelliJ Rider;

### Architecture

The components distribution are based on the Clean Architecture;

### Micro-services

We make use of micro-services to execute some of the functions in a distributed manner in order to meet the availability and scalability criteria;

### Centralized logging

[Elastic Stack](https://www.elastic.co/elastic-stack/) was chosen to fulfill this requirement making use of Logstash and their other products as necessary;

### Authentication and authorization

[IdentityServer4](https://github.com/IdentityServer/IdentityServer4) was chosen to support token based authentication with a centralized authentication server;

### Continuous delivery and continuous deployment:

[Jenkins](https://www.jenkins.io/) and [Docker](https://www.docker.com/) were chosen for this task as they are not vendor locked and we plug our development environment in Microsoft Azure, Google Cloud or Amazon AWS for example;

### Event sourcing

Event sourcing with a event store was used to help with distributed transactions, concurrency and tracking of all operations executed in a transaction. Optimistic locking (event versioning) was used together with event sourcing to guarantee no operation is executed more than once.;

## Conclusion

Those were the initial decisions made in this project to guarantee the initial requirements.
