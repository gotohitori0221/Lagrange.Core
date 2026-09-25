using System.Text;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    public struct BotCaptchaEventStruct : IEventStruct
    {
        public BotCaptchaEventStruct() { }

        public ByteArrayNative CaptchaUrl = new();

        public static implicit operator BotCaptchaEventStruct(BotCaptchaEvent e)
        {
            return new BotCaptchaEventStruct()
            {
                CaptchaUrl = Encoding.UTF8.GetBytes(e.CaptchaUrl)
            };
        }
        
        public static implicit operator BotCaptchaEvent(BotCaptchaEventStruct e)
        {
            return new BotCaptchaEvent(Encoding.UTF8.GetString(e.CaptchaUrl));
        }
    }
}
