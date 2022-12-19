using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// This is working without applying the .AddMvc()
builder.Services.AddApiVersioning(opt =>
{
    opt.AssumeDefaultVersionWhenUnspecified = false;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddMvc();
// When replacing the line above with the code below (yes also only .AddMvc) the `.well-known/example.toml` route
// is not found anymore.
// }).AddMvc().AddApiExplorer(opt => {
//     // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
//     // note: the specified format code will format the version as "'v'major[.minor]"
//     opt.GroupNameFormat = "'v'VV";
//     opt.SubstituteApiVersionInUrl = true;
// });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
// builder.Services.AddSwaggerGen()
//     .ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

// var apiVersionDescriptor = app.Services.GetService<IApiVersionDescriptionProvider>();
// if (apiVersionDescriptor == null) 
//     throw new InvalidOperationException("IApiVersionDescriptionProvider not found");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI(c =>
    // {
    //     foreach (var description in apiVersionDescriptor.ApiVersionDescriptions)
    //     {
    //         c.SwaggerEndpoint(
    //             $"/swagger/{description.GroupName}/swagger.json",
    //             $"{Assembly.GetEntryAssembly()?.GetName().Name} {description.GroupName}"
    //         );
    //     }
    //
    //     c.OAuthUsePkce();
    //     c.ConfigObject.DeepLinking = true;
    //     c.DocExpansion(DocExpansion.List);
    //     c.DisplayRequestDuration();
    // });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public class ConfigureSwaggerOptions
    : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _apiVersionDescriptor;

    public ConfigureSwaggerOptions(
        IApiVersionDescriptionProvider apiVersionDescriptor
    ) {
        _apiVersionDescriptor = apiVersionDescriptor;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // add swagger document for every API version discovered
        foreach (var description in _apiVersionDescriptor.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description)
            );
        }
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private static OpenApiInfo CreateVersionInfo(
        ApiVersionDescription description
    ) {
        var info = new OpenApiInfo
        {
            Version = description.ApiVersion.ToString(),
            Title = "Title",
            Description = @"Description.",
            TermsOfService = new Uri("https://example.com/general-terms-conditions"),
            Contact = new OpenApiContact
            {
                Name = "Support",
                Email = "support@example.com"
            }
        };

        if (!description.IsDeprecated)
            return info;

        info.Title = $"[deprecated] {info.Title}";
        info.Description = $"[deprecated] {info.Description}";

        return info;
    }
}
