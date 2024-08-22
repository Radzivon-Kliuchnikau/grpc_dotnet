using Grpc.Core;
using GrpcDotNet.Data;
using GrpcDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcDotNet.Services;

public class TaskActionsService(AppDbContext dbContext) : TasksActions.TasksActionsBase
{
    public override async Task<CreateTaskResponse> CreateDutyTask(CreateTaskRequest request, ServerCallContext context)
    {
        if (request.TaskName == string.Empty || request.Description == string.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "There must be a valid object provided to creat a new Duty Task..."));
        }

        var newDutyTask = new DutyTask
        {
            TaskName = request.TaskName,
            Description = request.Description
        };

        await dbContext.Tasks.AddAsync(newDutyTask);
        await dbContext.SaveChangesAsync();

        return await Task.FromResult(new CreateTaskResponse
        {
            Id = newDutyTask.Id
        });
    }

    public override async Task<ReadTaskResponse> GetSingleDutyTask(ReadTaskRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "The id of Task should not be less or equal to zero"));
        }

        var dutyTask = await dbContext.Tasks.FirstOrDefaultAsync(task => task.Id == request.Id);

        if (dutyTask != null)
        {
            return await Task.FromResult(new ReadTaskResponse
            {
                Id = dutyTask.Id,
                Description = dutyTask.Description,
                TaskName = dutyTask.TaskName,
                TaskStatus = dutyTask.TaskStatus
            });
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"There is no such Duty Task in the database with id equals {request.Id}"));
    }
}