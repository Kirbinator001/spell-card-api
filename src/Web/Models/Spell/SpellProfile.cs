using AutoMapper;

namespace Web;

public class SpellProfile : Profile
{
    public SpellProfile()
    {
        CreateMap<Spell, SpellEntity>();
        CreateMap<SpellEntity, Spell>();
        CreateMap<Spell, SpellViewDto>();
        CreateMap<SpellCreateDto, Spell>();
        CreateMap<SpellUpdateDto, Spell>();
    }
}