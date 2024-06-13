using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using NetworkService.AdditionalElements;
using NetworkService.Model;
//using System.Windows.Media;
using System.Windows.Forms;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading;
using System.IO;
using System.Reactive;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using InteractiveDataDisplay.WPF;
using System.Xml.Linq;
using System.Windows.Media.Effects;
using System.Reflection.Emit;

namespace NetworkService.ViewModel
{
    public class MeasurmentGraphViewModel : BindableBase
    {
        public MyICommand HelpCommand { get; set; }
        public MyICommand ShowEntitiesCommand { get; set; }
        public ObservableCollection<Markers> CircleMarkers { get; set; }
        public List<string> ComboBoxItems { get; set; } = Dodatno.ComboBoxItems.entityTypes.Keys.ToList();

        
        private string selectedType;
        private string errorMessage;




        public ObservableCollection<Temperature> Temperatures { get; set; }
        public MyICommand ToggleHelpVisibilityCommand { get; set; }
        private Visibility helpVisibility;
        private string richText;
       
        


        public MeasurmentGraphViewModel()
        {
            HelpCommand = new MyICommand(ExecuteHelpCommand);
            ToggleHelpVisibilityCommand = new MyICommand(ExecuteToggleHelpVisibilityCommand);
            HelpVisibility = Visibility.Visible;

			CircleMarkers = new ObservableCollection<Markers>();
			for (int i = 0; i <= 4; i++)
			{
				Markers marker = new Markers();
				CircleMarkers.Add(marker);
			}

			SelectedType = Dodatno.ComboBoxItems.entityTypes.Keys.ToList()[0];
			SetValuesToCircleMarkers(LoadLastFiveUpdates(SelectedType));




		}
		private List<Markers> LoadLastFiveUpdates(string selectedType)
		{
			if (!File.Exists("Log.txt"))
			{
				ErrorMessage = "Log file doesn't exist.";
				return null;
			}

			string[] lines = File.ReadAllLines("Log.txt");

			List<Markers> lastFiveUpdates = new List<Markers>();

			for (int i = lines.Count() - 1; i >= 0; i--)
			{
				string line = lines[i];

				string date = line.Substring(0, line.IndexOf(" "));
				line = line.Substring(line.IndexOf(" ") + 1);
				string time = line.Substring(0, line.IndexOf(" ") - 1);
				line = line.Substring(line.IndexOf(" ") + 1);
				string type = line.Substring(0, line.IndexOf(','));
				string val = line.Substring(line.IndexOf(',') + 2);

				if ((type == SelectedType) && (lastFiveUpdates.Count < 5))
				{
					lastFiveUpdates.Add(new Markers(type, int.Parse(val), date, time));
				}
			}

			return (lastFiveUpdates.Count == 5) ? lastFiveUpdates : null;
		}

		private void SetDefaultValuesToCircleMarkers()
		{
			for (int i = 0; i <= 4; i++)
			{
				CircleMarkers[i].CmType = "Type";
				CircleMarkers[i].CmValue = 0;
				CircleMarkers[i].CmDate = "Date";
				CircleMarkers[i].CmTime = "Time";
			}
		}

		private void SetValuesToCircleMarkers(List<Markers> markers)
		{
			if (markers != null)
			{
				for (int i = 0; i <= 4; i++)
				{
					CircleMarkers[i].CmType = markers[4 - i].CmType;
					CircleMarkers[i].CmValue = markers[4 - i].CmValue;
					CircleMarkers[i].CmDate = markers[4 - i].CmDate;
					CircleMarkers[i].CmTime = markers[4 - i].CmTime;
				}
			}
			else
			{
				// Ako se u log fajlu nalazi manje od 5 promena vrednosti
				// Na kruzice se postavljaju default vrednosti
				SetDefaultValuesToCircleMarkers();
			}
		}

		public void OnShow()
		{
			if (SelectedType != null)
			{
				ErrorMessage = String.Empty;

				List<Markers> markers = LoadLastFiveUpdates(SelectedType);

				SetValuesToCircleMarkers(markers);
			}
			else
			{
				ErrorMessage = "Type is required.";
			}
		}

		public string SelectedType
		{
			get { return selectedType; }
			set
			{
				selectedType = value;
				OnPropertyChanged("SelectedType");
			}
		}

		public string ErrorMessage
		{
			get { return errorMessage; }
			set
			{
				errorMessage = value;
				OnPropertyChanged("ErrorMessage");
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

       

        public Visibility HelpVisibility
        {
            get { return helpVisibility; }
            set
            {
                helpVisibility = value;
                OnPropertyChanged("HelpVisibility");
            }
        }

        private void ExecuteHelpCommand()
        {
            RichText = "In this window, you can display existing entities by selecting them in the list and display their values ​​using a graph.";
        }
        public void ExecuteToggleHelpVisibilityCommand()
        {
            HelpVisibility = (HelpVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            if (HelpVisibility == Visibility.Collapsed)
            {
                RichText = string.Empty;
            }
        }


    }
}