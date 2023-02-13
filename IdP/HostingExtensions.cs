using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using IdP;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.Saml.Configuration;
using Serilog;

namespace IdP;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        var isBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
                options.KeyManagement.Enabled = true;
                options.KeyManagement.SigningAlgorithms = new[]
                {
                    new SigningAlgorithmOptions("RS256") {UseX509Certificate = true}
                };

            })
            .AddTestUsers(TestUsers.Users)
            .AddSamlPlugin(options =>
            {
                options.Licensee = "DEMO";
                options.LicenseKey =
                    "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjMtMDMtMDlUMDE6MDA6MDEuMDM1MDAyNyswMDowMCIsImlhdCI6IjIwMjMtMDItMDdUMDE6MDA6MDEiLCJvcmciOiJERU1PIiwiYXVkIjoyfQ==.CH6Q42T5O7kfTPQdmqjAgbRF2YPwE6VMD3fisUH5gFYWScFTJELmHORrJBn1rwh0KNvDKjhDx2Jb1MxtswU8Dhlg/OZNygSpit72qAWI/+LjujIdP1K9Nam971dxxfKkrnFzsNK3u4HpVrrXiijT5ULf7zsdWU0jCsuygblkp6J/ND40jpo2v8FAhXZHJIJkddBx6FO+plR4R8TNJ9Td4AuOAw4hJhsaW0x9rMorsoytcplAKrY0J58Mxyqu/LsC22h24mdhpiDfpD0gIM1DxBzZ+NR5GZSR19IfcwivuAN4sQmeTYOxeAl9h2qYIOOYHWQlTIu6RFG8Ns7CWQiShovsr/FKAw3rr8VwRk6bki3rHIRSMy3C173UCt3BABvyzGOBBkUbX6q06aPl+SS4eWZyDZC5JeHNNd+tdIJ/1R4mHldZz3YwiFmbg0xmf0VuyZjvOC4PEdxgQLyV0kLUPZbbSr+s5E33sO9tjvqYbbDZXKDgA/njpFCKWc2if5Y+0go4+IonGujCjMvHh/os7o2JJqRWtlY3SrOptuYv6kY7u9nVsa++wqYoi0FAaZNIv162qCzqd8g33qlIfWKm2YPrUBAJY5qRXKTqx1IjiuL6/7yu5pp/aOEgxlXumhAyXvbSqV5Wj9kNDPcwoT+6zsMeuJ7EYJ+1E4c7jhHzlq0=";
                options.WantAuthenticationRequestsSigned = false;
            })
            .AddInMemoryServiceProviders(Config.ServiceProviders);

        // in-memory, code config
        isBuilder.AddInMemoryIdentityResources(Config.IdentityResources);
        isBuilder.AddInMemoryApiScopes(Config.ApiScopes);
        isBuilder.AddInMemoryClients(Config.Clients);


        // if you want to use server-side sessions: https://blog.duendesoftware.com/posts/20220406_session_management/
        // then enable it
        //isBuilder.AddServerSideSessions();
        //
        // and put some authorization on the admin/management pages
        //builder.Services.AddAuthorization(options =>
        //       options.AddPolicy("admin",
        //           policy => policy.RequireClaim("sub", "1"))
        //   );
        //builder.Services.Configure<RazorPagesOptions>(options =>
        //    options.Conventions.AuthorizeFolder("/ServerSideSessions", "admin"));


        builder.Services.AddAuthentication();

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseIdentityServerSamlPlugin();
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}