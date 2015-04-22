using System;
using System.Collections.Generic;
using System.Text;

namespace logMente
{
    class searchConditions
    {
        private string _name;
        private string _value;
        private string _checked;
        private string _index;

        protected string Name {set { this._name = value; }get { return this._name; }}
        protected string Value { set { this._value = value; } get { return this._value; } }
        protected string Checked { set { this._checked = value; } get { return this._checked; } }
        protected string Index { set { this._index = value; } get { return this._index; } }
    }
}
