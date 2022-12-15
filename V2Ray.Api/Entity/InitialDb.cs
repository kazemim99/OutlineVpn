using Microsoft.EntityFrameworkCore;
using V2Ray.Api.Common;
using V2Ray.Api.Database;
using V2Ray.Api.Shared;

namespace V2Ray.Api.Entity
{
    public static class InitialDb
    {
        public static void EnsureMigrationOfContext<T>(this IApplicationBuilder app, string envEnvironmentName) where T : DB
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var db = serviceScope.ServiceProvider.GetService<T>();
            var isTest = db.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
            if (!isTest)
            {
                db.Database.Migrate();
            }
            else
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
            SeedData.Seed(db, isTest);
        }
    }

    public class SeedData
    {
        internal static void Seed<T>(T db, bool isTest) where T : DB
        {
            try
            {
                CreateUser(db);
                CreateCity(db);

                //CreateLockerLog(db);
                if (!isTest)
                {
                    //CreateComplex(db);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private static void CreateCity<T>(T db) where T : DB
        {
            if (db.Cities.Any())
                return;
            db.Countries.Add(new Country
            {
                Title = "Germany",
                Flag = "/Images/flags/germany.png",
                Cities = new List<City>
                {
                    new City { Title = "Frankfurt"}
                }
            });

            db.Countries.Add(new Country
            {
                Title = "France",
                Flag = "/Images/flags/germany.png",
                Cities = new List<City>
                {
                    new City { Title = "Paris"}
                }
            });
            db.SaveChanges();
        }
        private static void CreateUser<T>(T db) where T : DB
        {
            if (db.Users.Any())
                return;

            db.Users.Add(new User
            {
                IsAdmin = true,
                UserState = true,
                FirstName = DefaultUserConst.FirstName,
                LastName = DefaultUserConst.LastName,
                Mobile = DefaultUserConst.Mobile,
                Avatar = DefaultUserConst.Avatar,
                Email = DefaultUserConst.Email,
                Roles = new List<UserRole> {
                    new UserRole
                    {
                        Role = new Role()
                        {
                            Title =  Policies.Admin,
                        }
                    },
                     new UserRole
                    {
                        Role = new Role()
                        {
                            Title =  Policies.User,
                        }
                    }
                },
            });
            db.SaveChanges();
        }
    }
}