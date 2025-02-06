using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CheckAmmount
{
    /// <summary>
    /// Логика взаимодействия для BdconnectionWindow.xaml
    /// </summary>
    public partial class BdconnectionWindow : Window
    {
        //private DatabaseHelper _dbHelper;
        //public event Action<DatabaseHelper> DatabaseHelperCreated;
        public BdconnectionWindow()
        {
            InitializeComponent();
        }

        public void OpenConnection(object sender, RoutedEventArgs e)
        {
            //var _connectionString = $"User Id={usernameTextbox.Text};Password={passwordTextbox.Text};Data Source={ipTextbox.Text};";
            //_dbHelper = new DatabaseHelper(_connectionString);

            //if (_dbHelper.TestConnection())
            //{
            //    DatabaseHelperCreated?.Invoke(_dbHelper); // Вызываем событие и передаем _dbHelper
            //    MessageBox.Show("Подключение успешно установлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            //    this.Close();
            //}
            //else
            //{
            //    MessageBox.Show("Не удалось установить подключение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
    }
}
