using DotNetCore.CAP.Messages;
using Application.Common.Exceptions;
using Application.Common.Filters;
using Application.Extensions;
using Infrastructure.Extensions;
using Persistence.Context;
using Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new ValidateFilterAttribute());
    opt.Filters.Add(typeof(ValidateJsonModelFilter));
})
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.IgnoreReadOnlyFields = true;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull; //db'de null olan deðerleri getirme
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region Dependency Injection
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddPersistenceLayer(builder.Configuration);
#endregion






#region CORS Settings
var allowedResources = "CorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedResources, policy => { policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});
#endregion





var app = builder.Build();

#region Auto migrate
using (var scope = app.Services.CreateScope())
{
    //var dotnetCapContext = scope.ServiceProvider.GetRequiredService<DotnetCapDbContext>();
    var apiContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


    //dotnetCapContext.Database.Migrate();
    //apiContext.Database.Migrate();
}
#endregion




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseStaticFiles();
app.UseCors(allowedResources);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<UseCustomExceptionHandler>();
app.Run();
