using System.Windows;
using System.Windows.Input;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private const char DecimalSeparator = '.';

        private bool haveDecimal = false;
        private MathOperation mathOperation;

        public MainWindow()
        {
            InitializeComponent();
            SetEventHandlers();
        }

        private decimal CurrentNumber
        {
            get
            {
                if (decimal.TryParse(ResultLabel.Content.ToString(), out var currentNumber))
                    return currentNumber;

                return 0;
            }
        }

        private void Calculate()
        {
        }

        private void AddNumber(decimal number)
        {
            if (CurrentNumber == 0)
            {
                ResultLabel.Content = number.ToString();
                return;
            }

            var newLabelContent = CurrentNumber.ToString();
            var appendDecimalSeparator =
                haveDecimal && newLabelContent.Contains(DecimalSeparator) == false;

            if (appendDecimalSeparator)
            {
                newLabelContent += DecimalSeparator;
            }

            ResultLabel.Content = newLabelContent + number.ToString();
        }

        private void SetMathOperation(MathOperation operation)
        {
            mathOperation = operation;
        }

        private void SetEventHandlers()
        {
            //Window
            CloseButton.Click += CloseButton_Click;
            MinimizeButton.Click += MinimizeButton_Click;
            MaximizeButton.Click += MaximizeButton_Click;
            TitleBar.MouseLeftButtonDown += TitleBarControl_MouseLeftButtonDown;

            // Math Operations
            PlusButton.Click += PlusButton_Click;
            MinusButton.Click += MinusButton_Click;
            DivideButton.Click += DivideButton_Click;
            MultiplyButton.Click += MultiplyButton_Click;
            EqualButton.Click += EqualButton_Click;

            // Numbers
            NumberOneButton.Click += NumberOneButton_Click;
            NumberTwoButton.Click += NumberTwoButton_Click;
            NumberThreeButton.Click += NumberThreeButton_Click;
            NumberFourButton.Click += NumberFourButton_Click;
            NumberFiveButton.Click += NumberFiveButton_Click;
            NumberSixButton.Click += NumberSixButton_Click;
            NumberSevenButton.Click += NumberSevenButton_Click;
            NumberEightButton.Click += NumberEightButton_Click;
            NumberNineButton.Click += NumberNineButton_Click;
            NumberZeroButton.Click += NumberZeroButton_Click;
            DecimalButton.Click += DecimalButton_Click;

            // Others
            ACButton.Click += ACButton_Click;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void MaximizeButton_Click(object sender, RoutedEventArgs e) =>
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;

        private void TitleBarControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e) => SetMathOperation(MathOperation.Plus);

        private void MinusButton_Click(object sender, RoutedEventArgs e) => SetMathOperation(MathOperation.Minus);

        private void DivideButton_Click(object sender, RoutedEventArgs e) => SetMathOperation(MathOperation.Divide);

        private void MultiplyButton_Click(object sender, RoutedEventArgs e) => SetMathOperation(MathOperation.Multiply);

        private void EqualButton_Click(object sender, RoutedEventArgs e) => Calculate();

        private void NumberOneButton_Click(object sender, RoutedEventArgs e) => AddNumber(1);

        private void NumberTwoButton_Click(object sender, RoutedEventArgs e) => AddNumber(2);

        private void NumberThreeButton_Click(object sender, RoutedEventArgs e) => AddNumber(3);

        private void NumberFourButton_Click(object sender, RoutedEventArgs e) => AddNumber(4);

        private void NumberFiveButton_Click(object sender, RoutedEventArgs e) => AddNumber(5);

        private void NumberSixButton_Click(object sender, RoutedEventArgs e) => AddNumber(6);

        private void NumberSevenButton_Click(object sender, RoutedEventArgs e) => AddNumber(7);

        private void NumberEightButton_Click(object sender, RoutedEventArgs e) => AddNumber(8);

        private void NumberNineButton_Click(object sender, RoutedEventArgs e) => AddNumber(9);

        private void NumberZeroButton_Click(object sender, RoutedEventArgs e) => AddNumber(0);

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (haveDecimal)
                return;

            ResultLabel.Content = $"{CurrentNumber}{DecimalSeparator}";
            haveDecimal = true;
        }

        private void ACButton_Click(object sender, RoutedEventArgs e)
        {
            ResultLabel.Content = "0";
            haveDecimal = false;
        }
    }
}