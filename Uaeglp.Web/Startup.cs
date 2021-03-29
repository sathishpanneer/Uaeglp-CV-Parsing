using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using Uaeglp.Contracts;
using Uaeglp.Repositories;
using Uaeglp.Services;
using Uaeglp.Services.WebService;
using Uaeglp.Services.Extensions;
using Uaeglp.Services.Mapper;
using Uaeglp.Services.Nlog;
using Uaeglp.Utilities;
using Uaeglp.Web.Filters;
using Microsoft.AspNetCore.Http;
using Hangfire;
using Hangfire.MemoryStorage;
using Uaeglp.ViewModels.Enums;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http.Features;

namespace Uaeglp.Web
{
    public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			LogManager.LoadConfiguration(System.String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
            var appSettingsSection = Configuration.GetSection("ApplicationSettings");
            services.Configure<AppSettings>(appSettingsSection);
            // To Enable CORS
            services.AddCors();
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Audience = appSettings.ApiScope;
                    options.Authority = appSettings.AuthorizeUrl;
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                });

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            //var youtubeConfigurationSection = Configuration.GetSection("YoutubeClientSecret");
            //services.Configure<YoutubeClientSecret>(youtubeConfigurationSection);

            var EIDValidationSection = Configuration.GetSection("EIDValidation");
            services.Configure<EIDValidation>(EIDValidationSection);

            var MinIoConfigSection = Configuration.GetSection("MinIoConfig");
            services.Configure<MinIoConfig>(MinIoConfigSection);

            var DBConnectionString = Configuration.GetSection("ConnectionStrings");
            services.Configure<DBConnectionString>(DBConnectionString);

            var CVParsingConfigSection = Configuration.GetSection("CVParsingConfig");
            services.Configure<CVParsingConfig>(CVParsingConfigSection);


            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            //services.AddDbContext<AppDbContext>(options =>
            //        options.UseSqlServer(Configuration.GetConnectionString("sqlConnection")),
            //    ServiceLifetime.Transient);
            services.AddDbContext<AppDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("sqlConnection")));
            services.AddDbContext<FileDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("FileDBConnection")));
            services.AddDbContext<LangAppDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("SqlLangConnection")));


            services.AddScoped<IPingService, PingService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.AddScoped<IApplicationProgressStatusService, ApplicationProgressStatusService>();
            services.AddScoped<IActivityAndChallengesService, ActivityAndChallengesService>();
            services.AddScoped<ILangService, LangService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISocialService, SocialService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IProfileAssessmentService, ProfileAssessmentService>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IKnowledgeHubService, KnowledgeHubService>();
            services.AddScoped<IMeetupService, MeetupService>();
            services.AddScoped<IMessagingService, MessagingService>();
            services.AddScoped<IProfilePercentageCalculationService, ProfilePercentageCalculationService>();
            services.AddScoped<ILeadershipPointSystemService, LeadershipPointSystemService>();
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<IYoutubeVideoUploadService, YoutubeVideoUploadService>();
            services.AddScoped<IProgramService, ProgramService>();
            services.AddScoped<IAppSettingService, AppSettingService>();
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRecommendLeaderService, RecommendLeaderService>();
            services.AddScoped<IUserRecommendationService, UserRecommendationService>();
            services.AddScoped<IReportProblemService, ReportProblemService>();
            services.AddScoped<IPeopleNearByService, PeopleNearByService>();
            services.AddScoped<IQRScanService, QRScanService>();
            services.AddScoped<IWebPortalNotificationService, WebPortalNotificationService>();
            services.AddScoped<IUserIPAddress, UserIPAddress>();
            services.AddSingleton <CitizenVerification_pttClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IEncryptionManager, RijndaelEncryption>();
            services.AddSingleton<Random>();
            services.AddSingleton<RandomStringGeneratorService>();
            services.AddSingleton<PasswordHashing>();
            services.AddSingleton<ILog, LogNLog>();

            services.AddDirectoryBrowser();

            services.AddTransient<MongoDbContext, MongoDbContext>(serviceProvider =>
            {
                return new MongoDbContext(Configuration.GetConnectionString("mongoConnection"), Configuration.GetConnectionString("mongoDb"));
            });
            services.AddControllers();

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });


            // services.Configure<AppSettings>(Configuration.GetSection("ApplicationSettings"));
            services.AddOptions();

            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseMemoryStorage());
                //.UseSqlServerStorage(Configuration.GetConnectionString("sqlConnection")));

            
            services.AddHangfireServer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "UAEGLP Service Document",
                    // Description = "Testing"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
                c.OperationFilter<FileUploadOperation>();
            });

            services.AddDataProtection();

            //services.AddAuthentication(options =>
            //{
            //	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //	options.Authority = "https://{yourOktaDomain}/oauth2/default";
            //	options.Audience = "api://default";
            //	options.RequireHttpsMetadata = false;
            //});

            //services.Configure<TokenOptions>(Configuration.GetSection("TokenOptions"));
            //var tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();

            //var signingConfigurations = new SigningConfigurations();
            //services.AddSingleton(signingConfigurations);

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //.AddJwtBearer(jwtBearerOptions =>
            //{
            //	jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
            //	{
            //		ValidateAudience = true,
            //		ValidateLifetime = true,
            //		ValidateIssuerSigningKey = true,
            //		ValidIssuer = tokenOptions.AuthenticatorIssuer,
            //		ValidAudience = tokenOptions.Audience,
            //		IssuerSigningKey = signingConfigurations.Key,
            //		ClockSkew = TimeSpan.Zero
            //	};
            //});

            //string domain = $"https://stagingauth.uaeglp.gov.ae/identity/connect/authorize";
            //services.AddAuthentication(options =>
            //{
            //	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(options =>
            //{
            //	options.Authority = domain;
            //	options.Audience = "API";
            //	options.TokenValidationParameters = new TokenValidationParameters
            //	{
            //		NameClaimType = ClaimTypes.NameIdentifier
            //	};
            //});

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //	.AddJwtBearer(options =>
            //{
            //	options.Authority = "https://stagingAuth.uaeglp.gov.ae/";
            //	options.Audience = "API";
            //	options.RequireHttpsMetadata = false;
            //});

            // configure strongly typed settings objects

            //configure jwt authentication

            //var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    x.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager,
            IServiceProvider serviceProvider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

            // To Enable CORS
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseHsts(h => h.MaxAge(days: 365).IncludeSubdomains().Preload());


            app.UseStaticFiles();

			app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "UAEGLP");
            });

            app.UseHangfireDashboard();

            //app.UseFileServer(new FileServerOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(Directory.GetCurrentDirectory(), @".well-known")),
            //    RequestPath = new PathString("/.well-known"),
            //    EnableDirectoryBrowsing = true,
            //});

            //app.UseRewriter(new RewriteOptions()
            //        .AddRewrite("(.*)/apple-app-site-association", "$1/apple-app-site-association.json", true));

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @".well-known")),
                RequestPath = new PathString("/.well-known"),
                ServeUnknownFileTypes = true,
                DefaultContentType = "text/plain"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"Redirection")),
                RequestPath = new PathString("/Redirection"),

                ServeUnknownFileTypes = true,
                DefaultContentType = "text/plain"
            });

            //    app.UseStaticFiles(new StaticFileOptions
            //    {
            //        FileProvider = new PhysicalFileProvider(
            //Path.Combine(env.WebRootPath, "images")),
            //        RequestPath = "/MyImages"
            //    });
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                   Path.Combine(Directory.GetCurrentDirectory(), @"Redirection")),
                RequestPath = new PathString("/Redirection")
            });
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @".well-known")),
                RequestPath = new PathString("/.well-known")
            });


            recurringJobManager.AddOrUpdate(
                "Running job every day once",
                () => serviceProvider.GetService<IReminderService>().SendReminder(),
                Cron.Daily(9, 0), TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Like",
                () => serviceProvider.GetService<IWebPortalNotificationService>().PostLikeNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Comment",
                () => serviceProvider.GetService<IWebPortalNotificationService>().PostCommentNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Share",
                () => serviceProvider.GetService<IWebPortalNotificationService>().PostShareNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Activity",
                () => serviceProvider.GetService<IWebPortalNotificationService>().ActivityAndChallengeNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Program",
                () => serviceProvider.GetService<IWebPortalNotificationService>().ProgrammeNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Batch",
                () => serviceProvider.GetService<IWebPortalNotificationService>().BatchNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Following",
                () => serviceProvider.GetService<IWebPortalNotificationService>().UserFollowingNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Network",
                () => serviceProvider.GetService<IWebPortalNotificationService>().NetworkGroupNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification AssignedAssessment",
                () => serviceProvider.GetService<IWebPortalNotificationService>().AssessmentAssignedNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification AssessmentMember",
                () => serviceProvider.GetService<IWebPortalNotificationService>().AssessmentMemberNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Meetup",
                () => serviceProvider.GetService<IWebPortalNotificationService>().MeetupNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Messaging",
                () => serviceProvider.GetService<IWebPortalNotificationService>().MessagingNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification Events",
                () => serviceProvider.GetService<IWebPortalNotificationService>().EventNotification(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            recurringJobManager.AddOrUpdate(
                "Custom Notification EngagementActiity",
                () => serviceProvider.GetService<IWebPortalNotificationService>().EngagementActiity(),
                "*/10 * * * * *", TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );

            RotativaConfiguration.Setup((Microsoft.AspNetCore.Hosting.IHostingEnvironment)env);
		}
	}
}
