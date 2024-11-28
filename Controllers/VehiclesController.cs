using Microsoft.AspNetCore.Mvc;
using vehicles_api.Models;
using vehicles_api.Utilities;

namespace vehicles_api.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _path;
    private readonly List<Vehicle> _vehicles;

    public VehiclesController(IWebHostEnvironment environment)
    {
        _environment = environment;
        _path = string.Concat(_environment.ContentRootPath, "/Data/vehicles.json");
        _vehicles = LoadVehicles();

    }

    [HttpGet()]
    public ActionResult ListVehicles()
    {

        return Ok(new { success = true, data = _vehicles });
    }

    [HttpGet("{id}")]
    public ActionResult FindVehicle(int id)
    {

        var vehicle = _vehicles.SingleOrDefault(v => v.Id == id);

        if (vehicle is not null)
        {
            return Ok(new { success = true, data = vehicle });
        }

        return NotFound(new { success = false, message = $"Tyvärr kunde vi inte hitta någon bil med id: {id}" });
    }

    [HttpPost()]
    public ActionResult AddVehicle(Vehicle vehicle)
    {
        // 1. Vi måste få in det data som ska sparas✅
        var newVehicle = Create(vehicle);

        // 4. Returnera korrekt statuskod till avsändaren (201 Created)...✅
        // HATEOS...
        // Mer korrekt sätta att returnera statuskod 201...
        // CreatedAtAction gör följande åt oss
        // 1. Skapar en statuskod 201,
        // 2. Första argumentet skapa en url för metoden FindVehicle
        //    http://localhost:5001/api/v1/vehicles/
        //    Andra argumentet skapar argumentet som FindVehicle behöver (int id)
        //    http://localhost:5001/api/v1/vehicles/20
        //    tredje argumentet kommer att returnera vår nya bil i body delen av http resultet...
        return CreatedAtAction(nameof(FindVehicle), new { id = newVehicle.Id }, newVehicle);
        // return Created("http://localhost:5001/api/v1/vehicles/" + vehicle.Id, vehicle);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteVehicle(int id)
    {
        Remove(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    public ActionResult UpdateVehicle(Vehicle vehicle, int id)
    {
        Update(id, vehicle);
        return NoContent();
    }

    private Vehicle Create(Vehicle vehicle)
    {

        // Skapa ett id genom att ta antalet bilar i listan och addera 1...✅
        vehicle.Id = _vehicles.Count + 1;
        // Lägga till vår nya bil i listan...✅
        _vehicles.Add(vehicle);

        // 3. Skriv ner den uppdaterade listan till json filen...✅
        Storage<Vehicle>.WriteJson(_path, _vehicles);

        return vehicle;
    }

    private void Remove(int id)
    {

        // 1. Leta upp bilen som ska raderas...
        // Lambda (vehicle => vehicle.Id == id)
        // första delen i lambda är en variabel(ni hittar på den själv)
        // det som står till höger om pilen(=>) är funktionen som ska köras...
        var toDelete = _vehicles.SingleOrDefault(vehicle => vehicle.Id == id);
        // 2. Ta bort bilen ur listan...
        // Vi använder en inbyggda metoden Remove som finns i typen List i .NET.
        _vehicles.Remove(toDelete);

        // 3. Uppdatera json dokumentet med den nya listan...
        Storage<Vehicle>.WriteJson(_path, _vehicles);
    }

    private void Update(int id, Vehicle updatedVehicle)
    {
        // 2. Filtrera listan av bilar och bara returnera de bilar som inte har
        //    id för den bil vi ska uppdatera...
        // Skapar en ny lista minus den bil som ska uppdateras.
        var filteredList = _vehicles.FindAll(vehicle => vehicle.Id != id);

        // 3. Lägg till bilen som är uppdaterad till den filtrerade listan...
        filteredList.Add(updatedVehicle);

        // 4. Skriv ner den nya listan...
        Storage<Vehicle>.WriteJson(_path, filteredList);
    }

    private List<Vehicle>LoadVehicles()
    {
        return Storage<Vehicle>.ReadJson(_path);
    }
}