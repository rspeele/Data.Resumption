﻿using System;
using System.Diagnostics;

namespace Data.Resumption.Execution
{
    /// <summary>
    /// An <see cref="IExecutionLog"/> that writes events to the debug output window.
    /// </summary>
    public class DebugExecutionLog : IExecutionLog
    {
        public void OnStepStart() => Debug.WriteLine("OnStepStart()");

        public void OnStepFinish() => Debug.WriteLine("OnStepFinish()");

        public void OnPrepare(IDataRequest request)
            => Debug.WriteLine($"OnPrepare({request.Identity})");

        public void OnPrepareFailure(Exception exception)
            => Debug.WriteLine($"OnException({exception.Message})");

        public void OnComplete(IDataRequest request, SuccessOrException response)
            => Debug.WriteLine($"OnComplete({request.Identity},{response})");
    }
}