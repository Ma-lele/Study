using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Extensions;
using XHS.Build.Common.Fleck;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Net.Auth;
using XHS.Build.Net.Filter;
using XHS.Build.Net.Helper;
using XHS.Build.Net.Logs;
using XHS.Build.Repository.Base;

namespace XHS.Build.Net
{
    public class Startup
    {
        private readonly AppConfig _appConfig;
        private readonly ConfigHelper _configHelper;
        private readonly IHostEnvironment _env;
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            _appConfig = Configuration.GetSection("AppConfig").Get<AppConfig>();
            configuration.GetSection("WebConfig").Bind(MyConfig.Webconfig);
        }

        public IConfiguration Configuration { get; }
        private static string basePath => AppContext.BaseDirectory;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {//�û���Ϣ
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IUserKey, UserKey>();
            services.TryAddSingleton<IUser, User>();

            services.AddScoped<DayunToken>();
            #region DB
            services.AddSqlsugar(Configuration);
            #endregion
            //Ӧ������
            services.AddSingleton(_appConfig);
            #region Swagger Api�ĵ�
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = "XHS.Build.Net"
                });

                var xmlPath = Path.Combine(basePath, "XHS.Build.Net.xml");
                c.IncludeXmlComments(xmlPath, true);

                var xmlModelPath = Path.Combine(basePath, "XHS.Build.Model.xml");
                c.IncludeXmlComments(xmlModelPath);

                //�������Token�İ�ť
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                //���Jwt��֤����
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

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = nameof(ApiResponseHandler); //401
                options.DefaultForbidScheme = nameof(ApiResponseHandler);    //403
            })
                .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration.GetSection("JWTConfig:issuer").Value,
                    ValidAudience = Configuration.GetSection("JWTConfig:audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("JWTConfig:securityKey").Value)),
                    ClockSkew = TimeSpan.Zero
                };

            })
            //�Զ��巵��
            .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { });

            services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionFilter>();
                if (_appConfig.Log.Operation)
                {
                    options.Filters.Add<LogActionFilter>();
                }
            })
            .AddNewtonsoftJson(options =>
            {
                //����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //ʹ���շ� ����ĸСд
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //����ʱ���ʽ
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            #region ������־
            if (_appConfig.Log.Operation)
            {
                services.AddSingleton<ILogHandler, LogHandler>();
            }
            #endregion
            #region IP����
            if (_appConfig.RateLimit)
            {
                services.AddIpRateLimit(Configuration);
            }
            #endregion
            #region automap
            var serviceAssembly = Assembly.Load("XHS.Build.Services");
            services.AddAutoMapper(serviceAssembly);
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

            #region WebSocket�����
            var wsConfig = Configuration.GetSection("WebSocket").Get<WsConfig>(); //_configHelper.Get<JwtConfig>("jwtconfig");
            services.TryAddSingleton(wsConfig);
            services.AddSingleton<IFleckSpecial, FleckSpecial>();
            #endregion

            #region WebSocket�ͻ���
            //var wsConfig = Configuration.GetSection("WebSocket").Get<WsConfig>(); //_configHelper.Get<JwtConfig>("jwtconfig");
            //services.TryAddSingleton(wsConfig);
            //services.AddSingleton<IFleckSpecialClient, FleckSpecialClient>();
            #endregion

            services.AddScoped<SwaggerGenerator>();
            services.AddScoped<SpireDocHelper>();

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 1024 * 1024 * 1;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IFleckSpecial fleck)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //����websocket����
            fleck.Start();

            if (Configuration.GetSection("ShowSwaggerDocument").Get<bool>())
            {
                #region Swagger Api�ĵ� �������ӿ� ֻ�ڿ�������ʾ
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/V1/swagger.json", "XHS.Build.Net V1");
                    c.RoutePrefix = "swagger";//ֱ�Ӹ�Ŀ¼���ʣ������IIS��������ע�͸���䣬����launchSettings.launchUrl
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);//�۵�Api


                    #region �Զ�����ʽ

                    //css ע��
                    c.InjectStylesheet("/css/swaggerdoc.css");
                    c.InjectStylesheet("/css/app.min.css");
                    //js ע��
                    c.InjectJavascript("/js/jquery.js");
                    c.InjectJavascript("/js/swaggerdoc.js");
                    c.InjectJavascript("/js/app.min.js");

                    #endregion

                });
                #endregion
            }
            //IP����
            if (_appConfig.RateLimit)
            {
                app.UseIpRateLimiting();
            }
            loggerFactory.AddLog4Net();
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();
            app.UseStaticFiles();
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region AutoFac IOC����
            try
            {
                #region SingleInstance
                //�޽ӿ�ע�뵥��
                var assemblyCore = Assembly.Load("XHS.Build.Net");
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

                #region MongoDB
                var connectionString = Configuration.GetSection("DataBaseSetting:ConnectionString").Value;
                var mongourl = new MongoUrl(connectionString);
                var databaseName = mongourl.DatabaseName;
                builder.Register(c => new MongoClient(mongourl).GetDatabase(databaseName)).InstancePerLifetimeScope();
                //MongoDbRepository
                builder.RegisterGeneric(typeof(MongoDBRepository<>)).As(typeof(IMongoDBRepository<>)).InstancePerLifetimeScope();
                #endregion

                builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();//ע��ִ�
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.InnerException);
            }
            #endregion
        }

    }
}
