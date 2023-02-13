using Rsk.AspNetCore.Authentication.Saml2p;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "cookie";
        options.DefaultChallengeScheme = "idp";
    }).AddCookie("cookie")
    .AddSaml2p("idp", options =>
    {
        options.Licensee = "DEMO";
        options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjMtMDMtMDlUMDE6MDA6MDEuMDM1MDAyNyswMDowMCIsImlhdCI6IjIwMjMtMDItMDdUMDE6MDA6MDEiLCJvcmciOiJERU1PIiwiYXVkIjoyfQ==.CH6Q42T5O7kfTPQdmqjAgbRF2YPwE6VMD3fisUH5gFYWScFTJELmHORrJBn1rwh0KNvDKjhDx2Jb1MxtswU8Dhlg/OZNygSpit72qAWI/+LjujIdP1K9Nam971dxxfKkrnFzsNK3u4HpVrrXiijT5ULf7zsdWU0jCsuygblkp6J/ND40jpo2v8FAhXZHJIJkddBx6FO+plR4R8TNJ9Td4AuOAw4hJhsaW0x9rMorsoytcplAKrY0J58Mxyqu/LsC22h24mdhpiDfpD0gIM1DxBzZ+NR5GZSR19IfcwivuAN4sQmeTYOxeAl9h2qYIOOYHWQlTIu6RFG8Ns7CWQiShovsr/FKAw3rr8VwRk6bki3rHIRSMy3C173UCt3BABvyzGOBBkUbX6q06aPl+SS4eWZyDZC5JeHNNd+tdIJ/1R4mHldZz3YwiFmbg0xmf0VuyZjvOC4PEdxgQLyV0kLUPZbbSr+s5E33sO9tjvqYbbDZXKDgA/njpFCKWc2if5Y+0go4+IonGujCjMvHh/os7o2JJqRWtlY3SrOptuYv6kY7u9nVsa++wqYoi0FAaZNIv162qCzqd8g33qlIfWKm2YPrUBAJY5qRXKTqx1IjiuL6/7yu5pp/aOEgxlXumhAyXvbSqV5Wj9kNDPcwoT+6zsMeuJ7EYJ+1E4c7jhHzlq0=";
        options.NameIdClaimType = "sub";
        options.CallbackPath = "/signin-saml";
        options.SignInScheme = "cookie";

        options.IdentityProviderMetadataAddress = "https://localhost:5001/saml/metadata";

        options.ServiceProviderOptions = new SpOptions
        {
            EntityId = "RSKSaml",
            MetadataPath = "/saml/metadata"
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();