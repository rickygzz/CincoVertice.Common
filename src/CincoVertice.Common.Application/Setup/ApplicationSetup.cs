using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CincoVertice.Common.Application.Setup;

public static class ApplicationSetup
{
    public static IServiceCollection AddApplicationLayer(IServiceCollection services)
    {
        services.AddSingleton<ILineCheckerService, LineCheckerService>();
        services.AddSingleton<ICodeCheckerService, CodeCheckerService>();

        return services;
    }
}
