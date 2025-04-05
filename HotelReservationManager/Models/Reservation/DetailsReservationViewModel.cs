using System.ComponentModel.DataAnnotations;
using HotelReservationManager.Data.Models;
namespace HotelReservationManager.Models.Reservation
{
    public class DetailsReservationViewModel
    {
        public string Id { get; set; }

        public string ClientId { get; set; }
        public string ClientFullName { get; set; } 

        public string ReservationId { get; set; }
        public string RoomNumber { get; set; }

        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }

        public bool Breakfast { get; set; }
        public bool AllInclusive { get; set; }

        public double TotalPrice { get; set; }

        public string CreatorName { get; set; }
    }
}
