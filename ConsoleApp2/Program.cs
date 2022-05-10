
using Autofac.Extensions.DependencyInjection;

using Firmus.Common;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Serilog;
using Serilog.Events;

using ServiceStack.Redis;

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
                        .ConfigureServices((context, services) =>
                        {
                            var configurationRoot = context.Configuration;

                            services
                            .AddSingleton<IRedisClientsManager, PooledRedisClientManager>( (sp) =>
                            {
                                var options = sp.GetService<IOptions<RedisOptions>>()!.Value;
                                Log.Information(options.ToString());
                                return new ($"{options.IpAddress}:{options.Port}");
                            })
                            .Configure<RedisOptions>(
                                configurationRoot.GetSection(nameof(RedisOptions)))
                            .AddHostedService<Subscriber>();
                        })
                        .Build();

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}