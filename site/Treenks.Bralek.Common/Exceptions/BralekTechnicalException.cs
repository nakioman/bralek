using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treenks.Bralek.Common.Resources;

namespace Treenks.Bralek.Common.Exceptions
{
    public class BralekTechnicalException : Exception
    {
        private ILogger _logger = NullLogger.Instance;

        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public BralekTechnicalException(string errorCode, Exception exception) 
            :base(errorCode, exception)
        {
            _logger.ErrorFormat(exception, "There was a technical exception with code: {0}", errorCode);
        }

        public override string Message
        {
            get
            {
                return ExceptionMessages.ResourceManager.GetString(base.Message);
            }
        }
    }
}
