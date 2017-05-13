using BaseStarShot.Logging;
using BaseStarShot.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public class ErrorService : BaseStarShot.Services.IErrorService
    {
        private readonly IErrorRepository repository;
        private readonly IEmailService emailService;

        public ErrorService(IErrorRepository repository, IEmailService emailService)
        {
            this.repository = repository;
            this.emailService = emailService;
        }

        public bool LogError(Exception ex)
        {
            return LogError(new ErrorData { Exception = ex.ToString() });
        }

        public bool LogError(ErrorData error)
        {
            error.ErrorId = Guid.NewGuid();
            error.DateTime = DateTime.Now;
            error.DateTimeOffset = DateTimeOffset.Now.Offset.TotalHours;
            error.UserAgent = Globals.UserAgent;
            error.IsSent = false;
            error.DateTimeSent = null;
            return repository.Save(error);
        }

        public async Task<int> SendAllPendingAsync()
        {
            try
            {
                return await Task.Run<int>(() =>
                {
                    int count = 0;
                    var errors = repository.GetAllPending();
                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            if (emailService.Send(Globals.Error.Subject, error.ToString(), null, null, Globals.Error.Email))
                            {
                                error.IsSent = true;
                                error.DateTimeSent = DateTime.UtcNow;
                                error.DateTimeSentOffset = DateTimeOffset.Now.Offset.TotalHours;
                                if (repository.Save(error))
                                    count++;
                            }
                        }
                    }
                    return count;
                });
            }
            catch (Exception ex)
            {
                Logger.WriteError("ErrorService", ex);
            }
            return -1;
        }
    }
}
