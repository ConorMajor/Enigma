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
            RotorLeft = rnd.Next(26);
            RotorRight = rnd.Next(26);
            RotorCentre = rnd.Next(26);
            DataContext = this;
        }



        private void Enigma_Start(char pressed, MainWindow window)
        {
            char currentCharacter = pressed;
            //TODO Encrypt



            currentCharacter = Encrypt(currentCharacter);
            DisplayCharacter(currentCharacter, window);

        }


        private char Encrypt(char currentCharacter)
        {
            if (plugboardConnections.ContainsKey(currentCharacter))
            {
                currentCharacter = plugboardConnections[currentCharacter];
            }

            RotorPass(ref currentCharacter, RotorTypes[RRName], RotorRight);
            RotorPass(ref currentCharacter, RotorTypes[RCName], RotorCentre);
            RotorPass(ref currentCharacter, RotorTypes[RLName], RotorLeft);
            RotorPass(ref currentCharacter, RotorTypes[RLName], RotorLeft);
            RotorPass(ref currentCharacter, RotorTypes[RCName], RotorCentre);
            RotorPass(ref currentCharacter, RotorTypes[RRName], RotorRight);

            RotorRightUp(null, null);


            if (plugboardConnections.ContainsKey(currentCharacter))
            {
                currentCharacter = plugboardConnections[currentCharacter];
            }

            return currentCharacter;
        }

        private void RotorPass(ref char currentCharacter, string rotorType, int rotorIndex)
        {
            char[] letters = rotorType.ToArray();
            int i = Array.IndexOf(letters, currentCharacter);
            Array.Reverse(letters,0, rotorIndex);
            Array.Reverse(letters, rotorIndex, letters.Length - rotorIndex);
            Array.Reverse(letters, 0, letters.Length - 1);

            char[] indexedletters = letters;
            
            currentCharacter = indexedletters[i];
        }


        #region Rotors

        

        public int RLName { get; set; } = 1;
        public int RCName { get; set; } = 1;
        public int RRName { get; set; } = 1;

        List<string> RotorTypes = new List<string>() {

        "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        ,"HTBZEVAFYROSUGPNLQMJCKWXID"
        ,"ZHKVMGRSBIEPJYWOANTXDFQLUC"
        ,"ZGFJYVRDCUSPIHWEXQNOMLKTBA"
        ,"YTZSGUKCINRWAEXQDHJMOPVBFL"
        ,"AWLYXNDKGZPRITMVCJBEFQOSHU"
        ,"IYKENXLGHRVJPZQASTCBDOMWFU"
        ,"JTIFGOLEMANVWYCKDUZRQBHXSP"
        ,"RSPAYBMILQHGWNFZCOUTKEDVXJ"
        ,"OYKBFDICWZLHSPREVMAGUNXQJT"
        };

        public int RotorLeft { get; set; } = 1;
        public int RotorCentre { get; set; } = 1;
        public int RotorRight { get; set; } = 1;

        private void RotorLeftDown(object sender, RoutedEventArgs e)
        {
            RotorLeft = RotorLeft - 1;
            if (RotorLeft < 1)
            {
                RotorLeft = 26;
            }

            RotorL.GetBindingExpression(Button.ContentProperty).UpdateTarget();
        }

        private void RotorCentreDown(object sender, RoutedEventArgs e)
        {
            RotorCentre = RotorCentre - 1;
            if (RotorCentre < 1)
            {
                RotorCentre = 26;
                RotorLeftDown(null, null);
            }

            RotorC.GetBindingExpression(Button.ContentProperty).UpdateTarget();
        }

        private void RotorRightDown(object sender, RoutedEventArgs e)
        {
            RotorRight = RotorRight - 1;
            if (RotorRight < 1)
            {
                RotorRight = 26;
                RotorCentreDown(null, null);
            }
            RotorR.GetBindingExpression(Button.ContentProperty).UpdateTarget();
        }

        private void RotorLeftUp(object sender, RoutedEventArgs e)
        {
            RotorLeft = RotorLeft + 1;
            if (RotorLeft > 26)
            {
                RotorLeft = 1;
            }
            RotorL.GetBindingExpression(Button.ContentProperty).UpdateTarget();
        }

        private void RotorCentreUp(object sender, RoutedEventArgs e)
        {
            RotorCentre = RotorCentre + 1;
            if (RotorCentre > 26)
            {
                RotorCentre = 1;
                RotorLeftUp(null, null);
            }
            RotorC.GetBindingExpression(Button.ContentProperty).UpdateTarget();
        }

        private void RotorRightUp(object sender, RoutedEventArgs e)
        {
            RotorRight = RotorRight + 1;
            if (RotorRight > 26)
            {
                RotorRight = 1;
                RotorCentreUp(null, null);
            }
            RotorR.GetBindingExpression(Button.ContentProperty).UpdateTarget();
        }

        

        #endregion

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
                        SolidColorBrush c = (SolidColorBrush)b.Foreground;
                        if(Colors.Black == c.Color)
                        {
                            return;
                        }

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


        private void RotorLeftTypeChange(object sender, RoutedEventArgs e)
        {
            RLName = RLName + 1;
            if(RLName > RotorTypes.Count)
            {
                RLName = 1;
            }

            RLButton.GetBindingExpression(Button.ContentProperty).UpdateTarget();
        }

        private void RotorCentreTypeChange(object sender, RoutedEventArgs e)
        {
            RCName = RCName + 1;
            if (RCName > RotorTypes.Count)
            {
                RCName = 1;
            }
            RCButton.GetBindingExpression(Button.ContentProperty).UpdateTarget();
        }

        private void RotorRightTypeChange(object sender, RoutedEventArgs e)
        {
            RRName = RRName + 1;
            if (RRName > RotorTypes.Count)
            {
                RRName = 1;
            }
            RRButton.GetBindingExpression(Button.ContentProperty).UpdateTarget();
        }
    }

}
