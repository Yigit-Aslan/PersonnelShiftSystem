using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using PersonnelShiftSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wangkanai.Detection.Services;

namespace PersonnelShiftSystem.Infrastructure.Repository
{
    public class BaseModel : IBaseModel
    {
        private IHttpContextAccessor _httpContextAccessor { get; set; }

        private IHttpContextFactory _httpFactory { get; set; }

        //private IhyaTekstilCore.AppConfig.IhyaTekstilConfiguration _appConfig;

        //public BaseUIStatusModel UIStatus { get; set; }

        //private IConfiguration _configuration;

        public IMapper Mapper { get; set; }

        public IUnitOfWork BaseUnitOfWork { get; set; }

        private readonly IDetectionService _detectionService;
        public BaseModel(IHttpContextAccessor httpContextAccessor,
                         IMapper mapper,
                         IUnitOfWork unitOfWork,
                         IDetectionService detectionService)
        {
            _httpContextAccessor = httpContextAccessor;
            Mapper = mapper;
            BaseUnitOfWork = unitOfWork;
            _detectionService = detectionService;
            //_appConfig = AppConfig.Value;

            SaveVisitorInfo();
        }


        public void WriteToSession(string key, string value)
        {
            _httpContextAccessor.HttpContext.Session.SetString(key, value);
        }

        public string ReadFromSession(string key)
        {
            string result = string.Empty;


            var isRetrieved = _httpContextAccessor.HttpContext!.Session.TryGetValue(key, out byte[] value);

            if (isRetrieved)
                result = Encoding.UTF8.GetString(value, 0, value.Length);

            return result;
        }


        public async void ClaimCookies()
        {
            var claims = new List<Claim>()
            {
                new Claim("UserId", ReadFromSession("UserId")),
                new Claim(ClaimTypes.Role, ReadFromSession("RoleName")),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        //public string GetLocalizedContent(string param)
        //{
        //    return _localizer.Get(param);
        //}

        //public string GetAppConfig(BookStore.AppConfig.AppConfigEnum appConfig)
        //{
        //    if (appConfig == BookStore.AppConfig.AppConfigEnum.RootPath)
        //        return _appConfig.RootPath;


        //    return string.Empty;
        //}


        public int? ItemNullInt()
        {
            int? nullInt = null;
            return nullInt;
        }

        public byte ItemActive() => Convert.ToByte(true);

        public byte ItemPassive() => Convert.ToByte(false);

        public void SaveVisitorInfo()
        {
            //CultureInfo ci = CultureInfo.InstalledUICulture;

            ////var path =  _httpContextAccessor.HttpContext.Request.Path.Value;

            //Visitorinfo visitorInfoViewModel = new Visitorinfo()
            //{
            //    Id = GetGuidId(),
            //    UserId = ReadFromSession("UserId"),
            //    OperatingSystemName = _detectionService.Platform.Name.ToString(),
            //    OperatingSystemProcessor = _detectionService.Platform.Processor.ToString(),
            //    OperatingSystemVersionMajor = Convert.ToInt32(_detectionService.Platform.Version.Major),
            //    OperatingSystemLanguage = ci.Name.ToString(),
            //    BrowserName = _detectionService.Browser.Name.ToString(),
            //    BrowserVersionMajor = Convert.ToInt32(_detectionService.Browser.Version.Major),
            //    BrowserLanguage = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"].ToString().Split(";").FirstOrDefault()?.Split(",").FirstOrDefault(),
            //    DeviceType = _detectionService.Device.Type.ToString(),
            //    EngineType = _detectionService.Engine.Name.ToString(),
            //    UserAgent = _detectionService.UserAgent.ToString(),
            //    UserIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
            //    VisitDate = DateTime.Now,
            //    Path = _httpContextAccessor.HttpContext.Request.Path.ToString()
            //};

            //BaseUnitOfWork.VisitorInfoRepository.Add(visitorInfoViewModel);
            //BaseUnitOfWork.VisitorInfoRepository.SaveChanges();
        }

        public string GetGuidId()
        {
            return Guid.NewGuid().ToString();
        }


    }
}
