namespace FuncAsyncTest
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var integerList = new[] { 1, 2, 3 };

            var listOfTasks = integerList
                .Select(x =>
                    new KeyValuePair<int, Task<string>>
                        (x, StringConvertAsync(x))
                )
                .ToList();

            var results = await listOfTasks.FlattenTasksAsync((key, taskResult) => new { Key = key, Result = taskResult });

            foreach (var result in results)
            {
                Console.WriteLine($"Key: {result.Key}, Result: {result.Result}");
            }
        }

        public static Task<string> StringConvertAsync(int number) => Task.FromResult($"Result {number}");

    }

    public static class TaskExtensionsLast
    {
        public static async Task<List<TResult>> FlattenTasksAsync<TKey, TTaskResult, TResult>
            (
            this IEnumerable<KeyValuePair<TKey, Task<TTaskResult>>> taskList,
            Func<TKey, TTaskResult, TResult> resultSelector
            )
        {
            await Task.WhenAll(taskList.Select(kv => kv.Value));

            return taskList.Select(kv =>
                   resultSelector(kv.Key, kv.Value.Result)).ToList();
        }
    }
}