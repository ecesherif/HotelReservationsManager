﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationManager.Models.Reservation
{
    public class CreateReservationViewModel
    {
        public List<Data.Models.Room> AvaiableRooms { get; set; } = new List<Data.Models.Room>();
        public List<Data.Models.Client> AvaiableClients { get; set; } = new List<Data.Models.Client>();

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
        [Display(Name = "All inclusive")]
        public bool AllInclusive { get; set; }
    }
}
