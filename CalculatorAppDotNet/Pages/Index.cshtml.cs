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
            _logger.LogInformation("OnPost called. Number: {number}, Operation: {operation}, " 
                                 + "Action: {action}, Calculate: {calculate}, " 
                                 + "MemoryAction: {memoryAction}, CurrentDisplay: {currentDisplay}", 
                                 number, operation, action, calculate, memoryAction, currentDisplay);

            // Preserve the current display value
            Display = currentDisplay;

            if (!string.IsNullOrEmpty(number))
            {
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
                if (Display.EndsWith("×") || Display.EndsWith("÷") || Display.EndsWith("+") || Display.EndsWith("-"))
                {
                    Display = Display.Substring(0, Display.Length - 1);
                }
                else if (Display.EndsWith("."))
                {
                    Display = Display.Substring(0, Display.Length - 1);
                }
                Display += $" {operation} ";
            }

            if (!string.IsNullOrEmpty(action))
            {
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
                try
                {
                    Display = Calculate(Display).ToString();
                }
                catch (Exception)
                {
                    Display = "Error";
                }
            }
        }

        private decimal Calculate(string input)
        {
            var dataTable = new System.Data.DataTable();
            var value = dataTable.Compute(input.Replace("×", "*").Replace("÷", "/"), "");
            return Convert.ToDecimal(value);
        }
    }
}