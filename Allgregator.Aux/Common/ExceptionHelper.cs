using System;
using System.Text;

namespace Allgregator.Aux.Common
{
    public static class ExceptionHelper
    {
        public static string GetMessage(Exception exception)
        {
            var s = new StringBuilder();
            while (exception != null)
            {
                s.Append(exception.Message);
                exception = exception.InnerException;
                if (exception != null)
                    s.Append("; ");
            }

            return s.ToString();
        }
    }
}
