using Microsoft.Extensions.DependencyInjection;

namespace CincoVertice.Common.Application.Calculator.Setup;

public static class CalculatorSetup
{
    public static IServiceCollection AddApplcationCalculatorLayer(IServiceCollection services)
    {
        services.AddSingleton<ExpressionCalculator>();

        return services;
    }
}
