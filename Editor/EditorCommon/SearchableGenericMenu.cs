using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace DouduckLibEditor
{
    public class SearchableGenericMenu
    {
        public delegate void MenuFunction();
        public delegate void MenuFunction2<T>(T userData);

        internal interface IMenuItemAction
        {
            void Execute();
        }

        internal class MenuItemAction<T> : IMenuItemAction
        {
            public MenuFunction2<T> func;
            public T userData;

            public void Execute()
            {
                func?.Invoke(userData);
            }
        }

        internal class MenuItem
        {
            public string content;
            public bool on;
            public bool disabled;
            public MenuFunction func;
            public IMenuItemAction action;
        }

        List<MenuItem> _items = new List<MenuItem>();

        public void AddItem(GUIContent content, bool on, MenuFunction func)
        {
            _items.Add(new MenuItem { content = content.text, on = on, func = func });
        }

        public void AddItem<T>(GUIContent content, bool on, MenuFunction2<T> func, T userData)
        {
            var action = new MenuItemAction<T> { func = func, userData = userData };
            _items.Add(new MenuItem { content = content.text, on = on, action = action });
        }

        public void AddDisabledItem(GUIContent content, bool on)
        {
            _items.Add(new MenuItem { content = content.text, on = on, disabled = true });
        }

        public int GetItemCount()
        {
            return _items.Count;
        }

        public void ShowAsContext()
        {
            var position = new Rect(Event.current.mousePosition, Vector2.zero);
            DropDown(position);
        }

        public void DropDown(Rect position)
        {
            var dropdown = new SearchableDropdownImpl(_items);
            dropdown.Show(position);
        }
    }

    class SearchableDropdownImpl : AdvancedDropdown
    {
        List<SearchableGenericMenu.MenuItem> _items;

        public SearchableDropdownImpl(List<SearchableGenericMenu.MenuItem> items) : base(new AdvancedDropdownState())
        {
            _items = items;
            minimumSize = new Vector2(250f, 300f);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Select Option");

            foreach (var item in _items)
            {
                var parts = item.content.Split('/');
                var current = root;

                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    if (i == parts.Length - 1)
                    {
                        var displayName = part;
                        if (item.on)
                        {
                            displayName = "✓ " + displayName;
                        }
                        if (item.disabled)
                        {
                            displayName += " (Disabled)";
                        }

                        var leaf = new SearchableDropdownItemWithData(displayName, item);
                        current.AddChild(leaf);
                    }
                    else
                    {
                        AdvancedDropdownItem found = null;
                        if (current.children != null)
                        {
                            foreach (var child in current.children)
                            {
                                if (child.name == part)
                                {
                                    found = child;
                                    break;
                                }
                            }
                        }

                        if (found == null)
                        {
                            found = new AdvancedDropdownItem(part);
                            current.AddChild(found);
                        }
                        current = found;
                    }
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is SearchableDropdownItemWithData itemWithData)
            {
                var menuItem = itemWithData.menuItem;
                if (menuItem.disabled)
                {
                    return;
                }

                if (menuItem.func != null)
                {
                    menuItem.func();
                }
                else if (menuItem.action != null)
                {
                    menuItem.action.Execute();
                }
            }
        }
    }

    class SearchableDropdownItemWithData : AdvancedDropdownItem
    {
        public SearchableGenericMenu.MenuItem menuItem { get; }

        public SearchableDropdownItemWithData(string name, SearchableGenericMenu.MenuItem item) : base(name)
        {
            menuItem = item;
        }
    }
}
