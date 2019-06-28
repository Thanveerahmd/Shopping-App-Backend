using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Project.Helpers;
using Project.Services;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Project.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Project.Data;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using pro.backend.Controllers;
using pro.backend.Services;
using pro.backend.iServices;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            // services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration.GetConnectionString("MSConnection")));

            // services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration.GetConnectionString("MSLocalConnction")));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<iAdminServices, AdminServices>();
            services.AddScoped<iProductService, ProductService>();
            services.AddScoped<Token>();
            services.AddScoped<iShoppingRepo, ShoppingRepo>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            Mapper.Reset();
            services.AddAutoMapper();



            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddControllersAsServices();


            //  // configure identity user
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = false;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            //  builder = new IdentityBuilder(builder.UserType,typeof(Role),builder.Services);
            // //Adds an Entity Framework implementation of identity information store
            //  builder.AddEntityFrameworkStores<DataContext>();
            //  builder.AddRoleValidator<Role>();
            //  builder.AddRoleManager<RoleManager<Role>>();
            //  builder.AddSignInManager<SignInManager<User>>();

            // configure strongly typed settings objects

            services.AddIdentity<User, Role>()
           .AddEntityFrameworkStores<DataContext>()
           .AddDefaultTokenProviders();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication for user
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var adminService = context.HttpContext.RequestServices.GetRequiredService<iAdminServices>();
                        var source = context.Request.Path.Value;
                        bool canParse = int.TryParse(context.Principal.Identity.Name, out var Id);

                        if (canParse)
                        {
                            var admin = adminService.GetById(Id);
                            if (admin == null)
                            {
                                // return unauthorized if user no longer exists
                                context.Fail("Unauthorized");
                            }
                        }
                        else
                        {
                            var user = userService.GetById(context.Principal.Identity.Name);
                            if (user == null)
                            {
                                context.Fail("Unauthorized");
                            }
                        }


                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                };
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        [System.Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {

                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddAppError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });

            }

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseIdentity();
            app.UseMvc();
        }
    }
}
