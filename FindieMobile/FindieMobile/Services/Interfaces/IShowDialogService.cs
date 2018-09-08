using System.Threading.Tasks;

namespace FindieMobile.Services.Interfaces
{
    public interface IShowDialogService
    {
        Task ShowDialog(string title, string content);
    }
}