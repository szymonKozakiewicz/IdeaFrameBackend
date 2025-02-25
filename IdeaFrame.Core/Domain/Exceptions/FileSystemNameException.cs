using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Domain.Exceptions
{
    public class FileSystemNameException : Exception
    {
        public FileSystemNameException()
        {
        }

        public FileSystemNameException(string? message) : base(message)
        {
        }

        public FileSystemNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
