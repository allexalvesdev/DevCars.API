using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCars.API.ViewModels
{
    public class CustomerViewModel
    {
        public CustomerViewModel(int id, string fullName, string document, DateTime birthDate)
        {
            Id = id;
            FullName = fullName;
            Document = document;
            BirthDate = birthDate;
        }

        public int Id { get; private set; }
        public string FullName { get; private set; }
        public string Document { get; private set; }
        public DateTime BirthDate { get; private set; }
    }
}
