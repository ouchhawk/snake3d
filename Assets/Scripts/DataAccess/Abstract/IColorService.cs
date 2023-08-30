using System.Net.Http;
using System.Threading.Tasks;

namespace Assets.Scripts.DataAccess.Abstract
{
    public interface IColorService
    {
        public Task<HttpResponseMessage> FetchColorPaletteAsync();
    }
}
