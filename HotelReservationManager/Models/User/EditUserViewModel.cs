using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Models.User
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Потребителско име")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Имейл")]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Презиме")]
        public string SecondName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "ЕГН-то трябва да е 10 цифри.")]
        public string EGN { get; set; }

        [Required]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }
    }
}
