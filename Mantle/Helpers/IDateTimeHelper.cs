﻿using System;
using System.Collections.ObjectModel;
using Mantle.Security.Membership;

namespace Mantle.Helpers
{
    public interface IDateTimeHelper
    {
        TimeZoneInfo FindTimeZoneById(string id);

        ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones();

        DateTime ConvertToUserTime(DateTime dateTime);

        DateTime ConvertToUserTime(DateTime dateTime, DateTimeKind sourceDateTimeKind);

        DateTime ConvertToUserTime(DateTime dateTime, TimeZoneInfo sourceTimeZone);

        DateTime ConvertToUserTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone);

        DateTime ConvertToUtcTime(DateTime dateTime);

        DateTime ConvertToUtcTime(DateTime dateTime, DateTimeKind sourceDateTimeKind);

        DateTime ConvertToUtcTime(DateTime dateTime, TimeZoneInfo sourceTimeZone);

        TimeZoneInfo GetUserTimeZone(MantleUser user);

        TimeZoneInfo DefaultTenantTimeZone { get; set; }

        TimeZoneInfo CurrentTimeZone { get; set; }
    }
}