using System;
using Contentful.Core.Errors;
using System.Text.Json;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Peereflits.Shared.Contentful.ModelsGenerator.Cli;

public class Program
{
    public const int Exception = 2;
    public const int Error = 1;
    public const int Ok = 0;

    static async Task<int> Main(string[] args)
    {
        try
        {
            return await CommandLineApplication.ExecuteAsync<ModelsGenerator>(args);
        }
        catch (ContentfulException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            await Console.Error.WriteLineAsync("There was an error communicating with the Contentful API: " +
                                               ex.GetType().Name);
            await Console.Error.WriteLineAsync(ex.Message);
            await Console.Error.WriteLineAsync();
            await Console.Error.WriteLineAsync($"Status code: {ex.StatusCode}");
            await Console.Error.WriteLineAsync($"Request ID: {ex.RequestId}");

            if (ex.ErrorDetails != null)
            {
                await Console.Error.WriteLineAsync();
                await Console.Error.WriteLineAsync("Errors:");
                await Console.Error.WriteLineAsync(JsonSerializer.Serialize(ex.ErrorDetails.Errors));
            }

            await Console.Error.WriteLineAsync();
            await Console.Error.WriteLineAsync("Please verify that your api key, space id and environment are correct.");
            await Console.Error.WriteLineAsync();
            Console.ResetColor();
            return Program.Error;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            await Console.Error.WriteLineAsync("Unexpected error: " + ex.GetType().Name);
            await Console.Error.WriteLineAsync(ex.Message);
            await Console.Error.WriteLineAsync();
            await Console.Error.WriteLineAsync(ex.StackTrace);
            Console.ResetColor();
            return Exception;
        }
    }
}