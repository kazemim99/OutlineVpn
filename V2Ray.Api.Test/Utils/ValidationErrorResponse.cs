namespace V2Ray.Api.Test.Utils
{
    public class ValidationErrorResponse<T>
    {
        public string Title { get; set; }

        public int Status { get; set; }

        public T Errors { get; set; }
    }
}