namespace Mantle.Web.Navigation;

internal class MenuItemComparer : IEqualityComparer<MenuItem>
{
    #region IEqualityComparer<MenuItem> Members

    public bool Equals(MenuItem x, MenuItem y)
    {
        string xTextHint = x.Text ?? null;
        string yTextHint = y.Text ?? null;
        if (!string.Equals(xTextHint, yTextHint))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(x.Url) && !string.IsNullOrWhiteSpace(y.Url))
        {
            if (!string.Equals(x.Url, y.Url))
            {
                return false;
            }
        }
        if (x.RouteValues != null && y.RouteValues != null)
        {
            if (x.RouteValues.Keys.Any(key => y.RouteValues.ContainsKey(key) == false))
            {
                return false;
            }
            if (y.RouteValues.Keys.Any(key => x.RouteValues.ContainsKey(key) == false))
            {
                return false;
            }
            foreach (string key in x.RouteValues.Keys)
            {
                if (!Equals(x.RouteValues[key], y.RouteValues[key]))
                {
                    return false;
                }
            }
        }

        return (string.IsNullOrWhiteSpace(x.Url) || y.RouteValues == null) && (string.IsNullOrWhiteSpace(y.Url) || x.RouteValues == null);
    }

    public int GetHashCode(MenuItem obj)
    {
        int hash = 0;

        if (!string.IsNullOrEmpty(obj.Text))
        {
            hash ^= obj.Text.GetHashCode();
        }

        //if (obj.Text != null && obj.Text != null)
        //{
        //    hash ^= obj.Text.GetHashCode();
        //}
        return hash;
    }

    #endregion IEqualityComparer<MenuItem> Members
}