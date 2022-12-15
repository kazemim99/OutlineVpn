namespace POD.Test.Integrations.Utils
{
    public class ValidationErrorResponse<T>
    {
        public string Title { get; set; }

        public int Status { get; set; }

        public T Errors { get; set; }
    }
}