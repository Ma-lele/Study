using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using XHS.Build.Admin.Auth;
using XHS.Build.Admin.Filters;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Extensions;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Repository.Base;
using XHS.Build.Services.TaskQz;
using XHS.Build.TaskJobs;

namespace XHS.Build.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private static string basePath => AppContext.BaseDirectory;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<CenterToken>();
            services.AddScoped<CityToken>();
            services.AddScoped<AqtToken>();
            services.AddScoped<SiteCityGuannanToken>();
            services.AddScoped<SiteCityFuningToken>();
            services.AddScoped<SiteCityNanjingToken>();
            services.AddScoped<XHSRealnameToken>();
            services.AddScoped<SiteCityWuzhongToken>();
            services.AddScoped<SiteCityXuzhouToken>();
            services.AddScoped<SiteCityXuweiToken>();
            services.AddScoped<DayunToken>();
            services.AddScoped<IPermissionHandler, PermissionHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IUser, User>();
            services.TryAddSingleton<IUserKey, UserKey>();

            #region DB
            services.AddSqlsugar(Configuration);
            #endregion
            //Ӧ������
            #region Cors ����
            services.AddCors(c =>
            {
                //c.AddPolicy("LimitRequests", policy =>
                //{
                //    policy
                //    .WithOrigins(_appConfig.CorUrls)
                //    .AllowAnyHeader()//Ensures that the policy allows any header.
                //    .AllowAnyMethod();
                //});

                // ���������������ҲҪ�����м��
                c.AddPolicy("LimitRequests", policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });
            });
            #endregion

            #region Swagger Api�ĵ�
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = "XHS.Build.Admin"
                });
                c.OrderActionsBy(o => o.RelativePath);

                var xmlPath = Path.Combine(basePath, "XHS.Build.Admin.xml");
                c.IncludeXmlComments(xmlPath, true);

                var xmlModelPath = Path.Combine(basePath, "XHS.Build.Model.xml");
                c.IncludeXmlComments(xmlModelPath);


                //��������Token�İ�ť
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                //����Jwt��֤����
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

            });

            #endregion

            #region Jwt������֤
            var jwtConfig = Configuration.GetSection("JWTConfig").Get<JwtConfig>(); //_configHelper.Get<JwtConfig>("jwtconfig");
            services.TryAddSingleton(jwtConfig);
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = nameof(ResponseAuthenticationHandler); //401
                options.DefaultForbidScheme = nameof(ResponseAuthenticationHandler);    //403
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecurityKey)),
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddScheme<AuthenticationSchemeOptions, ResponseAuthenticationHandler>(nameof(ResponseAuthenticationHandler), o => { }); ;
            #endregion

            #region ������־
            //if (_appConfig.Log.Operation)
            //{
            //    services.AddSingleton<ILogHandler, LogHandler>();
            //}
            #endregion

            #region ����
            services.AddMemoryCache();
            services.AddSingleton<ICache, MemoryCache>();
            if (!string.IsNullOrEmpty(Configuration.GetValue<string>("redis:connectionString")))
            {
                var csredis = new CSRedis.CSRedisClient(Configuration.GetValue<string>("redis:connectionString"));
                RedisHelper.Initialization(csredis);
                services.AddSingleton<IRedisCache, RedisCache>();
            }
            #endregion

            #region automap
            var serviceAssembly = Assembly.Load("XHS.Build.Services");
            services.AddAutoMapper(serviceAssembly);
            #endregion            
            #region ��ʱ����
            services.AddJobSetup();
            #endregion
            services.AddControllers(options =>
            {
                //�쳣
                options.Filters.Add<ExceptionFilter>();
                //if (_appConfig.Log.Operation)
                //{
                //    options.Filters.Add<LogActionFilter>();
                //}
            }).AddNewtonsoftJson(options =>
                {
                    //����ѭ������
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //ʹ���շ� ����ĸСд
                    //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();//��ΪĬ��     
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";//����ʱ���ʽ
                });
            services.AddRouting(option =>
            {
                option.LowercaseUrls = true;
            });
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region AutoFac IOC����
            try
            {
                #region SingleInstance
                //�޽ӿ�ע�뵥��
                var assemblyCore = Assembly.Load("XHS.Build.Admin");
                var assemblyCommon = Assembly.Load("XHS.Build.Common");
                builder.RegisterAssemblyTypes(assemblyCore, assemblyCommon)
                .Where(t => t.GetCustomAttribute<SingleInstanceAttribute>() != null)
                .SingleInstance();

                //�нӿ�ע�뵥��
                builder.RegisterAssemblyTypes(assemblyCore, assemblyCommon)
                .Where(t => t.GetCustomAttribute<SingleInstanceAttribute>() != null)
                .AsImplementedInterfaces()
                .SingleInstance();
                #endregion

                #region Aop
                var interceptorServiceTypes = new List<Type>();
                //if (_appConfig.Aop.Transaction)
                //{
                //    builder.RegisterType<TransactionInterceptor>();
                //    interceptorServiceTypes.Add(typeof(TransactionInterceptor));
                //}
                #endregion

                #region Repository
                var assemblyRepository = Assembly.Load("XHS.Build.Repository");
                builder.RegisterAssemblyTypes(assemblyRepository)
                .AsImplementedInterfaces()
                .InstancePerDependency();
                #endregion

                #region Service
                var assemblyServices = Assembly.Load("XHS.Build.Services");
                builder.RegisterAssemblyTypes(assemblyServices)
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .EnableInterfaceInterceptors()
                .InterceptedBy(interceptorServiceTypes.ToArray());
                #endregion

                builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();//ע��ִ�

                #region MongoDB
                var connectionString = Configuration.GetSection("DataBaseSetting:ConnectionString").Value;
                var mongourl = new MongoUrl(connectionString);
                var databaseName = mongourl.DatabaseName;
                builder.Register(c => new MongoClient(mongourl).GetDatabase(databaseName)).InstancePerLifetimeScope();
                //MongoDbRepository
                builder.RegisterGeneric(typeof(MongoDBRepository<>)).As(typeof(IMongoDBRepository<>)).InstancePerLifetimeScope();
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.InnerException);
            }
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, 
            ITasksQzServices tasksQzServices, ISchedulerCenter schedulerCenter)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (Configuration.GetSection("ShowSwaggerDocument").Get<bool>())
            {
                #region Swagger Api�ĵ� �������ӿ� ֻ�ڿ�������ʾ
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/V1/swagger.json", "XHS.Build.Admin V1");
                    c.RoutePrefix = "swagger";//ֱ�Ӹ�Ŀ¼���ʣ������IIS��������ע�͸���䣬����launchSettings.launchUrl
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);//�۵�Api
                });
                #endregion
            }


            //��֤
            app.UseAuthentication();
            //��־
            loggerFactory.AddLog4Net();
            //��Ȩ
            app.UseAuthorization();
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });
            app.UseRouting();
            // CORS����
            app.UseCors("LimitRequests");
            app.UseAuthorization();
            // ����QuartzNetJob���ȷ���
            app.UseQuartzJobMildd(tasksQzServices, schedulerCenter);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            //��̬�ļ�log �Զ�����Դ
            app.UseDefaultFiles();
            dynamic type = new Program().GetType();
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);
            string logPath = Path.Combine(Directory.GetParent(currentDirectory).FullName, "xjlog");
            //��ֹ���ص���·��������
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".log"] = "text/plain";

            //�ṩ�ļ�Ŀ¼������ʽ


            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(logPath),
                RequestPath = "/log",
                ContentTypeProvider = provider,
                ServeUnknownFileTypes = true,
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append(
                         "Cache-Control", $"public, max-age=64800");
                }
            })
            .UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(logPath),
                RequestPath = new PathString("/log")
            });
             
            //foreach (var key in options.Value.Keys)
            //{
            //    frontProvider.Mappings.Add(key, options.Value[key]);
            //}
            //ǰ��˺ϲ�
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),@"wwwroot")),
                ContentTypeProvider = new FileExtensionContentTypeProvider()
            });
        }
    }
}