using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share
{
    public enum Status
    {
        OK,
        ERROR,
        PWNED,
        HOST_NOT_SUPPORTED,
        NOT_FOUND
    }

    public class Message
    {
        public Message(Status status, string description)
        {
            Status = status;
            Description = description;
        }

        public Status Status { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return $"{System.Environment.NewLine}  {Status.ToString()} {System.Environment.NewLine} {System.Environment.NewLine} Description: {Description} {System.Environment.NewLine}";
        }
    }
}
