using Tutorial8.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Tutorial8.Models.DTOs;


namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString = "Server=localhost,1433;Database=apbd;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";

    public async Task<List<TripRegistrationDTO>> GetTripsByClientId(int id)
    {
        var trips = new List<TripRegistrationDTO>();
        
        
        // Queries trip id, name and date of registration and payment by client with specified id
        string query = "select t.IdTrip, t.name, registeredAt, PaymentDate from Client_Trip ct left join Trip t on t.idTrip = ct.iDTrip where ct.IdClient = @clientId;";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {

            cmd.Parameters.AddWithValue("@clientId", id);
            
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    int nameOrdinal = reader.GetOrdinal("name");
                    int registeredAtOrdinal = reader.GetOrdinal("registeredAt");
                    int paymentDateOrdinal = reader.GetOrdinal("PaymentDate");
                    
                    trips.Add(new TripRegistrationDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(nameOrdinal),
                        RegisteredAt = reader.GetInt32(registeredAtOrdinal),
                        PaymentDate = reader.GetInt32(paymentDateOrdinal)
                    });
                }
            }
        }
        
        return trips;
    }

    public async Task<int> AddClient(ClientDTO client)
    {
        // Inserts new client row
        string query = "insert into Client (FirstName, LastName, Email, Telephone, Pesel) values (@FirstName, @LastName, @Email, @Telephone, @Pesel);SELECT SCOPE_IDENTITY();";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("@LastName", client.LastName);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);

            await conn.OpenAsync();

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }    
    }

    public async Task<bool> RegisterClientToTrip(RegistreClientToTripDTO registration)
    {
        // Counts number of registrations for a specific trip
        var countQuery = "select count(*) from Client_Trip where iDTrip = @idTrip;SELECT SCOPE_IDENTITY();";
        // Queries max number of people for a specific trip
        var maxPeopleQuery = "select MaxPeople from Trip where IdTrip = @idTrip;SELECT SCOPE_IDENTITY();";
        // Inserts new Client_Trip row
        var insertQuery =
            "insert into Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate) values (@IdClient, @IdTrip, @RegisteredAt, @PaymentDate);";
        var currentDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmdCount = new SqlCommand(countQuery, conn))
        using (SqlCommand cmdMaxPeople = new SqlCommand(maxPeopleQuery, conn))    
        using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conn))
        {
            cmdCount.Parameters.AddWithValue("@idTrip", registration.TripId);
            
            cmdMaxPeople.Parameters.AddWithValue("@idTrip", registration.TripId);

            cmdInsert.Parameters.AddWithValue("@IdTrip", registration.TripId);
            cmdInsert.Parameters.AddWithValue("@idClient", registration.ClientId);
            cmdInsert.Parameters.AddWithValue("@RegisteredAt", currentDate);
            cmdInsert.Parameters.AddWithValue("@PaymentDate", currentDate);
            
            await conn.OpenAsync();
            
            var countResult = await cmdCount.ExecuteScalarAsync();
            var maxPeopleResult = await cmdMaxPeople.ExecuteScalarAsync();
            
            // Validation
            if(Convert.ToInt32(countResult) + 1 < Convert.ToInt32(maxPeopleResult)){
                await cmdInsert.ExecuteNonQueryAsync();
                return true;
            }

            return false;
        }    
    }

    public async Task<bool> RemoveClientFromTrip(RegistreClientToTripDTO registration)
    {
        // Client_Trip delete query
        var deleteQuery = "delete from Client_Trip where iDTrip = @idTrip and IdClient = @idClient;SELECT SCOPE_IDENTITY();";
        // Counts number of trip reserved by client (either 1 or 0) to check if row exists
        var countQuery = "select count(*) from Client_Trip where iDTrip = @idTrip and IdClient = @idClient;";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmdDelete = new SqlCommand(deleteQuery, conn))
        using (SqlCommand cmdCount = new SqlCommand(countQuery, conn))    
        {
            
            cmdCount.Parameters.AddWithValue("@idTrip", registration.TripId);
            cmdCount.Parameters.AddWithValue("@idClient", registration.ClientId);
            
            cmdDelete.Parameters.AddWithValue("@idTrip", registration.TripId);
            cmdDelete.Parameters.AddWithValue("@idClient", registration.ClientId);


            await conn.OpenAsync();
            
            var countResult = await cmdCount.ExecuteScalarAsync();

            if (Convert.ToInt32(countResult) == 0)
            {
                return false;
            }
            
            var result = await cmdDelete.ExecuteNonQueryAsync();
            
            return result > 0;
        }    
    }
}