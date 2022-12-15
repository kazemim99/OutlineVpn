using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using POD.Test.Integrations.Fixtures;
using POD.Test.Integrations.Utils;
using System.ComponentModel;
using System.Net;
using V2Ray.Api.Database;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.Server.Dto;

namespace POD.Test.Integrations
{
    public class ServerControllerTests : IntegrationTest
    {
        private string baseApi = "api/server";

        private string action = "";

        public ServerControllerTests(ApiWebApplicationFactory fixture)
            : base(fixture)
        {
            UsingDbContext(InitData);
        }

        [Fact]
        [Description("بعنوان دولوپر میخواهم موارد اجباری ارسال گردد و متن کلاینت ارسال گردد")]
        public async Task Prevent_process_invalid_user_input()
        {
            var serverInput = new CreateServerInput()
            {
                CityId = 0
            };

            var apiResponse = await _client.PostAsJsonAsync(baseApi, serverInput);
            var result = await apiResponse.ConvertToModel<ValidationErrorResponse<CreateServerValidationErrors>>();

            result.Title.Should().Contain("One or more validation errors occurred.");
            result.Errors.UserName.Should().Contain("The UserName field is required.");
            result.Errors.Password.Should().Contain("The Password field is required.");
            result.Errors.Url.Should().Contain("The Url field is required.");
            result.Status.Should().Be(400);
        }

        [Theory]
        [InlineData(1)]
        [Description("بعنوان ادمین میخواهم یک مجموعه جدید اضافه کنم")]
        public async Task Add_new_server(int userId)
        {
            var city = await UsingDbContextAsync(a =>
                a.Cities.FirstOrDefaultAsync(a => a.Title == "Frankfurt"));
         //Arrange
         var serverInput = Builder<CreateServerInput>
                .CreateNew()
                .With(a=>a.CityId = city.Id).Build();

            //Act
            var apiResponse = await _client.PostAsJsonAsync($"{baseApi}", serverInput);

            //Assert
            apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var server = await UsingDbContextAsync(a =>
                a.V2Servers.FirstOrDefaultAsync(a => a.Title == serverInput.Title));

            server.Should().BeEquivalentTo(serverInput);
        }

        [Theory]
        [InlineData(1)]
        [Description("بعنوان ادمین میخواهم یک سرور را ویرایش کنم")]
        public async Task Modify_exist_server(int userId)
        {
            var server = await UsingDbContextAsync(a =>a.V2Servers.LastOrDefaultAsync());
            //Arrange
            var serverInput = Builder<UpdateServerInput>
                   .CreateNew().Build();

            //Act
            var apiResponse = await _client.PutAsJsonAsync($"{baseApi}/{server.Id}", serverInput);

            //Assert
            apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var serverUpdated = await UsingDbContextAsync(a =>
                a.V2Servers.FirstOrDefaultAsync(a => a.Id == server.Id));

            serverInput.CityId.Should().Be(serverUpdated.CityId);
            serverInput.Url.Should().Be(serverUpdated.Url);
            serverInput.Title.Should().Be(serverUpdated.Title);
            serverInput.UserName.Should().Be(serverUpdated.UserName);
            serverInput.Password.Should().Be(serverUpdated.Password);
            serverInput.Port.Should().Be(serverUpdated.Port);
            serverInput.State.Should().Be(serverUpdated.State);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [Description("بعنوان ادمین میخواهم جزییات یک مجموعه را مشاهده کنم کنم")]
        public async Task Get_exist_server_details_by_id(int serverId)
        {
            //Act
            var apiResponse =
                await _client.DeserializeApiResponse<GetServerOutput>($"{baseApi}/{serverId}");

            //Assert
            var server = await UsingDbContextAsync(a =>
                a.V2Servers.Where(a => a.Id == serverId).FirstOrDefaultAsync());

            apiResponse.Title.Should().Be(server.Title);
            apiResponse.State.Should().Be(server.State);
            apiResponse.IP.Should().Be(server.IP);
            apiResponse.Url.Should().Be(server.Url);
            apiResponse.CityId.Should().Be(server.CityId);
         
        }

        [Theory]
        [InlineData("Fr", 1, 15, "id", false)]
        [InlineData("Test", 1, 15, "id", true)]
        [InlineData(null, 2, 10, "Title", true)]
        [InlineData(null, 3, 20, "Title", false)]
        [InlineData(null, 2, int.MaxValue, "Title", true)]
        [Description("بعنوان ادمین میخواهم لیست مجموعه ها را مشاهده و فیلتر کنم")]
        public async Task Get_server_list_and_paginate_and_filter(string name, int page, int pageItemCount, string orderBy, bool sortDesc)
        {
            //Arrange

            var userFilter = new ServerFilterInput()
            {
                SortBy = orderBy,
                SortDesc = sortDesc,
                Title = name,
                Page = page,
                ItemsPerPage = pageItemCount
            };

            var queryString = userFilter.GetQueryString("server/filter");
            //Act
            var apiResponse =
                await _client.DeserializeApiResponse<Pagination<GetServerListOutput>>(queryString);

            //Assert
            var serverex = await UsingDbContextAsync(a => a.V2Servers.Include(new[] { "City.Country"})
                .WhereIf(!string.IsNullOrEmpty(name), a => a.Title.Contains(name))
                .OrderBy(userFilter.OrderBy)
                .ToListAsync());
            var pageed = serverex.Skip((page - 1) * pageItemCount).Take(pageItemCount);

            var result = UsingMapper(a => a.Map<List<GetServerListOutput>>(pageed));

            apiResponse.TotalItems.Should().Be(serverex.Count);

            apiResponse.Result.Should().BeEquivalentTo(result);
        }

        private static void InitData(DB a)
        {
            for (int i = 0; i < 20; i++)
            {
                a.V2Servers.Add(new V2Server()
                {
                    Title = Guid.NewGuid().ToString(),
                    IP = Guid.NewGuid().ToString(),
                    State = i / 3 == 0,
                    Port = new Random().Next(10000, 60000),
                    Password = Guid.NewGuid().ToString(),
                    UserName = Guid.NewGuid().ToString(),
                    Url = Guid.NewGuid().ToString(),
                City = new City()
                {
                    Title =$"City_{i}",
                    Country = new Country()
                    {
                        Flag = Guid.NewGuid().ToString(),
                        Title   =Guid.NewGuid().ToString()
                    }
                }
                   
                }); ; ;
            }
        }
    }
}