// See https://aka.ms/new-console-template for more information
using Autofac.Extensions.DependencyInjection;

using Firmus.Protobuf.Messages;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Polly;

using Serilog;

using ServiceStack.Redis;

var redisClientManager = new PooledRedisClientManager("redis:6379");

using IHost host = Host.CreateDefaultBuilder(args)
                        .UseSerilog()
                        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                        .ConfigureServices((_, services) =>
                        {
                            services
                            .AddSingleton<IRedisClientsManager>(redisClientManager)
                            .AddTransient<Publisher>();
                        })
                        .Build();

using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;


var publisher = provider.GetRequiredService<Publisher>();


while (true)
{
    try
    {
        var person = new Person();
        person.Name = "Ollie";

        Policy
            .Handle<Exception>()
            .Retry()
            .Execute(() => publisher.Publish(person, "people"));
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    await Task.Delay(1000);
}


await host.RunAsync();



