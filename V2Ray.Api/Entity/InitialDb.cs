using Microsoft.EntityFrameworkCore;
using V2Ray.Api.Common;
using V2Ray.Api.Database;
using V2Ray.Api.Shared;
using static V2Ray.Api.Entity.SSHKey;

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
                CreateServer(db);

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

        private static void CreateServer<T>(T db) where T : DB
        {
            //db.V2Servers.RemoveRange(db.V2Servers.ToList());
            //db.SaveChanges();
            //if (!db.V2Servers.Any(a => a.Title != "Amesterdam-B"))
            //{
            //    db.V2Servers.Add(new V2Server
            //    {
            //        Title = "Amesterdam-B",
            //        CityId = db.Cities.First(a => a.Title == "Amesterdam").Id,
            //        IP = "",
            //        Password = "!Q@W3e4r",
            //        Port = 4152,
            //        UserName = "kazemi.mst",
            //        State = false,
            //        Url = "",
            //    });
            //}
            //if (!db.V2Servers.Any(a => a.Title != "Amesterdam-A"))
            //{
            //    db.V2Servers.Add(new V2Server
            //    {
            //        Title = "Amesterdam-A",
            //        CityId = db.Cities.First(a => a.Title == "Amesterdam").Id,
            //        IP = "",
            //        Password = "!Q@W3e4r",
            //        Port = 4152,
            //        UserName = "kazemi.mst",
            //        State = false,
            //        Url = "",
            //    });
            //}
            //if (!db.V2Servers.Any(a => a.Title != "Paris-A"))
            //{
            //    db.V2Servers.Add(new V2Server
            //    {
            //        Title = "Paris-A",
            //        CityId = db.Cities.First(a => a.Title == "Paris").Id,
            //        IP = "",
            //        Password = "!Q@W3e4r",
            //        Port = 4152,
            //        UserName = "kazemi.mst",
            //        State = true,
            //        Url = "",
            //    });
            //}

            //if (!db.V2Servers.Any(a => a.Title != "Paris-B"))
            //{
            //    db.V2Servers.Add(new V2Server
            //    {
            //        Title = "Paris-B",
            //        CityId = db.Cities.First(a => a.Title == "Paris").Id,
            //        IP = "",
            //        Password = "!Q@W3e4r",
            //        Port = 4152,
            //        UserName = "kazemi.mst",
            //        State = true,
            //        Url = "",
            //    });
            //}
            if (!db.V2Servers.Any())
            {
                var obj = new V2Server
                {
                    IP = "1",
                    Title = "Frankfurt-B",
                    Password = "!Q@W#E$R5t6y7u8i",
                    Port = 1027,
                    UserName = "root",
                    IsActive = true,
                    Url = "ssh1.iranv2ray.com",
                };
                db.V2Servers.Add(obj);
                db.SaveChanges();
                var serverId = obj.Id;

                if (db.SSHKeyInfos.Any(c => c.ServerId == null))
                {
                    var keyInfos = db.SSHKeyInfos.Where(c => c.ServerId == null);
                    foreach (var item in keyInfos)
                    {
                        item.ServerId = serverId;
                        db.Update(item);
                    }
                    db.SaveChanges();
                }
            }
          

          
            //if (!db.V2Servers.Any(a=>a.Title == "Frankfurt-A"))
            //{
            //    db.V2Servers.Add(new V2Server
            //    {
            //        Title = "Frankfurt-A",
            //        CityId = db.Cities.First(a => a.Title == "Frankfurt").Id,
            //        IP = "",
            //        Password = "!Q@W3e4r",
            //        Port = 4152,
            //        UserName = "kazemi.mst",
            //        State = true,
            //        IsActive =true,
            //        Url = "gra.irantrojan.ml",
            //    });
            //}

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
                IP = "192.168.1.1",
                IsAdmin = true,
                Enable = true,
                NeedConfirm = false,
                Password = BCrypt.Net.BCrypt.HashPassword("!Q@W3e4r"),
                FirstName = DefaultUserConst.FirstName,
                LastName = DefaultUserConst.LastName,
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