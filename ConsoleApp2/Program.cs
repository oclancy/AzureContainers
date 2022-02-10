using Firmus.Protobuf.Messages;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ServiceStack.Redis;

var redisClientManager = new PooledRedisClientManager("redis:6379");

using IHost host = Host.CreateDefaultBuilder(args)
                        .ConfigureServices((_, services) => {
                            services.AddLogging(configure => configure.AddConsole())
                            .AddSingleton<IRedisClientsManager>(redisClientManager)
                            .AddTransient<Subscriber>();
                        })
                        .Build();

using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;

var subscriber = provider.GetRequiredService<Subscriber>();



try
{
    subscriber.Subscribe("people");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}





await host.RunAsync();
