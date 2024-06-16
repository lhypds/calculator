using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CalculatorAppDotNet.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string Display { get; set; }

        [BindProperty]
        public string Left { get; set; }

        [BindProperty]
        public string Operation { get; set; }

        [BindProperty]
        public string Right { get; set; }

        [BindProperty]
        public string Result { get; set; }

        private static decimal? memory;

        public void OnGet()
        {
            Display = "0";
            Left = "0";
        }

        [ValidateAntiForgeryToken]
        public void OnPost(string number, string operation, string action, string calculate, string memoryAction, 
                           string currentDisplay, string currentLeft, string currentOperation, string currentRight, string currentResult)
        {
            // Preserve the current display value
            Display = currentDisplay;
            Left = currentLeft ?? null;
            Operation = currentOperation ?? null;
            Right = currentRight ?? null;
            Result = currentResult ?? null;

            // I. Handle number input
            if (!string.IsNullOrEmpty(number))
            {
                _logger.LogInformation("Number Input: {number}", number);

                // Prevent multiple decimal points
                if (number == "." && Display.Contains("."))
                {
                    return;
                }

                if (Display == "0" || Display == "Error" || IsOperation(Display.Trim()) || (Left != null && Operation != null && Right == null))
                {
                    Display = number;
                }
                else
                {
                    Display += number;
                }

                // Remove leading zero
                if (Display.StartsWith("0") && !Display.Contains("."))
                {
                    Display = Display.TrimStart('0');
                    if (string.IsNullOrEmpty(Display))
                    {
                        Display = "0";
                    }
                }

                if (string.IsNullOrEmpty(Operation))
                {
                    // Input left
                    Left = Display;
                    _logger.LogInformation("Set Left: {Left}", Left);
                }
                else if (!string.IsNullOrEmpty(Operation) && Left != null)
                {
                    // Input right
                    Right = Display;
                    _logger.LogInformation("Set Right: {Right}", Right);
                }
            }

            // II. Handle operation input
            if (!string.IsNullOrEmpty(operation))
            {
                _logger.LogInformation("Operation Input: {operation}", operation);
                if (Display == "Error")
                {
                    return;
                }

                // All set, calculate the result
                if (Left != null && !string.IsNullOrEmpty(Operation) && Right != null)
                {
                    // Already has operation, calculate the result
                    Right = Display;

                    _logger.LogInformation("Left: {Left}, Operation: {Operation}, Right: {Right}", Left, Operation, Right);
                    Display = Calculate($"{Left}{Operation}{Right}").ToString();

                    Operation = operation;
                    Left = Display;
                    Right = null;
                    _logger.LogInformation("Set Left: {Left}, Set Operation: {Operation}, Set Right: {Right}", Left, Operation, Right);
                }
                else if (Left != null && Operation != null && Right == null)
                {
                    // Right is empty, opreation is not empty
                    // Replace operation
                    Operation = operation;

                    // Replace display operation if the current display is an operation
                    if (IsOperation(Display.Trim()))
                    {
                        Display = Operation;
                    }

                    _logger.LogInformation("Replace Operation: {Operation}", Operation);
                }
                else if (Left != null && Operation == null && Right == null)
                {
                    // Opration is empty
                    // Set operation
                    Display = operation;
                    Operation = operation;
                    _logger.LogInformation("Set Operation: {Operation}", Operation);
                }
                else if (Left == null && Operation == null && Right == null)
                {
                    // Left will never be null
                    Display = "Error";
                }
            }

            // III. Handle action input
            if (!string.IsNullOrEmpty(action))
            {
                _logger.LogInformation("Action Input: {action}", action);

                if (action == "C")
                {
                    ResetCalculator();
                }
                else if (action == "AC")
                {
                    ResetCalculator();
                    memory = null;
                }
            }

            // IV. Handle memory action input
            if (!string.IsNullOrEmpty(memoryAction))
            {
                _logger.LogInformation("MemoryAction Input: {memoryAction}", memoryAction);

                if (decimal.TryParse(Display, out decimal currentValue))
                {
                    if (memoryAction == "M+")
                    {
                        memory = (memory ?? 0) + currentValue;
                    }
                    else if (memoryAction == "M-")
                    {
                        memory = (memory ?? 0) - currentValue;
                    }
                    else if (memoryAction == "MRC")
                    {
                        Display = memory?.ToString() ?? "0";
                    }
                }
            }

            // V. Handle calculate input
            if (!string.IsNullOrEmpty(calculate))
            {
                _logger.LogInformation("Calculate Input: {calculate}", calculate);

                try
                {
                    _logger.LogInformation("Left: {Left}, Operation: {Operation}, Right: {Right}", Left, Operation, Right);
                    Display = Calculate($"{Left}{Operation}{Right}").ToString();

                    Left = Display;
                    Operation = null;
                    Right = null;
                    _logger.LogInformation("Set Left: {Left}, Set Operation: {Operation}, Set Right: {Right}", Left, Operation, Right);
                }
                catch (Exception)
                {
                    Display = "Error";

                    // Reset the calculator
                    Left = "0";
                    Operation = null;
                    Right = null;
                    Result = null;
                    memory = null;
                }
            }

            // If the display is too long, truncate it
            if (Display.Length > 20)
            {
                Display = Display.Substring(0, 20);
            }
        }

        private bool IsOperation(string str)
        {
            str = str.Trim().Replace("×", "*").Replace("÷", "/");
            return str == "+" || str == "-" || str == "*" || str == "/";
        }

        private decimal Calculate(string input)
        {
            var dataTable = new System.Data.DataTable();
            var value = dataTable.Compute(input.Replace("×", "*").Replace("÷", "/"), "");

            var result = Convert.ToDecimal(value);

            _logger.LogInformation("Calculate result: {input} = {value}", input, value);
            return result;
        }

        private void ResetCalculator()
        {
            Display = "0";
            Left = "0";
            Operation = null;
            Right = null;
            Result = null;
        }
    }
}