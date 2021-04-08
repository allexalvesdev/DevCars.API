using DevCars.API.Entities;
using DevCars.API.InputModels;
using DevCars.API.Persistence;
using DevCars.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DevCars.API.Controllers
{
    //Rota padrao
    [Route("api/cars")]
    public class CarsController : ControllerBase
    {

        private readonly DevCarsDbContext _dbcontext;
        public CarsController(DevCarsDbContext dbContext)
        {
            _dbcontext = dbContext;
        }

        // GET api/cars
        /// <summary>
        ///     Lista de Carros Disponíveis para Venda
        /// </summary>
        /// <returns>Lista Criada.</returns>
        /// <responde code="200">Lista informada com Sucesso.</responde>
        [HttpGet("GetCarsAvailable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCarsAvailable()
        {
            //Retornar lista de CarItemViewModel
            var cars = _dbcontext.Cars;

            var carsViewModel = cars
                .Where(c=>c.Status == CarStatusEnum.Available)
                .Select(c => new CarItemViewModel(
                    c.Id,
                    c.Brand,
                    c.Model,
                    c.Price))
                .ToList();

            return Ok(carsViewModel);

        }

        // GET api/cars
        /// <summary>
        ///     Lista de Carros Vendidos
        /// </summary>
        /// <returns>Lista Criada.</returns>
        /// <responde code="200">Lista Informada com Sucesso.</responde>
        [HttpGet("GetCarsSold")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCarsSold()
        {
            //Retornar lista de CarItemViewModel
            var cars = _dbcontext.Cars;

            var carsViewModel = cars
                .Where(c => c.Status == CarStatusEnum.Sold)
                .Select(c => new CarItemViewModel(
                    c.Id,
                    c.Brand,
                    c.Model,
                    c.Price))
                .ToList();

            return Ok(carsViewModel);

        }

        // GET api/cars
        /// <summary>
        ///     Lista de Carros Suspensos para Venda
        /// </summary>
        /// <returns>Lista Criada.</returns>
        /// <responde code="200">Lista Informada com Sucesso.</responde>
        [HttpGet("GetCarsSuspended")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCarsSuspended()
        {
            //Retornar lista de CarItemViewModel
            var cars = _dbcontext.Cars;

            var carsViewModel = cars
                .Where(c => c.Status == CarStatusEnum.Suspended)
                .Select(c => new CarItemViewModel(
                    c.Id,
                    c.Brand,
                    c.Model,
                    c.Price))
                .ToList();

            return Ok(carsViewModel);

        }


        // GET api/cars/1
        /// <summary>
        ///     Detalhar Carro
        /// </summary>
        /// <param name="id"> Id do Carro.</param>
        /// <returns>Objeto Encontrado com Sucesso.</returns>
        /// <response code="200">Carro Encontrado.</response>
        /// <response code="404">Carro Não Encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            //Se o carro não existir de indentificador ID, retorna NotFound, senão, OK
            var car = _dbcontext.Cars.SingleOrDefault(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            var carDetailsViewModel = new CarDetailsViewModel(

                car.Id,
                car.Brand,
                car.Model,
                car.VinCode,
                car.Year,
                car.Price,
                car.Color,
                car.ProductionDate

                );

            return Ok(carDetailsViewModel);
        }

        // POST api/cars
        /// <summary>
        /// Cadastrar Carro
        /// </summary>
        /// <remarks>
        /// Requisição de Exemplo:
        /// {
        ///     "brand": "Honda",
        ///     "model": "Civic",
        ///     "vinCode": "ABC123",
        ///     "year": 2021,
        ///     "price": 120000,
        ///     "color": "Prata",
        ///     "productionModel": "2021-01-01"
        /// }
        /// </remarks>
        /// <param name="model">Dados de Entrada do Carro.</param>
        /// <returns>Carro Criado.</returns>
        /// <response code="201">Carro Criado com Sucesso.</response>
        /// <response code="400">Dados Inválidos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] AddCarInputModel model)
        {
            //Se o cadastro funcionar, retorna created (201)
            //Se os dados de entrada estiverem incorretos, retrorna BAD REQUEST (400)
            //Se o cadastro funcionar, mas nao tiver uma api de consular, retorna No Content 204
            if (model.Model.Length > 50)
            {
                return BadRequest("Modelo não pode ter mais de 50 caracteres.");
            }

            var car = new Car(

                model.VinCode,
                model.Brand,
                model.Model,
                model.Year,
                model.Price,
                model.Color,
                model.ProductionModel);

            _dbcontext.Cars.Add(car);

            _dbcontext.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = car.Id },
                model
                );
        }

        //PUT api/cars/1
        /// <summary>
        ///     Atualização do Carro
        /// </summary>
        /// <remarks>
        ///     Requisição para Atualizar:
        ///     {
        ///         "color": "Preto",
        ///         "price": 100000
        ///     }
        /// </remarks>
        /// <param name="id"> Id do Carro.</param>
        /// <param name="model"></param>
        /// <returns>Carro Atualizado com Sucesso</returns>
        /// <response code="200">Carro Atualizado.</response>
        /// <response code="404">Carro Não Encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Put(int id, [FromBody] UpdateCarInputModel model)
        {

            //Se a atualizacao funcionar, retorna 204 No Content
            //Se dados de estiverem incorretos, retorna 400
            //se nao estiver existir o id retornar 404


            var car = _dbcontext.Cars.SingleOrDefault(c => id == c.Id);

            if (car == null)
            {
                return NotFound();
            }


            car.Update(model.Color, model.Price);

            _dbcontext.SaveChanges();

            return Ok();
        }


        //Delete api/cars/2
        /// <summary>
        ///     Deletar um Carro
        /// </summary>
        /// <param name="id"> Id do Carro.</param>
        /// <returns>Objeto Excluido com Sucesso.</returns>
        /// <response code="204">Carro Excluido com Sucesso.</response>
        /// <response code="404">Carro Não Encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            //Se nao existir, retorna not found 404
            //se for com sucessor, retorna No Content 204

            var car = _dbcontext.Cars.SingleOrDefault(c => id == c.Id);

            if (car == null)
            {
                return NotFound();
            }

            car.SetAsSuspended();

            _dbcontext.SaveChanges();
            return NoContent();
        }
    }
}
