using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace V2Ray.Api.Extensions
{
    public class PaginationModelInput
    {
        public int Page { get; set; } = 1;

        public int ItemsPerPage { get; set; } = 10;

        [DataMember(IsRequired = false)]
        public string? GroupBy { get; set; }

        public bool? GroupDesc { get; set; }
        public bool? MustSort { get; set; }
        public bool? MultiSort { get; set; }

        [DataMember(IsRequired = false)]
        public string? SortBy { get; set; }

        public bool? SortDesc { get; set; } = false;

        [JsonIgnore]
        public string OrderBy
        {
            get
            {
                var asc = SortDesc.Value ? "desc" : "asc";
                SortBy = string.IsNullOrEmpty(SortBy) ? "id" : SortBy;
                return $"{SortBy}-{asc}";
            }
        }

        //[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        //public sealed class OrderAttribute : Attribute
        //{
        //    private readonly int order_;

        // public OrderAttribute([CallerLineNumber] int order = 0) { order_ = order; }

        //    public int Order { get { return order_; } }
        //}
    }
}