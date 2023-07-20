using Mantle.Localization;

namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure
{
    public class LanguagePackInvariant : ILanguagePack
    {
        #region ILanguagePack Members

        public string CultureCode => null;

        public IDictionary<string, string> LocalizedStrings => new Dictionary<string, string>
        {
            { LocalizableStrings.CalendarEventModel.CalendarId, "Calendar" },
            { LocalizableStrings.CalendarEventModel.EndDateTime, "End" },
            { LocalizableStrings.CalendarEventModel.Name, "Name" },
            { LocalizableStrings.CalendarEventModel.StartDateTime, "Start" },
            { LocalizableStrings.CalendarModel.Name, "Name" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.EditorTabs.AgendaOptions, "Agenda Options" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.EditorTabs.EventRendering, "Event Rendering" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.EditorTabs.General, "General" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.EditorTabs.Selection, "Selection" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.AllDaySlot, "All Day Slot" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.AllDayText, "All Day Text" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.AspectRatio, "Aspect Ratio" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.CalendarId, "Calendar" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.EventBackgroundColor, "Event Background Color" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.EventBorderColor, "Event Border Color" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.EventColor, "Event Color" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.EventTextColor, "Event Text Color" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.FirstDay, "First Day" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.FixedWeekCount, "Fixed Week Count" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.HandleWindowResize, "Handle Window Resize" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.MaxTime, "Max Time" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.MinTime, "Min Time" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.NextDayThreshold, "Next Day Threshold" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.ScrollTime, "Scroll Time" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.Selectable, "Selectable" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.SelectOverlap, "Select Overlap" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.SlotDuration, "Slot Duration" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.SlotEventOverlap, "Slot Event Overlap" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.Theme, "Theme" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.UnselectAuto, "Unselect Auto" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.Weekends, "Weekends" },
            { LocalizableStrings.ContentBlocks.FullCalendarBlock.WeekNumbers, "Week Numbers" },
            { LocalizableStrings.Events, "Events" },
            { LocalizableStrings.FullCalendar, "Full Calendar" },
        };

        #endregion ILanguagePack Members
    }
}