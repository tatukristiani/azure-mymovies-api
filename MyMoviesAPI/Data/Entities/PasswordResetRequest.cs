namespace MyMoviesAPI.Data.Entities
{
    public class PasswordResetRequest
    {
        public string ID { get; set; }
        public int UserID { get; set; }
        public DateTime RequestDate { get; set; }

    }
}
