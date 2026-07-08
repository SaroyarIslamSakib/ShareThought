namespace DevSkill.Blog.Web.Models
{
    public enum ResponseTypes
    {
        success,
        danger
    }
    public class ResponseModel
    {
        public string? Message { get; set; }
        public ResponseTypes Response { get; set; }

    }
}
