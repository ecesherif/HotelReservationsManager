using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Models.Client
{
    public class DetailsClientViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Възрастен")]
        public bool Mature { get; set; }

        public virtual List<Data.Models.Reservation> Reservations { get; set; }
    }
}
