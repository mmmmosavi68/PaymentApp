using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace PaymentApp.Security
{
    public static class ImageValidator
    {
        public static bool IsImage(this IFormFile file)
        {
            return false;
            //try
            //{
            //    var img = Image.FromStream(file.OpenReadStream());
            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }
    }
}
