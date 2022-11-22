using AutoMapper;

namespace Outline.Api.Mapping
{
    public class UserEntityMapping : Profile
    {
        private readonly IWebHostEnvironment _env;

        public UserEntityMapping()
        {
        }

        public UserEntityMapping(IWebHostEnvironment env)
        {
            _env = env;
        }
    }
}