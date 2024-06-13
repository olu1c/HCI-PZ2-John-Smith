using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NetworkService.AdditionalElements;
using NetworkService.Model;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        public ObservableCollection<string> TemperatureTypes { get; }
         public static List<Temperature> Temperatures { get; set; }
        
        public ObservableCollection<Temperature> SearcedTemperatures { get; set; }

        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand HelpCommand { get; set; }
        public MyICommand ToggleHelpVisibilityCommand { get; set; }

        private Temperature selectedTemperature;
        private string idText;
        private string nameText;
        private TypeEnum.type temperatureType;
        private string nameSearchText;
        private bool isSearchByName;
        private bool isSearchByType;
        private double measurementValue;
        private string validationText = "";
        private ImageSource imageSource;
        private Visibility helpVisibility;
        private string richText;

        


        public NetworkEntitiesViewModel()
        {
            LoadTemperatures();
            AddCommand = new MyICommand(OnAdd);
            DeleteCommand = new MyICommand(OnDelete);
            TemperatureTypes = new ObservableCollection<string>(Enum.GetNames(typeof(TypeEnum.type)));
            HelpVisibility = Visibility.Visible;
            ToggleHelpVisibilityCommand = new MyICommand(ExecuteToggleHelpVisibilityCommand);
            HelpCommand = new MyICommand(ExecuteHelpCommand);
            
           

        }

        public void LoadTemperatures()
        {
            ObservableCollection<Temperature> temperatures = new ObservableCollection<Temperature>();
            temperatures.Add(new Temperature { ID = 1, Name = "Temperature 1", TemperatureType = TypeEnum.type.RTD, MesurmentValue = 0.0,Img = AppDomain.CurrentDomain.BaseDirectory + "Slike\\RTD.jpg"}); ;
            temperatures.Add(new Temperature { ID = 2, Name = "Temperature 2", TemperatureType = TypeEnum.type.RTD, MesurmentValue = 0.0, Img = AppDomain.CurrentDomain.BaseDirectory + "Slike\\RTD.jpg" });
            temperatures.Add(new Temperature { ID = 3, Name = "Temperature 3", TemperatureType = TypeEnum.type.ThermoSprega, MesurmentValue = 0.0, Img = AppDomain.CurrentDomain.BaseDirectory + "Slike\\thermosprega.jpg", });

            Temperatures = temperatures.ToList();
            SearcedTemperatures = temperatures;
        }

        public Visibility HelpVisibility
        {
            get { return helpVisibility; }
            set
            {
                helpVisibility = value;
                OnPropertyChanged("HelpVisibility");
            }
        }
       
        public string NameSearchText
        {
            get { return nameSearchText; }
            set
            {
                if (nameSearchText != value)
                {
                    nameSearchText = value;
                    OnPropertyChanged("NameSearchText");
                    ApplySearch();
                }
            }
        }
        
        public string RichText
        {
            get { return richText; }
            set
            {
                richText = value;
                OnPropertyChanged("RichText");
            }
        }
        private void ExecuteHelpCommand()
        {
            RichText = "In this window, you can add new entities by filling in the fields and clicking the Add button, delete existing ones using the Delete button, search entities  by type or name from the table using Search";
        }
        public void ExecuteToggleHelpVisibilityCommand()
        {
            HelpVisibility = (HelpVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            if (HelpVisibility == Visibility.Collapsed)
            {
                RichText = string.Empty;
            }
        }


        public bool IsSearchByName
        {
            get { return isSearchByName; }
            set
            {
                if (isSearchByName != value)
                {
                    isSearchByName = value;
                    OnPropertyChanged("IsSearchByName");
                    ApplySearch();
                }
            }
        }


        public bool IsSearchByType
        {
            get { return isSearchByType; }
            set
            {
                if (isSearchByType != value)
                {
                    isSearchByType = value;
                    OnPropertyChanged("IsSearchByType");
                    ApplySearch();
                }
            }
        }

        public void ApplySearch()
        {
            ValidationText = "";
            SearcedTemperatures.Clear();
                foreach (Temperature temps in Temperatures)
            {
                if (IsSearchByName && !string.IsNullOrEmpty(NameSearchText) && !temps.Name.Contains(NameSearchText))
                {
                    SearcedTemperatures.Remove(temps);
                }
                else if (IsSearchByType && !string.IsNullOrEmpty(NameSearchText) && !temps.TemperatureType.ToString().Contains(NameSearchText))
                {
                    SearcedTemperatures.Remove(temps);
                }
                else if (!SearcedTemperatures.Contains(temps))
                    SearcedTemperatures.Add(temps);
            }
        }

        public string ValidationText
        {
            get
            {
                return validationText;
            }
            set
            {
                if (validationText != value)
                {
                    validationText = value;
                    OnPropertyChanged("ValidationText");
                }
            }
        }

        public string IDText
        {
            get
            {
                return idText;
            }

            set
            {
                if (idText != value)
                {
                    idText = value;
                    OnPropertyChanged("IDText");
                }
            }
        }

        public double MesurmentValueText
        {
            get
            {
                return measurementValue;
            }

            set
            {
                if (measurementValue != value)
                {
                    measurementValue = value;
                    OnPropertyChanged("MesurmentValueText");
                }
            }
        }

        public string NameText
        {
            get
            {
                return nameText;
            }

            set
            {
                if (nameText != value)
                {
                    nameText = value;
                    OnPropertyChanged("NameText");
                }
            }
        }
        public ImageSource ImageSource
        {
            get
            {
                return imageSource;
            }

            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    OnPropertyChanged("ImageSource");
                }
            }
        }

        public TypeEnum.type TemperatureType
        {
            get
            {
                return temperatureType;
            }

            set
            {
                if (temperatureType != value)
                {
                    TemperatureType = value;
                    OnPropertyChanged("TemperatureType");
                }
                if (value == TypeEnum.type.RTD)
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Slike/RTD.jpg", UriKind.Absolute));
                }
                else { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Slike/thermosprega.jpg", UriKind.Absolute)); }
            }
        }

       
        public Temperature SelectedTemperature
        {
            get
            {
                return selectedTemperature;
            }

            set
            {
                if (selectedTemperature != value)
                {
                    selectedTemperature = value;
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void OnAdd()
        {
           
            if (int.TryParse(IDText, out int id) && !Temperatures.Exists(m => m.ID == id))
            {
                Temperature temp = new Temperature
                {
                    ID = id,
                    Name = NameText,
                    TemperatureType = TemperatureType,
                };
                Temperatures.Add(temp);
                ApplySearch();

                MessageBox.Show("Entity successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                IDText = "";
                NameText = "";
                TemperatureType = TypeEnum.type.RTD;
                ImageSource = null;
            }
            else
            {
                ValidationText = "Id or Name field is not \n filled correctly";
            }


        }
    


          private void OnDelete()
          {

              Temperatures.Remove(SelectedTemperature);
              foreach(KeyValuePair<string, Temperature> t in NetworkDisplayViewModel.TemperaturesCanvas)
              {
                  if(t.Value.ID == SelectedTemperature.ID) { NetworkDisplayViewModel.TemperaturesCanvas.Remove(t.Key);break; }
              }
              ApplySearch();
            MessageBox.Show("Entity successfully deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
          }

         


    }
}
