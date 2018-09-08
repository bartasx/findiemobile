using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FindieMobile.Annotations;
using Microsoft.AspNetCore.Http;

namespace FindieMobile.Models
{
    public class EventModel : INotifyPropertyChanged
    {
        public string HostUsername
        {
            get => _hostUsername;
            set
            {
                _hostUsername = value;
                this.OnPropertyChanged();
            }
        }

        public string EventName
        {
            get => _eventName;
            set
            {
                _eventName = value;
                this.OnPropertyChanged();
            }
        }

        public string EventDescription
        {
            get => _eventDescription;
            set
            {
                _eventDescription = value;
                this.OnPropertyChanged();
            }
        }

        public DateTime DateOfCreation
        {
            get => _dateOfCreation;
            set
            {
                _dateOfCreation = value;
                this.OnPropertyChanged();
            }
        }

        public DateTime DateOfEvent
        {
            get => _dateOfEvent;
            set
            {
                _dateOfEvent = value;
                this.OnPropertyChanged();
            }
        }

        public double? Latitude
        {
            get => _latitude;
            set
            {
                _latitude = value;
                this.OnPropertyChanged();
            }
        }

        public double? Longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;
                this.OnPropertyChanged();
            }
        }

        public IFormFile File { get; set; }

        private string _hostUsername;
        private string _eventName;
        private string _eventDescription;
        private DateTime _dateOfCreation;
        private DateTime _dateOfEvent;
        private double? _latitude;
        private double? _longitude;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}