using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Common.ResultPattern
{
    public class Unit
    {
        public static readonly Unit Value = new ();
        private Unit() { }
    }
}
