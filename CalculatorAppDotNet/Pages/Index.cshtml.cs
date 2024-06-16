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

        private static decimal? memory;

        public void OnGet()
        {
            Display = "0";
        }

        [ValidateAntiForgeryToken]
        public void OnPost(string number, string operation, string action, string calculate, string memoryAction, string currentDisplay)
        {
            // Preserve the current display value
            Display = currentDisplay;

            if (!string.IsNullOrEmpty(number))
            {
                _logger.LogInformation("Number: {number}", number);

                if (Display == "0")
                {
                    Display = number;
                }
                else
                {
                    Display += number;
                }
            }

            if (!string.IsNullOrEmpty(operation))
            {
                _logger.LogInformation("Operation: {operation}", operation);

                // Add operation only if the last character is not an operation
                if (!string.IsNullOrEmpty(Display) && !IsOperation(Display.Trim()[^1]))
                {
                    Display += $" {operation} ";
                }
            }

            if (!string.IsNullOrEmpty(action))
            {
                _logger.LogInformation("Action: {action}", action);

                if (action == "C")
                {
                    Display = "0";
                }
                else if (action == "AC")
                {
                    Display = "0";
                    memory = null;
                }
            }

            if (!string.IsNullOrEmpty(memoryAction))
            {
                _logger.LogInformation("MemoryAction: {memoryAction}", memoryAction);

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

            if (!string.IsNullOrEmpty(calculate))
            {
                _logger.LogInformation("Calculate: {calculate}", calculate);

                try
                {
                    Display = Calculate(Display).ToString();
                }
                catch (Exception)
                {
                    Display = "Error";
                }
            }

            // If the display is too long, truncate it
            if (Display.Length > 20)
            {
                Display = Display.Substring(0, 20);
            }
        }

        private bool IsOperation(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/';
        }

        private decimal Calculate(string input)
        {
            var dataTable = new System.Data.DataTable();
            var value = dataTable.Compute(input.Replace("×", "*").Replace("÷", "/"), "");
            return Convert.ToDecimal(value);
        }
    }
}