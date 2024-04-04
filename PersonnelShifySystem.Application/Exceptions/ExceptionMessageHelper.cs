
namespace PersonnelShiftSystem.Application.Exceptions
{
    public class ExceptionMessageHelper
    {
        public const string UnexpectedSystemError = "Beklenmedik sistem hatası";
        public const string FattalSystemError = "Fattal System Error";
        public const string DataNotFound = "Kayıt bulunamadı";
        public const string TokenException = "Kullanıcı Token bilgisi geçersiz / Süresi dolmuş";
        public const string JsonParseException = "JSon datası hatalı!";
        public const string UserPasswordCanNotBeSameWithOldPasswords = "Eski şifre ve yeni şifre aynı olamaz!";
        public const string PasswordNotInvalid = "Şifre formatı geçersiz!";
        public const string OldPasswordNotMatch = "Değiştirmek istediğiniz şifre geçersiz!";
        public const string LoginError = "Hatalı kullanıcı adı / şifre";
        public const string WaitingConfirmation = "Kullanıcı onayı beklemede!";
        public const string EmailNotValid = "Email adresi formatı geçersiz!";
        public const string UserEmailNotFound = "Kullanıcıya kayıtlı email bulunamadı";
        public const string AlreadyRegisteredData = "Mevcut kayıt bulundu";
        public const string ModelStateIsNotValid = "Girilen kayıtlarda eksikler var";
        public const string AlreadyRegisteredMail = "Kayıtlı Email bulundu";
        public const string AlreadyTakenPhone = "Telefon Numarası kayıtlı";


        public static string LengthError(string FieldName) => string.Format("{0} alanı belirlenen sınırın dışında!", FieldName);
        public static string CannotEmptyField(string FieldName) => string.Format("{0} alanı boş geçilemez!", FieldName);
        public static string RequiredField(string FieldName) => string.Format("{0} alanı zorunlu!", FieldName);
        public static string IsInUse(string FieldName) => string.Format("{0} alanına kayıt yapılamaz! Kayıt kullanımda!", FieldName);
        public static string FieldNotFound(string FieldName) => string.Format("{0} bilgisi bulunamadı!", FieldName);
        public static string DataNotMatch(string FieldName) => string.Format("Does not match {0}", FieldName);
        public static string UnauthorizedAccess(int parmUserId) => string.Format("Yetkisiz Erişim. User Id : {0}", parmUserId);
        public static string UnauthorizedAccess(string parmDataName) => string.Format("Yetkisiz Erişim. Alan : {0}", parmDataName);
    }
}
