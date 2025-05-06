namespace Tutorial8.Models.DTOs;

public class TripDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<CountryDTO> Countries { get; set; }
}

public class CountryDTO
{
    public string Name { get; set; }
}

public class TripRegistrationDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int RegisteredAt { get; set; }
    public int PaymentDate { get; set; }
}

public class ClientDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Pesel { get; set; }
}

public class RegistreClientToTripDTO
{
    public int ClientId { get; set; }
    public int TripId { get; set; }
}