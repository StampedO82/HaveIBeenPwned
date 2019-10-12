using Share;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Share
{
    public interface IUserEmailGrain : Orleans.IGrainWithStringKey, IRemindable
    {
        Task<UserEmailState> AddEmailAddress(string emailAddress, string text = "");
        Task<UserEmailState> IsEmailAddressPwned(string mailAddress);
        //Task Init();
    }
}
