using System;

namespace PaymentApp.Web.Areas.User.ViewModels.User
{
    public class SetStationRequest
    {
        public int StationId { get; set; }
        public Guid UserId { get; set; }
    }
}
