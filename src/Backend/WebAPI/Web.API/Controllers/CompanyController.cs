using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase //ApiControllerBase
    {
        private readonly IMediator _mediator;

        public CompanyController(IMediator mediator)
        {
            _mediator = mediator;
        }


        //[HttpPost]
        //public async Task<ActionResult<Result<CreatedCompanyDto>>> CreateCompanyCommand(CreateCompanyCommand command)
        //{
        //    return await _mediator.Send(command);
        //}


        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Result<NoContent>>> DeleteCompanyCommand(int id)
        //{
        //    await _mediator.Send(new DeleteCompanyCommand(id));
        //    return NoContent();
        //}


        //[HttpPut]
        //public async Task<ActionResult<Result<NoContent>>> UpdateCompanyCommand([FromBody] UpdateCompanyCommand command)
        //{
        //    await _mediator.Send(command);

        //    return NoContent();
        //}
    }
}
