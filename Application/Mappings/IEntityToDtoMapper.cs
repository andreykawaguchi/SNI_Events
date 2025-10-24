namespace SNI_Events.Application.Mappings
{
    /// <summary>
    /// Classe para centralizar convers�o de entidades para DTOs
    /// Implementa um padr�o de mapeamento consistente
    /// </summary>
    public interface IEntityToDtoMapper<TEntity, TDto>
    {
        TDto MapToDto(TEntity entity);
        IEnumerable<TDto> MapToDto(IEnumerable<TEntity> entities);
    }
}
