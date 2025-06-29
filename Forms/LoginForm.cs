using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestaurantAuth.Models;
using RestaurantAuth.Models.Enums;
using RestaurantAuth.Services;

namespace RestaurantAuth.Forms
{
    public partial class LoginForm : Form
    {
        private readonly IAuthService _authService;
        private UserType _currentUserType = UserType.Employee;
        private Form _codeForm;
        private bool _isLoading = false;
        
        public LoginForm()
        {
            InitializeComponent();
            _authService = new AuthService();
            SetupForm();
        }
        
        private void SetupForm()
        {
            // Настройка формы
            this.Text = "Вход в систему ресторана";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            
            // Создание элементов управления
            CreateControls();
            
            // Начальная настройка
            SwitchToEmployeeMode();
        }
        
        private void CreateControls()
        {
            // Переключатель типа пользователя
            var userTypeGroup = new GroupBox
            {
                Text = "Тип пользователя",
                Location = new Point(20, 20),
                Size = new Size(350, 60)
            };
            
            var employeeRadio = new RadioButton
            {
                Text = "Сотрудник",
                Location = new Point(20, 25),
                Checked = true
            };
            employeeRadio.CheckedChanged += (s, e) => SwitchToEmployeeMode();
            
            var clientRadio = new RadioButton
            {
                Text = "Клиент",
                Location = new Point(150, 25)
            };
            clientRadio.CheckedChanged += (s, e) => SwitchToClientMode();
            
            userTypeGroup.Controls.Add(employeeRadio);
            userTypeGroup.Controls.Add(clientRadio);
            
            // Поля для сотрудника
            _loginLabel = new Label
            {
                Text = "Логин:",
                Location = new Point(20, 100),
                Size = new Size(100, 20)
            };
            
            _loginTextBox = new TextBox
            {
                Location = new Point(20, 125),
                Size = new Size(350, 25)
            };
            
            _passwordLabel = new Label
            {
                Text = "Пароль:",
                Location = new Point(20, 160),
                Size = new Size(100, 20)
            };
            
            _passwordTextBox = new TextBox
            {
                Location = new Point(20, 185),
                Size = new Size(350, 25),
                PasswordChar = '*'
            };
            
            // Поля для клиента
            _phoneLabel = new Label
            {
                Text = "Номер телефона:",
                Location = new Point(20, 100),
                Size = new Size(150, 20),
                Visible = false
            };
            
            _phoneTextBox = new TextBox
            {
                Location = new Point(20, 125),
                Size = new Size(350, 25),
                Visible = false,
                PlaceholderText = "+7(999)123-45-67"
            };
            
            _sendCodeButton = new Button
            {
                Text = "Прислать уведомление с кодом",
                Location = new Point(20, 160),
                Size = new Size(350, 30),
                Visible = false
            };
            _sendCodeButton.Click += SendCodeButton_Click;
            
            _codeLabel = new Label
            {
                Text = "Код подтверждения:",
                Location = new Point(20, 200),
                Size = new Size(150, 20),
                Visible = false
            };
            
            _codeTextBox = new TextBox
            {
                Location = new Point(20, 225),
                Size = new Size(350, 25),
                Visible = false,
                PlaceholderText = "Введите 6-значный код"
            };
            
            // Кнопка входа
            _loginButton = new Button
            {
                Text = "Вход",
                Location = new Point(20, 270),
                Size = new Size(350, 40),
                BackColor = Color.LightBlue
            };
            _loginButton.Click += LoginButton_Click;
            
            // Полоса загрузки
            _progressBar = new ProgressBar
            {
                Location = new Point(20, 320),
                Size = new Size(350, 20),
                Visible = false
            };
            
            _cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(20, 350),
                Size = new Size(350, 30),
                Visible = false,
                BackColor = Color.LightCoral
            };
            _cancelButton.Click += CancelButton_Click;
            
            // Добавление элементов на форму
            this.Controls.AddRange(new Control[]
            {
                userTypeGroup,
                _loginLabel, _loginTextBox, _passwordLabel, _passwordTextBox,
                _phoneLabel, _phoneTextBox, _sendCodeButton, _codeLabel, _codeTextBox,
                _loginButton, _progressBar, _cancelButton
            });
        }
        
        private void SwitchToEmployeeMode()
        {
            _currentUserType = UserType.Employee;
            
            // Показываем поля для сотрудника
            _loginLabel.Visible = true;
            _loginTextBox.Visible = true;
            _passwordLabel.Visible = true;
            _passwordTextBox.Visible = true;
            
            // Скрываем поля для клиента
            _phoneLabel.Visible = false;
            _phoneTextBox.Visible = false;
            _sendCodeButton.Visible = false;
            _codeLabel.Visible = false;
            _codeTextBox.Visible = false;
            
            // Перемещаем кнопку входа
            _loginButton.Location = new Point(20, 270);
        }
        
        private void SwitchToClientMode()
        {
            _currentUserType = UserType.Client;
            
            // Скрываем поля для сотрудника
            _loginLabel.Visible = false;
            _loginTextBox.Visible = false;
            _passwordLabel.Visible = false;
            _passwordTextBox.Visible = false;
            
            // Показываем поля для клиента
            _phoneLabel.Visible = true;
            _phoneTextBox.Visible = true;
            _sendCodeButton.Visible = true;
            _codeLabel.Visible = true;
            _codeTextBox.Visible = true;
            
            // Перемещаем кнопку входа
            _loginButton.Location = new Point(20, 270);
        }
        
        private async void SendCodeButton_Click(object sender, EventArgs e)
        {
            var phone = NormalizePhone(_phoneTextBox.Text);
            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Введите корректный номер телефона", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверяем существование клиента
            var clientExists = await _authService.ClientExistsAsync(phone);
            if (!clientExists)
            {
                // Кастомное окно с кнопками 'Продолжить' и 'Выход'
                var dialog = new Form
                {
                    Text = "Клиент не найден",
                    Size = new Size(400, 220),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    ControlBox = false
                };
                var label = new Label
                {
                    Text = "Клиент с таким номером телефона не найден в базе данных.\n" +
                           "Нажмите 'Продолжить' для получения кода или 'Выход' для отмены.",
                    Location = new Point(20, 20),
                    Size = new Size(350, 60)
                };
                var btnContinue = new Button
                {
                    Text = "Продолжить",
                    DialogResult = DialogResult.OK,
                    Location = new Point(60, 120),
                    Size = new Size(120, 35)
                };
                var btnExit = new Button
                {
                    Text = "Выход",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(220, 120),
                    Size = new Size(120, 35)
                };
                dialog.Controls.Add(label);
                dialog.Controls.Add(btnContinue);
                dialog.Controls.Add(btnExit);
                dialog.AcceptButton = btnContinue;
                dialog.CancelButton = btnExit;
                var result = dialog.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    CloseCodeWindow();
                    return;
                }
            }

            try
            {
                var code = await _authService.GenerateClientCodeAsync(phone);
                // Показываем окно с кодом
                ShowCodeWindow(code);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации кода: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ShowCodeWindow(int code)
        {
            CloseCodeWindow();

            _codeForm = new Form
            {
                Text = "Код подтверждения",
                Size = new Size(300, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                TopMost = true,
                ControlBox = false // Убираем крестик
            };
            _codeForm.FormClosing += (s, e) =>
            {
                // Запрещаем закрытие окна пользователем
                if (e.CloseReason == CloseReason.UserClosing)
                    e.Cancel = true;
            };

            var codeLabel = new Label
            {
                Text = $"Ваш код подтверждения:\n{code}",
                Location = new Point(20, 20),
                Size = new Size(250, 60),
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var copyButton = new Button
            {
                Text = "Копировать код",
                Location = new Point(20, 90),
                Size = new Size(250, 30)
            };
            copyButton.Click += (s, e) =>
            {
                Clipboard.SetText(code.ToString());
                MessageBox.Show("Код скопирован в буфер обмена", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            _codeForm.Controls.Add(codeLabel);
            _codeForm.Controls.Add(copyButton);
            _codeForm.Show();
        }
        
        private void CloseCodeWindow()
        {
            if (_codeForm != null && !_codeForm.IsDisposed)
            {
                _codeForm.Close();
                _codeForm.Dispose();
                _codeForm = null;
            }
        }
        
        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (_isLoading) return;
            
            try
            {
                _isLoading = true;
                ShowLoadingUI();
                
                if (_currentUserType == UserType.Employee)
                {
                    await AuthenticateEmployee();
                }
                else
                {
                    await AuthenticateClient();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isLoading = false;
                HideLoadingUI();
            }
        }
        
        private async Task AuthenticateEmployee()
        {
            var login = _loginTextBox.Text.Trim();
            var password = _passwordTextBox.Text;
            
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Симуляция загрузки данных
            await SimulateDataLoading();
            
            var employee = await _authService.AuthenticateEmployeeAsync(login, password);
            if (employee != null)
            {
                CloseCodeWindow();
                MessageBox.Show($"Добро пожаловать, {employee.Name}!", "Успешный вход",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Здесь можно открыть основное окно приложения
                // new MainForm(employee).Show();
                // this.Hide();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка авторизации",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async Task AuthenticateClient()
        {
            var phone = NormalizePhone(_phoneTextBox.Text);
            var codeText = _codeTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Введите корректный номер телефона", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (string.IsNullOrEmpty(codeText) || !int.TryParse(codeText, out var code))
            {
                MessageBox.Show("Введите корректный код подтверждения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Симуляция загрузки данных
            await SimulateDataLoading();
            
            var client = await _authService.AuthenticateClientAsync(phone, code);
            if (client != null)
            {
                CloseCodeWindow();
                MessageBox.Show($"Добро пожаловать, {client.Name}!", "Успешный вход",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Здесь можно открыть основное окно приложения
                // new MainForm(client).Show();
                // this.Hide();
            }
            else
            {
                MessageBox.Show("Неверный код подтверждения или код истек", "Ошибка авторизации",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async Task SimulateDataLoading()
        {
            for (int i = 0; i <= 100; i += 10)
            {
                _progressBar.Value = i;
                await Task.Delay(100);
            }
        }
        
        private void ShowLoadingUI()
        {
            _progressBar.Visible = true;
            _cancelButton.Visible = true;
            _loginButton.Enabled = false;
        }
        
        private void HideLoadingUI()
        {
            _progressBar.Visible = false;
            _cancelButton.Visible = false;
            _loginButton.Enabled = true;
            _progressBar.Value = 0;
        }
        
        private void CancelButton_Click(object sender, EventArgs e)
        {
            _isLoading = false;
            HideLoadingUI();
            _progressBar.ForeColor = Color.Red;
        }
        
        private string NormalizePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return null;
            
            // Убираем все нецифровые символы
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            
            // Если номер начинается с 8, заменяем на 7
            if (digits.Length == 11 && digits[0] == '8')
            {
                digits = '7' + digits.Substring(1);
            }
            
            // Проверяем корректность номера
            if (digits.Length == 11 && digits[0] == '7')
            {
                return $"+7({digits[1]}{digits[2]}{digits[3]}){digits[4]}{digits[5]}{digits[6]}-{digits[7]}{digits[8]}-{digits[9]}{digits[10]}";
            }
            
            return null;
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            CloseCodeWindow();
            base.OnFormClosing(e);
        }
        
        // Элементы управления
        private Label _loginLabel;
        private TextBox _loginTextBox;
        private Label _passwordLabel;
        private TextBox _passwordTextBox;
        private Label _phoneLabel;
        private TextBox _phoneTextBox;
        private Button _sendCodeButton;
        private Label _codeLabel;
        private TextBox _codeTextBox;
        private Button _loginButton;
        private ProgressBar _progressBar;
        private Button _cancelButton;
    }
} 