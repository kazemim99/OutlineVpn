using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using V2Ray.Api.Database;
using Xunit;

namespace V2Ray.Api.Test.Fixtures
{
    public abstract class IntegrationTest : IClassFixture<ApiWebApplicationFactory>
    {
        //private readonly Checkpoint _checkpoint = new Checkpoint
        //{
        //    SchemasToInclude = new[] {
        //    "Playground"
        //},
        //    WithReseed = true
        //};

        protected readonly ApiWebApplicationFactory _factory;

        protected readonly HttpClient _client;

        protected readonly IMapper _mapper;

        protected IntegrationTest(ApiWebApplicationFactory fixture)
        {
            _factory = fixture;
            _client = _factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("IntegrationTest");
            // if needed, reset the DB
            //_checkpoint.Reset(_factory.Configuration.GetConnectionString("SQL")).Wait();
        }

        protected async Task<TResult> UsingDbContextAsync<TResult>(Func<DB, Task<TResult>> func)
        {
            using var service = _factory.Services.CreateScope();
            var db = service.ServiceProvider.GetRequiredService<DB>();

            var result = await func(db);
            await db.SaveChangesAsync();

            return result;
        }

        protected void UsingDbContextAsync(Func<DB, Task> action)
        {
            using var service = _factory.Services.CreateScope();
            var context = service.ServiceProvider.GetRequiredService<DB>();

            action(context);
            context.SaveChangesAsync();
        }

        protected void UsingDbContext(Action<DB> action)
        {
            using var service = _factory.Services.CreateScope();
            var context = service.ServiceProvider.GetRequiredService<DB>();

            action(context);
            context.SaveChanges();
        }

        protected TResult UsingDbContext<TResult>(Func<DB, TResult> func)
        {
            using var service = _factory.Services.CreateScope();
            var db = service.ServiceProvider.GetRequiredService<DB>();

            var result = func(db);
            db.SaveChanges();

            return result;
        }

        protected TResult UsingMapper<TResult>(Func<IMapper, TResult> func)
        {
            using var service = _factory.Services.CreateScope();
            var db = service.ServiceProvider.GetRequiredService<IMapper>();
            var result = func(db);

            return result;
        }
    }
}