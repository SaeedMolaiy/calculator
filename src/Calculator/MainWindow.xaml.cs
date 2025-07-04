using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private const char DecimalSeparator = '.';

        private bool haveDecimal = false;

        private readonly RoutedEventArgs _clickEventArgs;
        private readonly DataTable _dataTable;

        private readonly Dictionary<decimal, Button> _numberToButtonMap;

        public MainWindow()
        {
            InitializeComponent();

            _dataTable = new DataTable();
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

        private string ResultLableContent => ResultLabel.Content?.ToString() ?? string.Empty;

        private string CurrentExpression => ExpressionLabel.Content?.ToString() ?? string.Empty;

        private decimal CurrentNumber
        {
            get
            {
                if (decimal.TryParse(ResultLableContent.ToString(), out var currentNumber))
                    return currentNumber;

                return 0;
            }
        }

        private void Calculate()
        {
            var expression =
                CurrentExpression.Trim() + $"{CurrentNumber}";

            var result =
                _dataTable.Compute(expression, null).ToString() ?? "0";

            SetExpressionLabelContent(string.Empty);
            SetResultLabelContent(result);
            haveDecimal = false;
        }

        private void SetMathOperation(MathOperation operation)
        {
            if (CurrentNumber == 0)
                return;

            string operationSymbol = operation switch
            {
                MathOperation.Plus => "+",
                MathOperation.Minus => "-",
                MathOperation.Multiply => "*",
                MathOperation.Divide => "/",
                _ => ""
            };

            var expression =
                $"{CurrentExpression}{CurrentNumber} {operationSymbol} ";

            SetExpressionLabelContent(expression);
            SetResultLabelContent("0");
            haveDecimal = false;
        }

        private void AddNumber(decimal number)
        {
            if (ResultLableContent == "0")
            {
                SetResultLabelContent(number.ToString());
                return;
            }

            var newLabelContent = CurrentNumber.ToString();
            var appendDecimalSeparator =
                haveDecimal && newLabelContent.Contains(DecimalSeparator) == false;

            if (appendDecimalSeparator)
            {
                newLabelContent += DecimalSeparator;
            }

            SetResultLabelContent(newLabelContent + number.ToString());
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
            PercentageButton.Click += PercentageButton_Click;
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

        private void PercentageButton_Click(object sender, RoutedEventArgs e)
        {
            Calculate();

            SetResultLabelContent(
                (CurrentNumber / 100).ToString());
        }

        private void EqualButton_Click(object sender, RoutedEventArgs e) => Calculate();

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ResultLableContent))
                return;

            if (haveDecimal && ResultLableContent.Contains(DecimalSeparator))
                return;

            SetResultLabelContent($"{CurrentNumber}{DecimalSeparator}");
            haveDecimal = true;
        }

        private void ACButton_Click(object sender, RoutedEventArgs e)
        {
            SetExpressionLabelContent(string.Empty);
            SetResultLabelContent("0");

            haveDecimal = false;
        }

        private void NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            SetResultLabelContent((CurrentNumber * -1).ToString());
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
            if (TryParseNumberFromKeyboardKey(key, out var pressedNumber) == false)
                return;

            var clickButton =
                _numberToButtonMap[pressedNumber];

            if (clickButton != null)
                _ = SimulateButtonClick(
                        clickButton, App.NumberButtonBrush, App.NumberButtonClickBrush);

            return;
        }

        private void HandleCopyAndPasteKeyPress(Key key)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) == false)
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
                        SetResultLabelContent(pastedText);
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
                case Key.Add:
                    clickedButton = PlusButton;
                    break; ;

                case Key.OemMinus:
                case Key.Subtract:
                    clickedButton = MinusButton;
                    break;

                case Key.OemQuestion:
                case Key.Divide:
                    clickedButton = DivideButton;
                    break;

                case Key.Multiply:
                    clickedButton = MultiplyButton;
                    break;
            }

            if (clickedButton != null)
            {
                var defaultColorBrush = App.OperationButtonBrush;
                var targetColorBrush = App.OperationButtonClickBrush;

                if (clickedButton.Equals(EqualButton))
                {
                    defaultColorBrush = App.EqualButtonBrush;
                    targetColorBrush = App.EqualButtonClickBrush;
                }

                _ = SimulateButtonClick(
                        clickedButton, defaultColorBrush, targetColorBrush);
            }
        }

        private void HandleDecimalSeperatorKeyPress(Key key)
        {
            if (key == Key.OemPeriod)
            {
                _ = SimulateButtonClick(
                        DecimalButton, App.NumberButtonBrush, App.NumberButtonClickBrush);
            }
        }

        private void HandleBackSpaceKeyPress(Key key)
        {
            if (key == Key.Back)
            {
                var content = ResultLableContent;

                if (content.Length == 1)
                    content = "0";
                else
                    content = content[..^1];

                SetResultLabelContent(content);
            }
        }

        private async Task SimulateButtonClick(Button button, SolidColorBrush defaultColorBrush, SolidColorBrush targetColorBrush)
        {
            button.RaiseEvent(_clickEventArgs);

            button.Background = targetColorBrush;
            await Task.Delay(120);
            button.Background = defaultColorBrush;
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

        private void SetResultLabelContent(string content)
        {
            ResultLabel.Content = content;
        }

        private void SetExpressionLabelContent(string content)
        {
            ExpressionLabel.Content = content;
        }
    }
}