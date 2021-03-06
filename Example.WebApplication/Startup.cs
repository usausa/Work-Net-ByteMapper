namespace Example.WebApplication
{
    using System.Text;

    using Example.WebApplication.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    using Smart.AspNetCore.Formatters;
    using Smart.IO.ByteMapper;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var mapperFactory = new MapperFactoryConfig()
                .UseOptionsDefault()
                .DefaultEncoding(Encoding.GetEncoding(932))
                .CreateMapByExpression<SampleData>(59, c => c
                    .ForMember(x => x.Code, m => m.Ascii(13))
                    .ForMember(x => x.Name, m => m.Text(20))
                    .ForMember(x => x.Qty, m => m.Integer(6))
                    .ForMember(x => x.Price, m => m.Decimal(10, 2))
                    .ForMember(x => x.Date, m => m.DateTime("yyyyMMdd")))
                .CreateMapByExpression<SampleData>("short", 35, c => c
                    .ForMember(x => x.Code, m => m.Ascii(13))
                    .ForMember(x => x.Name, m => m.Text(20)))
                .ToMapperFactory();

            services.AddControllersWithViews(options =>
            {
                var config = new ByteMapperFormatterConfig { MapperFactory = mapperFactory };
                config.SupportedMediaTypes.Add("text/x-fixedrecord");

                options.OutputFormatters.Add(new ByteMapperOutputFormatter(config));
                options.InputFormatters.Add(new ByteMapperInputFormatter(config));

                options.FormatterMappings.SetMediaTypeMappingForFormat("dat", "text/x-fixedrecord");
            });

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("example", new OpenApiInfo
                {
                    Title = "Example API",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/example/swagger.json", "Example API");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
