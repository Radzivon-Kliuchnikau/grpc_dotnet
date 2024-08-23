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
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                "There must be a valid object provided to creat a new Duty Task..."));
        }

        var newDutyTask = new DutyTask // use dtos here
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
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                "The id of Task should not be less or equal to zero"));
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

        throw new RpcException(new Status(StatusCode.NotFound,
            $"There is no such Duty Task in the database with id equals {request.Id}"));
    }

    public override async Task<GetAllTasksResponse> GetAllDutyTasks(GetAllTasksRequest request,
        ServerCallContext context)
    {
        var response = new GetAllTasksResponse();
        var allDutyTasks = await dbContext.Tasks.ToListAsync();

        foreach (var task in allDutyTasks)
        {
            response.DutyTask.Add(new ReadTaskResponse
            {
                Description = task.Description,
                TaskName = task.TaskName,
                Id = task.Id,
                TaskStatus = task.TaskStatus
            });
        }

        return await Task.FromResult(response);
    }

    public override async Task<UpdateTaskResponse> UpdateDutyTask(UpdateTaskRequest request, ServerCallContext context)
    {
        if (request.Id <= 0 || request.TaskName == string.Empty || request.Description == string.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                "You should provide a correct Id that not less then zero. TaskName and Description shouldn't be empty"));
        }

        var dutyTaskToUpdate = await dbContext.Tasks.FirstOrDefaultAsync(task => task.Id == request.Id);

        if (dutyTaskToUpdate == null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                $"There is no such Duty Task to update with id equals {request.Id}"));
        }

        dutyTaskToUpdate.TaskName = request.TaskName;
        dutyTaskToUpdate.Description = request.Description;
        dutyTaskToUpdate.TaskStatus = request.TaskStatus;

        // dbContext.Tasks.Update(dutyTaskToUpdate);
        await dbContext.SaveChangesAsync();

        return await Task.FromResult(new UpdateTaskResponse
        {
            Id = dutyTaskToUpdate.Id
        });
    }

    public override async Task<DeleteTaskResponse> RemoveDutyTask(DeleteTaskRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                "The id of Task should not be less or equal to zero"));
        }

        var dutyTaskToDelete = await dbContext.Tasks.FirstOrDefaultAsync(task => task.Id == request.Id);

        if (dutyTaskToDelete == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"There is no such Duty Task with id equals {request.Id}"));
        }

        dbContext.Tasks.Remove(dutyTaskToDelete);
        await dbContext.SaveChangesAsync();

        return await Task.FromResult(new DeleteTaskResponse
        {
            Id = dutyTaskToDelete.Id
        });
    }
}