using CarServiceAdminClient.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace CarServiceAdminClient.ViewModels
{
    public enum CurrentView { Clients, Cars, Orders }

    public class MainViewModel : BaseViewModel
    {
        #region Колекції Даних
        private readonly ObservableCollection<Client> _allClients;
        private readonly ObservableCollection<Car> _allCars;
        private readonly ObservableCollection<Order> _allOrders;
        #endregion

        #region View-Властивості для Клієнтів
        public ICollectionView ClientsView { get; private set; }
        private Client? _selectedClient;
        public Client? SelectedClient
        {
            get => _selectedClient;
            set { _selectedClient = value; OnPropertyChanged(); UpdateAllCommands(); }
        }
        private string _clientSearchText = string.Empty;
        public string ClientSearchText
        {
            get => _clientSearchText;
            set { _clientSearchText = value; OnPropertyChanged(); ClientsView.Refresh(); }
        }
        #endregion

        #region View-Властивості для Авто
        public ICollectionView? CarsView { get; private set; }
        private Car? _selectedCar;
        public Car? SelectedCar
        {
            get => _selectedCar;
            set { _selectedCar = value; OnPropertyChanged(); UpdateAllCommands(); }
        }
        private string _carSearchText = string.Empty;
        public string CarSearchText
        {
            get => _carSearchText;
            set { _carSearchText = value; OnPropertyChanged(); CarsView?.Refresh(); }
        }
        #endregion

        #region View-Властивості для Замовлень
        public ICollectionView? OrdersView { get; private set; } // Історія завершених
        private Order? _selectedOrder;
        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set { _selectedOrder = value; OnPropertyChanged(); UpdateAllCommands(); }
        }

        // Властивість для активного (незавершеного) замовлення
        private Order? _activeOrder;
        public Order? ActiveOrder
        {
            get => _activeOrder;
            set { _activeOrder = value; OnPropertyChanged(); }
        }
        #endregion

        #region Навігація та Команди
        private CurrentView _activeView = CurrentView.Clients;
        public CurrentView ActiveView
        {
            get => _activeView;
            set { _activeView = value; OnPropertyChanged(); }
        }

        public RelayCommand AddClientCommand { get; }
        public RelayCommand EditClientCommand { get; }
        public RelayCommand DeleteClientCommand { get; }
        public RelayCommand AddCarCommand { get; }
        public RelayCommand EditCarCommand { get; }
        public RelayCommand DeleteCarCommand { get; }
        public RelayCommand AddOrderCommand { get; }
        public RelayCommand SetStatusCommand { get; }
        public RelayCommand ClearOrderHistoryCommand { get; }
        public RelayCommand ShowCarsViewCommand { get; }
        public RelayCommand ShowOrdersViewCommand { get; }
        public RelayCommand BackToClientsCommand { get; }
        public RelayCommand BackToCarsCommand { get; }
        public RelayCommand ExitApplicationCommand { get; }
        #endregion

        public MainViewModel()
        {
            _allClients = new ObservableCollection<Client>();
            _allCars = new ObservableCollection<Car>();
            _allOrders = new ObservableCollection<Order>();

            LoadInitialData();

            ClientsView = CollectionViewSource.GetDefaultView(_allClients);
            ClientsView.Filter = FilterClients;

            AddClientCommand = new RelayCommand(AddClient);
            EditClientCommand = new RelayCommand(EditClient, CanModifyClient);
            DeleteClientCommand = new RelayCommand(DeleteClient, CanModifyClient);
            AddCarCommand = new RelayCommand(AddCar, CanAddCar);
            EditCarCommand = new RelayCommand(EditCar, CanModifyCar);
            DeleteCarCommand = new RelayCommand(DeleteCar, CanModifyCar);
            AddOrderCommand = new RelayCommand(AddOrder, CanAddOrder);
            SetStatusCommand = new RelayCommand(SetStatus, CanChangeStatus);
            ClearOrderHistoryCommand = new RelayCommand(ClearOrderHistory, CanClearHistory);
            ShowCarsViewCommand = new RelayCommand(ShowCarsView, CanModifyClient);
            ShowOrdersViewCommand = new RelayCommand(ShowOrdersView, CanModifyCar);
            BackToClientsCommand = new RelayCommand(p => { ActiveView = CurrentView.Clients; SelectedCar = null; CarSearchText = string.Empty; });
            BackToCarsCommand = new RelayCommand(p => { ActiveView = CurrentView.Cars; SelectedOrder = null; });
            ExitApplicationCommand = new RelayCommand(p => Application.Current.Shutdown());
        }

        #region Методи для Клієнтів
        private bool CanModifyClient(object? p) => SelectedClient != null;

        private void AddClient(object? p)
        {
            var clientWindow = new AddClientWindow(new Client());
            if (clientWindow.ShowDialog() == true && clientWindow.DataContext is AddClientViewModel vm)
            {
                var newClient = vm.Client;
                newClient.Id = _allClients.Any() ? _allClients.Max(c => c.Id) + 1 : 1;
                _allClients.Add(newClient);
                var successBox = new CustomMessageBox($"Клієнт {newClient.FullName} успішно доданий!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                successBox.ShowDialog();
            }
        }

        private void EditClient(object? p)
        {
            if (SelectedClient == null) return;
            var clientCopy = new Client { Id = SelectedClient.Id, FirstName = SelectedClient.FirstName, LastName = SelectedClient.LastName, PhoneNumber = SelectedClient.PhoneNumber, Email = SelectedClient.Email };
            var clientWindow = new AddClientWindow(clientCopy);

            if (clientWindow.ShowDialog() == true && clientWindow.DataContext is AddClientViewModel vm)
            {
                var updatedClient = vm.Client;
                var originalClient = _allClients.FirstOrDefault(c => c.Id == updatedClient.Id);
                if (originalClient != null)
                {
                    originalClient.FirstName = updatedClient.FirstName;
                    originalClient.LastName = updatedClient.LastName;
                    originalClient.PhoneNumber = updatedClient.PhoneNumber;
                    originalClient.Email = updatedClient.Email;
                }
                var successBox = new CustomMessageBox($"Дані клієнта {updatedClient.FullName} успішно оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                successBox.ShowDialog();
            }
        }

        private void DeleteClient(object? p)
        {
            if (SelectedClient == null) return;
            var messageBox = new CustomMessageBox($"Ви впевнені, що хочете видалити клієнта\n{SelectedClient.FullName}?", "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);
            messageBox.ShowDialog();
            if (messageBox.Result == MessageBoxResult.Yes)
            {
                _allClients.Remove(SelectedClient);
                SelectedClient = null;
                var successBox = new CustomMessageBox("Клієнта успішно видалено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                successBox.ShowDialog();
            }
        }
        #endregion

        #region Методи для Автомобілів
        private bool CanAddCar(object? p) => SelectedClient != null;
        private bool CanModifyCar(object? p) => SelectedCar != null;

        private void AddCar(object? p)
        {
            if (SelectedClient == null) return;
            var newCar = new Car { ClientId = SelectedClient.Id };
            var carWindow = new AddCarWindow(newCar);
            if (carWindow.ShowDialog() == true && carWindow.DataContext is AddCarViewModel vm)
            {
                var carToAdd = vm.Car;
                carToAdd.Id = _allCars.Any() ? _allCars.Max(c => c.Id) + 1 : 1;
                _allCars.Add(carToAdd);
                CarsView?.Refresh();
            }
        }

        private void EditCar(object? p)
        {
            if (SelectedCar == null) return;
            var carCopy = new Car { Id = SelectedCar.Id, ClientId = SelectedCar.ClientId, Brand = SelectedCar.Brand, Model = SelectedCar.Model, VIN = SelectedCar.VIN, Year = SelectedCar.Year };
            var carWindow = new AddCarWindow(carCopy);
            if (carWindow.ShowDialog() == true && carWindow.DataContext is AddCarViewModel vm)
            {
                var updatedCar = vm.Car;
                var originalCar = _allCars.FirstOrDefault(c => c.Id == updatedCar.Id);
                if (originalCar != null)
                {
                    originalCar.Brand = updatedCar.Brand;
                    originalCar.Model = updatedCar.Model;
                    originalCar.VIN = updatedCar.VIN;
                    originalCar.Year = updatedCar.Year;
                }
            }
        }

        private void DeleteCar(object? p)
        {
            if (SelectedCar == null) return;
            var messageBox = new CustomMessageBox($"Ви впевнені, що хочете видалити автомобіль\n{SelectedCar.Brand} {SelectedCar.Model}?", "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);
            messageBox.ShowDialog();
            if (messageBox.Result == MessageBoxResult.Yes)
            {
                _allCars.Remove(SelectedCar);
                SelectedCar = null;
                var successBox = new CustomMessageBox("Автомобіль успішно видалено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                successBox.ShowDialog();
            }
        }
        #endregion

        #region Методи для Замовлень
        private bool CanAddOrder(object? p) => SelectedCar != null && ActiveOrder == null;
        private bool CanChangeStatus(object? p) => ActiveOrder != null;
        private bool CanClearHistory(object? p) => OrdersView != null && !OrdersView.IsEmpty;

        private void AddOrder(object? p)
        {
            if (SelectedCar == null) return;
            var newOrder = new Order { CarId = SelectedCar.Id, CreationDate = DateTime.Now, Status = "В очікуванні" };
            var orderWindow = new AddOrderWindow(newOrder);

            if (orderWindow.ShowDialog() == true)
            {
                newOrder.Id = _allOrders.Any() ? _allOrders.Max(o => o.Id) + 1 : 1;
                _allOrders.Add(newOrder);
                ActiveOrder = newOrder;
                UpdateAllCommands();
            }
        }

        private void SetStatus(object? parameter)
        {
            if (ActiveOrder == null || parameter is not string newStatus) return;
            ActiveOrder.Status = newStatus;

            if (newStatus == "Завершено")
            {
                ActiveOrder = null;
                OrdersView?.Refresh();
            }
            UpdateAllCommands();
        }

        private void ClearOrderHistory(object? p)
        {
            if (SelectedCar == null) return;
            var messageBox = new CustomMessageBox($"Ви впевнені, що хочете очистити всю історію замовлень для\n{SelectedCar.Brand} {SelectedCar.Model}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
            messageBox.ShowDialog();

            if (messageBox.Result == MessageBoxResult.Yes)
            {
                var ordersToRemove = _allOrders.Where(o => o.CarId == SelectedCar.Id && o.Status == "Завершено").ToList();
                foreach (var order in ordersToRemove)
                {
                    _allOrders.Remove(order);
                }
                OrdersView?.Refresh();
            }
        }
        #endregion

        #region Фільтри та Навігація
        private bool FilterClients(object item)
        {
            if (string.IsNullOrEmpty(ClientSearchText) || item is not Client c) return true;
            return c.FullName.ToLower().Contains(ClientSearchText.ToLower()) || c.PhoneNumber.Contains(ClientSearchText);
        }

        private bool FilterCars(object item)
        {
            if (item is not Car car) return false;
            if (car.ClientId != SelectedClient?.Id) return false;
            if (string.IsNullOrEmpty(CarSearchText)) return true;
            return car.Brand.ToLower().Contains(CarSearchText.ToLower()) || car.Model.ToLower().Contains(CarSearchText.ToLower()) || car.VIN.ToLower().Contains(CarSearchText.ToLower());
        }

        private bool FilterOrders(object item)
        {
            if (item is not Order order) return false;
            return order.CarId == SelectedCar?.Id && order.Status == "Завершено";
        }

        private void ShowCarsView(object? p)
        {
            if (SelectedClient == null) return;
            CarsView = CollectionViewSource.GetDefaultView(_allCars);
            CarsView.Filter = FilterCars;
            OnPropertyChanged(nameof(CarsView));
            ActiveView = CurrentView.Cars;
        }

        private void ShowOrdersView(object? p)
        {
            if (SelectedCar == null) return;
            ActiveOrder = _allOrders.FirstOrDefault(o => o.CarId == SelectedCar.Id && o.Status != "Завершено");
            OrdersView = CollectionViewSource.GetDefaultView(_allOrders);
            OrdersView.Filter = FilterOrders;
            OnPropertyChanged(nameof(OrdersView));
            UpdateAllCommands();
            ActiveView = CurrentView.Orders;
        }

        private void UpdateAllCommands()
        {
            (AddClientCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (EditClientCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteClientCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ShowCarsViewCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (AddCarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (EditCarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteCarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ShowOrdersViewCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (AddOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (SetStatusCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ClearOrderHistoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private void LoadInitialData()
        {
            var clients = new[]
            {
                new Client { Id = 1, FirstName = "Іван", LastName = "Петренко", PhoneNumber = "0671112233", Email = "ivan@test.com" },
                new Client { Id = 2, FirstName = "Олена", LastName = "Ковальчук", PhoneNumber = "0994445566", Email = "olena@test.com" },
                new Client { Id = 3, FirstName = "Сергій", LastName = "Мельник", PhoneNumber = "0937778899", Email = "serhiy@test.com" },
                new Client { Id = 4, FirstName = "Марія", LastName = "Шевченко", PhoneNumber = "0501112233", Email = "maria@test.com" },
                new Client { Id = 5, FirstName = "Андрій", LastName = "Бондаренко", PhoneNumber = "0684445566", Email = "andriy@test.com" }
            };
            foreach (var c in clients) _allClients.Add(c);

            var cars = new[]
            {
                new Car { Id = 101, ClientId = 1, Brand = "Toyota", Model = "Camry", VIN = "JTD12345ABC", Year = 2018 },
                new Car { Id = 102, ClientId = 1, Brand = "Lexus", Model = "RX 350", VIN = "LEX98765XYZ", Year = 2021 },
                new Car { Id = 103, ClientId = 2, Brand = "Skoda", Model = "Octavia", VIN = "SKD54321QWE", Year = 2020 },
                new Car { Id = 104, ClientId = 3, Brand = "Volkswagen", Model = "Passat", VIN = "VWG11223RTY", Year = 2019 },
            };
            foreach (var c in cars) _allCars.Add(c);

            var orders = new[]
{
    // Історія для Toyota Camry + 1 активне замовлення
    new Order { Id = 1001, CarId = 101, CreationDate = new DateTime(2025, 9, 20), Status = "Завершено", Description = "Планова заміна мастила та фільтрів.", IsRecurringProblem = false },
    new Order { Id = 1003, CarId = 101, CreationDate = DateTime.Now.AddDays(-2), Status = "В роботі", Description = "Проблеми з гальмами, чути скрип.", IsRecurringProblem = true },

    // Історія для Skoda Octavia, без активних замовлень
    new Order { Id = 1002, CarId = 103, CreationDate = new DateTime(2025, 9, 28), Status = "Завершено", Description = "Діагностика ходової частини.", IsRecurringProblem = false },
    new Order { Id = 1005, CarId = 103, CreationDate = new DateTime(2025, 10, 1), Status = "Завершено", Description = "Заміна літньої гуми на зимову.", IsRecurringProblem = false },


    // Активне замовлення для Volkswagen Passat, без історії
    new Order { Id = 1004, CarId = 104, CreationDate = DateTime.Now.AddDays(-1), Status = "Очікує запчастин", Description = "Заміна свічок запалювання.", IsRecurringProblem = false },
    
    // Історія для Lexus RX 350
    new Order { Id = 1006, CarId = 102, CreationDate = new DateTime(2025, 8, 15), Status = "Завершено", Description = "Полірування кузова та нанесення кераміки.", IsRecurringProblem = false },
};
            foreach (var o in orders) _allOrders.Add(o);
        }
        #endregion
    }
}

