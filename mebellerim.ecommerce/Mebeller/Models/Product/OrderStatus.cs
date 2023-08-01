using System.ComponentModel;

namespace Mebeller.Models.Product
{
    public enum OrderStatus
    {
        [Description("Canceled")] Canceled,
        [Description("Pending payment")] AwaitingPayment,
        [Description("In progress")] Doing,
        [Description("Pending review")] AwaitingReview,
        [Description("completed")] Completed,
        [Description("returned")] Returned
    }
}