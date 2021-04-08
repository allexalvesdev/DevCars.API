using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCars.API.ViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel(int id, decimal totalCost, int idCustomer, int idCar)
        {
            Id = id;
            TotalCost = totalCost;
            IdCustomer = idCustomer;
            IdCar = idCar;
        }

        public int Id { get; private set; }
        public decimal TotalCost { get; private set; }
        public int IdCustomer { get; private set; }
        public int IdCar { get; private set; }
    }
}
