using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Models.Client
{
    public class EditClientViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Име")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Възрастен")]
        public bool Mature { get; set; }
    }
}
