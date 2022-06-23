﻿using System;

namespace Cnp.Sdk.Core
{
    public class CnpConnectionLimitExceededException : Exception
    {
        public CnpConnectionLimitExceededException(string message) : base(message)
        {

        }

        public CnpConnectionLimitExceededException(string message, Exception e) : base(message, e)
        {

        }
    }
}
