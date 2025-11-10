using CarServiceAdminClient.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using CarServiceAdminClient.Api;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CarServiceAdminClient.ViewModels
{
    public enum CurrentView { Clients, Cars, Orders }

    public class MainViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;

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
            set
            {
                if (_selectedClient != value)
                {
                    _selectedClient = value;
                    OnPropertyChanged();
                    UpdateAllCommands();
                    LoadClientDiscountAsync();
                }
            }
        }
        private string _currentClientDiscount = "0%";
        public string CurrentClientDiscount
        {
            get => _currentClientDiscount;
            set { _currentClientDiscount = value; OnPropertyChanged(); }
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
            set
            {
                if (_selectedCar != value)
                {
                    _selectedCar = value;
                    OnPropertyChanged();
                    UpdateAllCommands();
                    (ShowOrdersViewCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }
        private string _carSearchText = string.Empty;
        public string CarSearchText
        {
            get => _carSearchText;
            set { _carSearchText = value; OnPropertyChanged(); CarsView?.Refresh(); }
        }
        #endregion

        #region View-Властивості для Замовлень
        public ICollectionView? OrdersView { get; private set; }
        private Order? _selectedOrder;
        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (_selectedOrder != value) // Додали перевірку
                {
                    _selectedOrder = value;
                    OnPropertyChanged();
                    UpdateAllCommands(); // Потрібно для оновлення CanExecute кнопки "Видалити вибране"
                }
            }
        }
        private Order? _activeOrder;
        public Order? ActiveOrder
        {
            get => _activeOrder;
            set { _activeOrder = value; OnPropertyChanged(); UpdateAllCommands(); }
        }
        #endregion

        #region Властивості для Сортування
        public ObservableCollection<string> ClientSortOptions { get; } = new ObservableCollection<string> { "Прізвище (А-Я)", "Прізвище (Я-А)" };
        private string _selectedClientSortOption = "Прізвище (А-Я)";
        public string SelectedClientSortOption
        {
            get => _selectedClientSortOption;
            set { _selectedClientSortOption = value; OnPropertyChanged(); ApplyClientSorting(); } // Викликаємо сортування при зміні
        }

        public ObservableCollection<string> CarSortOptions { get; } = new ObservableCollection<string> { "Марка (А-Я)", "Рік (Новіші)", "Рік (Старіші)" };
        private string _selectedCarSortOption = "Марка (А-Я)";
        public string SelectedCarSortOption
        {
            get => _selectedCarSortOption;
            set { _selectedCarSortOption = value; OnPropertyChanged(); ApplyCarSorting(); } // Викликаємо сортування при зміні
        }

        public ObservableCollection<string> OrderSortOptions { get; } = new ObservableCollection<string> { "Дата (Новіші)", "Дата (Старіші)", "Вартість (Найдорожчі)", "Вартість (Найдешевші)" };
        private string _selectedOrderSortOption = "Дата (Новіші)";
        public string SelectedOrderSortOption
        {
            get => _selectedOrderSortOption;
            set { _selectedOrderSortOption = value; OnPropertyChanged(); ApplyOrderSorting(); } // Викликаємо сортування при зміні
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
        public RelayCommand DeleteSelectedOrderCommand { get; }
        public RelayCommand ClearOrderHistoryCommand { get; }
        public RelayCommand ShowCarsViewCommand { get; }
        public RelayCommand ShowOrdersViewCommand { get; }
        public RelayCommand BackToClientsCommand { get; }
        public RelayCommand BackToCarsCommand { get; }
        public RelayCommand ExitApplicationCommand { get; }
        #endregion

        public MainViewModel()
        {
            _apiClient = new ApiClient();
            _allClients = new ObservableCollection<Client>();
            _allCars = new ObservableCollection<Car>();
            _allOrders = new ObservableCollection<Order>();

            ClientsView = CollectionViewSource.GetDefaultView(_allClients);
            ClientsView.Filter = FilterClients;
            ApplyClientSorting();

            AddClientCommand = new RelayCommand(async (p) => await AddClient(p));
            EditClientCommand = new RelayCommand(async (p) => await EditClient(p), CanModifyClient);
            DeleteClientCommand = new RelayCommand(async (p) => await DeleteClient(p), CanModifyClient);
            AddCarCommand = new RelayCommand(async (p) => await AddCar(p), CanAddCar);
            EditCarCommand = new RelayCommand(async (p) => await EditCar(p), CanModifyCar);
            DeleteCarCommand = new RelayCommand(async (p) => await DeleteCar(p), CanModifyCar);
            AddOrderCommand = new RelayCommand(async (p) => await AddOrder(p), CanAddOrder);
            SetStatusCommand = new RelayCommand(async (p) => await SetStatus(p), CanChangeStatus);
            DeleteSelectedOrderCommand = new RelayCommand(async (p) => await DeleteSelectedOrder(p), CanDeleteSelectedOrder); // Тільки одна ініціалізація
            ClearOrderHistoryCommand = new RelayCommand(async (p) => await ClearOrderHistory(p), CanClearHistory);

            ShowCarsViewCommand = new RelayCommand(ShowCarsView, CanModifyClient);
            ShowOrdersViewCommand = new RelayCommand(ShowOrdersView, CanModifyCar);
            BackToClientsCommand = new RelayCommand(p =>
            {
                ActiveView = CurrentView.Clients;
                SelectedCar = null;
                CarSearchText = string.Empty;
            });
            BackToCarsCommand = new RelayCommand(p =>
            {
                ActiveView = CurrentView.Cars;
                SelectedOrder = null;
                ActiveOrder = null;
            });

            ExitApplicationCommand = new RelayCommand(p => Application.Current.Shutdown());
        }

        public async Task InitializeAsync()
        {
            var request = new Request { Command = "GET_ALL_DATA" };
            var response = await _apiClient.SendRequestAsync(request);

            if (response.IsSuccess && !string.IsNullOrEmpty(response.Payload))
            {
                var allData = JsonConvert.DeserializeObject<AllDataDto>(response.Payload);
                if (allData != null)
                {
                    _allClients.Clear();
                    allData.Clients.ForEach(c => _allClients.Add(new Client { Id = c.Id, FirstName = c.FirstName, LastName = c.LastName, PhoneNumber = c.PhoneNumber, Email = c.Email }));
                    _allCars.Clear();
                    allData.Cars.ForEach(c => _allCars.Add(new Car { Id = c.Id, ClientId = c.ClientId, Brand = c.Brand, Model = c.Model, VIN = c.VIN, Year = c.Year }));
                    _allOrders.Clear();
                    allData.Orders.ForEach(o => _allOrders.Add(new Order { Id = o.Id, CarId = o.CarId, Description = o.Description, Status = o.Status, CreationDate = o.CreationDate, IsRecurringProblem = o.IsRecurringProblem, Cost = o.Cost }));
                }
            }
            else
            {
                ShowServerError($"Не вдалося завантажити дані: {response.Message}");
            }
        }

        private async Task LoadClientDiscountAsync()
        {
            if (SelectedClient == null) { CurrentClientDiscount = "0%"; return; }
            CurrentClientDiscount = "Завантаження...";
            try
            {
                var request = new Request { Command = "GET_CLIENT_DISCOUNT", Payload = SelectedClient.Id.ToString() };
                var response = await _apiClient.SendRequestAsync(request);
                if (response.IsSuccess && int.TryParse(response.Payload, out int discount)) { CurrentClientDiscount = $"{discount}%"; }
                else { CurrentClientDiscount = "Помилка"; Console.WriteLine($"Error fetching discount: {response.Message}"); }
            }
            catch (Exception ex) { CurrentClientDiscount = "Помилка"; Console.WriteLine($"Exception fetching discount: {ex.Message}"); }
        }

        private void ShowServerError(string message)
        {
            var errorBox = new CustomMessageBox($"Сталася помилка на сервері:\n{message}", "Помилка сервера", MessageBoxButton.OK, MessageBoxImage.Error);
            errorBox.ShowDialog();
        }

        #region Методи для Клієнтів
        private bool CanModifyClient(object? p) => SelectedClient != null;

        private async Task AddClient(object? p)
        {
            var clientWindow = new AddClientWindow(new Client());
            if (clientWindow.ShowDialog() == true && clientWindow.DataContext is AddClientViewModel vm)
            {
                var newClient = vm.Client;
                var request = new Request { Command = "ADD_CLIENT", Payload = JsonConvert.SerializeObject(newClient) };
                var response = await _apiClient.SendRequestAsync(request);
                if (response.IsSuccess)
                {
                    var clientFromDb = JsonConvert.DeserializeObject<Client>(response.Payload);
                    _allClients.Add(clientFromDb);
                    var successBox = new CustomMessageBox($"Клієнт {clientFromDb.FullName} успішно доданий!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    successBox.ShowDialog();
                }
                else { ShowServerError(response.Message); }
            }
        }

        private async Task EditClient(object? p)
        {
            if (SelectedClient == null) return;
            var clientCopy = new Client { Id = SelectedClient.Id, FirstName = SelectedClient.FirstName, LastName = SelectedClient.LastName, PhoneNumber = SelectedClient.PhoneNumber, Email = SelectedClient.Email };
            var clientWindow = new AddClientWindow(clientCopy);
            if (clientWindow.ShowDialog() == true && clientWindow.DataContext is AddClientViewModel vm)
            {
                var updatedClient = vm.Client;
                var request = new Request { Command = "UPDATE_CLIENT", Payload = JsonConvert.SerializeObject(updatedClient) };
                var response = await _apiClient.SendRequestAsync(request);
                if (response.IsSuccess)
                {
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
                else { ShowServerError(response.Message); }
            }
        }

        private async Task DeleteClient(object? p)
        {
            if (SelectedClient == null) return;
            var clientToDelete = SelectedClient;
            var messageBox = new CustomMessageBox($"Ви впевнені, що хочете видалити клієнта\n{clientToDelete.FullName}?\n\n(УВАГА: Всі авто та замовлення цього клієнта також будуть видалені!)", "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);
            // *** ВИПРАВЛЕНО: Перевірка CustomResult ***
            if (messageBox.ShowDialog() == true && messageBox.CustomResult == MessageBoxResult.Yes)
            {
                var request = new Request { Command = "DELETE_CLIENT", Payload = clientToDelete.Id.ToString() };
                var response = await _apiClient.SendRequestAsync(request);
                if (response.IsSuccess)
                {
                    _allClients.Remove(clientToDelete);
                    SelectedClient = null;
                    var successBox = new CustomMessageBox("Клієнта успішно видалено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    successBox.ShowDialog();
                }
                else { ShowServerError(response.Message); }
            }
        }
        #endregion

        #region Методи для Автомобілів
        private bool CanAddCar(object? p) => SelectedClient != null;
        private bool CanModifyCar(object? p) => SelectedCar != null;

        private async Task AddCar(object? p)
        {
            if (SelectedClient == null) return;
            var newCar = new Car { ClientId = SelectedClient.Id };
            var carWindow = new AddCarWindow(newCar);
            if (carWindow.ShowDialog() == true && carWindow.DataContext is AddCarViewModel vm)
            {
                var carToAdd = vm.Car;
                var request = new Request { Command = "ADD_CAR", Payload = JsonConvert.SerializeObject(carToAdd) };
                var response = await _apiClient.SendRequestAsync(request);
                if (response.IsSuccess)
                {
                    var carFromDb = JsonConvert.DeserializeObject<Car>(response.Payload);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _allCars.Add(carFromDb);
                        CarsView?.Refresh();
                    });
                }
                else { ShowServerError(response.Message); }
            }
        }

        private async Task EditCar(object? p)
        {
            if (SelectedCar == null) return;
            var carCopy = new Car { Id = SelectedCar.Id, ClientId = SelectedCar.ClientId, Brand = SelectedCar.Brand, Model = SelectedCar.Model, VIN = SelectedCar.VIN, Year = SelectedCar.Year };
            var carWindow = new AddCarWindow(carCopy);
            if (carWindow.ShowDialog() == true && carWindow.DataContext is AddCarViewModel vm)
            {
                var updatedCar = vm.Car;
                var request = new Request { Command = "UPDATE_CAR", Payload = JsonConvert.SerializeObject(updatedCar) };
                var response = await _apiClient.SendRequestAsync(request);
                if (response.IsSuccess)
                {
                    var originalCar = _allCars.FirstOrDefault(c => c.Id == updatedCar.Id);
                    if (originalCar != null)
                    {
                        originalCar.Brand = updatedCar.Brand;
                        originalCar.Model = updatedCar.Model;
                        originalCar.VIN = updatedCar.VIN;
                        originalCar.Year = updatedCar.Year;
                    }
                }
                else { ShowServerError(response.Message); }
            }
        }

        private async Task DeleteCar(object? p)
        {
            if (SelectedCar == null) return;
            var carToDelete = SelectedCar;
            var messageBox = new CustomMessageBox($"Ви впевнені, що хочете видалити автомобіль\n{carToDelete.Brand} {carToDelete.Model}?", "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);
            // *** ВИПРАВЛЕНО: Перевірка CustomResult ***
            if (messageBox.ShowDialog() == true && messageBox.CustomResult == MessageBoxResult.Yes)
            {
                var request = new Request { Command = "DELETE_CAR", Payload = carToDelete.Id.ToString() };
                var response = await _apiClient.SendRequestAsync(request);
                if (response.IsSuccess)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _allCars.Remove(carToDelete);
                        SelectedCar = null;
                        CarsView?.Refresh();
                    });
                    var successBox = new CustomMessageBox("Автомобіль успішно видалено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    successBox.ShowDialog();
                }
                else { ShowServerError(response.Message); }
            }
        }
        #endregion

        #region Методи для Замовлень
        private bool CanAddOrder(object? p) => SelectedCar != null && ActiveOrder == null;
        private bool CanChangeStatus(object? p) => ActiveOrder != null;
        private bool CanClearHistory(object? p) => OrdersView != null && !OrdersView.IsEmpty && OrdersView.Cast<Order>().Any(); // Додали перевірку на пустий список
        private bool CanDeleteSelectedOrder(object? p) => SelectedOrder != null; // Тільки одна версія

        // *** Тільки ОДНА версія методу DeleteSelectedOrder ***
        private async Task DeleteSelectedOrder(object? p)
        {
            if (SelectedOrder == null) return;
            var orderToDelete = SelectedOrder;
            var messageBox = new CustomMessageBox(
                $"Ви впевнені, що хочете видалити вибране замовлення?\n\nОпис: {orderToDelete.Description}\nДата: {orderToDelete.CreationDate:dd.MM.yyyy}",
                "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // *** ВИПРАВЛЕНО: Перевірка CustomResult ***
            if (messageBox.ShowDialog() == true && messageBox.CustomResult == MessageBoxResult.Yes)
            {
                var request = new Request { Command = "DELETE_ORDER", Payload = orderToDelete.Id.ToString() };
                var response = await _apiClient.SendRequestAsync(request);
                if (response.IsSuccess)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _allOrders.Remove(orderToDelete);
                        OrdersView?.Refresh();
                        // Оновлюємо CanExecute кнопок
                        (DeleteSelectedOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
                        (ClearOrderHistoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    });
                    var successBox = new CustomMessageBox("Замовлення успішно видалено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    successBox.ShowDialog();
                }
                else { ShowServerError(response.Message); }
            }
        }


        private async Task AddOrder(object? p)
        {
            if (SelectedCar == null) return;
            var newOrder = new Order { CarId = SelectedCar.Id, CreationDate = DateTime.Now, Status = "В очікуванні" };
            var orderWindow = new AddOrderWindow(newOrder);
            if (orderWindow.ShowDialog() == true)
            {
                var request = new Request { Command = "ADD_ORDER", Payload = JsonConvert.SerializeObject(newOrder) };
                var response = await _apiClient.SendRequestAsync(request);
                if (response.IsSuccess)
                {
                    var orderFromDb = JsonConvert.DeserializeObject<Order>(response.Payload);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _allOrders.Add(orderFromDb);
                        ActiveOrder = orderFromDb;
                        OrdersView?.Refresh(); // Оновлюємо історію (хоча воно ще не там)
                        UpdateAllCommands();
                    });
                }
                else { ShowServerError(response.Message); }
            }
        }

        private async Task SetStatus(object? parameter)
        {
            if (ActiveOrder == null || parameter is not string newStatus) return;
            string oldStatus = ActiveOrder.Status;
            ActiveOrder.Status = newStatus;

            // *** ВАЖЛИВО: Оновлюємо і інші поля, які могли змінитися в UI ***
            // (Description, Cost, IsRecurringProblem)
            var request = new Request { Command = "UPDATE_ORDER_STATUS", Payload = JsonConvert.SerializeObject(ActiveOrder) };
            var response = await _apiClient.SendRequestAsync(request);

            if (response.IsSuccess)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (newStatus == "Завершено")
                    {
                        ActiveOrder = null;
                        OrdersView?.Refresh(); // Оновлюємо історію
                    }
                    UpdateAllCommands();
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ActiveOrder != null) ActiveOrder.Status = oldStatus; // Відкат статусу
                    ShowServerError(response.Message);
                    UpdateAllCommands();
                });
            }
        }

        private async Task ClearOrderHistory(object? p)
        {
            if (SelectedCar == null) return;
            var carIdToClear = SelectedCar.Id;
            var messageBox = new CustomMessageBox($"Ви впевнені, що хочете очистити всю історію замовлень для\n{SelectedCar.Brand} {SelectedCar.Model}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
            // *** ВИПРАВЛЕНО: Перевірка CustomResult ***
            if (messageBox.ShowDialog() == true && messageBox.CustomResult == MessageBoxResult.Yes)
            {
                var request = new Request { Command = "CLEAR_ORDER_HISTORY", Payload = carIdToClear.ToString() };
                var response = await _apiClient.SendRequestAsync(request);
                if (response.IsSuccess)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var ordersToRemove = _allOrders.Where(o => o.CarId == carIdToClear && o.Status == "Завершено").ToList();
                        foreach (var order in ordersToRemove) { _allOrders.Remove(order); }
                        OrdersView?.Refresh();
                        // Оновлюємо CanExecute кнопок
                        (DeleteSelectedOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
                        (ClearOrderHistoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    });
                }
                else { ShowServerError(response.Message); }
            }
        }
        #endregion

        #region Методи Сортування
        private void ApplyClientSorting()
        {
            if (ClientsView == null) return;
            ClientsView.SortDescriptions.Clear();
            switch (SelectedClientSortOption)
            {
                case "Прізвище (А-Я)":
                    ClientsView.SortDescriptions.Add(new SortDescription(nameof(Client.LastName), ListSortDirection.Ascending));
                    ClientsView.SortDescriptions.Add(new SortDescription(nameof(Client.FirstName), ListSortDirection.Ascending));
                    break;
                case "Прізвище (Я-А)":
                    ClientsView.SortDescriptions.Add(new SortDescription(nameof(Client.LastName), ListSortDirection.Descending));
                    ClientsView.SortDescriptions.Add(new SortDescription(nameof(Client.FirstName), ListSortDirection.Descending));
                    break;
            }
        }

        private void ApplyCarSorting()
        {
            // Перевіряємо чи CarsView взагалі створено перед сортуванням
            if (CarsView == null) return;
            CarsView.SortDescriptions.Clear();
            switch (SelectedCarSortOption)
            {
                case "Марка (А-Я)":
                    CarsView.SortDescriptions.Add(new SortDescription(nameof(Car.Brand), ListSortDirection.Ascending));
                    CarsView.SortDescriptions.Add(new SortDescription(nameof(Car.Model), ListSortDirection.Ascending));
                    break;
                case "Рік (Новіші)":
                    CarsView.SortDescriptions.Add(new SortDescription(nameof(Car.Year), ListSortDirection.Descending));
                    break;
                case "Рік (Старіші)":
                    CarsView.SortDescriptions.Add(new SortDescription(nameof(Car.Year), ListSortDirection.Ascending));
                    break;
            }
        }

        private void ApplyOrderSorting()
        {
            // Перевіряємо чи OrdersView взагалі створено перед сортуванням
            if (OrdersView == null) return;
            OrdersView.SortDescriptions.Clear();
            switch (SelectedOrderSortOption)
            {
                case "Дата (Новіші)":
                    OrdersView.SortDescriptions.Add(new SortDescription(nameof(Order.CreationDate), ListSortDirection.Descending));
                    break;
                case "Дата (Старіші)":
                    OrdersView.SortDescriptions.Add(new SortDescription(nameof(Order.CreationDate), ListSortDirection.Ascending));
                    break;
                case "Вартість (Найдорожчі)":
                    OrdersView.SortDescriptions.Add(new SortDescription(nameof(Order.Cost), ListSortDirection.Descending));
                    break;
                case "Вартість (Найдешевші)":
                    OrdersView.SortDescriptions.Add(new SortDescription(nameof(Order.Cost), ListSortDirection.Ascending));
                    break;
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
            if (SelectedClient == null || item is not Car car) return false;
            if (car.ClientId != SelectedClient.Id) return false;
            if (string.IsNullOrEmpty(CarSearchText)) return true;
            return car.Brand.ToLower().Contains(CarSearchText.ToLower())
                || car.Model.ToLower().Contains(CarSearchText.ToLower())
                || car.VIN.ToLower().Contains(CarSearchText.ToLower());
        }

        private bool FilterOrders(object item)
        {
            if (SelectedCar == null || item is not Order order) return false;
            return order.CarId == SelectedCar.Id && order.Status == "Завершено";
        }

        private void ShowCarsView(object? p)
        {
            if (SelectedClient == null) return;
            CarsView = CollectionViewSource.GetDefaultView(_allCars);
            CarsView.Filter = FilterCars;
            ApplyCarSorting();
            OnPropertyChanged(nameof(CarsView));
            ActiveView = CurrentView.Cars;
        }

        private void ShowOrdersView(object? p)
        {
            // ...
            OrdersView = CollectionViewSource.GetDefaultView(_allOrders);
            OrdersView.Filter = FilterOrders;
            ApplyOrderSorting(); 
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
            (DeleteSelectedOrderCommand as RelayCommand)?.RaiseCanExecuteChanged(); // Тільки одна перевірка
            (ClearOrderHistoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
        #endregion
    }
}