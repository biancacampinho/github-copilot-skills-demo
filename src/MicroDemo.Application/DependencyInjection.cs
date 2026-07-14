using System.Reflection;
using FluentValidation;
using MediatR;
using MicroDemo.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace MicroDemo.Application;

/// <summary>
/// Registration of the Application-layer services (MediatR, FluentValidation, behaviors).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        // Order matters: logging on the outside, validation on the inside.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
