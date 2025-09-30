using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IAgoraTokenProvider
    {
        /// <summary>
        /// Generate an Agora RTC token for given channelName and uid (string allowed).
        /// Implementation can call internal token server or use native builder.
        /// </summary>
        Task<(string Token, DateTime IssuedAt, DateTime ExpireAt, string AgoraUid)> GenerateRtcTokenAsync(
            string channelName,
            string uid,
            string callType,
            int expireInSeconds = 3600,
            int role = 1);
    }
}
