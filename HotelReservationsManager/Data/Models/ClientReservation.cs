namespace HotelReservationsManager.Data.Models
{
    public class ClientReservation
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public required Client Client { get; set; }

        public int ReservationId { get; set; }
        public required Reservation Reservation { get; set; }
    }
}
