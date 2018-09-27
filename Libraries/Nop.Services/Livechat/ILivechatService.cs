using Nop.Core;
using Nop.Core.Domain.Livechat;
using System.Collections.Generic;

namespace Nop.Services.Livechat
{
    public interface ILivechatService
    {
        IPagedList<LivechatSummarizedList> GetSummarizedConversationList(
            int storeId,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            int currentUser = 0
        );

        IList<LivechatMessagePack> GetChannelMessagesById(string id);
    }
}
