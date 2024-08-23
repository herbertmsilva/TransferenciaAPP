namespace Transferencia.Core.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        public DateTime DataInclusao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }
    }
}