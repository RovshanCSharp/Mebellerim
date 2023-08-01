using Mebeller.Data.Context.Enums;

namespace Mebeller.Models.Media
{
    public class Notification
    {
        public string Message { get; set; }

        public NotificationType Type { get; set; }
    }
}
