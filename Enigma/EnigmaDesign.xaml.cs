using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;

namespace Enigma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.KeyDown += new KeyEventHandler(Window_KeyDown);
            this.KeyUp += new KeyEventHandler(Window_KeyUp);
            InitializeComponent();
        }



        private void Enigma_Start(char pressed, MainWindow window)
        {
            char currentCharacter = pressed;
            //TODO Encrypt
            if (plugboardConnections.ContainsKey(currentCharacter))
            {
                currentCharacter = plugboardConnections[currentCharacter];
            }


            currentCharacter = Encrypt(currentCharacter);
            DisplayCharacter(currentCharacter, window);

        }


        private char Encrypt(char currentCharacter)
        {
            return currentCharacter;
        }


        #region Output
        private void DisplayCharacter(char pressed, MainWindow window)
        {

            TabControl tabControl = (TabControl)window.Content;
            TabItem tab = (TabItem)tabControl.Items[0];
            Grid topGrid = (Grid)tab.Content;
            Grid keyboard = (Grid)topGrid.Children[1];

            foreach (Button b in keyboard.Children)
            {
                if (b.Content.ToString()[0] == pressed)
                {
                    b.Foreground = new SolidColorBrush(Colors.Black);
                    Ellipse ellipse = (Ellipse)b.Template.FindName("ellipse", b);
                    ellipse.Fill = new SolidColorBrush(Colors.Yellow);
                }
            }
        }
        #endregion

        #region Button Picking
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            Button clicked = (Button)sender;
            string letter = clicked.Content.ToString();

            MainWindow window = (MainWindow)GetWindow(clicked);
            if (window.ActiveTab.SelectedIndex == 0)
            {
                InteractPress(letter, window);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

                MainWindow window = (MainWindow)sender;
                string letter = e.Key.ToString();

            if (window.ActiveTab.SelectedIndex == 0)
            {
                InteractPress(letter, window);
            }
        }

        private void InteractPress(string letter, object sender)
        {
            if (Char.IsLetter(letter[0]) && letter.Length == 1)
            {
                char pressed = letter[0];
                MainWindow window = (MainWindow)sender;
                TabControl tabControl = (TabControl)window.Content;
                TabItem tab = (TabItem)tabControl.Items[0];
                Grid topGrid = (Grid)tab.Content;
                Grid keyboard = (Grid)topGrid.Children[0];

                foreach (Button b in keyboard.Children)
                {
                    if (b.Content.ToString()[0] == pressed)
                    {
                        b.Foreground = new SolidColorBrush(Colors.Black);
                        Ellipse ellipse = (Ellipse)b.Template.FindName("ellipse", b);
                        ellipse.Fill = new SolidColorBrush(Colors.White);
                        break;
                    }
                }

                Enigma_Start(pressed, window);
            }

        }

        #endregion

        #region Cleanup
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (Char.IsLetter(e.Key.ToString()[0]) && e.Key.ToString().Length == 1)
            {
                char pressed = e.Key.ToString()[0];
                MainWindow window = (MainWindow)sender;
                TabControl tabControl = (TabControl)window.Content;
                TabItem tab = (TabItem)tabControl.Items[0];
                Grid topGrid = (Grid)tab.Content;
                Grid keyboard = (Grid)topGrid.Children[0];

                foreach (Button b in keyboard.Children)
                {
                    if (b.Content.ToString()[0] == pressed)
                    {
                        Cleanup(b);
                    }
                }
            }

        }

        private void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cleanup((Button)sender);
        }

        private void Cleanup(Button b)
        {
            MainWindow window = (MainWindow)GetWindow(b);

            if (window.ActiveTab.SelectedIndex == 0)
            {

                TabControl tabControl = (TabControl)window.Content;
                TabItem tab = (TabItem)tabControl.Items[0];
                Grid topGrid = (Grid)tab.Content;
                Grid display = (Grid)topGrid.Children[1];

                foreach (Button i in display.Children)
                {
                    Ellipse iellipse = (Ellipse)i.Template.FindName("ellipse", i);
                    iellipse.Fill = new SolidColorBrush(Colors.Gray);
                }

                b.Foreground = new SolidColorBrush(Colors.White);
                Ellipse ellipse = (Ellipse)b.Template.FindName("ellipse", b);
                ellipse.Fill = new SolidColorBrush(Colors.Black);
                return;
            }
        }



        #endregion

        #region Plugboard
        private Color random;
        private Dictionary<char, char> plugboardConnections = new Dictionary<char, char>();
        private char activePlug = ' ';
        private Random rnd = new Random();

        private void PlugSelect(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            char chosenPlug = clicked.Content.ToString()[0];
            Ellipse ellipse = (Ellipse)clicked.Template.FindName("ellipse", clicked);
            SolidColorBrush c = (SolidColorBrush)ellipse.Fill;
            MainWindow window = (MainWindow)GetWindow(clicked);

            if (c.Color == Colors.Gray || !plugboardConnections.ContainsKey(chosenPlug))
            {

                if (activePlug == ' ')
                {
                    activePlug = chosenPlug;
                    random = Color.FromArgb(255, (byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
                    ellipse.Fill = new SolidColorBrush(random);
                }
                else
                {

                    if (activePlug == chosenPlug)
                    {
                        activePlug = ' ';
                        ellipse.Fill = new SolidColorBrush(Colors.Gray);
                    }
                    else
                    {
                        char connectingPlug = activePlug;
                        activePlug = ' ';

                        plugboardConnections.Add(connectingPlug, chosenPlug);
                        plugboardConnections.Add(chosenPlug, connectingPlug);

                        ellipse.Fill = new SolidColorBrush(random);
                    }

                }
            }
            else
            {
                char connector = plugboardConnections[chosenPlug];
                plugboardConnections.Remove(chosenPlug);
                plugboardConnections.Remove(connector);

                ellipse.Fill = new SolidColorBrush(Colors.Gray);

                TabControl tabControl = (TabControl)window.Content;
                TabItem tab = (TabItem)tabControl.Items[1];
                Grid keyboard = (Grid)tab.Content;

                foreach (Button b in keyboard.Children)
                {
                    if (b.Content.ToString()[0] == connector)
                    {
                        Ellipse ellipse2 = (Ellipse)b.Template.FindName("ellipse", b);
                        ellipse2.Fill = new SolidColorBrush(Colors.Gray);
                    }
                }
            }
        }

        #endregion
    }

}
