namespace SNI_Events.Domain.Interfaces.Specifications
{
    /// <summary>
    /// Interface base para o padrão Specification
    /// Centraliza a lógica de query complexa no domínio
    /// </summary>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Critérios para filtrar entidades
        /// </summary>
        IQueryable<T> ToQuery(IQueryable<T> query);
    }
}
