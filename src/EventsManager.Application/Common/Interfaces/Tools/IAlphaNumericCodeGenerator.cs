using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Common.Interfaces.Tools
{
    public interface IAlphaNumericCodeGenerator
    {
        string Generate(int length);
    }
}
