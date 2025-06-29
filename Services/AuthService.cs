using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RestaurantAuth.Models;
using RestaurantAuth.Models.Enums;

namespace RestaurantAuth.Services
{
    /// <summary>
    /// Реализация сервиса авторизации
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly Dictionary<string, Employee> _employees = new Dictionary<string, Employee>();
        private readonly Dictionary<string, Client> _clients = new Dictionary<string, Client>();
        private readonly Dictionary<string, int> _clientCodes = new Dictionary<string, int>();
        private readonly Dictionary<string, DateTime> _codeExpiration = new Dictionary<string, DateTime>();
        
        public AuthService()
        {
            // Инициализация тестовых данных
            InitializeTestData();
        }
        
        public async Task<Employee?> AuthenticateEmployeeAsync(string login, string password)
        {
            return await Task.Run(() =>
            {
                if (_employees.TryGetValue(login, out var employee))
                {
                    if (VerifyPassword(password, employee.Password))
                    {
                        return employee;
                    }
                }
                return null;
            });
        }
        
        public async Task<int> GenerateClientCodeAsync(string phone)
        {
            return await Task.Run(() =>
            {
                var random = new Random();
                var code = random.Next(100000, 999999); // 6-значный код
                
                _clientCodes[phone] = code;
                _codeExpiration[phone] = DateTime.Now.AddMinutes(5); // Код действителен 5 минут
                
                return code;
            });
        }
        
        public async Task<Client?> AuthenticateClientAsync(string phone, int code)
        {
            return await Task.Run(() =>
            {
                if (_clientCodes.TryGetValue(phone, out var storedCode) &&
                    _codeExpiration.TryGetValue(phone, out var expiration))
                {
                    if (code == storedCode && DateTime.Now <= expiration)
                    {
                        // Удаляем использованный код
                        _clientCodes.Remove(phone);
                        _codeExpiration.Remove(phone);
                        // Если клиент есть в базе — возвращаем его
                        var client = _clients.Values.FirstOrDefault(c => c.Phone == phone);
                        if (client != null)
                            return client;
                        // Если клиента нет — возвращаем временного клиента
                        return new Client
                        {
                            Id = -1, // временный id
                            Name = "Гость",
                            Phone = phone
                        };
                    }
                }
                return null;
            });
        }
        
        public async Task<bool> ClientExistsAsync(string phone)
        {
            return await Task.Run(() => _clients.Values.Any(c => c.Phone == phone));
        }
        
        private void InitializeTestData()
        {
            // Создаем тестовых сотрудников
            var admin = new Employee
            {
                Id = 1,
                Name = "Администратор",
                Phone = "+7(999)123-45-67",
                EmployeeType = EmployeeType.Administrator,
                Login = "admin",
                Password = HashPassword("Admin123!"),
                EfficiencyMetric = 0
            };
            
            var waiter = new Employee
            {
                Id = 2,
                Name = "Официант",
                Phone = "+7(999)234-56-78",
                EmployeeType = EmployeeType.Waiter,
                Login = "waiter",
                Password = HashPassword("Waiter123!"),
                EfficiencyMetric = 0
            };
            
            var chef = new Employee
            {
                Id = 3,
                Name = "Повар",
                Phone = "+7(999)345-67-89",
                EmployeeType = EmployeeType.Chef,
                Login = "chef",
                Password = HashPassword("Chef123!"),
                EfficiencyMetric = 0
            };
            
            var courier = new Employee
            {
                Id = 4,
                Name = "Курьер",
                Phone = "+7(999)456-78-90",
                EmployeeType = EmployeeType.Courier,
                Login = "courier",
                Password = HashPassword("Courier123!"),
                EfficiencyMetric = 0
            };
            
            _employees[admin.Login] = admin;
            _employees[waiter.Login] = waiter;
            _employees[chef.Login] = chef;
            _employees[courier.Login] = courier;
            
            // Создаем тестового клиента
            var client = new Client
            {
                Id = 1,
                Name = "Тестовый Клиент",
                Phone = "+7(999)111-22-33",
                Address = "ул. Тестовая, д. 1",
                Street = "Тестовая",
                House = "1",
                Apartment = "1"
            };
            
            _clients[client.Phone] = client;
        }
        
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        
        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            var inputHash = HashPassword(inputPassword);
            return inputHash == hashedPassword;
        }
    }
} 