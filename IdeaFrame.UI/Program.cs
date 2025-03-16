using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.Core.Services;
using IdeaFrame.Infrastructure.DbContextCustom;
using IdeaFrame.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
.AddEntityFrameworkStores<MyDbContexSqlServer>()
.AddDefaultTokenProviders()
.AddUserStore<UserStore<ApplicationUser, ApplicationRole,
MyDbContexSqlServer, Guid>>()
.AddRoleStore<RoleStore<ApplicationRole, MyDbContexSqlServer, Guid>>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"], 
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSymmetricalKey"]))
    };


});


builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:4200")
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddHttpContextAccessor();


builder.Services.AddDbContext<MyDbContexSqlServer>(
    options => {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            x => x.UseHierarchyId()
            );
    }
);
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IUserService,UserService>();
builder.Services.AddTransient<IDirectoryService, DirectoryService>();
builder.Services.AddTransient<IDirectoryRepository, DirectoryRepository>();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<IJwtRepository, JwtRepository>();
builder.Services.AddAuthorization(options =>
{
    
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});


var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.Run();

public partial class Program
{

}
