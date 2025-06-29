namespace RestaurantAuth.Models
{
    /// <summary>
    /// Базовый интерфейс для всех доменных объектов
    /// </summary>
    public interface IDomainObject
    {
        int Id { get; set; }
    }
} 