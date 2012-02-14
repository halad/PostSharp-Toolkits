using System.Reflection;
using NLog;

namespace PostSharp.Toolkit.Tests.Data
{
    public class ClassForNLog
    {
        private static Logger logger = LogManager.GetLogger("ClassForNLog");

        public void InvokeAction()
        {
            if (logger.IsTraceEnabled)
            {
                logger.Trace("Entering {0}", MethodBase.GetCurrentMethod());
            }

            Database.Save();
        }
    }

    public class Database
    {
        public static void Save()
        {
            
        }
    }
}