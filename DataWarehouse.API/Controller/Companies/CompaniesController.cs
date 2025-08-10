using DataWarehouse.API.Services.Interfaces.Companies;
using DataWarehouse.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DataWarehouse.API.Controller.Companies
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _companyService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var company = await _companyService.GetByIdAsync(id);
            return company is null ? NotFound() : Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUpdateCompanyDto dto)
        {
            var created = await _companyService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateUpdateCompanyDto dto)
        {
            var updated = await _companyService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _companyService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}