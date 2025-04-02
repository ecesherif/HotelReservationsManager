using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Models.Client
{
    public class DeleteClientViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Възрастен")]
        public bool Mature { get; set; }
    }
}
