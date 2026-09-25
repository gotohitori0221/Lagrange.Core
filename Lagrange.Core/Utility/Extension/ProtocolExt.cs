using System.Runtime.CompilerServices;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Message;

namespace Lagrange.Core.Utility.Extension;

internal static class ProtocolExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAndroid(this Protocols protocol) => (protocol & ~Protocols.Android) == Protocols.None;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPC(this Protocols protocol) => (protocol & ~Protocols.PC) == Protocols.None;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGroup(this BotMessage message) => message.Contact is BotGroupMember;
}