using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CincoVertice.Common.Application.Setup;

public static class ApplicationSetup
{
    public static IServiceCollection AddApplicationLayer(IServiceCollection services)
    {
        // C# and JavaScript, each file by its extension
        services.AddSingleton<ICodeCheckerService>(_ => new CodeCheckerService(CodeLanguageModel.All));
        services.AddSingleton<ICodeFixerService>(_ => new CodeFixerService(CodeLanguageModel.All));

        return services;
    }
}
