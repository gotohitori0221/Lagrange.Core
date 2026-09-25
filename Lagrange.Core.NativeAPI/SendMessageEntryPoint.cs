using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Message;
using Lagrange.Core.NativeAPI.NativeModel.Common;
using Lagrange.Core.NativeAPI.NativeModel.Message;

namespace Lagrange.Core.NativeAPI
{
    public static class SendMessageEntryPoint
    {
        [UnmanagedCallersOnly(EntryPoint = "CreateMessageBuilder")]
        public static int CreateMessageBuilder(int index)
        {
            if (Program.Contexts.Count <= index)
            {
                return 0;
            }

            var context = Program.Contexts[index];
            return context.SendMessageContext.CreateMessageBuilder();
        }

        [UnmanagedCallersOnly(EntryPoint = "AddText")]
        public static void AddText(int index, int id, ByteArrayNative byteArrayNative)
        {
            if (Program.Contexts.Count <= index)
            {
                return;
            }

            var context = Program.Contexts[index];
            context.SendMessageContext.AddText(id, byteArrayNative.ToByteArrayWithoutFree());
        }

        
        [UnmanagedCallersOnly(EntryPoint = "AddImage")]
        public static void AddImage(
            int index,
            int id,
            ByteArrayNative byteArrayNative,
            ByteArrayNative summary,
            int subType
        )
        {
            if (Program.Contexts.Count <= index)
            {
                return;
            }

            byte[]? sum = summary.IsEmpty() ? null : summary.ToByteArrayWithoutFree();

            var context = Program.Contexts[index];
            context.SendMessageContext.AddImage(
                id,
                byteArrayNative.ToByteArrayWithoutFree(),
                sum,
                subType
            );
        }

        
        [UnmanagedCallersOnly(EntryPoint = "AddLocalImage")]
        public static void AddLocalImage(
            int index,
            int id,
            ByteArrayNative byteArrayNative,
            ByteArrayNative summary,
            int subType
        )
        {
            if (Program.Contexts.Count <= index)
            {
                return;
            }

            byte[]? sum = summary.IsEmpty() ? null : summary.ToByteArrayWithoutFree();

            var context = Program.Contexts[index];
            context.SendMessageContext.AddLocalImage(
                id,
                byteArrayNative.ToByteArrayWithoutFree(),
                sum,
                subType
            );
        }

        
        [UnmanagedCallersOnly(EntryPoint = "AddMention")]
        public static void AddMention(
            int index,
            int id,
            long uin,
            ByteArrayNative display
        )
        {
            if (Program.Contexts.Count <= index)
            {
                return;
            }

            byte[]? ds = display.IsEmpty() ? null : display.ToByteArrayWithoutFree();

            var context = Program.Contexts[index];
            context.SendMessageContext.AddMention(
                id, uin, ds
            );
        }

        [UnmanagedCallersOnly(EntryPoint = "AddMultiMsg")]
        public static void AddMultiMsg(
            int index,
            int id,
            ByteArrayNative resId
        )
        {
            if (Program.Contexts.Count <= index)
            {
                return;
            }

            var context = Program.Contexts[index];
            context.SendMessageContext.AddMultiMsg(
                id, resId.ToByteArrayWithoutFree()
            );
        }

        [UnmanagedCallersOnly(EntryPoint = "AddRecord")]
        public static void AddRecord(int index, int id, ByteArrayNative byteArrayNative)
        {
            if (Program.Contexts.Count <= index)
            {
                return;
            }

            var context = Program.Contexts[index];
            context.SendMessageContext.AddRecord(id, byteArrayNative.ToByteArrayWithoutFree());
        }

        [UnmanagedCallersOnly(EntryPoint = "AddLocalRecord")]
        public static void AddLocalRecord(int index, int id, ByteArrayNative byteArrayNative)
        {
            if (Program.Contexts.Count <= index)
            {
                return;
            }

            var context = Program.Contexts[index];
            context.SendMessageContext.AddLocalRecord(
                id,
                byteArrayNative.ToByteArrayWithoutFree()
            );
        }

        [UnmanagedCallersOnly(EntryPoint = "AddVideo")]
        public static void AddVideo(
            int index,
            int id,
            ByteArrayNative byteArrayNative,
            ByteArrayNative thumbnail
        )
        {
            if (Program.Contexts.Count <= index)
            {
                return;
            }

            byte[]? thumb = thumbnail.IsEmpty() ? null : thumbnail.ToByteArrayWithoutFree();

            var context = Program.Contexts[index];
            context.SendMessageContext.AddVideo(
                id,
                byteArrayNative.ToByteArrayWithoutFree(),
                thumb
            );
        }

        [UnmanagedCallersOnly(EntryPoint = "AddLocalVideo")]
        public static void AddLocalVideo(
            int index,
            int id,
            ByteArrayNative byteArrayNative,
            ByteArrayNative thumbnail
        )
        {
            if (Program.Contexts.Count <= index)
            {
                return;
            }

            byte[]? thumb = thumbnail.IsEmpty() ? null : thumbnail.ToByteArrayWithoutFree();

            var context = Program.Contexts[index];
            context.SendMessageContext.AddLocalVideo(
                id,
                byteArrayNative.ToByteArrayWithoutFree(),
                thumb
            );
        }

        [UnmanagedCallersOnly(EntryPoint = "SendFriendMessage")]
        public static IntPtr SendFriendMessage(int index, int id, long friendUin)
        {
            if (Program.Contexts.Count <= index)
            {
                return IntPtr.Zero;
            }

            var context = Program.Contexts[index];
            var chain = context.SendMessageContext.Build(id);
            if (chain == null)
            {
                return IntPtr.Zero;
            }

            try
            {
                var message = context.BotContext.SendFriendMessage(friendUin, chain).GetAwaiter().GetResult();

                IntPtr messagePtr = Marshal.AllocHGlobal(Marshal.SizeOf<BotMessageStruct>());
                Marshal.StructureToPtr((BotMessageStruct)message, messagePtr, false);
                return messagePtr;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "SendGroupMessage")]
        public static IntPtr SendGroupMessage(int index, int id, long groupUin)
        {
            if (Program.Contexts.Count <= index)
            {
                return IntPtr.Zero;
            }

            var context = Program.Contexts[index];
            var chain = context.SendMessageContext.Build(id);
            if (chain == null)
            {
                return IntPtr.Zero;
            }

            try
            {
                var message = context.BotContext.SendGroupMessage(groupUin, chain).GetAwaiter().GetResult();

                IntPtr messagePtr = Marshal.AllocHGlobal(Marshal.SizeOf<BotMessageStruct>());
                Marshal.StructureToPtr((BotMessageStruct)message, messagePtr, false);
                return messagePtr;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "RecallMessage")]
        public static void RecallMessage(int index, BotMessageStruct message)
        {
            if (Program.Contexts.Count <= index)
            {
                return;
            }

            var context = Program.Contexts[index];
            var message1 = (BotMessage)RuntimeHelpers.GetUninitializedObject(typeof(BotMessage));
            message1.Sequence = message.Sequence;
            message1.GetClientSequence() = message.ClientSequence;

            if ((MessageType)message.Type == MessageType.Group)
            {
                BotGroupMemberStruct member = new();
                Marshal.PtrToStructure(
                   message.Contact,
                   member
               );
                message1.GetContact() = (BotGroupMember)member;
                context.BotContext.RecallMessage(message1);
                return;
            }

            BotFriendStruct contact = new();
            BotFriendStruct receiver = new();
            Marshal.PtrToStructure(message.Contact, contact);
            Marshal.PtrToStructure(message.Receiver, receiver);
            message1.GetContact() = (BotFriend)contact;
            message1.GetReceiver() = (BotFriend)receiver;
            context.BotContext.RecallMessage(message1);
        }
    }
}
