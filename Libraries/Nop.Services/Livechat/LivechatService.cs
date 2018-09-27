using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Livechat;

namespace Nop.Services.Livechat
{
    public class LivechatService : ILivechatService
    {
        private readonly IStringKeyRepository<LivechatChannel> livechatChannelRepository;
        private readonly IStringKeyRepository<LivechatMessagePack> livechatMessagePackRepository;
        private readonly IStringKeyRepository<LivechatUser> livechatUserRepository;
        private readonly IStringKeyRepository<LivechatChannelUser> livechatChannelUserRepository;
        private readonly IRepository<Customer> customerRepository;
        private readonly IRepository<CustomerRole> customerRoleRepository;

        public LivechatService(
            IStringKeyRepository<LivechatChannel> livechatChannelRepository,
            IStringKeyRepository<LivechatMessagePack> livechatMessagePackRepository,
            IStringKeyRepository<LivechatUser> livechatUserRepository,
            IStringKeyRepository<LivechatChannelUser> livechatChannelUserRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository
            )
        {
            this.livechatChannelRepository = livechatChannelRepository;
            this.livechatMessagePackRepository = livechatMessagePackRepository;
            this.livechatUserRepository = livechatUserRepository;
            this.livechatChannelUserRepository = livechatChannelUserRepository;
            this.customerRepository = customerRepository;
            this.customerRoleRepository = customerRoleRepository;
        }

        public IList<LivechatMessagePack> GetChannelMessagesById(string id)
        {
            return (from messages in livechatMessagePackRepository.TableNoTracking
                   where messages.ChannelId == id
                   orderby messages.CreatedAt
                   select messages).ToList();
        }

        public IPagedList<LivechatSummarizedList> GetSummarizedConversationList(
            int storeId,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            int currentUser = 0)
        {
            var query = livechatChannelRepository.TableNoTracking
                .Include(t => t.Messages);

            var queryNoChannel = livechatUserRepository.TableNoTracking
                .Include(t => t.Channels)
                .Where(t => !t.Channels.Any() && t.CustomerId.HasValue && !string.IsNullOrEmpty(t.Phone));

            //var queryNoChannel = from user in livechatUserRepository.TableNoTracking
            //                     join customer in customerRepository.TableNoTracking
            //                        on user.CustomerId equals customer.Id into userCustomer
            //                     let firstUserCustomer = userCustomer.FirstOrDefault()
            //                     join channel in livechatChannelRepository.TableNoTracking.Include(t => t.Users)
            //                        on user.Id != null into userc
            //                     from channel in userc.DefaultIfEmpty()
            //                     where channel.Id == null
            //                     select new LivechatChannel
            //                     {
            //                         CreatedAt = user.CreatedAt,
            //                         IsFinished = true,
            //                         Messages = new List<LivechatMessagePack>(),
            //                         Name = user.Name,
            //                         Users = new List<LivechatUser>(),
            //                         StoreId = firstUserCustomer == null ? 1 : firstUserCustomer.RegisteredInStoreId,
            //                         StoreName = ""
            //                     };

            if (currentUser > 0)
                query = query.Where(t => t.Users.Any(u => u.CustomerId == currentUser));

            if (storeId > 0)
            {
                queryNoChannel = queryNoChannel.Where(t => t.Customer.RegisteredInStoreId == storeId);
                query = query.Where(t => t.StoreId == storeId);
            }

            query = query.OrderByDescending(t => t.CreatedAt);

            var summarizedComposing = query
                .Select(ch =>
                    new LivechatSummarizedList
                    {
                        CreatedAt = ch.CreatedAt,
                        CustomerName = ch.Name.Replace("()", "").Trim(),
                        ChannelId = ch.Id,
                        MessagesCount = ch.Messages.Count,
                        IsFinished = ch.IsFinished
                    }
                );


            summarizedComposing = summarizedComposing.Union(queryNoChannel.Select(t => new LivechatSummarizedList
            {
                CreatedAt = t.CreatedAt,
                CustomerName = t.Name.Replace("()", "").Trim(),
                ChannelId = "",
                MessagesCount = 0,
                IsFinished = true
            }))
            .OrderByDescending(t => t.CreatedAt);

            var channels = new PagedList<LivechatSummarizedList>(summarizedComposing, pageIndex - 1, pageSize);

            var extraData = livechatChannelRepository.TableNoTracking
                .Include(t => t.Messages)
                .Where(t => query.Any(c => c.Id == t.Id))
                .ToList();

            channels.ForEach(c =>
            {
                var channel = extraData
                    .Where(t => t.Id == c.ChannelId)
                    .FirstOrDefault();

                if (channel == null)
                {
                    c.AgentName = "Sem Agente";
                    c.LastMessage = DateTime.MinValue;
                    return;
                }

                c.AgentName = channel.Messages
                    .Where(t => t.FromName.ToLower() != c.CustomerName.ToLower() && t.FromName != "Moveleiros")
                    .Select(t => t.FromName)
                    .FirstOrDefault() ?? "Sem Agente";

                c.LastMessage = channel.Messages
                    .OrderByDescending(t => t.CreatedAt)
                    .Select(t => t.CreatedAt)
                    .FirstOrDefault();
            });

            // channels.Concat(queryNoChannel.ToList());

            return channels;
        }
    }
}
