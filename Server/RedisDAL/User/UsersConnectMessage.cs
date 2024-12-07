﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDAL.User
{
    public class UsersConnectMessage
    {
        private readonly IDatabase _db;
        private readonly ISubscriber _subscriber;

        public UsersConnectMessage(RedisConfigure redisConfigure)
        {
            _db = redisConfigure.GetDatabase();
            _subscriber = redisConfigure.GetSubscriber();
            /*_subscriber.Subscribe("__keyevent@0__:expired", (channel, message) =>
            {
                NotifyServer(message);
            });*/
        }

        public async Task UpdateUserConnection(string userId, string connectionId)
        {
            var key = $"UsersConnect:{userId}";
            var fields = new HashEntry[]
            {
                new HashEntry("connectionId", connectionId),
                new HashEntry("lastPingTime", DateTime.UtcNow.ToString())
            };
            await _db.HashSetAsync(key, fields);
            await _db.KeyExpireAsync(key, TimeSpan.FromSeconds(5));
        }

        public async Task<HashEntry[]> GetUserConnection(string userId)
        {
            var key = $"UsersConnect:{userId}";
            return await _db.HashGetAllAsync(key);
        }


        public async Task RemoveUser(string userId)
        {
            var key = $"UsersConnect:{userId}";
            await _db.KeyDeleteAsync(key);
        }


        public async Task<DateTime?> GetLastPingTime(string userId)
        {
            var key = $"UsersConnect:{userId}";
            var lastPing = await _db.HashGetAsync(key, "lastPingTime");
            return lastPing.HasValue ? DateTime.Parse(lastPing) : null;
        }


        public async Task<IEnumerable<string>> GetAllUsers()
        {
            var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints()[0]);
            return server.Keys(pattern: "UsersConnect:*").Select(k => k.ToString());
        }

        /*private async Task NotifyServer(string userId)
        {
            
        }

        public void SubscribeToMessages()
        {
            _subscriber.Subscribe("user_notifications", (channel, message) =>
            {
                HandleNotification(message.ToString());
            });
        }

        private string HandleNotification(string userId)
        {
            return userId;
        }*/

    }
}