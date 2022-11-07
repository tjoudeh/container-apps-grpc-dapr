A code repository with a [companion detailed post](https://bit.ly/3NGAuxp) that covers how two services deployed to **Azure Container Apps** communicate **synchronously over gRPC** without using Dapr and then we will **Daperize** the two services and utilize the service-to-service invocation features coming with Dapr.

The scenarios I'll cover are the following:
- **Scenario 1**: Invoke gRPC services deployed to Container Apps using **GrpcClient.**
![ACA-Tutorial-grpc-aca-plain](https://user-images.githubusercontent.com/3114431/200207514-d6fad82b-89eb-4365-a956-a3f977df1c45.jpg)
---
- **Scenario 2**: Invoke gRPC services deployed to Container Apps via **Dapr Sidecar** using **GrpcClient**.
- **Scenario 3**: Invoke gRPC services deployed to Container Apps via **Dapr Sidecar** using **DaprClient SDK.**
![ACA-Tutorial-grpc-aca-dapr](https://user-images.githubusercontent.com/3114431/200207535-60092223-820a-472b-8767-7ce569f236ec.jpg)
