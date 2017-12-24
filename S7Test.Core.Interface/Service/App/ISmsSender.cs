using System.Threading.Tasks;

namespace S7Test.Core.Interface.Service.App
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
