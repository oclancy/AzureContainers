using Google.Protobuf;
using ServiceStack.Redis;
using Microsoft.Extensions.Logging;
using Google.Protobuf.WellKnownTypes;
using Firmus.Protobuf.Messages;
using Microsoft.Extensions.Hosting;
using Polly;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Firmus.Common
{
    public class Publisher : IHostedService
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Starting at {SystemClock.Instance.GetCurrentInstant().InUtc()}");

            return Task.Run(async () =>
           {
               while (!cancellationToken.IsCancellationRequested)
               {
                   cancellationToken.ThrowIfCancellationRequested();
                   try
                   {
                       var person = new Person();
                       person.Name = "Ollie";

                       Policy
                           .Handle<Exception>()
                           .Retry()
                           .Execute(() => Publish(person, "people"));
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine(ex.Message);
                   }

                   await Task.Delay(1000);
               }
           }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Shutting down at {SystemClock.Instance.GetCurrentInstant().InUtc()}");
            return Task.CompletedTask;
        }
    }

    public class Subscriber : IHostedService
    {
        public ILogger<Subscriber> Logger { get; }
        public IRedisClientsManager RedisClientManager { get; }
        private JsonParser Parser { get; }
        public TaskCompletionSource<object> SubscriberTask { get; }

        public Subscriber(ILogger<Subscriber> logger,
                          IRedisClientsManager redisClientManager)
        {
            Logger = logger;
            RedisClientManager = redisClientManager;

            Parser = new JsonParser(
                new JsonParser.Settings(JsonParser.Settings.Default.RecursionLimit,
                                        ProtobufTypeRegistry.TypeRegistry));

            SubscriberTask = new TaskCompletionSource<object>();
        }

        public IRedisPubSubServer? RedisClient { get; set; }

        public IRedisPubSubServer Subscribe(string channel)
        {
            RedisClient = new RedisPubSubServer(RedisClientManager, channel);
            RedisClient.OnMessage += RedisClient_OnMessage;
            return RedisClient;
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Starting at {SystemClock.Instance.GetCurrentInstant().InUtc()}");

            Policy
                 .Handle<HttpRequestException>()
                 .RetryAsync()
                 .ExecuteAndCaptureAsync(() =>
                {
                    Subscribe("people")
                        .Start();

                    return Task.CompletedTask;
                });

            return SubscriberTask.Task;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Shutting down at {SystemClock.Instance.GetCurrentInstant().InUtc()}");
            SubscriberTask.SetResult(0);
            return SubscriberTask.Task;
        }
    }
}