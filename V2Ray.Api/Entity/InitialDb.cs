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
            bool isTest = db.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
            if (!isTest)
            {
                db.Database.Migrate();
            }
            else
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
            //SeedData.Seed(db, isTest);
        }
    }

    public class SeedData
    {
        internal static void Seed<T>(T db, bool isTest) where T : DB
        {
            try
            {
                CreateDNS(db);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private static void CreateDNS<T>(T db) where T : DB
        {
            if (db.DNSRecords.Any())
                return;

            db.DNSRecords.Add(new DNSRecord
            {
              Enable = true,
              IsActive = true,
              Ipv4 = "31.7.70.29",
              UpdateDate = DateTime.Now,
            });
            db.SaveChanges();
        }
    }
}