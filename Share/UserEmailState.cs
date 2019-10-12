using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GrainAccessibility
{
    [Serializable]
    public class UserEmailState
    {
        List<Description> _descriptions = null;
        public List<Description> Descriptions
        {
            get
            {
                if (_descriptions == null)
                    _descriptions = new List<Description>();
                return _descriptions;
            }
            set { _descriptions = value; }
        }

        Message _status = null;
        public Message Message
        {
            get
            {
                if (_status == null)
                    _status = new Message(Status.OK, Constants.VALID_EMAIL);
                return _status;
            }

            set { _status = value; }
        }

        public string Host
        {
            get
            {
                if (MailAddress != null)
                    return MailAddress.Host;
                return string.Empty;
            }
        }

        public string Address
        {
            get
            {
                if (MailAddress != null)
                    return MailAddress.Address;
                return string.Empty;
            }
        }

        MailAddress MailAddress { get; set; }
        //MailAddress IUserEmail.MailAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public static UserEmailState CreateDummyUserEmail(Status status, string description)
        {
            var dummyUserEmail = new UserEmailState(Constants.DUMMY_MAIL_ADDRESS)
            {
                Message = new Message(status, description)
            };
            return dummyUserEmail;
        }

        public UserEmailState()
        {
            MailAddress = null;
        }

        public UserEmailState(string mailAddress) 
        {
            MailAddress = new MailAddress(mailAddress);
        }

        public void AddDescription(string text)
        {
            Descriptions.Add(new Description(text));
        }

        public override string ToString()
        {
            return $"{Constants.CAPTION_EMAIL}{Address}{System.Environment.NewLine}{System.Environment.NewLine}{string.Join(System.Environment.NewLine, Constants.CAPTION_DESCRIPTION,  Descriptions)}";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void VerifyHost(string validHost)
        {
            Message = !validHost.EndsWith(Host) ? new Message(GrainAccessibility.Status.HOST_NOT_SUPPORTED, Constants.INVALID_EMAIL_DOMAIN) : Message;
        }
    }
}
