namespace App.API.Session.Models
{
    public class SessionConfiguration {
        public string scene { get; set; }
        public int  lighting { get; set; }
    }

    public class NewSession
    {
        public string SessionID { get; set; }
        public string User { get; set; }
        public string Requester { get; set; }
        public DateTime Date { get; set; }

        public SessionConfiguration configuration { get; set;}
    }
}