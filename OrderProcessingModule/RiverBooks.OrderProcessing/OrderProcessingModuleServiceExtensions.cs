﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RiverBooks.OrderProcessing;

public static class OrderProcessingModuleServiceExtensions
{
    public static IServiceCollection AddOrderProcessingModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    List<System.Reflection.Assembly> mediatRAssemblies)
    {
        string? connectionString = config.GetConnectionString("OrderProcessingConnectionString");
        services.AddDbContext<OrderProcessingDbContext>(config => config.UseSqlServer(connectionString));

        // Add Serives
        services.AddScoped<IOrderRepository, EfOrderRepository>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(OrderProcessingModuleServiceExtensions).Assembly);

        logger.Information("{Module} module services registered", "OrderProcessing");

        return services;
    }
}
