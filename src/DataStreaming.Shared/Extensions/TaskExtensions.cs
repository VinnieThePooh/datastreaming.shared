namespace DataStreaming.Extensions;

public static class TaskExtensions
{
    public static async Task WhenAll(this IEnumerable<Task> tasks)
    {
        var allTasks = Task.WhenAll(tasks);
        try
        {
            await allTasks;
        }
        catch (Exception)
        {
            if (allTasks.Exception is not null)
                throw allTasks.Exception.Flatten();
            throw;
        }
    }

    public static async Task WhenAll(params Task[] tasks)
    {
        var allTasks = Task.WhenAll(tasks);
        try
        {
            await allTasks;
        }
        catch(Exception)
        {
            if (allTasks.Exception is not null)
                throw allTasks.Exception.Flatten();
            throw;
        }
    }
}