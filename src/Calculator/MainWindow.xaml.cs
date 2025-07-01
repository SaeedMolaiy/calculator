using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private const string NumberButtonsDefaultColor = "#3a3a3b";
        private const string NumberButtonsClickColor = "#333233";

        private const string EqualButtonDefaultColor = "#4dc2fe";
        private const string EqualButtonClickColor = "#1b9cd8";

        private const string OtherButtonsDefaultColor = "#333233";
        private const string OtherButtonsClickColor = "#3a3a3b";

        private const char DecimalSeparator = '.';

        private bool haveDecimal = false;
        private MathOperation mathOperation;

        private readonly ColorConverter _colorConverter;
        private readonly RoutedEventArgs _clickEventArgs;

        private readonly Dictionary<decimal, Button> _numberToButtonMap;

        public MainWindow()
        {
            InitializeComponent();

            _colorConverter = new ColorConverter();
            _clickEventArgs = new RoutedEventArgs(Button.ClickEvent);

            _numberToButtonMap = new Dictionary<decimal, Button>()
            {
                {1, NumberOneButton },
                {2, NumberTwoButton },
                {3, NumberThreeButton },
                {4, NumberFourButton },
                {5, NumberFiveButton },
                {6, NumberSixButton },
                {7, NumberSevenButton },
                {8, NumberEightButton },
                {9, NumberNineButton },
                {0, NumberZeroButton }
            };

            ConfigureWindowEventHandlers();
            ConfigureMathOperationEventHandlers();
            ConfigureOtherEventHandlers();
            ConfigureNumberButtons();
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

        private void ConfigureWindowEventHandlers()
        {
            CloseButton.Click += CloseButton_Click;
            MinimizeButton.Click += MinimizeButton_Click;
            MaximizeButton.Click += MaximizeButton_Click;
            TitleBar.MouseLeftButtonDown += TitleBarControl_MouseLeftButtonDown;
            PreviewKeyDown += HandleKeyboardInput;
        }

        private void ConfigureMathOperationEventHandlers()
        {
            PlusButton.Click += PlusButton_Click;
            MinusButton.Click += MinusButton_Click;
            DivideButton.Click += DivideButton_Click;
            MultiplyButton.Click += MultiplyButton_Click;
            EqualButton.Click += EqualButton_Click;
        }

        private void ConfigureOtherEventHandlers()
        {
            ACButton.Click += ACButton_Click;
            NegativeButton.Click += NegativeButton_Click;
            DecimalButton.Click += DecimalButton_Click;
        }

        private void ConfigureNumberButtons()
        {
            foreach (var each in _numberToButtonMap)
            {
                var number = each.Key;
                var button = each.Value;

                button.Content = number.ToString();
                button.Click += NumberButton_Click;
            }
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

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            var content =
                ResultLabel.Content.ToString();

            if (string.IsNullOrEmpty(content))
                return;

            if (haveDecimal && content.Contains(DecimalSeparator))
                return;

            ResultLabel.Content = $"{CurrentNumber}{DecimalSeparator}";
            haveDecimal = true;
        }

        private void ACButton_Click(object sender, RoutedEventArgs e)
        {
            ResultLabel.Content = "0";
            haveDecimal = false;
        }

        private void NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            ResultLabel.Content = (CurrentNumber * -1).ToString();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            if (button == null)
                return;

            var number =
                GetButtonContentAsDecimal(button);

            AddNumber(number);
        }

        private void HandleKeyboardInput(object sender, KeyEventArgs e)
        {
            var key = e.Key;

            HandleNumberKeyPress(key);
            HandleCopyAndPasteKeyPress(key);
            HandleMathOperationsKeyPress(key);
            HandleDecimalSeperatorKeyPress(key);
            HandleBackSpaceKeyPress(key);

            e.Handled = true;
        }

        private void HandleNumberKeyPress(Key key)
        {
            if (IsHoldingShift())
                return;

            if (TryParseNumberFromKeyboardKey(key, out var pressedNumber) == false)
                return;

            var clickButton =
                _numberToButtonMap[pressedNumber];

            if (clickButton != null)
                _ = SimulateButtonClick(
                    clickButton, NumberButtonsDefaultColor, NumberButtonsClickColor);

            return;
        }

        private void HandleCopyAndPasteKeyPress(Key key)
        {
            if (IsHoldingControl() == false)
                return;

            if (key == Key.C)
            {
                Clipboard.SetText(CurrentNumber.ToString());
            }
            else if (key == Key.V)
            {
                if (Clipboard.ContainsText())
                {
                    var pastedText =
                        Clipboard.GetText();

                    if (decimal.TryParse(pastedText, out _))
                    {
                        ResultLabel.Content = pastedText;
                    }
                }
            }
        }

        private void HandleMathOperationsKeyPress(Key key)
        {
            Button? clickedButton = null;

            switch (key)
            {
                case Key.Enter:
                    clickedButton = EqualButton;
                    break;

                case Key.OemPlus:
                    clickedButton =
                        IsHoldingShift() ? EqualButton : PlusButton;
                    break;

                case Key.OemMinus:
                    clickedButton = MinusButton;
                    break;

                case Key.OemQuestion:
                    clickedButton = DivideButton;
                    break;

                case Key.D8:
                    if (IsHoldingShift())
                        clickedButton = MultiplyButton;
                    break;
            }

            if (clickedButton != null)
            {
                var defaultColor = OtherButtonsDefaultColor;
                var color = OtherButtonsClickColor;

                if (clickedButton.Equals(EqualButton))
                {
                    defaultColor = EqualButtonDefaultColor;
                    color = EqualButtonClickColor;
                }

                _ = SimulateButtonClick(
                    clickedButton, defaultColor, color);
            }
        }

        private void HandleDecimalSeperatorKeyPress(Key key)
        {
            if (key == Key.OemPeriod)
            {
                _ = SimulateButtonClick(
                    DecimalButton, NumberButtonsDefaultColor, NumberButtonsClickColor);
            }
        }

        private void HandleBackSpaceKeyPress(Key key)
        {
            if (key == Key.Back)
            {
                var content =
                    ResultLabel.Content.ToString() ?? "";

                if (content.Length == 1)
                    content = "0";
                else
                    content = content[..^1];

                ResultLabel.Content = content;
            }
        }

        private async Task SimulateButtonClick(Button button, string defaultColorHex, string colorHex)
        {
            button.RaiseEvent(_clickEventArgs);

            var defaultColor = GetColorFromHex(defaultColorHex);
            var color = GetColorFromHex(colorHex);

            button.Background =
                new SolidColorBrush(color);

            await Task.Delay(120);

            button.Background =
                new SolidColorBrush(defaultColor);
        }

        private Color GetColorFromHex(string hex)
        {
            var convertedColor = _colorConverter.ConvertFrom(hex);

            if (convertedColor != null)
                return (Color)convertedColor;

            return new Color();
        }

        private static bool TryParseNumberFromKeyboardKey(Key key, out decimal digit)
        {
            digit = key switch
            {
                >= Key.D0 and <= Key.D9 => key - Key.D0,
                >= Key.NumPad0 and <= Key.NumPad9 => key - Key.NumPad0,
                _ => -1
            };

            return digit >= 0;
        }

        private static decimal GetButtonContentAsDecimal(Button button)
        {
            var content =
                button.Content.ToString();

            if (string.IsNullOrEmpty(content))
                return 0;

            return decimal.Parse(content);
        }

        private static bool IsHoldingShift() =>
             Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

        private static bool IsHoldingControl() =>
             Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
    }
}