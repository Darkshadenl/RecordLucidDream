using System;

namespace HouseScraping.Helpers;

public static class PermissionHelper
{
    public static async Task<PermissionStatus> CheckAndRequestMicrophonePermission()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Microphone>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
        }

        return status;
    }
}

