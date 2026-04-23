using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Nexo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration => {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            //configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            //configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            //configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            //configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(DomainEventDispatcherBehavior<,>));
            //configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            //configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
            //configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
