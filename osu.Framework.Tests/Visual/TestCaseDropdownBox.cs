﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Input.States;
using osu.Framework.Testing;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Framework.Tests.Visual
{
    public class TestCaseDropdownBox : ManualInputManagerTestCase
    {
        private const int items_to_add = 10;

        public TestCaseDropdownBox()
        {
            StyledDropdown styledDropdown, styledDropdownMenu2, keyboardInputDropdown1, keyboardInputDropdown2, keyboardInputDropdown3;
            var testItems = new string[10];
            int i = 0;
            while (i < items_to_add)
                testItems[i] = @"test " + i++;

            Add(styledDropdown = new StyledDropdown
            {
                Width = 150,
                Position = new Vector2(50, 70),
                Items = testItems
            });

            Add(styledDropdownMenu2 = new StyledDropdown
            {
                Width = 150,
                Position = new Vector2(250, 70),
                Items = testItems
            });

            PlatformActionContainer platformActionContainer1, platformActionContainer2;

            Add(keyboardInputDropdown1 = new StyledDropdown
            {
                Width = 150,
                Position = new Vector2(450, 70),
                Items = testItems
            });
            keyboardInputDropdown1.Menu.Height = 80;

            Add(platformActionContainer1 = new PlatformActionContainer
            {
                Child = keyboardInputDropdown2 = new StyledDropdown
                {
                    Width = 150,
                    Position = new Vector2(650, 70),
                    Items = testItems
                }
            });
            keyboardInputDropdown2.Menu.Height = 80;

            Add(platformActionContainer2 = new PlatformActionContainer
            {
                Child = keyboardInputDropdown3 = new StyledDropdown
                {
                    Width = 150,
                    Position = new Vector2(850, 70),
                    Items = testItems
                }
            });
            keyboardInputDropdown3.Menu.Height = 80;

            AddStep("click dropdown1", () => toggleDropdownViaClick(styledDropdown));
            AddAssert("dropdown is open", () => styledDropdown.Menu.State == MenuState.Open);

            AddRepeatStep("add item", () => styledDropdown.AddDropdownItem("test " + i++), items_to_add);
            AddAssert("item count is correct", () => styledDropdown.Items.Count() == items_to_add * 2);

            AddStep("click item 13", () => styledDropdown.SelectItem(styledDropdown.Menu.Items[13]));

            AddAssert("dropdown1 is closed", () => styledDropdown.Menu.State == MenuState.Closed);
            AddAssert("item 13 is selected", () => styledDropdown.Current == styledDropdown.Items.ElementAt(13));

            AddStep("select item 15", () => styledDropdown.Current.Value = styledDropdown.Items.ElementAt(15));
            AddAssert("item 15 is selected", () => styledDropdown.Current == styledDropdown.Items.ElementAt(15));

            AddStep("click dropdown1", () => toggleDropdownViaClick(styledDropdown));
            AddAssert("dropdown1 is open", () => styledDropdown.Menu.State == MenuState.Open);

            AddStep("click dropdown2", () => toggleDropdownViaClick(styledDropdownMenu2));

            AddAssert("dropdown1 is closed", () => styledDropdown.Menu.State == MenuState.Closed);
            AddAssert("dropdown2 is open", () => styledDropdownMenu2.Menu.State == MenuState.Open);

            AddStep("select 'invalid'", () => styledDropdown.Current.Value = "invalid");

            AddAssert("'invalid' is selected", () => styledDropdown.Current == "invalid");
            AddAssert("label shows 'invalid'", () => styledDropdown.Header.Label == "invalid");

            AddStep("select item 2", () => styledDropdown.Current.Value = styledDropdown.Items.ElementAt(2));
            AddAssert("item 2 is selected", () => styledDropdown.Current == styledDropdown.Items.ElementAt(2));

            AddStep("Select last item using down key", () =>
            {
                while (keyboardInputDropdown1.SelectedItem != keyboardInputDropdown1.Menu.DrawableMenuItems.Last().Item)
                {
                    keyboardInputDropdown1.Header.TriggerEvent(new KeyDownEvent(new InputState(), Key.Down));
                    keyboardInputDropdown1.Header.TriggerEvent(new KeyUpEvent(new InputState(), Key.Down));
                }
            });

            AddAssert("Last item is selected", () => keyboardInputDropdown1.SelectedItem == keyboardInputDropdown1.Menu.DrawableMenuItems.Last().Item);

            AddStep("Select first item using up key", () =>
            {
                while (keyboardInputDropdown1.SelectedItem != keyboardInputDropdown1.Menu.DrawableMenuItems.First().Item)
                {
                    keyboardInputDropdown1.Header.TriggerEvent(new KeyDownEvent(new InputState(), Key.Up));
                    keyboardInputDropdown1.Header.TriggerEvent(new KeyUpEvent(new InputState(), Key.Up));
                }
            });

            AddAssert("First item is selected", () => keyboardInputDropdown1.SelectedItem == keyboardInputDropdown1.Menu.DrawableMenuItems.First().Item);

            void performPlatformAction(PlatformAction action, PlatformActionContainer platformActionContainer, Drawable drawable)
            {
                var tIsHovered = drawable.IsHovered;
                var tHasFocus = drawable.HasFocus;

                drawable.IsHovered = true;
                drawable.HasFocus = true;

                platformActionContainer.TriggerPressed(action);
                platformActionContainer.TriggerReleased(action);

                drawable.IsHovered = tIsHovered;
                drawable.HasFocus = tHasFocus;
            }

            AddStep("Select last item", () => performPlatformAction(new PlatformAction(PlatformActionType.ListEnd), platformActionContainer1, keyboardInputDropdown2.Header));

            AddAssert("Last item selected", () => keyboardInputDropdown2.SelectedItem == keyboardInputDropdown2.Menu.DrawableMenuItems.Last().Item);

            AddStep("Select first item", () => performPlatformAction(new PlatformAction(PlatformActionType.ListStart), platformActionContainer1, keyboardInputDropdown2.Header));

            AddAssert("First item selected", () => keyboardInputDropdown2.SelectedItem == keyboardInputDropdown2.Menu.DrawableMenuItems.First().Item);

            AddStep("click keyboardInputDropdown3", () => toggleDropdownViaClick(keyboardInputDropdown3));
            AddAssert("dropdown is open", () => keyboardInputDropdown3.Menu.State == MenuState.Open);

            AddStep("Preselect last item using down key", () =>
            {
                while (keyboardInputDropdown3.Menu?.PreselectedItem?.Item != keyboardInputDropdown3.Menu.DrawableMenuItems.Last().Item)
                {
                    keyboardInputDropdown3.Menu.TriggerEvent(new KeyDownEvent(new InputState(), Key.Down));
                    keyboardInputDropdown3.Menu.TriggerEvent(new KeyUpEvent(new InputState(), Key.Down));
                }
            });

            AddAssert("Last item is preselected", () => keyboardInputDropdown3.Menu.PreselectedItem.Item == keyboardInputDropdown3.Menu.DrawableMenuItems.Last().Item);

            AddStep("Preselect first item using up key", () =>
            {
                while (keyboardInputDropdown3.Menu?.PreselectedItem?.Item != keyboardInputDropdown3.Menu.DrawableMenuItems.First().Item)
                {
                    keyboardInputDropdown3.Menu.TriggerEvent(new KeyDownEvent(new InputState(), Key.Up));
                    keyboardInputDropdown3.Menu.TriggerEvent(new KeyUpEvent(new InputState(), Key.Up));
                }
            });

            int lastVisibleIndexOnTheCurrentPage = 0;
            AddStep("Preselect last visible item on the current page", () =>
            {
                lastVisibleIndexOnTheCurrentPage = keyboardInputDropdown3.Menu.DrawableMenuItems.ToList().IndexOf(keyboardInputDropdown3.Menu.VisibleMenuItems.Last());
                keyboardInputDropdown3.Menu.TriggerEvent(new KeyDownEvent(new InputState(), Key.PageDown));
                keyboardInputDropdown3.Menu.TriggerEvent(new KeyUpEvent(new InputState(), Key.PageDown));
            });

            AddAssert("Last visible item on the current page preselected", () => keyboardInputDropdown3.PreselectedIndex == lastVisibleIndexOnTheCurrentPage);

            int lastVisibleIndexOnTheNextPage = 0;
            AddStep("Preselect last visible item on the next page", () =>
            {
                lastVisibleIndexOnTheNextPage =
                    MathHelper.Clamp(lastVisibleIndexOnTheCurrentPage + keyboardInputDropdown3.Menu.VisibleMenuItems.Count(), 0, keyboardInputDropdown3.Menu.Items.Count - 1);
                keyboardInputDropdown3.Menu.TriggerEvent(new KeyDownEvent(new InputState(), Key.PageDown));
                keyboardInputDropdown3.Menu.TriggerEvent(new KeyUpEvent(new InputState(), Key.PageDown));
            });

            AddAssert("Last visible item on the next page preselected", () => keyboardInputDropdown3.PreselectedIndex == lastVisibleIndexOnTheNextPage);

            int firstVisibleIndexOnTheCurrentPage = 0;
            AddStep("Preselect first visible item on the current page", () =>
            {
                firstVisibleIndexOnTheCurrentPage = keyboardInputDropdown3.Menu.DrawableMenuItems.ToList().IndexOf(keyboardInputDropdown3.Menu.VisibleMenuItems.First());
                keyboardInputDropdown3.Menu.TriggerEvent(new KeyDownEvent(new InputState(), Key.PageUp));
                keyboardInputDropdown3.Menu.TriggerEvent(new KeyUpEvent(new InputState(), Key.PageUp));
            });

            AddAssert("First visible item on the current page preselected", () => keyboardInputDropdown3.PreselectedIndex == firstVisibleIndexOnTheCurrentPage);

            int firstVisibleIndexOnThePreviousPage = 0;
            AddStep("Preselect first visible item on the previous page", () =>
            {
                firstVisibleIndexOnThePreviousPage = MathHelper.Clamp(firstVisibleIndexOnTheCurrentPage - keyboardInputDropdown3.Menu.VisibleMenuItems.Count(), 0,
                    keyboardInputDropdown3.Menu.Items.Count - 1);
                keyboardInputDropdown3.Menu.TriggerEvent(new KeyDownEvent(new InputState(), Key.PageUp));
                keyboardInputDropdown3.Menu.TriggerEvent(new KeyUpEvent(new InputState(), Key.PageUp));
            });

            AddAssert("First visible item on the previous page selected", () => keyboardInputDropdown3.PreselectedIndex == firstVisibleIndexOnThePreviousPage);

            AddAssert("First item is preselected", () => keyboardInputDropdown3.Menu.PreselectedItem.Item == keyboardInputDropdown3.Menu.DrawableMenuItems.First().Item);

            AddStep("Preselect last item", () => performPlatformAction(new PlatformAction(PlatformActionType.ListEnd), platformActionContainer2, keyboardInputDropdown3));

            AddAssert("Last item preselected", () => keyboardInputDropdown3.Menu.PreselectedItem.Item == keyboardInputDropdown3.Menu.DrawableMenuItems.Last().Item);

            AddStep("Preselect first item", () => performPlatformAction(new PlatformAction(PlatformActionType.ListStart), platformActionContainer2, keyboardInputDropdown3));

            AddAssert("First item preselected", () => keyboardInputDropdown3.Menu.PreselectedItem.Item == keyboardInputDropdown3.Menu.DrawableMenuItems.First().Item);
        }

        private void toggleDropdownViaClick(StyledDropdown dropdown)
        {
            InputManager.MoveMouseTo(dropdown.Children.First());
            InputManager.Click(MouseButton.Left);
        }

        private class StyledDropdown : BasicDropdown<string>
        {
            public new DropdownMenu Menu => base.Menu;

            protected override DropdownMenu CreateMenu() => new StyledDropdownMenu();

            protected override DropdownHeader CreateHeader() => new StyledDropdownHeader();

            internal new DropdownMenuItem<string> SelectedItem => base.SelectedItem;

            public void SelectItem(MenuItem item) => ((StyledDropdownMenu)Menu).SelectItem(item);

            public int SelectedIndex => Menu.DrawableMenuItems.Select(d => d.Item).ToList().IndexOf(SelectedItem);
            public int PreselectedIndex => Menu.DrawableMenuItems.ToList().IndexOf(Menu.PreselectedItem);

            private class StyledDropdownMenu : DropdownMenu
            {
                public void SelectItem(MenuItem item) => Children.FirstOrDefault(c => c.Item == item)?
                    .TriggerEvent(new ClickEvent(GetContainingInputManager().CurrentState, MouseButton.Left));
            }
        }

        private class StyledDropdownHeader : DropdownHeader
        {
            private readonly SpriteText label;

            protected internal override string Label
            {
                get { return label.Text; }
                set { label.Text = value; }
            }

            public StyledDropdownHeader()
            {
                Foreground.Padding = new MarginPadding(4);
                BackgroundColour = new Color4(255, 255, 255, 100);
                BackgroundColourHover = Color4.HotPink;
                Children = new[]
                {
                    label = new SpriteText(),
                };
            }
        }
    }
}
