using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
using XHS.Build.Analyst.Web.Auth;
using XHS.Build.Analyst.Web.Filters;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Repository.Base;
using XHS.Build.Services.TaskQz;

namespace XHS.Build.Analyst.Web
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
            
            services.AddScoped<IPermissionHandler, PermissionHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IUser, User>();
            services.TryAddSingleton<IUserKey, UserKey>();
            services.AddScoped<XHSRealnameToken>();

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
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = "XHS.Build.Analyst.Web"
                });
                c.OrderActionsBy(o => o.RelativePath);

                var xmlPath = Path.Combine(basePath, "XHS.Build.Analyst.Web.xml");
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
            })
            .AddScheme<AuthenticationSchemeOptions, ResponseAuthenticationHandler>(nameof(ResponseAuthenticationHandler), o => { }); ;
            #endregion

            #region 操作日志
            //if (_appConfig.Log.Operation)
            //{
            //    services.AddSingleton<ILogHandler, LogHandler>();
            //}
            #endregion

            #region 缓存
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
            #region 定时任务
            //services.AddJobSetup();
            #endregion
            services.AddControllers(options =>
            {
                //异常
                options.Filters.Add<ExceptionFilter>();
                //if (_appConfig.Log.Operation)
                //{
                //    options.Filters.Add<LogActionFilter>();
                //}
            }).AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //使用驼峰 首字母小写
                //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();//改为默认     
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";//设置时间格式
            });
            services.AddRouting(option =>
            {
                option.LowercaseUrls = true;
            });
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region AutoFac IOC容器
            try
            {
                #region SingleInstance
                //无接口注入单例
                var assemblyCore = Assembly.Load("XHS.Build.Analyst.Web");
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

                builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();//注册仓储

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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, ITasksQzServices tasksQzServices)
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
                    c.SwaggerEndpoint("/swagger/V1/swagger.json", "XHS.Build.Analyst.Web V1");
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
            // 开启QuartzNetJob调度服务
           // app.UseQuartzJobMildd(tasksQzServices, schedulerCenter);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            //静态文件log 自定义资源
            dynamic type = new Program().GetType();
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);
            string logPath = System.IO.Path.Combine(Directory.GetParent(currentDirectory).FullName, "xjlog/HJT212-2005");
            //防止本地调试路径不符合
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            //提供文件目录访问形式

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".log"] = "text/plain";

            //提供文件目录访问形式
            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(logPath),
                RequestPath = new PathString("/log")
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(logPath),
                RequestPath = "/log",
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append(
                         "Cache-Control", $"public, max-age=64800");
                }
            });
        }
    }
}
