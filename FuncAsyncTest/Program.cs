namespace FuncAsyncTest
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var integerList = new[] { 1, 2, 3 };

            var listOfTasks =  new List<(int,Task<string>)>();

            listOfTasks.AddRange(
                integerList
                    .Select(x => (x, StringConvertAsync(x)))
                    .ToList()
                );

            var results = await listOfTasks
                .FlattenTasksAsync(
                    (key, taskResult) => new { Key = key, Result = taskResult }
                    );

            foreach (var result in results)
            {
                Console.WriteLine($"Key: {result.Key}, Result: {result.Result}");
            }
        }

        public static Task<string> StringConvertAsync(int number) => Task.FromResult($"Result {number}");

    }

    public static class TaskExtensionsLast
    {
        public static async Task<IEnumerable<TResult>> FlattenTasksAsync<TKey, TTaskResult, TResult>
            (
            this IEnumerable<(TKey, Task<TTaskResult>)> taskList,
            Func<TKey, TTaskResult, TResult> resultSelector
            )
        {
            await Task.WhenAll(taskList.Select(kv => kv.Item2));

            return taskList.Select(kv =>
                   resultSelector(kv.Item1, kv.Item2.Result));
        }
    }
}