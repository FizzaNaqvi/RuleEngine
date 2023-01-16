using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_PropertyGridPractice
{
    class MyPropertyGrid
    {
        public enum ETestEnum { option1, option2, option3 }


        [CategoryAttribute("Category 1"),
        DisplayName("Field 1 Text"),
        DescriptionAttribute("Field 1 Text")]
        public ETestEnum ComboData
        {
            get;
            set;
        }

        [CategoryAttribute("Category 2"),
        DisplayName("NumericUpdow Field Text"),
        DescriptionAttribute("Test Description")]
        public int MyNumericUpdown
        {
            get;
            set;
        }
        [CategoryAttribute("Category 2"),
        DisplayName("Textbox Field Text"),
        DescriptionAttribute("Test Description")]
        public string MyTextbox
        {
            get;
            set;
        }
    }
}

