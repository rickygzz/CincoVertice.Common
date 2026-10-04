using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVerticeCommon.Application.CodeChecker;
using Microsoft.Extensions.DependencyInjection;

namespace CincoVertice.Common.Application.Setup;

public static class ApplicationSetup
{
    public static IServiceCollection AddApplicationLayer(IServiceCollection services)
    {
        services.AddSingleton<ICodeCheckerService, CodeCheckerService>();

        return services;
    }
}
