using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Lagrange.Core.Events.EventArgs;

namespace Lagrange.Core.Events;

public sealed class EventInvoker(BotContext context) : IDisposable
{
    private const string Tag = nameof(EventInvoker);

    private static readonly ConcurrentDictionary<Delegate, Delegate> SyncHandlerCache = new();
    private static readonly ConcurrentDictionary<Delegate, Delegate> AsyncHandlerCache = new();

    private readonly ConcurrentDictionary<Type, Delegate?> _syncHandlers = new();
    private readonly ConcurrentDictionary<Type, Delegate?> _asyncHandlers = new();

    public delegate void  LagrangeEventHandler <in TEvent>(BotContext ctx, TEvent e) where TEvent : EventBase;
    public delegate Task  LagrangeAsyncEventHandler<in TEvent>(BotContext ctx, TEvent e) where TEvent : EventBase;

    public void RegisterEvent<TEvent>(LagrangeEventHandler<TEvent> handler) where TEvent : EventBase
    {
        Debug.Assert(handler.Method.IsStatic is false);
        
        var wrappedHandler = BuildSyncDelegate(handler);
        AddHandler(_syncHandlers, typeof(TEvent), wrappedHandler);
    }

    public void RegisterEvent<TEvent>(LagrangeAsyncEventHandler<TEvent> handler) where TEvent : EventBase
    {
        Debug.Assert(handler.Method.IsStatic is false);
        
        var wrappedHandler = BuildAsyncDelegate(handler);
        AddHandler(_asyncHandlers, typeof(TEvent), wrappedHandler);
    }

    public void UnregisterEvent<TEvent>(LagrangeEventHandler<TEvent> handler) where TEvent : EventBase
    {
        var wrappedHandler = BuildSyncDelegate(handler);
        RemoveHandler(_syncHandlers, typeof(TEvent), wrappedHandler);
    }

    public void UnregisterEvent<TEvent>(LagrangeAsyncEventHandler<TEvent> handler) where TEvent : EventBase
    {
        var wrappedHandler = BuildAsyncDelegate(handler);
        RemoveHandler(_asyncHandlers, typeof(TEvent), wrappedHandler);
    }
    
    public void UnregisterEvent<TEvent>() where TEvent : EventBase
    {
        _syncHandlers.TryRemove(typeof(TEvent), out _);
        _asyncHandlers.TryRemove(typeof(TEvent), out _);
    }

    internal void PostEvent<T>(T ev) where T : EventBase => Task.Run(async () =>
    {
        await context.EventContext.HandleOutgoingEvent(ev);

        try
        {
            if (_syncHandlers.TryGetValue(typeof(T), out var @delegate) && @delegate is not null)
            {
                ((Action<BotContext, EventBase>)@delegate).Invoke(context, ev);
            }

            if (_asyncHandlers.TryGetValue(typeof(T), out var asyncDelegate) && asyncDelegate is not null)
            {
                await ((Func<BotContext, EventBase, Task>)asyncDelegate)(context, ev);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            if (typeof(T) == typeof(BotLogEvent))
            {
                Console.WriteLine($"Failed to post event: {ev}");
                return;
            }
            PostEvent(new BotLogEvent(Tag, LogLevel.Error, ex.ToString(), ex));
        }
    });

    public void Dispose()
    {
        _syncHandlers.Clear();
        _asyncHandlers.Clear();
    }

    private static void AddHandler(ConcurrentDictionary<Type, Delegate?> dict, Type key, Delegate handler)
    {
        dict.AddOrUpdate(key, handler, (_, old) => Delegate.Combine(old, handler));
    }

    private static void RemoveHandler(ConcurrentDictionary<Type, Delegate?> dict, Type key, Delegate handler)
    {
        dict.AddOrUpdate(key, _ => null, (_, old) => Delegate.Remove(old, handler)!);
        
        if (dict.TryGetValue(key, out var currentDelegate) && currentDelegate == null)
        {
            dict.TryRemove(key, out _);
        }
    }

    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "DynamicMethod is not supported in AOT, but this branch is skipped there")]
    private static Delegate BuildSyncDelegate<TEvent>(LagrangeEventHandler<TEvent> h) where TEvent : EventBase
    {
        return SyncHandlerCache.GetOrAdd(h, handler =>
        {
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                return (Action<BotContext, EventBase>)((ctx, ev) => ((LagrangeEventHandler<TEvent>)handler)(ctx, (TEvent)ev));
            }

            var dm = new DynamicMethod($"EventInvoker_{typeof(TEvent).Name}_{Guid.NewGuid()}", typeof(void), [handler.Target!.GetType(), typeof(BotContext), typeof(EventBase)], typeof(EventInvoker), true);
            var il = dm.GetILGenerator();
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Castclass, typeof(TEvent));
            il.Emit(OpCodes.Callvirt, handler.Method);
            il.Emit(OpCodes.Ret);

            return dm.CreateDelegate<Action<BotContext, EventBase>>(handler.Target);
        });
    }

    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "DynamicMethod is not supported in AOT, but this branch is skipped there")]
    private static Delegate BuildAsyncDelegate<TEvent>(LagrangeAsyncEventHandler<TEvent> h) where TEvent : EventBase
    {
        return AsyncHandlerCache.GetOrAdd(h, handler =>
        {
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                return (Func<BotContext, EventBase, Task>)((ctx, ev) => ((LagrangeAsyncEventHandler<TEvent>)handler)(ctx, (TEvent)ev));
            }

            var dm = new DynamicMethod($"EventInvoker_{typeof(TEvent).Name}_Async_{Guid.NewGuid()}", typeof(Task), [handler.Target!.GetType(), typeof(BotContext), typeof(EventBase)], typeof(EventInvoker), true);
            var il = dm.GetILGenerator();
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Castclass, typeof(TEvent));
            il.Emit(OpCodes.Callvirt, handler.Method);
            il.Emit(OpCodes.Ret);

            return dm.CreateDelegate<Func<BotContext, EventBase, Task>>(handler.Target);
        });
    }
}