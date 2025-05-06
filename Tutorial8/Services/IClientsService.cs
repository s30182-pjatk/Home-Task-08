using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<TripRegistrationDTO>> GetTripsByClientId(int id);
    Task<int> AddClient(ClientDTO client);
    Task<bool> RegisterClientToTrip(RegistreClientToTripDTO registration);
    Task<bool> RemoveClientFromTrip(RegistreClientToTripDTO registration);
}