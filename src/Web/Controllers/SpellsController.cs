using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.Exceptions;
using Web.Models.Spell;
using Web.Services;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpellsController(SpellService spellService, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpellViewDto>>> GetSpells()
        {
            var spells = await spellService.GetSpellsAsync();

            var spellsView = mapper.Map<IEnumerable<SpellViewDto>>(spells);

            return Ok(spellsView);
        }

        [HttpPost]
        public async Task<ActionResult<SpellViewDto>> CreateSpell([FromBody] SpellCreateDto spellCreateDto)
        {
            var spellToCreate = mapper.Map<Spell>(spellCreateDto);
            var createdSpell = await spellService.CreateSpellAsync(spellToCreate);

            var createdSpellView = mapper.Map<SpellViewDto>(createdSpell);

            return CreatedAtAction(nameof(CreateSpell), createdSpellView);
        }

        [HttpGet("{spellId}")]
        public async Task<ActionResult<SpellViewDto>> GetSpell([FromRoute] string spellId)
        {
            var existingSpell = await spellService.GetSpellAsync(spellId);

            if (existingSpell == null) return NotFound();

            var existingSpellView = mapper.Map<SpellViewDto>(existingSpell);

            return Ok(existingSpellView);
        }

        [HttpDelete("{spellId}")]
        public async Task<ActionResult> DeleteSpell([FromRoute] string spellId)
        {
            await spellService.DeleteSpellAsync(spellId);

            return Ok();
        }

        [HttpPut("{spellId}")]
        public async Task<ActionResult> UpdateClass([FromRoute] string spellId, [FromBody] SpellUpdateDto spellUpdateDto)
        {
            if (!string.Equals(spellId, spellUpdateDto.Id)) return BadRequest();

            var spellToUpdate = mapper.Map<Spell>(spellUpdateDto);

            try
            {
                await spellService.UpdateSpellAsync(spellToUpdate);
            }
            catch (SpellNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
