using System;
using System.Collections.Generic;
using System.Text;

namespace BaseStarShot.Services
{
    public enum AuthorizationResult
    {
        NoConsumerKeyAndSecret,
        Authorized,
        Cancelled,
        ConnectionFailed,
        FailedToRetrieveAccessToken
    }
}
