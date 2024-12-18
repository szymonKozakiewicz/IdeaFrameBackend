using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.Core.Services;
using IdeaFrame.Infrastructure.DbContextCustom;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddDbContext<MyDbContexSqlServer>(
    options => {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
);
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
.AddEntityFrameworkStores<MyDbContexSqlServer>()
.AddDefaultTokenProviders()
.AddUserStore<UserStore<ApplicationUser, ApplicationRole,
MyDbContexSqlServer, Guid>>()
.AddRoleStore<RoleStore<ApplicationRole, MyDbContexSqlServer, Guid>>();
builder.Services.AddTransient<IUserService,UserService>();


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
