using System.Web;
using System.Linq;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;

namespace FramasVietNam.Common
{
    public static class Language
    {
        //Get title label from Resources
        public static string label(string name)
        {
            HttpCookie ck = HttpContext.Current.Request.Cookies.Get("Language");
            if (ck != null && ck.Value != null)
            {
                //English
                if (ck.Value == "1")
                {
                    return Resources.Label_EG.ResourceManager.GetString(name);
                }
                //VietNam
                else if (ck.Value == "2")
                {
                    return Resources.Label_VN.ResourceManager.GetString(name);
                }
                //Germany
                else if (ck.Value == "3")
                {
                    return Resources.Label_GR.ResourceManager.GetString(name);
                }
                //Default English
                else
                {
                    return Resources.Label_EG.ResourceManager.GetString(name);
                }
            }
            //Default English
            return Resources.Label_EG.ResourceManager.GetString(name);
        }
    }
}