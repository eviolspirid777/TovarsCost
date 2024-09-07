using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace CheckAmmount
{
	public partial class MainWindow : Window
	{
		static List<Product> products = new List<Product>();

		static List<List<(string Name, int Count, double Cost)>> combinations = new List<List<(string Name, int Count, double Cost)>>();

		public MainWindow()
		{
			InitializeComponent();
		}

		public void OpenFileClick(object sender, RoutedEventArgs e)
		{
			// Создайте диалоговое окно для выбора файла
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
				Title = "Open JSON File"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				// Чтение файла и десериализация
				try
				{
					string jsonString = File.ReadAllText(openFileDialog.FileName);
					products = JsonSerializer.Deserialize<List<Product>>(jsonString);

					// Обновите источник данных для вашего представления
					ValuesTable.ItemsSource = products;
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка");
				}
			}
		}

		public void CombinationClick(object sender, RoutedEventArgs e)
		{
			var ifPosible = Double.TryParse(ammountTextBox.Text, out double Cost);
			if (ifPosible)
			{

				Task.Run(() =>
				{
					combinations.Clear();
					FindCombinations(products, Cost, new List<(string Name, int Count, double Cost)>(), combinations);

					Dispatcher.Invoke(() =>
					{
						var results = combinations.Select(combination => new
						{
							Name = string.Join(", ", combination.Select(c => $"{c.Count}x {c.Name}")),
							Count = combination.Sum(c => c.Count),
							Cost = combination.Sum(c => c.Cost).ToString("C")
						}).ToList();

						CombinationsList.ItemsSource = results;
					});
				});
			}
			else
			{
				MessageBox.Show("Вы ввели не число или указали точку вместо запятой!", "Ошибка");
			}
		}

		static void FindCombinations(List<Product> products, double targetSum, List<(string Name, int Count, double Cost)> currentCombination, List<List<(string Name, int Count, double Cost)>> allCombinations)
		{
			double currentSum = 0;
			foreach (var item in currentCombination)
			{
				currentSum += item.Cost;
			}

			// Сравниваем текущую сумму с целевой с учетом небольшого допуска
			if (Math.Abs(currentSum - targetSum) < 0.01)
			{
				allCombinations.Add(new List<(string, int, double)>(currentCombination));
				return; // Находим одну комбинацию, выходим
			}

			if (currentSum > targetSum)
			{
				return; // Превышение суммы, выходим
			}

			foreach (var product in products)
			{
				for (int count = 1; count <= product.Quantity; count++)
				{
					double cost = product.Price * count;
					currentCombination.Add((product.Name, count, cost));
					FindCombinations(products, targetSum, currentCombination, allCombinations);
					currentCombination.RemoveAt(currentCombination.Count - 1); // Убираем последний добавленный элемент
				}
			}
		}
	}
}
