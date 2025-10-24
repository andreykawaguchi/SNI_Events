namespace SNI_Events.Application.Mappings
{
    /// <summary>
    /// Classe para centralizar conversão de entidades para DTOs
    /// Implementa um padrão de mapeamento consistente
    /// </summary>
    public interface IEntityToDtoMapper<TEntity, TDto>
    {
        TDto MapToDto(TEntity entity);
        IEnumerable<TDto> MapToDto(IEnumerable<TEntity> entities);
    }
}
