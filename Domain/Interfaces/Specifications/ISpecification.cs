namespace SNI_Events.Domain.Interfaces.Specifications
{
    /// <summary>
    /// Interface base para o padr�o Specification
    /// Centraliza a l�gica de query complexa no dom�nio
    /// </summary>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Crit�rios para filtrar entidades
        /// </summary>
        IQueryable<T> ToQuery(IQueryable<T> query);
    }
}
