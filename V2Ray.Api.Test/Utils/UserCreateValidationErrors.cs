using System.Collections.Generic;

namespace V2Ray.Api.Test.Utils
{
    public class CreateServerValidationErrors
    {
        public List<string> Title { get; set; }

        public List<string> UserName { get; set; }

        public List<string> Password { get; set; }


        public List<string> Port { get; set; }
        public List<string> Url { get; set; }
        public List<string> City { get; set; }
    }


}