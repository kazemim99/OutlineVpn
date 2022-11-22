using FizzWare.NBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Outline.Api.Common;
using Outline.Api.Database;
using Outline.Api.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Outline.Api.Entity
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

        private static void CreateUser<T>(T db) where T : DB
        {
            if (db.Users.Any())
                return;

            db.Users.Add(new User
            {
                IsAdmin = true,
                Code = new Random().Next(111111, 999999).ToString(),
                UserState = true,
                FirstName = DefaultUserConst.FirstName,
                LastName = DefaultUserConst.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(DefaultUserConst.Password),
                Mobile = DefaultUserConst.Mobile,
                Avatar = DefaultUserConst.Avatar,
                Phone = DefaultUserConst.Phone,
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