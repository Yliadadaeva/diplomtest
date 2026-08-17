using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace NovosibirskForestRegistry
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<ForestPlot> _allPlots;
        private ObservableCollection<ForestPlot> _filteredPlots;
        private ForestPlot _selectedPlot;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ForestPlot> FilteredPlots
        {
            get => _filteredPlots;
            set
            {
                _filteredPlots = value;
                OnPropertyChanged();
            }
        }

        public ForestPlot SelectedPlot
        {
            get => _selectedPlot;
            set
            {
                _selectedPlot = value;
                OnPropertyChanged();
                UpdatePlotDetails();
                UpdateButtonsState();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            LoadMockData();
            InitializeEvents();
            UpdateStatistics();

            CurrentDateText.Text = $"📅 {DateTime.Now:dd.MM.yyyy}";
            LastUpdateText.Text = $"✅ Последнее обновление: {DateTime.Now:dd.MM.yyyy HH:mm}";
        }

        private void LoadMockData()
        {
            _allPlots = new ObservableCollection<ForestPlot>
            {
                new ForestPlot { Id = 1, Name = "Заельцовский бор", CadastralNumber = "54:35:021501:122", Area = 342.5, Category = "Особо охраняемые природные территории", District = "Заельцовский", Status = "Охраняется", RegistrationDate = DateTime.Parse("2015-03-10"), Description = "Крупный лесной массив" },
                new ForestPlot { Id = 2, Name = "Берёзовая роща", CadastralNumber = "54:35:021502:045", Area = 128.3, Category = "Рекреационные зоны", District = "Центральный", Status = "Охраняется", RegistrationDate = DateTime.Parse("2016-07-22"), Description = "Популярное место отдыха" },
                new ForestPlot { Id = 3, Name = "Обь-Зырянское лесничество", CadastralNumber = "54:35:021503:078", Area = 2156.8, Category = "Защитные леса", District = "Октябрьский", Status = "Охраняется", RegistrationDate = DateTime.Parse("2014-11-05"), Description = "Водоохранная зона" },
                new ForestPlot { Id = 4, Name = "Парк 'У моря Обского'", CadastralNumber = "54:35:021504:012", Area = 45.2, Category = "Рекреационные зоны", District = "Дзержинский", Status = "Требует внимания", RegistrationDate = DateTime.Parse("2018-09-15"), Description = "Требуется обновление" },
                new ForestPlot { Id = 5, Name = "Лесопарк им. Кирова", CadastralNumber = "54:35:021505:099", Area = 89.7, Category = "Эксплуатационные леса", District = "Калининский", Status = "Охраняется", RegistrationDate = DateTime.Parse("2017-04-20"), Description = "Зона санитарной охраны" },
                new ForestPlot { Id = 6, Name = "Дендрологический парк", CadastralNumber = "54:35:021506:033", Area = 23.4, Category = "Особо охраняемые природные территории", District = "Заельцовский", Status = "Охраняется", RegistrationDate = DateTime.Parse("2019-08-14"), Description = "Коллекция растений" },
                new ForestPlot { Id = 7, Name = "Земельный участок", CadastralNumber = "54:35:091061:343", Area = 0.758, Category = "Земли населенных участков", District = "Держинский", Status = "Охраняется", RegistrationDate = DateTime.Parse("2025-10-24"), Description = ""}
            };

            FilteredPlots = new ObservableCollection<ForestPlot>(_allPlots);
        }

        private void InitializeEvents()
        {
            ApplyFilterButton.Click += (s, e) => ApplyFilters();
            ResetFilterButton.Click += (s, e) => ResetFilters();
            EditPlotButton.Click += (s, e) => EditSelectedPlot();
            GenerateReportButton.Click += (s, e) => GenerateReport();
            SearchBox.TextChanged += (s, e) => ApplyFilters();
            CategoryFilter.SelectionChanged += (s, e) => ApplyFilters();
            DistrictFilter.SelectionChanged += (s, e) => ApplyFilters();
            MinArea.TextChanged += (s, e) => ApplyFilters();
            MaxArea.TextChanged += (s, e) => ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allPlots.AsEnumerable();

            string searchText = SearchBox.Text?.ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(p => p.Name.ToLower().Contains(searchText) || p.CadastralNumber.ToLower().Contains(searchText));
            }

            if (CategoryFilter.SelectedItem is ComboBoxItem catItem && catItem.Content.ToString() != "Все категории")
            {
                filtered = filtered.Where(p => p.Category == catItem.Content.ToString());
            }

            if (DistrictFilter.SelectedItem is ComboBoxItem distItem && distItem.Content.ToString() != "Все районы")
            {
                filtered = filtered.Where(p => p.District == distItem.Content.ToString());
            }

            if (double.TryParse(MinArea.Text, out double minArea))
            {
                filtered = filtered.Where(p => p.Area >= minArea);
            }

            if (double.TryParse(MaxArea.Text, out double maxArea))
            {
                filtered = filtered.Where(p => p.Area <= maxArea);
            }

            FilteredPlots = new ObservableCollection<ForestPlot>(filtered);
            UpdateStatistics();
        }

        private void ResetFilters()
        {
            SearchBox.Text = "";
            CategoryFilter.SelectedIndex = 0;
            DistrictFilter.SelectedIndex = 0;
            MinArea.Text = "";
            MaxArea.Text = "";
            ApplyFilters();
        }

        private void UpdateStatistics()
        {
            TotalPlotsText.Text = $"📊 Всего учтено: {FilteredPlots.Count} из {_allPlots.Count}";
            PlotsCountText.Text = $"({FilteredPlots.Count})";
        }

        private void UpdatePlotDetails()
        {
            if (SelectedPlot != null)
            {
                SelectedPlotName.Text = SelectedPlot.Name;
                SelectedPlotCadastral.Text = SelectedPlot.CadastralNumber;
                SelectedPlotArea.Text = SelectedPlot.Area.ToString("F1");
                SelectedPlotCategory.Text = SelectedPlot.Category;
                SelectedPlotDistrict.Text = SelectedPlot.District;
                SelectedPlotStatus.Text = $"Статус: {SelectedPlot.Status} | Дата регистрации: {SelectedPlot.RegistrationDate:dd.MM.yyyy}";
            }
            else
            {
                SelectedPlotName.Text = "Не выбран";
                SelectedPlotCadastral.Text = "—";
                SelectedPlotArea.Text = "—";
                SelectedPlotCategory.Text = "—";
                SelectedPlotDistrict.Text = "—";
                SelectedPlotStatus.Text = "";
            }
        }

        private void UpdateButtonsState()
        {
            bool isSelected = SelectedPlot != null;
            EditPlotButton.IsEnabled = isSelected;
            GenerateReportButton.IsEnabled = isSelected;
        }

        private void EditSelectedPlot()
        {
            if (SelectedPlot != null)
            {
                MessageBox.Show($"Редактирование участка:\n{SelectedPlot.Name}\n{SelectedPlot.CadastralNumber}\n\nФункция редактирования будет доступна в следующей версии.", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GenerateReport()
        {
            if (SelectedPlot != null)
            {
                string report = $"ОТЧЁТ ПО ЛЕСНОМУ УЧАСТКУ\n================================\nНазвание: {SelectedPlot.Name}\nКадастровый номер: {SelectedPlot.CadastralNumber}\nПлощадь: {SelectedPlot.Area:F1} га\nКатегория: {SelectedPlot.Category}\nРайон: {SelectedPlot.District}\nСтатус: {SelectedPlot.Status}\nДата регистрации: {SelectedPlot.RegistrationDate:dd.MM.yyyy}\nОписание: {SelectedPlot.Description}\n================================\nОтчёт сгенерирован: {DateTime.Now:dd.MM.yyyy HH:mm:ss}";

                MessageBox.Show(report, $"Отчёт: {SelectedPlot.Name}", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}