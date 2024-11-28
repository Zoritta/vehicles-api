using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vehicles_api.Models;

    public class Vehicle
    {
        public int Id { get; set;}
        public string RegistrationNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int ModelYear { get; set; }
        public string ImageUrl { get; set; }
        public int Mileage { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
    }
