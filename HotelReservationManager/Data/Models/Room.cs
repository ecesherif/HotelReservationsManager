using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Data.Models
{
    public class Room
    {
        public int Id { get; set; }
        public uint Capacity { get; set; }

        public RoomType Type { get; set; }

        public bool Free { get; set; }

        [Display(Name = "Цена за възрастен")]
        public double PriceAdult { get; set; }

        [Display(Name = "Цена за дете")]
        public double PriceKid { get; set; }

        [StringLength(50)]
        public string Number { get; set; }
    }
}
