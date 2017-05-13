using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    /// <summary>
    /// Services for mapping font styles to actual font name.
    /// </summary>
    public interface IFontService
    {
        /// <summary>
        /// Get font name for given font style.
        /// </summary>
        /// <param name="fontStyle"></param>
        /// <returns></returns>
        string GetFontName(FontStyle fontStyle);
    }
}
