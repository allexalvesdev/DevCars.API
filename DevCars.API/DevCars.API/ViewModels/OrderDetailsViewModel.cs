using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCars.API.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderDetailsViewModel(int id, int idCar, int idCostumer, decimal totalCost, List<string> extraItems)
        {
            Id = id;
            IdCar = idCar;
            IdCostumer = idCostumer;
            TotalCost = totalCost;
            ExtraItems = extraItems;
        }
        public int Id { get; private set; }
        public int IdCar { get; set; }
        public int IdCostumer { get; set; }
        public decimal TotalCost { get; set; }
        public List<string> ExtraItems { get; set; }
    }
}
