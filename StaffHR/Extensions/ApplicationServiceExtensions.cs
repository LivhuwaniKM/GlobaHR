using Microsoft.EntityFrameworkCore;
using SHDomain.Data;
using SHDomain.Helpers;
using SHServices.AgentService;
using SHServices.ApartmentService;
using SHServices.EmployeeService;
using SHServices.Mapper;
using SHServices.MediaService;
using SHServices.UserService;
using SHServices.VehicleService;

namespace StaffHR.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("StaffManagementDB"), c =>
                {
                    c.MigrationsAssembly("SHServices");
                });
            });

            services.AddSwaggerGen();
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowCorsPolicy", c =>
                {
                    c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddMvc();
            services.AddSession();
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IApartmentService, ApartmentService>();
            services.AddScoped<IAgentService, AgentService>();

            services.AddScoped<IResponseHelper, ResponseHelper>();

            services.AddScoped<IMediaService, MediaService>();

            services.AddAutoMapper(typeof(AutoMapperProfile));

            return services;
        }
    }
}
