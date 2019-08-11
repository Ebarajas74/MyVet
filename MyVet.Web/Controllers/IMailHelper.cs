namespace MyVet.Web.Controllers
{
    public interface IMailHelper
    {
        void SendMail(string to, string subject, string body);
    }
}