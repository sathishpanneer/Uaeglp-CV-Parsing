using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Contracts;
using Uaeglp.Repositories;
using Uaeglp.Services;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.FileExtensions;
using Uaeglp.Utilities;
using System.IO;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Uaeglp.Services.Extensions;
using Uaeglp.Services.Mapper;
using Uaeglp.Services.Nlog;

namespace Uaeglp.Tests
{

    public class ContainerResolver 
    {
        public static ILoggerFactory LoggerFactory;
        public static IConfigurationRoot Configuration;
        public IServiceProvider Container
        {
            get; set;
        }

        public ContainerResolver()
        {


            //string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //if (String.IsNullOrWhiteSpace(environment))
            //    throw new ArgumentNullException("Environment not found in ASPNETCORE_ENVIRONMENT");

            //Console.WriteLine("Environment: {0}", environment);

            var services = new ServiceCollection();

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true);

            builder
                  .AddJsonFile($"appsettings.json", optional: false);
            //if (environment == "Development")
            //{

            //    builder
            //        .AddJsonFile(
            //            Path.Combine(AppContext.BaseDirectory, string.Format("..{0}..{0}..{0}", Path.DirectorySeparatorChar), $"appsettings.{environment}.json"),
            //            optional: true
            //        );
            //}
            //else
            //{
            //    builder
            //        .AddJsonFile($"appsettings.{environment}.json", optional: false);
            //}

            Configuration = builder.Build();

            //LoggerFactory = new LoggerFactory()
            //    .AddConsole(Configuration.GetSection("Logging"))
            //    .AddDebug();

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);


            services.AddDbContext<LangAppDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("SqlLangConnection")));
            services.AddDbContext<AppDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("sqlConnection")));
            services.AddDbContext<FileDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("FileDBConnection")));


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
            services.AddScoped<IProfilePercentageCalculationService, ProfilePercentageCalculationService>();
            services.AddScoped<ILeadershipPointSystemService, LeadershipPointSystemService>();
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<IYoutubeVideoUploadService, YoutubeVideoUploadService>();
            services.AddScoped<IProgramService, ProgramService>();
            services.AddScoped<IAppSettingService, AppSettingService>();
            services.AddScoped<IFileService, FileService>();
            services.AddSingleton<IEncryptionManager, RijndaelEncryption>();
            services.AddSingleton<Random>();
            services.AddSingleton<RandomStringGeneratorService>();
            services.AddSingleton<PasswordHashing>();
            services.AddSingleton<ILog, LogNLog>();


            services.AddSingleton<IEncryptionManager, RijndaelEncryption>();
            services.AddSingleton<Random>();
            services.AddTransient<MongoDbContext, MongoDbContext>(serviceProvider =>
            {
                return new MongoDbContext(Configuration.GetConnectionString("mongoConnection"), Configuration.GetConnectionString("mongoDb"));
            });

            services.Configure<AppSettings>(Configuration.GetSection("ApplicationSettings").Bind);

            Container = services.BuildServiceProvider();



        }

    }

}

