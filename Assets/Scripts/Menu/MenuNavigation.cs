using System.Collections.Generic;
using UnityEngine.UI;

public static class MenuNavigation
{
    // links the items into an up/down chain, skipping inactive ones
    public static void WireVertical(IList<MenuWidget> items, bool wrap = true)
    {
        var active = new List<Selectable>();
        foreach (var item in items)
        {
            if (item != null && item.gameObject.activeSelf)
                active.Add(item.GetComponent<Selectable>());
        }

        for (var i = 0; i < active.Count; i++)
        {
            var nav = active[i].navigation;
            nav.mode = Navigation.Mode.Explicit;
            
            nav.selectOnLeft = null;
            nav.selectOnRight = null;

            var hasPrev = i > 0 || wrap;
            var hasNext = i < active.Count - 1 || wrap;
            nav.selectOnUp = hasPrev ? active[(i - 1 + active.Count) % active.Count] : null;
            nav.selectOnDown = hasNext ? active[(i + 1) % active.Count] : null;

            active[i].navigation = nav;
        }
    }

    // for popups
    public static void WireHorizontal(MenuWidget left, MenuWidget right)
    {
        if (left == null || right == null)
            return;

        var leftSelectable = left.GetComponent<Selectable>();
        var rightSelectable = right.GetComponent<Selectable>();

        var leftNav = leftSelectable.navigation;
        leftNav.mode = Navigation.Mode.Explicit;
        leftNav.selectOnUp = null;
        leftNav.selectOnDown = null;
        leftNav.selectOnLeft = rightSelectable;
        leftNav.selectOnRight = rightSelectable;
        leftSelectable.navigation = leftNav;

        var rightNav = rightSelectable.navigation;
        rightNav.mode = Navigation.Mode.Explicit;
        rightNav.selectOnUp = null;
        rightNav.selectOnDown = null;
        rightNav.selectOnLeft = leftSelectable;
        rightNav.selectOnRight = leftSelectable;
        rightSelectable.navigation = rightNav;
    }
}
