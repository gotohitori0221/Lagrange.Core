using System.Text;
using Lagrange.Core.Common.Entity;

namespace Lagrange.Core.NativeAPI.NativeModel.Common
{
    public struct BotFriendCategoryStruct
    {
        public BotFriendCategoryStruct() { }

        public int Id = 0;

        public ByteArrayNative Name = new();

        public int Count = 0;

        public int SortId = 0;
        
        public static implicit operator BotFriendCategoryStruct(BotFriendCategory category)
        {
            return new BotFriendCategoryStruct()
            {
                Id = category.Id,
                Name = Encoding.UTF8.GetBytes(category.Name),
                Count = category.Count,
                SortId = category.SortId
            };
        }
        
        public static implicit operator BotFriendCategory(BotFriendCategoryStruct category)
        {
            return new BotFriendCategory(
                category.Id,
                Encoding.UTF8.GetString(category.Name),
                category.Count,
                category.SortId
            );
        }
    }
}
