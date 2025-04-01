using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Data.Models
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Име")]
        [StringLength(50)]
        public string FirstName { get; set; } 

        [Required]
        [Display(Name = "Фамилия")]
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

        public virtual List<ClientReservation> ClientReservations { get; set; } 
    }
}
