using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
using xhs.build.app.Attributes;
using xhs.build.app.AuthHelp;
using xhs.build.app.Filters;
using xhs.build.app.Logs;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Repository.Base;

namespace xhs.build.app
{
    public class Startup
    {
        private readonly AppConfig _appConfig;
        //private readonly IHostEnvironment _env;
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            //var build = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);//configuration;
            //if (string.IsNullOrEmpty(env.EnvironmentName))
            //{
            //    build.AddJsonFile("appsettings." + env.EnvironmentName + ".json", optional: false, reloadOnChange: true);
            //}
            Configuration =configuration;// build.Build();
            //_env = env;
            _appConfig = Configuration.GetSection("AppConfig").Get<AppConfig>();
        }
        private static string basePath => AppContext.BaseDirectory;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //用户信息
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IUser, User>();
            services.TryAddSingleton<IUserKey, UserKey>();

            services.AddScoped<DayunToken>();
            #region DB
            services.AddSqlsugar(Configuration);
            #endregion
            //应用配置
            #region Cors 跨域
            services.AddCors(c =>
            {
                //c.AddPolicy("LimitRequests", policy =>
                //{
                //    policy
                //    .WithOrigins(_appConfig.CorUrls)
                //    .AllowAnyHeader()//Ensures that the policy allows any header.
                //    .AllowAnyMethod();
                //});

                // 允许任意跨域请求，也要配置中间件
                c.AddPolicy("LimitRequests", policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });
            });
            #endregion

            #region Swagger Api文档
            services.AddSwaggerGen(c =>
            {
                //遍历出全部的版本，做文档信息展示
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Version = version,
                        Title = $"XHS.Build.App {version}接口文档",
                        Description = $"XHS.Build.App HTTP API " + version,
                        Contact = new OpenApiContact { Name = version },
                        License = new OpenApiLicense { Name = version + " 官方文档",}
                    });
                    c.OrderActionsBy(o => o.RelativePath);
                });

                var xmlPath = Path.Combine(basePath, "xhs.build.app.xml");
                c.IncludeXmlComments(xmlPath, true);

                var xmlModelPath = Path.Combine(basePath, "XHS.Build.Model.xml");
                c.IncludeXmlComments(xmlModelPath);


                //添加设置Token的按钮
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Bearer {token}",
                    Name = "Authorization",
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

            #region Jwt身份认证
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
                options.Events = new JwtBearerEvents
                {
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
            .AddScheme<AuthenticationSchemeOptions, ResponseAuthenticationHandler>(nameof(ResponseAuthenticationHandler), o => { }); ;
            #endregion

            #region 操作日志
            if (_appConfig.Log.Operation)
            {
                services.AddSingleton<ILogHandler, LogHandler>();
            }
            #endregion

            #region 缓存

                services.AddMemoryCache();
                services.AddSingleton<ICache, MemoryCache>();
            
            #endregion

            #region automap
            var serviceAssembly = Assembly.Load("XHS.Build.Services");
            services.AddAutoMapper(serviceAssembly);
            #endregion            

            services.AddControllers(options =>
            {
                //异常
                options.Filters.Add<ExceptionFilter>();
                if (_appConfig.Log.Operation)
                {
                    options.Filters.Add<LogActionFilter>();
                }
                //禁止去除ActionAsync后缀
                options.SuppressAsyncSuffixInActionNames = false;
            })
                .AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //使用驼峰 首字母小写
                //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();//改为默认 
                //设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.AddRouting(option =>
            {
                option.LowercaseUrls = true;
            });
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 5000; // 5000 items max
                options.ValueLengthLimit = 1024 * 1024 * 100; // 100MB max len form data
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region AutoFac IOC容器
            try
            {
                #region SingleInstance
                //无接口注入单例
                var assemblyCore = Assembly.Load("xhs.build.app");
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
                    typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                    {
                        c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"XHS.Build.App {version}");
                    });
                    c.RoutePrefix = "swagger";//直接根目录访问，如果是IIS发布可以注释该语句，并打开launchSettings.launchUrl
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);//折叠Api
                });
                #endregion
            }


            //认证
            app.UseAuthentication();
            //日志
            loggerFactory.AddLog4Net();
            //授权
            app.UseAuthorization();
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });
            app.UseRouting();
            // CORS跨域
            app.UseCors("LimitRequests");
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
