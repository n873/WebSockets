using System.ComponentModel;

namespace WebSockets.Domain.Rate.Enum
{
    public enum Country
    {
        America,
        [Description("South Africa")]
        SouthAfrica
    }
    public enum Currency
    {
        USD,
        ZAR
    }
}
