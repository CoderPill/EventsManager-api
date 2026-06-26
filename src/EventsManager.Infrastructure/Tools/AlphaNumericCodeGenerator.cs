using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Core.Constants;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EventsManager.Infrastructure.Tools
{
    public class AlphaNumericCodeGenerator : IAlphaNumericCodeGenerator
    {
        public string Generate(int CodeLength)
        {
            Span<char> chars = stackalloc char[CodeLength];

            for (var i = 0; i < CodeLength; i++)
            {
                var index = RandomNumberGenerator.GetInt32(SystemValues.Infrastructure.AlphaNumericAlphabet.Length);
                chars[i] = SystemValues.Infrastructure.AlphaNumericAlphabet[index];
            }

            return new string(chars);
        }
    }
}
