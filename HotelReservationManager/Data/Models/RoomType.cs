using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationManager.Data.Models
{
    public enum RoomType
    {
        [Display(Name = "Two beds")]
        TwoBeds,
        Appartment,
        [Display(Name = "Double bed")]
        DoubleBed, 
        Pentahouse,
        Maisonette,
        [Display(Name = "Single")]
        Single
    }
}
