using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DMCApp.Models.Entities
{
    // Базовый класс для общих полей
    public abstract class BaseEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; } = Environment.UserDomainName + "\\" + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        public byte Deleted { get; set; } = 0;
    }
}