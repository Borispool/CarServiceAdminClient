using CarServiceAdminClient.ViewModels;
using System;

namespace CarServiceAdminClient.Models
{
    public class Order : BaseViewModel
    {
        private int _id;
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }

        private int _carId;
        public int CarId { get => _carId; set { _carId = value; OnPropertyChanged(); } }

        private string _description = string.Empty;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }

        private string _status = string.Empty;
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

        private DateTime _creationDate;
        public DateTime CreationDate { get => _creationDate; set { _creationDate = value; OnPropertyChanged(); } }

        private decimal _cost;
        public decimal Cost
        {
            get => _cost;
            set { _cost = value; OnPropertyChanged(); }
        }
        private bool _isRecurringProblem;
        public bool IsRecurringProblem { get => _isRecurringProblem; set { _isRecurringProblem = value; OnPropertyChanged(); } }
    }
}