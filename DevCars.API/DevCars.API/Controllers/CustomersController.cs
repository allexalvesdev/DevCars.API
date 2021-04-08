using DevCars.API.Entities;
using DevCars.API.InputModels;
using DevCars.API.Persistence;
using DevCars.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DevCars.API.Controllers
{
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly DevCarsDbContext _dbcontext;

        public CustomersController(DevCarsDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        //GET api/customer
        /// <summary>
        ///     Lista de Clientes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {

            var costumers = _dbcontext.Customers;

            var customerViewModel = costumers.Select(c => new CustomerViewModel(c.Id, c.FullName, c.Document, c.BirthDate));

            return Ok(customerViewModel);
        }

        //GET api/customer/getlistorder
        /// <summary>
        ///     Lista de Pedidos
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetListOrder")]
        public IActionResult GetListOrder()
        {

            var orders = _dbcontext.Orders;

            var orderViewModel = orders.Select(o => new OrderViewModel(o.Id, o.TotalCost, o.IdCustomer, o.IdCar));

            return Ok(orderViewModel);
        }

        //Get api/customer/id
        /// <summary>
        ///     Detalhar Cliente
        /// </summary>
        /// <param name="id">Id do Cliente</param>
        /// <returns>Cliente encontrado com Sucesso.</returns>
        /// <response code="200">Cliente Encontrado.</response>
        /// <response code="404">Cliente Não Encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var customers = _dbcontext.Customers.SingleOrDefault(c => c.Id == id);

            if (customers == null)
            {
                return NotFound();
            }

            var customerViewModel = new CustomerViewModel(

                customers.Id,
                customers.FullName,
                customers.Document,
                customers.BirthDate

                );

            return Ok(customerViewModel);
        }


        // GET api/customers/2/orders/3
        /// <summary>
        ///     Detalhar Pedido
        /// </summary>
        /// <param name="id">Id do Cliente</param>
        /// <param name="orderid">Id do Pedido</param>
        /// <returns>Pedido Detalhado.</returns>
        /// <response code="200">Pedido Detalhado com Sucesso.</response>
        /// <response code="404">Pedido Não Encontrado.</response>
        [HttpGet("{id}/orders/{orderid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetOrder(int id, int orderid)
        {
            //var customer = _dbcontext.Customers.SingleOrDefault(c => c.Id == id);
            //if (customer == null)
            //{
            //    return NotFound();
            //}
            //var order = customer.Orders.SingleOrDefault(o => o.Id == orderid);

            var order = _dbcontext.Orders
                .Include(o => o.ExtraItems)
                .SingleOrDefault(o => o.Id == orderid);

            if (order == null)
            {
                return NotFound();
            }

            var extraItems = order
                .ExtraItems
                .Select(e => e.Description)
                .ToList();

            var orderViewModel = new OrderDetailsViewModel(order.Id, order.IdCar, order.IdCustomer, order.TotalCost, extraItems);

            return Ok(orderViewModel);
        }

        // POST api/customers
        /// <summary>
        ///     Cadastro do Cliente
        /// </summary>
        /// <remarks>
        ///     Requisição para o Cadastro:
        ///     {
        ///         "fullName": "Fulano da Silva Alves",
        ///         "document": "ABC123",
        ///         "birthDate": "1990-01-01",
        ///         "dateTime": "2021-04-08"
        ///     }
        /// </remarks>
        /// <param name="model"></param>
        /// <returns>Cliente Cadastrado.</returns>
        /// <response code="201">Cliente Cadastrado com Sucesso.</response>
        /// <response code="400">Houve um Erro ao Cadastrar o Cliente.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] AddCustomerInputModel model)
        {

            if (model.FullName == null)
            {
                return BadRequest("Campo Nome Completo é obrigatório");
            }

            var customer = new Customer(

                model.FullName,
                model.Document,
                model.BirthDate

                );

            _dbcontext.Customers.Add(customer);

            _dbcontext.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id },
                model
                );
        }

        // Post api/customers/2/orders
        /// <summary>
        ///    Cadastrar Pedido - Necessário que já tenha criado um Cliente e um Carro.
        /// </summary>
        /// <remarks>
        ///     Requisição para criar um Pedido:
        ///     {
        ///         "idCar": 1,
        ///         "idCustomer": 1,
        ///         "extraItems": [
        ///         {
        ///             "description": "Seguro do Veículo",
        ///             "price": 5000
        ///         }
        ///       ]
        ///     }
        /// </remarks>
        /// <param name="id">Id Pedido</param>
        /// <param name="model"></param>
        /// <returns>Pedido Criado</returns>
        /// <response code="201">Pedido Criado com Sucesso.</response>
        /// <response code="400">Não foi Possível registrar o Pedido.</response>
        [HttpPost("{id}/orders")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult PostOrder(int id, [FromBody] AddOrderInputModel model)
        {
            var extraItems = model.ExtraItems
                .Select(e => new ExtraOrderItem(e.Description, e.Price))
                .ToList();

            var car = _dbcontext.Cars.SingleOrDefault(c => c.Id == model.IdCar);

            var order = new Order(model.IdCar, model.IdCustomer, car.Price, extraItems);

            var listOrder = _dbcontext.Orders.Select(o => o.Id);
            var orderid = _dbcontext.Orders.SingleOrDefault(o => o.Id == id);

            if (orderid == null)
            {
                car.SetAsSold();

                _dbcontext.Add(order);

                _dbcontext.SaveChanges();

                return CreatedAtAction(
                    nameof(GetOrder),
                    new { id = order.IdCustomer, orderid = order.Id },
                    model
                    );
            }

            return BadRequest($"Já existe um Pedido Registado com o Id= {id}");

            //var customer = _dbcontext.Customers.SingleOrDefault(c => c.Id ==model.IdCustomer);
            //customer.Purchase(order);


        }

    }
}
