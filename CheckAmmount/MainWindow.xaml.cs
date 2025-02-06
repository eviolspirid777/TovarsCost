using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OfficeOpenXml;
using System.IO;
using System.Formats.Asn1;
using System.Xml.Linq;
using System.Threading;
using System.Data;


namespace CheckAmmount
{
    public partial class MainWindow : Window
    {
        static List<Product> products = new List<Product>();
        static List<List<(string Name, int Count, double Cost)>> combinations = new List<List<(string Name, int Count, double Cost)>>();
        CancellationTokenSource cts = new CancellationTokenSource();

        //public DatabaseHelper CurrentDbHelper { get; private set; }
        DataTable table;

        public MainWindow()
        {
            InitializeComponent();

            if (products.Count == 0)
            {
                SubmitButton.IsEnabled = false;
            }
        }

        public void OpenBdConnectionWindow(object sedner, RoutedEventArgs e)
        {
            BdconnectionWindow bdWindow = new BdconnectionWindow();
            //bdWindow.DatabaseHelperCreated += OnDatabaseHelperCreated;
            bdWindow.Show();
        }

        //private void OnDatabaseHelperCreated(DatabaseHelper dbHelper)
        //{
        //    CurrentDbHelper = dbHelper; // Если у вас есть свойство для хранения экземпляра
        //    string query = dbHelper.SetQuery();
        //    table = dbHelper.ExecuteQuery(query);
        //    if (table == null || table.Rows.Count == 0)
        //    {
        //        MessageBox.Show("Ошибка при выборке данных. Таблица пуста", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return;
        //    }
        //    ValuesTable.ItemsSource = table.DefaultView;
        //}

        public void SaveFileClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Excel Workbook|*.xlsx",
                Title = "Сохранить файл Excel"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Информация о товарах");
                    worksheet.Cells[1, 1].Value = "Название";
                    worksheet.Cells[1, 2].Value = "Колличество";
                    worksheet.Cells[1, 3].Value = "Цена";
                    for (int i = 0; i < CombinationsList.Items.Count; i++)
                    {
                        var item = CombinationsList.Items[i];
                        
                        worksheet.Cells[i + 2, 1].Value = ((dynamic)item).Name;
                        worksheet.Cells[i + 2, 2].Value = ((dynamic)item).Count;
                        worksheet.Cells[i + 2, 3].Value = ((dynamic)item).Cost;
                    }
                    FileInfo excelFile = new FileInfo(filePath);
                    package.SaveAs(excelFile);
                }
                MessageBox.Show("Файл успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void OpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|XML files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "Open XML File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    // Определяем тип файла по расширению
                    string fileExtension = Path.GetExtension(filePath).ToLower();

                    switch (fileExtension)
                    {
                        case ".json":
                        {
                            HandleJsonFile(filePath);
                            SubmitButton.IsEnabled = true;
                            break;
                        }
                        case ".xml":
                        {
                            HandleXmlFile(filePath);
                            SubmitButton.IsEnabled = true;
                            break;
                        }
                        default:
                        {
                            MessageBox.Show("Неподдерживаемый формат файла!", "Ошибка");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка");
                }
            }
        }

        private void HandleJsonFile(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                products = JsonSerializer.Deserialize<List<Product>>(jsonString);

                // Обновите источник данных для вашего представления
                ValuesTable.ItemsSource = products;

                MessageBox.Show("JSON файл загружен!", "Информация");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке JSON файла: {ex.Message}", "Ошибка");
            }
        }

        private void HandleXmlFile(string filePath)
        {
            try
            {
                XDocument xDoc = XDocument.Load(filePath);
                products = (from p in xDoc.Descendants("ROW")
                            select new Product
                            {
                                Name = (string)p.Element("PRODUCT_ID"),
                                Quantity = (int)p.Element("INCOME_COUNT"),
                                Price = Convert.ToDouble(((string)p.Element("INCOME_PRICE_NO_NDS")))
                            }).ToList();

                ValuesTable.ItemsSource = products;

                MessageBox.Show("XML файл обработан успешно!", "Информация");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке XML файла: {ex.Message}", "Ошибка");
            }
        }

        private void CombinationsList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (CombinationsList.SelectedItem != null)
                {
                    var selectedCombination = CombinationsList.SelectedItem;
                    string dataToCopy = $"Name: {((dynamic)selectedCombination).Name}, Count: {((dynamic)selectedCombination).Count}, Cost: {((dynamic)selectedCombination).Cost}";
                    Clipboard.SetText(dataToCopy);
                    MessageBox.Show("Скопировано в буфер обмена: " + dataToCopy); // Уведомление пользователя
                }
            }
        }

        public void CombinationClick(object sender, RoutedEventArgs e)
        {
            ammountTextBox.Text =  ammountTextBox.Text.Replace(".", ",");
            var ifPosible = Double.TryParse(ammountTextBox.Text, out double Cost);
            if (ifPosible)
            {
                SubmitButton.IsEnabled = false;

                cts = new CancellationTokenSource(); // Создаем новый токен отмены
                CancellationToken token = cts.Token;

                Task.Run(() =>
                {
                    combinations.Clear();
                    FindCombinationsDynamic(products, Cost, combinations);

                    Dispatcher.Invoke(() =>
                    {
                        if (combinations.Count == 0)
                        {
                            // Если комбинации не найдены
                            SaveFileMenuItem.IsEnabled = false;
                            CombinationsList.ItemsSource = new List<object> { new { Name = "Записи не найдены", Count = "", Cost = "" } };
                            CombinationCountLabel.Content = "Найдено 0 записей";

                        }
                        else
                        {
                            // Формируем результаты, если комбинации найдены
                            SaveFileMenuItem.IsEnabled = true;
                            var results = combinations.Select(combination => new
                            {
                                Name = string.Join(", ", combination.Select(c => $"{c.Count}x {c.Name}")),
                                Count = combination.Sum(c => c.Count),
                                Cost = combination.Sum(c => c.Cost).ToString("C")
                            }).ToList();

                            CombinationsList.ItemsSource = results;
                            CombinationCountLabel.Content = $"Найдено {results.Count} записей";
                        }

                        SubmitButton.IsEnabled = true;
                    });
                }, token);
            }
            else
            {
                MessageBox.Show("Вы ввели не число или указали точку вместо запятой!", "Ошибка");
            }
        }

        static int combinationCount = 0; // Счетчик для отслеживания количества комбинаций
        static readonly int maxCombinations = 100; // Максимальное количество комбинаций
        static void FindCombinationsDynamic(List<Product> products, double targetSum, List<List<(string Name, int Count, double Cost)>> allCombinations)
        {
            if (combinationCount >= maxCombinations) return;

            // Создаем таблицу для хранения комбинаций
            var dp = new Dictionary<double, List<List<(string Name, int Count, double Cost)>>>();

            dp[0] = new List<List<(string Name, int Count, double Cost)>> { new List<(string Name, int Count, double Cost)>() };

            foreach (var product in products)
            {
                var keys = dp.Keys.ToList(); // Получаем текущие суммы

                foreach (var key in keys)
                {
                    for (int count = 1; count <= product.Quantity; count++)
                    {
                        double newSum = key + product.Price * count;

                        if (Math.Abs(newSum - targetSum) < 0.01 && combinationCount < maxCombinations)
                        {
                            var newCombination = new List<(string Name, int Count, double Cost)>(dp[key].First());
                            newCombination.Add((product.Name, count, product.Price * count));
                            allCombinations.Add(newCombination);
                            combinationCount++;
                        }

                        if (!dp.ContainsKey(newSum) && newSum <= targetSum)
                        {
                            dp[newSum] = new List<List<(string Name, int Count, double Cost)>>();
                            foreach (var comb in dp[key])
                            {
                                var newCombination = new List<(string Name, int Count, double Cost)>(comb);
                                newCombination.Add((product.Name, count, product.Price * count));
                                dp[newSum].Add(newCombination);
                            }
                        }
                    }
                }
            }
        }

    }
}
