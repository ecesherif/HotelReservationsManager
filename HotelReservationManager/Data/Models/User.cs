using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationManager.Data.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Телефон")]
        public override string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Потребителско име")]
        public override string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string SecondName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Невалидно ЕГН!")]
        public string EGN { get; set; }

        [Required]
        [Display(Name = "Дата на назначаване")]
        public DateTime HireDate { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Дата на освобождаване")]
        public DateTime? FireDate { get; set; }
    }
}
