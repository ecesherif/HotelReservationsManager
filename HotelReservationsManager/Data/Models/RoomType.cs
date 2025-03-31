using System.ComponentModel.DataAnnotations;

namespace HotelReservationsManager.Data.Models
{
    public enum RoomType
    {
        [Display(Name = "Две легла")]
        TwoBeds,

        [Display(Name = "Апартамент")]
        Appartment,

        [Display(Name = "Двойно легло")]
        DoubleBed,

        [Display(Name = "Пентхаус")]
        Pentahouse,

        [Display(Name = "Мезонет")]
        Maisonette
    }
}
