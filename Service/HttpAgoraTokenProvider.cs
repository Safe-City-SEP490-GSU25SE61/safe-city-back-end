using AgoraIO.Media;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AgoraTokenProvider : IAgoraTokenProvider
    {
        private readonly string _appId;
        private readonly string _appCertificate;

        public AgoraTokenProvider(IConfiguration configuration)
        {
            _appId = configuration["Agora:AppId"] ?? throw new ArgumentNullException("Agora AppId not found");
            _appCertificate = configuration["Agora:AppCertificate"] ?? throw new ArgumentNullException("Agora AppCertificate not found");
        }

        //public Task<(string Token, DateTime IssuedAt, DateTime ExpireAt, string AgoraUid)> GenerateRtcTokenAsync(
        //    string channelName,
        //    string uid,
        //    string callType,
        //    int expireInSeconds = 3600,
        //    int role = 1)
        //{
        //    var agoraRole = role == 1
        //        ? RtcTokenBuilder.Role.RolePublisher
        //        : RtcTokenBuilder.Role.RoleSubscriber;

        //    var issuedAt = DateTime.UtcNow;
        //    var expireAt = issuedAt.AddSeconds(expireInSeconds);

        //    var agoraUid = uid;

        //    uint expiresSeconds = (uint)(expireAt - issuedAt).TotalSeconds;
        //    string token = RtcTokenBuilder.buildTokenWithUID(
        //        _appId,
        //        _appCertificate,
        //        channelName,
        //        Convert.ToUInt32(agoraUid.GetHashCode() & 0x7fffffff),
        //        agoraRole,
        //        expiresSeconds
        //    );


        //    return Task.FromResult((token, issuedAt, expireAt, agoraUid));
        //}
        public Task<(string Token, DateTime IssuedAt, DateTime ExpireAt, string AgoraUid)> GenerateRtcTokenAsync(
        string channelName,
        string uid,
        string callType,
        int expireInSeconds = 3600,
        int role = 1)
        {
            var issuedAt = DateTime.UtcNow;
            var expireAt = issuedAt.AddSeconds(expireInSeconds);

            var agoraUid = uid;
            var expireTs = (uint)expireInSeconds;

            var token = new AccessToken2(_appId, _appCertificate, expireTs);

            var rtcService = new AccessToken2.ServiceRtc(channelName, agoraUid);

            rtcService.addPrivilegeRtc(AccessToken2.PrivilegeRtcEnum.PRIVILEGE_JOIN_CHANNEL, expireTs);

            if (role == 1) // Publisher
            {
                rtcService.addPrivilegeRtc(AccessToken2.PrivilegeRtcEnum.PRIVILEGE_PUBLISH_AUDIO_STREAM, expireTs);
                rtcService.addPrivilegeRtc(AccessToken2.PrivilegeRtcEnum.PRIVILEGE_PUBLISH_VIDEO_STREAM, expireTs);
                rtcService.addPrivilegeRtc(AccessToken2.PrivilegeRtcEnum.PRIVILEGE_PUBLISH_DATA_STREAM, expireTs);
            }

            token.addService(rtcService);

            string rtcToken = token.build();

            return Task.FromResult((rtcToken, issuedAt, expireAt, agoraUid));
        }

    }

}
