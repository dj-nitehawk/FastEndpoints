﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors.Security;

namespace FastEndpoints.NSwag;

/// <summary>
/// a set of extension methods for adding swagger support
/// </summary>
public static class Extensions
{
    //key: stripped route
    private static readonly Dictionary<string, (int ver, object op)> ops = new();

    /// <summary>
    /// enable support for FastEndpoints in swagger
    /// </summary>
    /// <param name="tagIndex">the index of the route path segment to use for tagging/grouping endpoints</param>
    public static void EnableFastEndpoints(this AspNetCoreOpenApiDocumentGeneratorSettings settings, int tagIndex, int maxEndpointVersion)
    {
        settings.Title = AppDomain.CurrentDomain.FriendlyName;
        settings.SchemaNameGenerator = new DefaultSchemaNameGenerator();
        settings.OperationProcessors.Add(new DefaultOperationProcessor(tagIndex));
        settings.DocumentProcessors.Add(new DocProcessor());
        settings.PostProcess = doc =>
        {
            var xxx = doc;
        };
    }

    /// <summary>
    /// enable jwt bearer authorization support
    /// </summary>
    public static void EnableJWTBearerAuth(this AspNetCoreOpenApiDocumentGeneratorSettings settings)
    {
        settings.AddSecurity("JWTBearerAuth", new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter a JWT token to authorize the requests..."
        });

        settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWTBearerAuth"));
    }

    /// <summary>
    /// enable swagger support for FastEndpoints with a single call.
    /// </summary>
    /// <param name="settings">swaggergen config settings</param>
    /// <param name="serializerOptions">json serializer options</param>
    /// <param name="addJWTBearerAuth">set to false to disable auto addition of jwt bearer auth support</param>
    /// <param name="tagIndex">the index of the route path segment to use for tagging/grouping endpoints</param>
    public static IServiceCollection AddNSwag(this IServiceCollection services,
        Action<AspNetCoreOpenApiDocumentGeneratorSettings>? settings = null,
        Action<JsonOptions>? serializerOptions = null,
        bool addJWTBearerAuth = true,
        int tagIndex = 1,
        int maxEndpointVersion = 0)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(s =>
        {
            s.EnableFastEndpoints(tagIndex, maxEndpointVersion);
            if (addJWTBearerAuth) s.EnableJWTBearerAuth();
            settings?.Invoke(s);
        });

        if (serializerOptions is not null)
            services.AddMvcCore().AddJsonOptions(serializerOptions);
        else
            services.AddMvcCore().AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

        return services;
    }

    /// <summary>
    /// configure swagger ui with some sensible defaults for FastEndpoints which can be overridden if needed.
    /// </summary>
    public static void ConfigureDefaults(this SwaggerUi3Settings s)
    {
        s.AdditionalSettings["filter"] = true;
        s.AdditionalSettings["persistAuthorization"] = true;
        s.AdditionalSettings["displayRequestDuration"] = true;
        s.AdditionalSettings["tryItOutEnabled"] = true;
        s.TagsSorter = "alpha";
        s.OperationsSorter = "alpha";
        s.CustomInlineStyles = ".servers-title,.servers{display:none} .swagger-ui .info{margin:10px 0} .swagger-ui .scheme-container{margin:10px 0;padding:10px 0} .swagger-ui .info .title{font-size:25px} .swagger-ui textarea{min-height:150px}";
    }

    internal static string Remove(this string value, string removeString)
    {
        int index = value.IndexOf(removeString, StringComparison.Ordinal);
        return index < 0 ? value : value.Remove(index, removeString.Length);
    }
}

internal class DocProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var yyy = context;
    }
}