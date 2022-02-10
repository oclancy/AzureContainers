using Google.Protobuf;
using ServiceStack.Redis;
using Microsoft.Extensions.Logging;
using Google.Protobuf.WellKnownTypes;

namespace Firmus.Protobuf.Messages
{
    public class Publisher
    {
        public Publisher(ILogger<Publisher> logger, 
                         IRedisClientsManager redisClientManager)
        {
            Logger = logger;
            RedisClientManager = redisClientManager;
            Formatter = new JsonFormatter(
                            new JsonFormatter.Settings(true, ProtobufTypeRegistry.TypeRegistry));
        }

        public ILogger<Publisher> Logger { get; }
        public IRedisClientsManager RedisClientManager { get; }
        public JsonFormatter Formatter { get; }

        public void Publish(IMessage messageToPublish, string channel)
        {
            Logger.LogInformation($"Publishing {messageToPublish.Descriptor.FullName}");

            var message = Any.Pack(messageToPublish);
            var encoded = Formatter.Format(message);

            using var client = RedisClientManager.GetClient();
            client.PublishMessage(channel, encoded);
        }
    }

    public class Subscriber
    {
        public ILogger<Subscriber> Logger { get; }
        public IRedisClientsManager RedisClientManager { get; }
        private JsonParser Parser { get; }
        public Subscriber(ILogger<Subscriber> logger,
                          IRedisClientsManager redisClientManager)
        {
            Logger = logger;
            RedisClientManager = redisClientManager;
            Parser = new JsonParser(
                new JsonParser.Settings(JsonParser.Settings.Default.RecursionLimit, 
                                        ProtobufTypeRegistry.TypeRegistry));
        }

        public IRedisPubSubServer? RedisClient { get; set;}

        public void Subscribe(string channel)
        {
            RedisClient = new RedisPubSubServer(RedisClientManager, channel);
            RedisClient.OnMessage += RedisClient_OnMessage;
            RedisClient.Start();
        }

        private void RedisClient_OnSubscribeAsync(string message)
        {
            Logger.LogInformation($"Subscribed {message} ");
        }

        private void RedisClient_OnMessage(string channel, string encodedMessage)
        {
            var any = Parser.Parse<Any>(encodedMessage);

            if (any.Is(Person.Descriptor))
            {
                var person = any.Unpack<Person>();
                Logger.LogInformation($"Recieved {person.Name} on {channel}");
            }

        }
    }
}