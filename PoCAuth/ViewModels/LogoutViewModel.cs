using System.Linq;
using System.Threading.Tasks;

namespace PoCAuth.ViewModels
{
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; }
    }
}
