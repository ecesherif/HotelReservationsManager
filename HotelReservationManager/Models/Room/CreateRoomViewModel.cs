using HotelReservationManager.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        [Display(Name = "Price for children")]
        public double PriceChildren { get; set; }

        [Required]
        [StringLength(50)]
        public string Number { get; set; }
    }
}
