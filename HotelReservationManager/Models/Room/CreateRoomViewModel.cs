using HotelReservationManager.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Models.Room
{
    public class CreateRoomViewModel
    {
        [Required]
        public uint Capacity { get; set; }

        [Required]
        public RoomType Type { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Цена за деца")]
        public double PriceChildren { get; set; }

        [Required]
        [StringLength(50)]
        public string Number { get; set; }
    }
}
