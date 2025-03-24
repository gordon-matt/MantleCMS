using Mantle.Localization.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Plugins.Widgets.FullCalendar.ContentBlocks;

public class FullCalendarBlock : ContentBlockBase
{
    [BlockProperty]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.CalendarId)]
    public int CalendarId { get; set; }

    #region General Display

    /// <summary>
    /// The day that each week begins.
    /// </summary>
    [BlockProperty(DayOfWeek.Sunday)]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.FirstDay)]
    public DayOfWeek FirstDay { get; set; }

    /// <summary>
    /// Enables/disables use of jQuery UI theming.
    /// </summary>
    [BlockProperty]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.Theme)]
    public bool Theme { get; set; }

    /// <summary>
    /// Whether to include Saturday/Sunday columns in any of the calendar views.
    /// </summary>
    [BlockProperty(true)]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.Weekends)]
    public bool Weekends { get; set; }

    /// <summary>
    /// <para>Determines the number of weeks displayed in a month view.</para>
    /// <para>If true, the calendar will always be 6 weeks tall.</para>
    /// <para>If false, the calendar will have either 4, 5, or 6 weeks, depending on the month.</para>
    /// </summary>
    [BlockProperty(true)]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.FixedWeekCount)]
    public bool FixedWeekCount { get; set; }

    /// <summary>
    /// Determines if week numbers should be displayed on the calendar.
    /// </summary>
    [BlockProperty]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.WeekNumbers)]
    public bool WeekNumbers { get; set; }

    /// <summary>
    /// Determines the width-to-height aspect ratio of the calendar.
    /// </summary>
    [BlockProperty(1.35f)]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.AspectRatio)]
    public float AspectRatio { get; set; }

    /// <summary>
    /// Whether to automatically resize the calendar when the browser window resizes.
    /// </summary>
    [BlockProperty(true)]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.HandleWindowResize)]
    public bool HandleWindowResize { get; set; }

    #endregion General Display

    #region Agenda Options

    /// <summary>
    /// Determines if the "all-day" slot is displayed at the top of the calendar.
    /// </summary>
    [BlockProperty(true)]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.AllDaySlot)]
    public bool AllDaySlot { get; set; }

    /// <summary>
    /// The text titling the "all-day" slot at the top of the calendar.
    /// </summary>
    [BlockProperty]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.AllDayText)]
    public string AllDayText { get; set; }

    /// <summary>
    /// The frequency for displaying time slots.
    /// </summary>
    [BlockProperty("00:30:00")]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.SlotDuration)]
    public string SlotDuration { get; set; }

    /// <summary>
    /// Determines how far down the scroll pane is initially scrolled down.
    /// </summary>
    [BlockProperty("06:00:00")]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.ScrollTime)]
    public string ScrollTime { get; set; }

    /// <summary>
    /// Determines the starting time that will be displayed, even when the scrollbars have been scrolled all the way up.
    /// </summary>
    [BlockProperty("00:00:00")]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.MinTime)]
    public string MinTime { get; set; }

    /// <summary>
    /// Determines the end time (exclusively) that will be displayed, even when the scrollbars have been scrolled all the way down.
    /// </summary>
    [BlockProperty("24:00:00")]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.MaxTime)]
    public string MaxTime { get; set; }

    /// <summary>
    /// Determines if timed events in agenda view should visually overlap.
    /// </summary>
    [BlockProperty(true)]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.SlotEventOverlap)]
    public bool SlotEventOverlap { get; set; }

    #endregion Agenda Options

    #region Selection

    /// <summary>
    /// Allows a user to highlight multiple days or timeslots by clicking and dragging.
    /// </summary>
    [BlockProperty]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.Selectable)]
    public bool Selectable { get; set; }

    /// <summary>
    /// Whether clicking elsewhere on the page will cause the current selection to be cleared.
    /// </summary>
    [BlockProperty(true)]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.UnselectAuto)]
    public bool UnselectAuto { get; set; }

    /// <summary>
    /// Determines whether the user is allowed to select periods of time that are occupied by events.
    /// </summary>
    [BlockProperty(true)]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.SelectOverlap)]
    public bool SelectOverlap { get; set; }

    #endregion Selection

    #region Event Rendering

    /// <summary>
    /// Sets the background and border colors for all events on the calendar.
    /// </summary>
    [BlockProperty]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.EventColor)]
    public string EventColor { get; set; }

    /// <summary>
    /// Sets the background color for all events on the calendar.
    /// </summary>
    [BlockProperty]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.EventBackgroundColor)]
    public string EventBackgroundColor { get; set; }

    /// <summary>
    /// Sets the border color for all events on the calendar.
    /// </summary>
    [BlockProperty]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.EventBorderColor)]
    public string EventBorderColor { get; set; }

    /// <summary>
    /// Sets the text color for all events on the calendar.
    /// </summary>
    [BlockProperty]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.EventTextColor)]
    public string EventTextColor { get; set; }

    /// <summary>
    /// When an event's end time spans into another day, the minimum time it must be in order for it to render as if it were on that day.
    /// </summary>
    [BlockProperty("09:00:00")]
    [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FullCalendarBlock.NextDayThreshold)]
    public string NextDayThreshold { get; set; }

    #endregion Event Rendering

    // TODO: Support editable mode later
    //  See: http://fullcalendar.io/docs/event_ui/ for all options, etc..
    //#region Event Dragging & Resizing

    ///// <summary>
    ///// Determines whether the events on the calendar can be modified.
    ///// </summary>
    //public bool Editable { get; set; }

    //#endregion

    #region ContentBlockBase Overrides

    public override string Name => "Full Calendar";

    public override string DisplayTemplatePath => "/Plugins/Widgets.FullCalendar/Views/Shared/DisplayTemplates/FullCalendarBlock.cshtml";

    public override string EditorTemplatePath => "/Plugins/Widgets.FullCalendar/Views/Shared/EditorTemplates/FullCalendarBlock.cshtml";

    #endregion ContentBlockBase Overrides
}