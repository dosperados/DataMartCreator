using DMCApp.Data;
using DMCApp.ViewModels;
using DMCApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace DMCApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        /*
        private readonly AppDbContext _dbContext;
        private ObservableCollection<Db> _databases;
        private Db _selectedDatabase;
        public MainViewModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            LoadDatabasesCommand = new RelayCommand(LoadDatabases);
            LoadDatabases();
        }
        public ObservableCollection<Db> Databases
        {
            get => _databases;
            set
            {
                _databases = value;
                OnPropertyChanged();
            }
        }
        public Db SelectedDatabase
        {
            get => _selectedDatabase;
            set
            {
                _selectedDatabase = value;
                OnPropertyChanged();
            }
        }
        public ICommand LoadDatabasesCommand { get; }
        private void LoadDatabases()
        {
            Databases = new ObservableCollection<Db>(_dbContext.Dbs.ToList());
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        */
        private readonly AppDbContext _context;
        private bool showDeleted = false;

        public ObservableCollection<Db> Dbs { get; set; }
        public ObservableCollection<Table> Tables { get; set; }
        public ObservableCollection<Table> SelectedTables { get; set; }
        public ObservableCollection<Column> Columns { get; set; }

        public bool ShowDeleted
        {
            get => showDeleted;
            set => SetProperty(ref showDeleted, value);
        }

        private Db _selectedDb;
        public Db SelectedDb
        {
            get => _selectedDb;
            set
            {
                if (_selectedDb != value)
                {
                    _selectedDb = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand SaveChangesCommand { get; }
        public RelayCommand AddNewTableCommand { get; }
        public RelayCommand ToggleShowAllCommand { get; }
        public RelayCommand SoftDeleteCommand { get; private set; }

        public MainViewModel()
        {
            _context = new AppDbContext(new DbContextOptions<AppDbContext>());

            // Initialize commands and collections first
            SaveChangesCommand = new RelayCommand(SaveChanges);
            AddNewTableCommand = new RelayCommand(AddNewTable);
            ToggleShowAllCommand = new RelayCommand(ExecuteToggleShowAll);
            SoftDeleteCommand = new RelayCommand(ExecuteSoftDelete, CanExecuteSoftDelete);
            SelectedTables = new ObservableCollection<Table>();
            SelectedTables.CollectionChanged += (s, e) => SoftDeleteCommand.RaiseCanExecuteChanged();

            LoadData();

                       

            Debug.WriteLine("Dbs count: " + Dbs.Count); // должно быть > 0

            foreach (var db in Dbs)
            {
                Debug.WriteLine($"DbId={db.DBId}, Name={db.DBName}");
            }

            foreach (var entry in _context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Detached:
                        Debug.WriteLine($"{entry.Entity.GetType().Name}: {entry.State}");
                        break;
                    case EntityState.Deleted:
                        Debug.WriteLine($"{entry.Entity.GetType().Name}: {entry.State}");
                        break;
                    case EntityState.Modified:
                        Debug.WriteLine($"{entry.Entity.GetType().Name}: {entry.State}");
                        break;
                    case EntityState.Added:
                        Debug.WriteLine($"{entry.Entity.GetType().Name}: {entry.State}");
                        break;
                    default:
                        break;
                }
                
            }
        }

        
        public void AddNewTable()
        {
            if (SelectedDb == null)
                return;

            var newTable = new Table { 
                TableName = "NewTable",
                SchemaName = "dbo",
                Type = "Dest",
                Db = SelectedDb, //DBId = SelectedDb.DBId //Db = SelectedDb
                DBId = SelectedDb.DBId
            };

            _context.Tables.Add(newTable);
            Tables.Add(newTable);
            
            //SaveChanges();
        }

        private void SaveChanges()
        {
            try
            {
                _context.SaveChanges(); // Persists to DB
                LoadData();
                // Detach all tracked entities to ensure a clean reload from the database.
                // This is a robust way to ensure fresh data.
                /*
                 * Состояние	Что означает	                        Что EF делает при SaveChanges()
                    Added	        Объект создан и добавлен в контекст	Вставит новую строку в базу данных
                    Modified	    Свойства изменились	                Выполнит UPDATE
                    Deleted	        Объект помечен на удаление	        Выполнит DELETE
                    Unchanged	    Объект не менялся	                Ничего не делает
                    Detached	    Контекст не отслеживает объект	    Не делает ничего вообще — будто объекта не существует
                 */
                var trackedEntries = _context.ChangeTracker.Entries(); //.ToList()
                foreach (var entry in trackedEntries)
                {
                    if (entry.Entity != null)
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    Debug.WriteLine($"trackedEntries.Count(): {trackedEntries.Count()}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error! Cannot save data: {ex.Message}");
            }
        }

        private void LoadData()
        {
            // Detach all tracked entities to ensure a clean reload.
            var trackedEntries = _context.ChangeTracker.Entries();
            foreach (var entry in trackedEntries)
            {
                switch (entry.State)
                {
                    //case EntityState.Modified:
                    //case EntityState.Added:
                    //    entry.State = EntityState.Unchanged;
                    //    break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Detached;
                        break;
                    default:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }

            if (ShowDeleted)
            {
                _context.Dbs.IgnoreQueryFilters().Load();
                _context.Tables.IgnoreQueryFilters().Include(t => t.Db).Load();
                _context.Columns.IgnoreQueryFilters().Load();
            }
            else
            {
                _context.Dbs.Load();
                _context.Tables.Include(t => t.Db).Load();
                _context.Columns.Load();
            }

            // Переинициализируем коллекции из Local (без повторной загрузки из БД)
            //Dbs = _context.Dbs.Local.ToObservableCollection();
            Dbs = new ObservableCollection<Db>(_context.Dbs.Local.ToList());
            //Tables = _context.Tables.Local.ToObservableCollection();
            Tables = new ObservableCollection<Table>(_context.Tables.Local.ToList());
            //Columns = _context.Columns.Local.ToObservableCollection();
            Columns = new ObservableCollection<Column>(_context.Columns.Local.ToList());

            // Обновляем привязки UI
            OnPropertyChanged(nameof(Dbs));
            OnPropertyChanged(nameof(Tables));
            OnPropertyChanged(nameof(Columns));
        }

        private void ExecuteToggleShowAll()
        {
            ShowDeleted = !ShowDeleted;
            LoadData();
        }

        private void ExecuteSoftDelete()
        {
            if (SelectedTables == null || !SelectedTables.Any())
            {
                MessageBox.Show("No tables selected to delete.", "Soft Delete", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to soft delete {SelectedTables.Count} selected table(s)?",
                "Confirm Soft Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmResult == MessageBoxResult.No)
            {
                return;
            }

            bool changesMade = false;
            foreach (var table in SelectedTables)
            {
                if (table.Deleted == 0) // Only act if not already deleted
                {
                    table.Deleted = 1;
                    // ModifiedAt and ModifiedBy should be handled by LastModifieTracker in AppDbContext
                    // If explicit setting is needed:
                    // table.ModifiedAt = DateTime.Now;
                    // table.ModifiedBy = Environment.UserDomainName + "\" + System.Security.Principal.WindowsIdentity.GetCurrent().Name; 
                    _context.Entry(table).State = EntityState.Modified;
                    changesMade = true;
                }
            }

            if (changesMade)
            {
                try
                {
                    _context.SaveChanges(); // Save changes to the database
                    LoadData(); // Refresh data in the UI (respects IsShowingAll)
                    MessageBox.Show($"{SelectedTables.Count} table(s) have been soft deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during soft delete: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    // Optionally, reload data even if save fails to revert optimistic changes in UI
                    LoadData();
                }
            }
            else if (SelectedTables.Any()) // If items were selected but all were already deleted
            {
                MessageBox.Show("Selected table(s) were already deleted.", "Soft Delete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Clear selection after operation? Optional.
            // SelectedTables.Clear(); 
            // OnPropertyChanged(nameof(SelectedTables)); // If clearing and want UI to reflect, though SelectedTables itself might not be directly bound in a way that clearing it here updates the DataGrid selection.
        }

        private bool CanExecuteSoftDelete()
        {
            // Enable if there are any items in SelectedTables that are not already marked as deleted.
            return SelectedTables != null && SelectedTables.Any(t => t.Deleted == 0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
