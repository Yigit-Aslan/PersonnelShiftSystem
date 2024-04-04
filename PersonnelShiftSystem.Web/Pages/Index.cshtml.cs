using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Domain.Interfaces;

namespace PersonnelShiftSystem.Web.Pages
{
    public class IndexModel : PageModel
    {
        private IBaseModel baseModel;
        private readonly LoginService _loginService;
        [BindProperty]
        public LoginDto LoginModel { get; set; }

        public IndexModel(IBaseModel _baseModel, LoginService loginService)
        {
            baseModel = _baseModel;
            _loginService = loginService;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostLogin()
        {
            var loginResult = await _loginService.LoginAsync(LoginModel);

            if(loginResult is OkObjectResult)
            {
                return new JsonResult(new { isSuccess = true, url = "Dashboard" });
            }
            else
            {
                return new JsonResult(new { isSuccess = false });

            }
        }
    }
}
