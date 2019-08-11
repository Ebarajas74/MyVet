using System.Threading.Tasks;

namespace MyVet.Web.Helpers
{
    public interface IAgendaHelper
    {
        Task AddDays(int days);
    }
}