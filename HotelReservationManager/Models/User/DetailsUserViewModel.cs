using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Models.User
{
    public class DetailsUserViewModel
    {
        [Display(Name = "Потребителско име")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Имейл")]
        public string Email { get; set; }

        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Display(Name = "Презиме")]
        public string SecondName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        public string EGN { get; set; }

        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }
    }
}
