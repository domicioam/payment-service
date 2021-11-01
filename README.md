# Payment Services

Contents

- [Payment Services](#payment-services)
  - [Introduction](#introduction)
  - [How to setup](#how-to-setup)
  - [How to test](#how-to-test)
  - [Architecture decisions](#architecture-decisions)
  - [Things to improve](#things-to-improve)

## Introduction

This project simulates a payment system by using C#, micro-services, Docker, Postgres, RabbitMQ, TDD, Event Sourcing and etc.

## How to setup

- Install Docker in your local computer;
- Go to the root of the this repository;
- Execute `docker-compose up -d --build`;
- Open the [swagger index page](http://localhost:8080/swagger/index.html);

## How to test

You can use the swagger api to play with the solution. There is already a Merchant created and active in the database with the id `3fa85f64-5717-4562-b3fc-2c963f66afa6`;

- In swagger you can send REST calls to create a transaction using the merchant id;
- You can also send commands to a transaction already created (capture, refund, void);

## Architecture decisions

Please visit the [architecture diagrams](Docs/Architecture/architecture-diagrams.md) and [architecture decisions](Docs/Architecture/architecture-decisions.md) pages to find out more.

## Things to improve

- Logging: the centralized logging server is to be done;
- Use state pattern in the Transaction.cs class to refactor if else;
- Security to be done;
- Integration tests to be done;
- Webhooks / SignalR to communicate back to the merchant;
- Reflect solution folders in the real project structure. See [solution structure](Docs/Architecture/architecture-diagrams.md#project-structure);
