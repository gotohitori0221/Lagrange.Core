using System.CommandLine;
using Lagrange.Proto.CodeGen.Commands;

namespace Lagrange.Proto.CodeGen;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var filesOptions = new Option<string[]>("--files", "The proto files to generate code for.")
        {
            IsRequired = false,
            AllowMultipleArgumentsPerToken = true,
        };
        
        var folderOption = new Option<string>("--folder", "The folder to generate code in.")
        {
            IsRequired = false
        };
        
        var recursiveOption = new Option<bool>("--recursive", () => false, "Whether to search for proto files recursively.")
        {
            IsRequired = false
        };
        recursiveOption.AddAlias("-r");

        var fileOption = new Option<string>("--file", "The managed dll file to dump proto files from.")
        {
            IsRequired = true
        };
        
        var dumpCommand = new Command("dump", "Dump the proto files to the specified folder from managed dll file")
        {
            fileOption
        };
        dumpCommand.SetHandler(DumpCommand.Invoke, fileOption);

        var generateCommand = new Command("generate", "Generate the proto files to the specified folder")
        {
            filesOptions, folderOption, recursiveOption
        };
        generateCommand.SetHandler(GenerateCommand.Invoke, filesOptions, folderOption, recursiveOption);
        
        var rootCommand = new RootCommand("Lagrange Proto Code Generator")
        {
            dumpCommand,
            generateCommand
        };

        await rootCommand.InvokeAsync(args);
    }
}