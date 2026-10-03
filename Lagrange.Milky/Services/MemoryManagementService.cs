using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lagrange.Milky.Services;

public class MemoryManagementService(ILogger<MemoryManagementService> logger) : BackgroundService
{
    private readonly ILogger<MemoryManagementService> _logger = logger;

    [DllImport("psapi.dll", EntryPoint = "EmptyWorkingSet")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EmptyWorkingSet(IntPtr hProcess);

    public static void TrimMemory()
    {
        try
        {
            GC.Collect(2, GCCollectionMode.Aggressive, blocking: false, compacting: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(2, GCCollectionMode.Aggressive, blocking: false, compacting: true);

            if (OperatingSystem.IsWindows())
            {
                using var process = Process.GetCurrentProcess();
                _ = EmptyWorkingSet(process.Handle);
            }
        }
        catch
        {
            // ignored
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            TrimMemory();
            _logger.LogDebug("Initial memory trim completed.");
        }
        catch (OperationCanceledException)
        {
            return;
        }

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                TrimMemory();
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
