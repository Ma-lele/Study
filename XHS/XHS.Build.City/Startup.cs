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
using XHS.Build.Common.Sqlsugar;
using XHS.Build.City.Auth;
using XHS.Build.City.Filter;
using XHS.Build.City.Logs;
using XHS.Build.Repository.Base;

namespace XHS.Build.City
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
        {//用户信息
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IUserKey, UserKey>();
            services.TryAddSingleton<IUserAqtKey, UserAqtKey>();
            services.TryAddSingleton<IUser, User>();
            services.AddScoped<DayunToken>();
            #region DB
            services.AddSqlsugar(Configuration);
            #endregion
            //应用配置
            services.AddSingleton(_appConfig);
            #region Swagger Api文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = "XHS.Build.City"
                });

                var xmlPath = Path.Combine(basePath, "XHS.Build.City.xml");
                c.IncludeXmlComments(xmlPath, true);

                var xmlModelPath = Path.Combine(basePath, "XHS.Build.Model.xml");
                c.IncludeXmlComments(xmlModelPath);

                //添加设置Token的按钮
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Bearer {token}",
                    Name = "accessToken",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                //添加Jwt验证设置
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
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Headers["accessToken"];
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                    {
                        //var token =Convert.ToString(context.Request.Headers["Authorization"]).Replace("Bearer ", "");
                        //var userClaims = new UserToken(jwtConfig).Decode(token);
                        //var refreshExpiresValue = userClaims.FirstOrDefault(a => a.Type == ClaimAttributes.RefreshExpires)?.Value;
                        //var refreshExpires = Convert.ToDateTime(refreshExpiresValue);
                        // 如果过期
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            //if (refreshExpires >= DateTime.Now)//超过刷新时间，长时间未登录，重新登录
                            //{
                            //    context.Response.ContentType = "application/json";
                            //    context.Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;
                            //    context.Response.WriteAsync(JsonConvert.SerializeObject(
                            //        new ResponseStatusData
                            //        {
                            //            Code = Enums.StatusCodes.Status4001Unauthorized,
                            //            Msg = Enums.StatusCodes.Status4001Unauthorized.ToDescription()
                            //        },
                            //        new JsonSerializerSettings()
                            //        {
                            //            ContractResolver = new CamelCasePropertyNamesContractResolver()
                            //        }
                            //    ));
                            //}
                        }
                        return Task.CompletedTask;
                    }
                };
        })
            //自定义返回
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
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //使用驼峰 首字母小写
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            #region 操作日志
            if (_appConfig.Log.Operation)
            {
                services.AddSingleton<ILogHandler, LogHandler>();
            }
            #endregion
            #region IP限流
            if (_appConfig.RateLimit)
            {
                services.AddIpRateLimit(Configuration);
            }
            #endregion
            #region automap
            var serviceAssembly = Assembly.Load("XHS.Build.Services");
            services.AddAutoMapper(serviceAssembly);
            #endregion
            #region 缓存

            services.AddMemoryCache();
            services.AddSingleton<ICache, MemoryCache>();
            
            #endregion
            services.AddScoped<AqtToken>();
            services.AddScoped<SwaggerGenerator>();
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 1024 * 1024 * 1;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (Configuration.GetSection("ShowSwaggerDocument").Get<bool>())
            {
                #region Swagger Api文档 第三方接口 只在开发下显示
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/V1/swagger.json", "XHS.Build.City V1");
                    c.RoutePrefix = "swagger";//直接根目录访问，如果是IIS发布可以注释该语句，并打开launchSettings.launchUrl
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);//折叠Api


                    #region 自定义样式

                    //css 注入
                    c.InjectStylesheet("/css/swaggerdoc.css");
                    c.InjectStylesheet("/css/app.min.css");
                    //js 注入
                    c.InjectJavascript("/js/jquery.js");
                    c.InjectJavascript("/js/swaggerdoc.js");
                    c.InjectJavascript("/js/app.min.js");

                    #endregion

                });
                #endregion
            }
            //IP限流
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
            #region AutoFac IOC容器
            try
            {
                #region SingleInstance
                //无接口注入单例
                var assemblyCore = Assembly.Load("XHS.Build.City");
                var assemblyCommon = Assembly.Load("XHS.Build.Common");
                builder.RegisterAssemblyTypes(assemblyCore, assemblyCommon)
                .Where(t => t.GetCustomAttribute<SingleInstanceAttribute>() != null)
                .SingleInstance();
                //有接口注入单例
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

                builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();//注册仓储
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.InnerException);
            }
            #endregion
        }

    }
}
