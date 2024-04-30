using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PersonnelShiftSystem.Web.Pages.LogoutAccount
{
    public class LogoutModel : PageModel
    {
        private IHttpContextAccessor _contextAccessor;

        public LogoutModel(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public IActionResult OnGet()
        {
            _contextAccessor.HttpContext.Session.Clear(); // Session'ý temizle
            _contextAccessor.HttpContext.Response.Cookies.Delete("Ihya.Sessions"); // Belirli bir Cookie'yi temizle
            _contextAccessor.HttpContext.Response.Cookies.Delete("Ihya");

            return RedirectToPage("/Index");
        }
    }
}
