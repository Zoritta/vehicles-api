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

    [HttpGet("manufacturer/{name}")]
    public ActionResult FindVehicle(string name)
    {
        var vehicles = _vehicles.FindAll(vehicle => vehicle.Manufacturer.ToLower() == name.ToLower());
        return Ok(new { success = true, data = vehicles });
    }

    [HttpPost()]
    public ActionResult AddVehicle(Vehicle vehicle)
    {
        var newVehicle = Create(vehicle);
        return CreatedAtAction(nameof(FindVehicle), new { id = newVehicle.Id }, newVehicle);
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
        vehicle.Id = _vehicles.Count + 1;
        _vehicles.Add(vehicle);
        SaveVehicle(_vehicles);

        return vehicle;
    }

    private void Remove(int id)
    {
        var toDelete = _vehicles.SingleOrDefault(vehicle => vehicle.Id == id);
        _vehicles.Remove(toDelete);
        SaveVehicle(_vehicles);
    }

    private void Update(int id, Vehicle updatedVehicle)
    {
        var filteredList = _vehicles.FindAll(vehicle => vehicle.Id != id);

        filteredList.Add(updatedVehicle);

        SaveVehicle(filteredList);
    }

    private List<Vehicle> LoadVehicles()
    {
        return Storage<Vehicle>.ReadJson(_path);
    }

    private void SaveVehicle(List<Vehicle> list)
    {
        Storage<Vehicle>.WriteJson(_path, list);
    }
}