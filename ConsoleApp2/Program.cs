using Firmus.Protobuf.Messages;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ServiceStack.Redis;
using Serilog;
using Serilog.Events;
using Autofac.Extensions.DependencyInjection;
using Polly;

var redisClientManager = new PooledRedisClientManager("redis:6379");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{

    using IHost host = Host.CreateDefaultBuilder(args)
                        .UseSerilog()
                        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                        .ConfigureServices((_, services) =>
                        {
                            services
                            .AddSingleton<IRedisClientsManager>(redisClientManager)
                            .AddTransient<Subscriber>();
                        })
                        .Build();

    using IServiceScope serviceScope = host.Services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var subscriber = provider.GetRequiredService<Subscriber>();

    await Policy
              .Handle<HttpRequestException>()
              .RetryAsync()
              .ExecuteAndCaptureAsync(() => {
                  subscriber.Subscribe("people");
                  return host.RunAsync();
              });
    
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}