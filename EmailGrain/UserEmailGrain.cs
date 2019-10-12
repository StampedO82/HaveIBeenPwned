using GrainAccessibility;
using GrainInterfaces;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailGrain
{
    [StorageProvider(ProviderName = "AzureBlobStorage")]
    public class UserEmailGrain : Orleans.Grain<List<UserEmailState>>, IUserEmailGrain
    {
        Task<IGrainReminder> GrainReminder { get; set; }

        /// <summary>
        /// Adding email address and text to the internal collection. (Example of commenting the methods, etc.)
        /// </summary>
        /// <param name="emailAddress">email address</param>
        /// <param name="text">Text about reasons for breaching</param>
        /// <returns>Returns email object wrapped in asynchronious operation</returns>
        public Task<UserEmailState> AddEmailAddress(string emailAddress, string text = "")
        {
            UserEmailState userEmail = GetMailAddressFromCollection(emailAddress);

            switch (userEmail.Message.Status)
            {
                case Status.NOT_FOUND:
                    userEmail.Message.Status = Status.OK;
                    userEmail.Message.Description = Constants.PWNED_EMAIL;
                    //userEmail.AddDescription(text);
                    State.Add(userEmail);
                    break;
                case Status.ERROR: break;
                case Status.OK: break;
                case Status.HOST_NOT_SUPPORTED: break;
                case Status.PWNED:
                   // userEmail.AddDescription(text);
                    break;
            }

            return Task.FromResult(userEmail);
        }

        UserEmailState CreateUserMailAddress(string mailAddress)
        {
            UserEmailState userEmail;
            try
            {
                userEmail = new UserEmailState(mailAddress);
                userEmail.VerifyHost(GrainReference.ToString());
                return userEmail;
            }
            catch (ArgumentNullException ex)
            {//TODO: write user friendly message..
                return UserEmailState.CreateDummyUserEmail(Status.ERROR, ex.Message);
            }
            catch (ArgumentException ex)
            {//TODO: write user friendly message..
                return UserEmailState.CreateDummyUserEmail(Status.ERROR, ex.Message);
            }
            catch (FormatException ex)
            {//TODO: write user friendly message..
                return UserEmailState.CreateDummyUserEmail(Status.ERROR, ex.Message);
            }
        }

        UserEmailState GetMailAddressFromCollection(string mailAddress)
        {
            UserEmailState userEmail = CreateUserMailAddress(mailAddress);

            if (userEmail.Message.Status == Status.OK)
            {
                try
                {
                    //if (State.Address.ToLower() == userEmail.Address.ToLower())
                    //    return State;
                    userEmail = State.First(be => be.Address.ToLower() == userEmail.Address.ToLower());
                }
                catch (ArgumentNullException ex)
                {
                    //TODO: could have written more user friendly message..
                    return UserEmailState.CreateDummyUserEmail(Status.ERROR, ex.Message);
                }
                catch (InvalidOperationException)
                {
                    //TODO: could have written more user friendly message..
                    userEmail.Message.Status = Status.NOT_FOUND;
                }
            }

            return userEmail;
        }

        public Task<UserEmailState> IsEmailAddressPwned(string mailAddress)
        {
            ReadStateAsync(); //before every check, load the latest state
            return Task.FromResult(GetMailAddressFromCollection(mailAddress));
        }

        public override Task OnActivateAsync()
        {
            Console.WriteLine("Activated grain {0}", this.GrainReference.ToKeyString());
            //reminder for every 5min
            GrainReminder = RegisterOrUpdateReminder("reminderEvery5min", new TimeSpan(0, 0, 0), new TimeSpan(0,5,0));
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            Console.WriteLine("Deactivated grain {0}", this.GrainReference.ToKeyString());
            WriteStateAsync();//save the latest state
            UnregisterReminder(GrainReminder.Result); 
            return base.OnDeactivateAsync();
        }

        public override void Participate(IGrainLifecycle lifecycle)
        {
            base.Participate(lifecycle);
        }

        public Task ReceiveReminder(string reminderName, TickStatus status)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine("5 min Reminder triggered!");
            Console.WriteLine("-------------------------");
            WriteStateAsync();
            return Task.CompletedTask;
        }
    }
}
