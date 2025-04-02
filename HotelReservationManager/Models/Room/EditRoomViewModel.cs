using HotelReservationManager.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Models.Room
{
    public class EditRoomViewModel
    {
        public string Id { get; set; }

        [Required]
        public uint Capacity { get; set; }

        [Required]
        public RoomType Type { get; set; }

        [Required]
        public bool Free { get; set; }

        [Required]
        public double PriceAdult { get; set; }

        [Required]
        [Display(Name = "Цена за деца")]
        public double PriceKid { get; set; }

        [Required]
        [StringLength(50)]
        public string Number { get; set; }
    }
}
