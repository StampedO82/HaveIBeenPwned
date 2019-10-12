using System;

namespace GrainAccessibility
{
    public class Description
    {
        public Description(string description)
        {
            Text = description;
            DateTime = DateTime.Now;
        }

        //TODO: not used anywhere
        public Description(string description, DateTime dateTime)
        {
            Text = description;
            DateTime = dateTime;
        }
        public DateTime DateTime { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"Time: {DateTime.ToString()} {Text}{System.Environment.NewLine} ";
        }
    }
}