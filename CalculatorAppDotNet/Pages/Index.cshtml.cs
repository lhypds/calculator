using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CalculatorAppDotNet.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Display { get; set; }

        private static decimal? memory;

        public void OnGet()
        {
            Display = string.Empty;
        }

        [ValidateAntiForgeryToken]
        public void OnPost(string number, string operation, string action, string calculate, string memoryAction, string currentDisplay)
        {
            // Preserve the current display value
            Display = currentDisplay;

            if (!string.IsNullOrEmpty(number))
            {
                Display += number;
            }

            if (!string.IsNullOrEmpty(operation))
            {
                Display += $" {operation} ";
            }

            if (!string.IsNullOrEmpty(action))
            {
                if (action == "C")
                {
                    Display = string.Empty;
                }
                else if (action == "AC")
                {
                    Display = string.Empty;
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