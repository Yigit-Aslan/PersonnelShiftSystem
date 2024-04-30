using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonnelShiftSystem.Domain.Interfaces;

namespace PersonnelShiftSystem.Web.Pages
{
    public class DashboardModel : PageModel
    {
        private IBaseModel baseModel;

        public DashboardModel(IBaseModel _baseModel)
        {
            baseModel = _baseModel;
        }
        public async Task OnGet()
        {
            await baseModel.SaveVisitorInfo();

        }
    }
}
