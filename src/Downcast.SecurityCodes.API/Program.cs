using Downcast.Common.Errors.Handler.Config;
using Downcast.Common.Logging;
using Downcast.SecurityCodes.API.Config;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig();

builder.AddSecurityCodesApiServices();
builder.AddSerilog();
builder.AddErrorHandlerOptions();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();
app.UseErrorHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseForwardedHeaders();
app.MapControllers();

app.Run();