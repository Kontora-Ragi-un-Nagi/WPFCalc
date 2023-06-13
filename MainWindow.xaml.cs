using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WPFCalc.Utils;

namespace WPFCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool EqualFlag = false;
        string DecimalSeparator => CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
        decimal FirstValue { get; set; }
        decimal? SecondValue { get; set; }

        IOperation CurrentOperation;

        public MainWindow()
        {
            InitializeComponent();
            btnPoint.Content = DecimalSeparator;
            btnSum.Tag = new Sum();
            btnSubtraction.Tag = new Subtraction();
            btnDivision.Tag = new Division();
            btnMultiplication.Tag = new Multiplication();
        }

        private void regularButtonClick(object sender, RoutedEventArgs e)
            => SendToInput(((Button)sender).Content.ToString());

        private void SendToInput(string content)
        {
            if (EqualFlag) { txtInput.Text = ""; EqualFlag = false; }

            //Prevent 0 from appearing on the left of new numbers
            if (txtInput.Text == "0")
                txtInput.Text = "";

            txtInput.Text = $"{txtInput.Text}{content}";
        }

        private void btnPoint_Click(object sender, RoutedEventArgs e)
        {
            if (txtInput.Text.Contains(this.DecimalSeparator))
                return;

            regularButtonClick(sender, e);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //Prevent from clearing zero
            if (txtInput.Text == "0")
                return;

            txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1);
            if (txtInput.Text == "")
                txtInput.Text = "0";
        }

        private void operationButton_Click(object sender, RoutedEventArgs e)
        {
            EqualFlag = false;

            //if current operation is not null then we already have the FirstValue
            if (CurrentOperation == null)
                FirstValue = Convert.ToDecimal(txtInput.Text);

            CurrentOperation = (IOperation)((Button)sender).Tag;
            SecondValue = null;
            txtInput.Text = "";
        }

        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            switch (e.Text)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    SendToInput(e.Text);
                    break;

                case "*":
                    btnMultiplication.PerformClick();
                    break;

                case "-":
                    WorkWithMinus();

                    break;

                case "+":
                    btnSum.PerformClick();
                    break;

                case "/":
                    btnDivision.PerformClick();
                    break;

                case "=":
                    btnEquals.PerformClick();
                    break;

                case "q": btnSQRT_Click(sender, e); break;
                case ".":
                case ",": btnPoint_Click(btnPoint, e); break;

                default:
                    //Can't use directly from switch because it is not a constant
                    if (e.Text == DecimalSeparator)
                        btnPoint.PerformClick();
                    else if (e.Text[0] == (char)8)
                        btnBack.PerformClick();
                    else if (e.Text[0] == (char)13)
                        btnEquals.PerformClick();

                    break;
            }

            //This will prevent other buttons focus firing its click event on <ENTER> while typing
            btnEquals.Focus();
        }


        bool minusflag = false;
        private void WorkWithMinus()
        {
            if (txtInput.Text == "0")
            {
                txtInput.Text = "-";
                minusflag = true;
            }
            else if (txtInput.Text == "-")
            {
                txtInput.Text = "0";
            }
            else
            {
                operationButton_Click(btnSubtraction, null);
            }
        }

        private void btnEquals_Click(object sender, RoutedEventArgs e)
        {
            EqualFlag = true;

            if (CurrentOperation == null)
                return;

            if (txtInput.Text == "")
                return;

            //SecondValue is used for multiple clicks on Equals bringing the newest result of last operation
            decimal val2 = SecondValue ?? Convert.ToDecimal(txtInput.Text);
            try
            {
                txtInput.Text = (FirstValue = CurrentOperation.DoOperation(FirstValue, (decimal)(SecondValue = val2))).ToString();
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show("Can't divide by zero", "Divided by zero", MessageBoxButton.OK, MessageBoxImage.Error);
                btnClearAll.PerformClick();
            }
        }

        private void btnClearEntry_Click(object sender, RoutedEventArgs e)
            => txtInput.Text = "0";

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            FirstValue = 0;
            CurrentOperation = null;
            txtInput.Text = "0";
        }

        private void btnSQRT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string CurrentDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string StringValue = txtInput.Text;
                if (StringValue.Contains(".")) StringValue = StringValue.Replace(".", CurrentDecimalSeparator);
                else
                if (StringValue.Contains(",")) StringValue = StringValue.Replace(",", CurrentDecimalSeparator);

                double value = double.Parse(StringValue);
                if (value < 0)
                {
                    MessageBox.Show("Nedrīkst izvilkt kvadrātsakni no negatīva skaitļa!");
                }
                else
                {
                    txtInput.Text = Math.Sqrt(value).ToString();
                }
            }
            catch (ArgumentException argex)
            {
                MessageBox.Show("Vertība nepareizājā formātā");
            }
        }

        private void btnSubtraction_Click(object sender, RoutedEventArgs e)
        {
            WorkWithMinus();
        }
    }
}
