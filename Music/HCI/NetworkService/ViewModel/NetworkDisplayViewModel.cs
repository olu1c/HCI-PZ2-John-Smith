using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NetworkService.AdditionalElements;
using NetworkService.Model;
using OxyPlot;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;


namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
      
        public static BindingList<Temperature> Temperatures { get; set; }
        public List<Canvas> CanvasList { get; set; } = new List<Canvas>();
        public static Dictionary<string, Temperature> TemperaturesCanvas { get; set; } = new Dictionary<string, Temperature>();


        private int selectedIndex = 0;
        public static Temperature draggedItem = null;
        private bool dragging = false;
        private Canvas selectedCanvas = null;

        private static bool postoji = false;
        private ListView listViewItem;

        public MyICommand<Temperature> SelectionChangedCommand { get; set; }
        public MyICommand<ListView> MouseLeftButtonUpCommand { get; set; }
        public MyICommand<Canvas> DragOverCommand { get; set; }
        public MyICommand<Canvas> DragLeaveCommand { get; set; }
        public MyICommand<Canvas> DropCommand { get; set; }
        public MyICommand<Canvas> FreeCommand { get; set; }
        public MyICommand<Canvas> LoadCommand { get; set; }
        public MyICommand<ListView> LoadListViewCommand { get; set; }
        public MyICommand<Canvas> ConnectCommand { get; set; }
        public MyICommand<Canvas> LineDelCommand { get; set; }
        public MyICommand<Canvas> RemoveAllCommand { get; set; }
        public MyICommand HelpCommand { get; set; }
        public MyICommand ToggleHelpVisibilityCommand { get; set; }

        private Visibility helpVisibility;
        private string richText;

        public MyICommand<Canvas> SelectionChangedCanvasCommand { get; set; }

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");

            }
        }
         

        public NetworkDisplayViewModel()
        {
            Temperatures = new BindingList<Temperature>();
            HelpVisibility = Visibility.Visible;

            SelectionChangedCommand = new MyICommand<Temperature>(OnSelectionChanged);
            MouseLeftButtonUpCommand = new MyICommand<ListView>(OnMouseLeftButtonUp);
            DragOverCommand = new MyICommand<Canvas>(OnDragOver);
            DragLeaveCommand = new MyICommand<Canvas>(OnDragLeave);
            DropCommand = new MyICommand<Canvas>(OnDrop);
            FreeCommand = new MyICommand<Canvas>(OnFree);
            LoadCommand = new MyICommand<Canvas>(OnLoad);
            LoadListViewCommand = new MyICommand<ListView>(OnLoadListView);
            ConnectCommand = new MyICommand<Canvas>(OnConnect, CanConnect);
            LineDelCommand = new MyICommand<Canvas>(OnLineDel);
            RemoveAllCommand = new MyICommand<Canvas>(OnRemoveAll);

            SelectionChangedCanvasCommand = new MyICommand<Canvas>(OnSelectionCanvasChanged);

            HelpCommand = new MyICommand(ExecuteHelpCommand);
            ToggleHelpVisibilityCommand = new MyICommand(ExecuteToggleHelpVisibilityCommand);
 

            Load();
        }

        public void OnConnect(Canvas canvas)
        {
            if(selectedCanvas == null)
            {
                selectedCanvas = canvas;
            }
            else
            {
                Canvas parentCanvas = FindParentCanvas(canvas);
                double x1 = Canvas.GetLeft(canvas) + canvas.ActualWidth / 2;
                double y1 = Canvas.GetTop(canvas) + canvas.ActualHeight / 2;
                double x2 = Canvas.GetLeft(selectedCanvas) + selectedCanvas.ActualWidth / 2;
                double y2 = Canvas.GetTop(selectedCanvas) + selectedCanvas.ActualHeight / 2;

                Line line = new Line();
                line.X1 = x1;  
                line.Y1 = y1;  
                line.X2 = x2; 
                line.Y2 = y2; 
                line.Stroke = Brushes.Black; 

       
                parentCanvas.Children.Add(line);

                selectedCanvas = null;
            }
        }
        public void OnLineDel(Canvas canvas)
        {
            var linesToRemove = canvas.Children.OfType<Line>().ToList();

            foreach (var line in linesToRemove)
            {
                canvas.Children.Remove(line);
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
             RichText = "Here you can drag the entities from the table to the marked fields and return them. You can also connect them with a line by clicking the button below the field.";
         }
         public void ExecuteToggleHelpVisibilityCommand()
         {
             HelpVisibility = (HelpVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
             if (HelpVisibility == Visibility.Collapsed)
             {
                 RichText = string.Empty;
             }
         }
 

        


        public Canvas FindParentCanvas(UIElement element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);

            while (parent != null && !(parent is Canvas))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as Canvas;
        }
        public bool CanConnect(Canvas canvas)
        {
            return canvas.Resources["taken"] != null;
        }

        public void OnSelectionChanged(Temperature t)
        {
            if (!dragging)                                                 
            {
                dragging = true;
                draggedItem = t;
                DragDrop.DoDragDrop(listViewItem, draggedItem, DragDropEffects.Move);
            }
        }

        public void OnMouseLeftButtonUp(ListView lw)
        {                                                                  
            draggedItem = null;
            lw.SelectedItem = null;
            dragging = false;
        }

        public void OnRemoveAll(Canvas canvas)
        {
            for(int i = 2; i <= 13; i++) {
            DependencyObject child = VisualTreeHelper.GetChild(canvas, i);
                if(child != null && (child is Canvas))
                {
                    try
                    {
                        Canvas ca = child as Canvas;
                        if (ca.Resources["taken"] != null)
                        {
                            ca.Background = Brushes.White;
                            foreach (Temperature temp in NetworkEntitiesViewModel.Temperatures)
                            {
                                if (!Temperatures.Contains(temp) && TemperaturesCanvas[ca.Name].ID == temp.ID)
                                    Temperatures.Add(temp);
                            }
                            ((Label)(ca).Children[2]).Content = "";
                            ca.Resources.Remove("taken");
                            TemperaturesCanvas.Remove(ca.Name);
                            ConnectCommand.RaiseCanExecuteChanged();
                        }
                    }
                    catch (Exception) { }
                }

            }

        }

        public void OnSelectionCanvasChanged(Canvas canvas)
        {
            if (!dragging)
            {
                if (canvas.Resources["taken"] != null)
                {
                    dragging = true;
                    draggedItem = TemperaturesCanvas[canvas.Name];

                    canvas.Background = Brushes.White;
                    canvas.Resources.Remove("taken");
                    TemperaturesCanvas.Remove(canvas.Name);
                    DragDrop.DoDragDrop(listViewItem, draggedItem, DragDropEffects.Move);
                }
            }
        }


        public void Load()
        {

            foreach (Temperature temp in NetworkEntitiesViewModel.Temperatures)
            {
                postoji = false;
                foreach (Temperature temp2 in TemperaturesCanvas.Values)
                {
                    if (temp.ID == temp2.ID)
                    {
                        postoji = true;
                        break;
                    }

                }
                if (postoji == false)
                    Temperatures.Add(new Temperature() { ID = temp.ID, Name = temp.Name, MesurmentValue = temp.MesurmentValue, Img= temp.Img, TemperatureType = temp.TemperatureType });
            }

        }

        public void OnDragOver(Canvas canvas)
        {                                                                   
            if (canvas.Resources["taken"] != null)
            {
                ((TextBlock)(canvas).Children[1]).Text = "X";
                ((TextBlock)(canvas).Children[1]).FontSize = 30;
                ((TextBlock)(canvas).Children[1]).FontWeight = System.Windows.FontWeights.ExtraBold;
            }
  
        }

        public void OnDragLeave(Canvas canvas)
        {                                                                   
            if (((TextBlock)(canvas).Children[1]).Text == "X")
            {
                ((TextBlock)(canvas).Children[1]).Text = "";
                ((TextBlock)(canvas).Children[1]).Background = Brushes.Transparent;
            }
        }

        public void OnDrop(Canvas canvas)
        {   

            if (draggedItem != null)                                        
            {
                if (canvas.Resources["taken"] == null)
                {
                    BitmapImage logo = new BitmapImage();

                    logo.BeginInit();
                    logo.UriSource = new Uri("pack://application:,,,/Slike/" + draggedItem.TemperatureType.ToString() + ".jpg", UriKind.Absolute);
                    logo.EndInit();

                    canvas.Background = new ImageBrush(logo);
                    TemperaturesCanvas[canvas.Name] = draggedItem;
                    ((Label)(canvas).Children[2]).Content = draggedItem.Name;
                    canvas.Resources.Add("taken", true);
                    Temperatures.Remove(Temperatures.Single(x => x.ID == draggedItem.ID));
                    SelectedIndex = 0;
                    CheckValue(canvas);
                    ConnectCommand.RaiseCanExecuteChanged();
                }
                else
                {                                                          
                    ((TextBlock)(canvas).Children[1]).Text = "";
                    ((TextBlock)(canvas).Children[1]).Background = Brushes.Transparent;
                }
                dragging = false;
            }

        }

        public void OnFree(Canvas canvas)
        {
            try
            {
                if (canvas.Resources["taken"] != null)
                {
                                                                           
                    canvas.Background = Brushes.White;
                    foreach (Temperature temp in NetworkEntitiesViewModel.Temperatures)
                    {
                        if (!Temperatures.Contains(temp) && TemperaturesCanvas[canvas.Name].ID == temp.ID)
                            Temperatures.Add(temp);
                    }
                    ((Label)(canvas).Children[2]).Content = "";
                    canvas.Resources.Remove("taken");
                    TemperaturesCanvas.Remove(canvas.Name);
                    ConnectCommand.RaiseCanExecuteChanged();
                }
            }
            catch (Exception) { }

        }

       
        public void OnLoadListView(ListView listview)
        {
            listViewItem = listview;
        }

        public void OnLoad(Canvas canvas)
        {
            if (TemperaturesCanvas.ContainsKey(canvas.Name))
            {
               
                BitmapImage logo = new BitmapImage();

                logo.BeginInit();
                Temperature temp = TemperaturesCanvas[canvas.Name];
                logo.UriSource = new Uri("pack://application:,,,/Slike/" + temp.TemperatureType.ToString() +".jpg", UriKind.Absolute);
                logo.EndInit();

                ((Label)(canvas).Children[2]).Content = temp.Name;
                
                canvas.Background = new ImageBrush(logo);
                ((TextBlock)(canvas).Children[1]).Text = "";
                canvas.Resources.Add("taken", true);

                CheckValue(canvas);

            }
            if (!CanvasList.Contains(canvas))
            {
                CanvasList.Add(canvas);
            }
        }

        private void CheckValue(Canvas canvas)
        {
            Dictionary<int, Temperature> temperatures = new Dictionary<int, Temperature>();
            foreach (var temp in NetworkEntitiesViewModel.Temperatures)
            {
                temperatures.Add(temp.ID, temp);
            }
            Task.Delay(1000).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ((Border)(canvas).Children[0]).BorderBrush = Brushes.Transparent;

                    if (TemperaturesCanvas.Count != 0)
                    {
                        if (TemperaturesCanvas.ContainsKey(canvas.Name))
                        {
                            if(!TemperaturesCanvas.ContainsKey(canvas.Name) || !temperatures.ContainsKey(TemperaturesCanvas[canvas.Name].ID))
                            {
                                return;
                            }
                             else if (temperatures[TemperaturesCanvas[canvas.Name].ID].MesurmentValue > 250 || temperatures[TemperaturesCanvas[canvas.Name].ID].MesurmentValue < 350)
                            {
                                ((Border)(canvas).Children[0]).BorderBrush = Brushes.Yellow;
                            }
                            else
                            {
                                ((Border)(canvas).Children[0]).BorderBrush = Brushes.Red;
                            }
                        }
                    }
                });
                CheckValue(canvas);
            });
        }

    }
}
