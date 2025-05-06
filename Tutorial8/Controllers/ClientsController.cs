using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly IClientsService _clientsService;

    public ClientsController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }
    
    // Returns list of all trips reserved by a client
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetTripsByClientId(int id)
    {
        var trips = await _clientsService.GetTripsByClientId(id);
        return trips.Count != 0 ? Ok(trips) : NotFound();
    }

    // Adds new client
    [HttpPost]
    public async Task<IActionResult> AddClient([FromBody] ClientDTO client)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var clientId = await _clientsService.AddClient(client);
        return Ok(clientId);
    }
    
    // Create a reservations by a client of a trip
    [HttpPut("{clientId}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClientToTrip(int clientId, int tripId)
    {
        var result = await _clientsService.RegisterClientToTrip(new RegistreClientToTripDTO()
        {
            ClientId = clientId,
            TripId = tripId
        });
        
        return result ? Ok() : BadRequest();
    }
    
    // Deletes a reservation by a client of a trip
    [HttpDelete("{clientId}/trips/{tripId}")]
    public async Task<IActionResult> RemoveClientFromTrip(int clientId, int tripId)
    {
        var result = await _clientsService.RemoveClientFromTrip(new RegistreClientToTripDTO()
        {
            ClientId = clientId,
            TripId = tripId
        });
        return result ? Ok() : BadRequest();
    }
}