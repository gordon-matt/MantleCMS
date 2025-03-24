namespace Mantle.Web.Collections;

public static class EnumerableExtensions
{
    public static SelectList ToSelectList(
        this IEnumerable<string> enumerable) => enumerable.ToSelectList(x => x, x => x);

    public static SelectList ToSelectList<T>(
        this IEnumerable<T> enumerable, Func<T, object> valueFieldSelector, Func<T, string> textFieldSelector)
    {
        var values = enumerable.Select(x =>
            new
            {
                ValueField = Convert.ToString(valueFieldSelector(x)),
                TextField = textFieldSelector(x)
            });

        return new SelectList(values, "ValueField", "TextField");
    }

    public static SelectList ToSelectList<T>(
        this IEnumerable<T> enumerable, Func<T, object> valueFieldSelector, Func<T, string> textFieldSelector, string emptyText)
    {
        var values = enumerable.Select(x =>
            new
            {
                ValueField = Convert.ToString(valueFieldSelector(x)),
                TextField = textFieldSelector(x)
            }).ToList();

        if (emptyText != null) // we don't check for empty, because empty string can be valid for emptyText value.
        {
            values.Insert(0, new { ValueField = string.Empty, TextField = emptyText });
        }

        return new SelectList(values, "ValueField", "TextField");
    }

    public static SelectList ToSelectList<T>(
        this IEnumerable<T> enumerable, Func<T, object> valueFieldSelector, Func<T, string> textFieldSelector, object selectedValue)
    {
        var values = enumerable.Select(x =>
            new
            {
                ValueField = Convert.ToString(valueFieldSelector(x)),
                TextField = textFieldSelector(x)
            });

        return new SelectList(values, "ValueField", "TextField", selectedValue);
    }

    public static SelectList ToSelectList<T>(
        this IEnumerable<T> enumerable, Func<T, object> valueFieldSelector, Func<T, string> textFieldSelector, object selectedValue, string emptyText)
    {
        var values = enumerable.Select(x =>
            new
            {
                ValueField = Convert.ToString(valueFieldSelector(x)),
                TextField = textFieldSelector(x)
            }).ToList();

        if (emptyText != null) // we don't check for empty, because empty string can be valid for emptyText value.
        {
            values.Insert(0, new { ValueField = string.Empty, TextField = emptyText });
        }
        return new SelectList(values, "ValueField", "TextField", selectedValue);
    }

    public static MultiSelectList ToMultiSelectList<T, TValue>(
        this IEnumerable<T> enumerable,
        Func<T, object> valueFieldSelector,
        Func<T, string> textFieldSelector,
        IEnumerable<TValue> selectedValues,
        string emptyText = null)
    {
        var values = enumerable.Select(x =>
            new
            {
                ValueField = Convert.ToString(valueFieldSelector(x)),
                TextField = textFieldSelector(x)
            }).ToList();

        if (emptyText != null) // we don't check for empty, because empty string can be valid for emptyText value.
        {
            values.Insert(0, new { ValueField = string.Empty, TextField = emptyText });
        }

        return new MultiSelectList(values, "ValueField", "TextField", selectedValues);
    }
}