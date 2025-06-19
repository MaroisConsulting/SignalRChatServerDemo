var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true);
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors();

app.UseHttpsRedirection();

app.MapHub<ChatHub>("/chathub");

app.Run();
