﻿//
// CAVOKATOR APP
// Website: https://github.com/Manuito83/Cavokator
// License GNU General Public License v3.0
// Manuel Ortega, 2018
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text.Style;

namespace Cavokator
{

    /// <summary>
    /// Class used to create instances of ClickableSpan incorporating the actual text
    /// that was clicked as eventargs
    /// </summary>
    class ClickableSpan : Android.Text.Style.ClickableSpan
    {
        public event EventHandler<MyClickableSpanArgs> ClickedMyClickableSpan;

        private string _text_value;

        public ClickableSpan(string input_value)
        {
            _text_value = input_value;
        }

        public override void OnClick(View widget)
        {
            ClickedMyClickableSpan?.Invoke(this, new MyClickableSpanArgs() { Clicklable_Text = _text_value });
        }
    }

    class MyClickableSpanArgs : EventArgs
    {
        public string Clicklable_Text { get; set; }
    }
}