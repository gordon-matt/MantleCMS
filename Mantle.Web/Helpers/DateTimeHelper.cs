﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using Mantle.Web.Configuration.Services;
using Mantle.Web.Security.Membership;

namespace Mantle.Web.Helpers;

public partial class DateTimeHelper : IDateTimeHelper
{
    private readonly IWorkContext workContext;
    private readonly IGenericAttributeService genericAttributeService;
    private readonly ISettingService settingService;
    private readonly DateTimeSettings dateTimeSettings;

    public DateTimeHelper(
        IWorkContext workContext,
        IGenericAttributeService genericAttributeService,
        ISettingService settingService,
        DateTimeSettings dateTimeSettings)
    {
        this.workContext = workContext;
        this.genericAttributeService = genericAttributeService;
        this.settingService = settingService;
        this.dateTimeSettings = dateTimeSettings;
    }

    public TimeZoneInfo FindTimeZoneById(string id) => TimeZoneInfo.FindSystemTimeZoneById(id);

    public ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones() => TimeZoneInfo.GetSystemTimeZones();

    public DateTime ConvertToUserTime(DateTime dateTime) => ConvertToUserTime(dateTime, dateTime.Kind);

    public DateTime ConvertToUserTime(DateTime dateTime, DateTimeKind sourceDateTimeKind)
    {
        dateTime = DateTime.SpecifyKind(dateTime, sourceDateTimeKind);
        var currentUserTimeZoneInfo = CurrentTimeZone;
        return TimeZoneInfo.ConvertTime(dateTime, currentUserTimeZoneInfo);
    }

    public DateTime ConvertToUserTime(DateTime dateTime, TimeZoneInfo sourceTimeZone)
    {
        var currentUserTimeZoneInfo = CurrentTimeZone;
        return ConvertToUserTime(dateTime, sourceTimeZone, currentUserTimeZoneInfo);
    }

    public DateTime ConvertToUserTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone) =>
        TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone);

    public DateTime ConvertToUtcTime(DateTime dateTime) => ConvertToUtcTime(dateTime, dateTime.Kind);

    public DateTime ConvertToUtcTime(DateTime dateTime, DateTimeKind sourceDateTimeKind)
    {
        dateTime = DateTime.SpecifyKind(dateTime, sourceDateTimeKind);
        return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc);
    }

    public DateTime ConvertToUtcTime(DateTime dateTime, TimeZoneInfo sourceTimeZone)
    {
        if (sourceTimeZone.IsInvalidTime(dateTime))
        {
            //could not convert
            return dateTime;
        }

        return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, TimeZoneInfo.Utc);
    }

    public TimeZoneInfo GetUserTimeZone(MantleUser user)
    {
        //registered user
        TimeZoneInfo timeZoneInfo = null;
        if (dateTimeSettings.AllowUsersToSetTimeZone)
        {
            string timeZoneId = string.Empty;

            if (user != null)
            {
                timeZoneId = user.GetAttribute<string>(SystemUserAttributeNames.TimeZoneId, genericAttributeService);
            }

            try
            {
                if (!string.IsNullOrEmpty(timeZoneId))
                {
                    timeZoneInfo = FindTimeZoneById(timeZoneId);
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
        }

        //default timezone
        timeZoneInfo ??= DefaultTenantTimeZone;

        return timeZoneInfo;
    }

    public TimeZoneInfo DefaultTenantTimeZone
    {
        get
        {
            TimeZoneInfo timeZoneInfo = null;
            try
            {
                if (!string.IsNullOrEmpty(dateTimeSettings.DefaultTimeZoneId))
                {
                    timeZoneInfo = FindTimeZoneById(dateTimeSettings.DefaultTimeZoneId);
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            timeZoneInfo ??= TimeZoneInfo.Local;
            return timeZoneInfo;
        }
        set
        {
            string defaultTimeZoneId = string.Empty;
            if (value != null)
            {
                defaultTimeZoneId = value.Id;
            }

            dateTimeSettings.DefaultTimeZoneId = defaultTimeZoneId;
            settingService.SaveSettings(dateTimeSettings);
        }
    }

    public TimeZoneInfo CurrentTimeZone
    {
        get => GetUserTimeZone(workContext.CurrentUser);
        set
        {
            if (!dateTimeSettings.AllowUsersToSetTimeZone)
            {
                return;
            }

            string timeZoneId = string.Empty;
            if (value != null)
            {
                timeZoneId = value.Id;
            }

            genericAttributeService.SaveAttribute(
                workContext.CurrentUser,
                SystemUserAttributeNames.TimeZoneId,
                timeZoneId);
        }
    }
}