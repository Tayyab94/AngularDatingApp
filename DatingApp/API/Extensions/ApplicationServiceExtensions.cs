using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Repositories;
using API.Repositories.ImplementedRepositories;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudnarySettings>(configuration.GetSection("CloudnarySettings"));

            // We Are Defining the Connection string for our Project
            services.AddDbContext<DataContext>(options=>{
               options.UseSqlite(configuration.GetConnectionString("DefaultConnection")); 
            });
            
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenServices, TokenServices>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ILikedRepository, LikedRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<LogUserActivity>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            return services;
        }
        
    }
}