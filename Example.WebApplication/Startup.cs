﻿namespace Example.WebApplication
{
    using System.Text;

    using Example.WebApplication.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Smart.AspNetCore.Formatters;
    using Smart.IO.ByteMapper;

    using Swashbuckle.AspNetCore.Swagger;

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

            services.AddMvc(options =>
            {
                var outputFormatter = new ByteMapperOutputFormatter(mapperFactory);
                outputFormatter.SupportedMediaTypes.Add("text/x-fixrecord");
                options.OutputFormatters.Add(outputFormatter);
                var inputFormatter = new ByteMapperInputFormatter(mapperFactory);
                inputFormatter.SupportedMediaTypes.Add("text/x-fixrecord");
                options.InputFormatters.Add(inputFormatter);
                options.FormatterMappings.SetMediaTypeMappingForFormat("dat", "text/x-fixrecord");
            });

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("example", new Info { Title = "Example API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/example/swagger.json", "Example API");
            });
        }
    }
}
