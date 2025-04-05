using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationManager.Models.Reservation
{
    public class EditReservationViewModel
    {
        public string Id { get; set; }
        public List<Data.Models.Room> AvaiableRooms { get; set; }
        public List<Data.Models.Client> AvaiableClients { get; set; }

        [Display(Name = "Room number")]
        public string RoomId { get; set; }
        public string CreatorId { get; set; }

        [Display(Name = "Clients")]
        public List<string> ClientIds { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in")]
        public DateTime CheckInTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out")]
        public DateTime CheckOutTime { get; set; }

        [Required]
        public bool Breakfast { get; set; }
        
        [Required]
        public bool AllInclusive { get; set; }
    }
}
