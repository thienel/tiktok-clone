
namespace TikTokClone.Domain.Entities
{
    public class EmailVerification
    {
        public int Id { get; private set; }
        public string Email { get; init; }
        public string Code { get; private set; }
        public DateTime Expiry { get; private set; }
        public DateTime LastTimeGenerateCode { get; private set; }
        public const int ExpiryTimeInHours = 48;

        public EmailVerification(string email)
        {
            Email = email;
            Code = GenerateRandomSixDigitCode();
            Expiry = DateTime.UtcNow.AddHours(48);
            SetGenerateCodeTime();
        }

        public bool IsVertificationCodeActive()
        {
            return Expiry > DateTime.UtcNow;
        }

        public bool GenerateNewCode()
        {
            if (DateTime.UtcNow.Subtract(LastTimeGenerateCode) <= TimeSpan.FromSeconds(60)) return false;

            Code = GenerateRandomSixDigitCode();
            Expiry = DateTime.UtcNow.AddMinutes(ExpiryTimeInHours);
            SetGenerateCodeTime();
            return true;
        }

        private string GenerateRandomSixDigitCode()
        {
            var random = new Random();
            return random.Next(0, 1_000_000).ToString("D6");
        }

        private void SetGenerateCodeTime()
        {
            LastTimeGenerateCode = DateTime.UtcNow;
        }
    }
}
