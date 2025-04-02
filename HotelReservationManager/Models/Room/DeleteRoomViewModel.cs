using HotelReservationManager.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Models.Room
{
    public class DeleteRoomViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Капацитет")]
        public uint Capacity { get; set; }

        [Display(Name = "Тип на стаята")]
        public RoomType Type { get; set; }

        [Display(Name = "Свободна стая")]
        public bool Free { get; set; }

        [Display(Name = "Цена")]
        public double Price { get; set; }

        [Display(Name = "Цена за деца")]
        public double PriceChildren { get; set; }

        [Display(Name = "Номер на стаята")]
        public string Number { get; set; }
    }
}
