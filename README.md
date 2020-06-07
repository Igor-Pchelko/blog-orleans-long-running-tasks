# Orleans long-running tasks execution

This project demonstrates the approach of managing long running tasks in Orleans cluster.

[Long-running tasks in Orleans dotnet blog post](https://pcholko.com/posts/2020-06-07/orleans-long-running-tasks/)


Start application:

```shell
dotnet run --project src/LongRunningTasks
```

Swagger Open API UI is accessable at [https://localhost:5001/swagger/index.html](https://localhost:5001/swagger/index.html)

Run tests:

```shell
dotnet test test/LongRunningTasks.Tests
```
