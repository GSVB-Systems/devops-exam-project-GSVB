using Microsoft.EntityFrameworkCore;
using Npgsql;
using DevOpsAppRepo;
using DevOpsAppRepo.Interfaces;
using DevOpsAppRepo.Repos;
using DevOpsAppService.Interfaces;
using DevOpsAppService.Services;
using Sieve.Services;
using DevOpsAppService.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DevOpsAppService.EggApi;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using api;
using DevOpsAppApi.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.GenerateApiClientsFromOpenApi("/../../../DevOpsClient/src/api/ServerApi.ts").GetAwaiter().GetResult();


app.UseOpenApi();
app.UseSwaggerUi();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
