using System.Threading.Tasks;
using RestaurantAuth.Models;

namespace RestaurantAuth.Services
{
    /// <summary>
    /// Интерфейс для сервиса авторизации
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Авторизация сотрудника по логину и паролю
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Сотрудник или null если авторизация не удалась</returns>
        Task<Employee?> AuthenticateEmployeeAsync(string login, string password);
        
        /// <summary>
        /// Генерация кода для клиента
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns>Сгенерированный код</returns>
        Task<int> GenerateClientCodeAsync(string phone);
        
        /// <summary>
        /// Авторизация клиента по номеру телефона и коду
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <param name="code">Код подтверждения</param>
        /// <returns>Клиент или null если авторизация не удалась</returns>
        Task<Client?> AuthenticateClientAsync(string phone, int code);
        
        /// <summary>
        /// Проверка существования клиента по номеру телефона
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns>True если клиент существует</returns>
        Task<bool> ClientExistsAsync(string phone);
    }
} 