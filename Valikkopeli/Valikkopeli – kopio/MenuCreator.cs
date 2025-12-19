using System;
using System.Numerics;
using ZeroElectric.Vinculum;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RayGuiCreator
{
    public class MenuCreator
    {
        // Shared font and style settings

        static Font defaultFont;
        static bool defaultFontSet = false;

        /// <summary>
        /// Sets a font that is used by all menus.
        /// </summary>
        /// <param name="filename">Filename of the font file.</param>
        public static void SetDefaultFont(string filename)
        {
            defaultFont = Raylib.LoadFont(filename);
            if (Raylib.IsFontReady(defaultFont))
            {
                defaultFontSet = true;
            }
        }


        int startY;
        /// <summary>
        /// Current drawing X coordinate. Next element will be drawn to this coordinate
        /// </summary>
        public int drawX { get; private set; }
        /// <summary>
        /// Current drawing Y coordinate. Next element will be drawn to this coordinate
        /// </summary>
        public int drawY { get; private set; }

        int menuWidth;
        int rowHeight;
        int betweenRows;
        int textSize;
        Font font;

        Stack<MultipleChoiceEntry> dropDowns;

        /// <summary>
        /// Creates a MenuCreator object that can be used to
        /// create menu elements and layout them automatically
        /// </summary>
        /// <param name="x">Top left x coordinate of the menu</param>
        /// <param name="y">Top left y coordinate of the menu</param>
        /// <param name="rowHeight">Minimum height of one element. Text is made to always fit vertically inside a row or element</param>
        /// <param name="width">Minimum width of elements. Text is always made fit horizontally and be fully visible</param>
        /// <param name="betweenItems">Amount of pixels between items vertically, default 1 px</param>
        /// <param name="textHeightAdjust">Text height is rowHeight + this value. Default value is 0. Negative values make the text smaller</param>
        public MenuCreator(int x, int y, int rowHeight, int width, int betweenItems = 1, int textHeightAdjust = 0)
        {
            drawX = x;
            drawY = y;
            startY = y;
            this.rowHeight = rowHeight;
            betweenRows = betweenItems;
            menuWidth = width;
            textSize = rowHeight + textHeightAdjust;
            RayGui.GuiSetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.TEXT_SIZE, textSize);
            font = RayGui.GuiGetFont();
            if (defaultFontSet)
            {
                font = defaultFont;
            }
            RayGui.GuiSetFont(font);
            dropDowns = new Stack<MultipleChoiceEntry>();
        }

        private Rectangle GetRectangleForText(string text, GuiControl controlType)
        {
            Vector2 textWH = GetTextArea(controlType, text);
            return new Rectangle(drawX, drawY, textWH.X, textWH.Y);
        }


        /// <summary>
        /// Creates a text label.
        /// </summary>
        /// <param name="text">Text to be shown</param>
        public void Label(string text)
        {
            Rectangle r = GetRectangleForText(text, GuiControl.LABEL);
            RayGui.GuiLabel(r, text);

            drawY += (int)r.height + betweenRows;
        }

        /// <summary>
        /// Creates a clickable button with text and background rectangle
        /// </summary>
        /// <param name="text">Text on the button</param>
        /// <returns>True if clicked</returns>
        public bool Button(string text)
        {
            Rectangle r = GetRectangleForText(text, GuiControl.BUTTON);
            bool click = (RayGui.GuiButton(r, text) == 1);
            drawY += (int)r.height + betweenRows;
            return click;
        }

        /// <summary>
        /// Creates a button without the background rectangle
        /// </summary>
        /// <param name="text">Text on the button</param>
        /// <returns>True if clicked</returns>
        public bool LabelButton(string text)
        {
            Rectangle r = GetRectangleForText(text, GuiControl.LABEL);
            bool clicked = (RayGui.GuiLabelButton(r, text) == 1);
            drawY += (int)r.height + betweenRows;
            return clicked;
        }

        /// <summary>
        /// Creates a checkbox that can be toggled on and off
        /// </summary>
        /// <param name="text">Text on the left of the box</param>
        /// <param name="value">Is the value toggled. This value changes if the box is clicked</param>
        /// <returns>True if the checkbox was clicked.</returns>
        public bool Checkbox(string text, ref bool value)
        {
            Rectangle r = GetRectangleForText(text, GuiControl.CHECKBOX);

            int clicked = RayGui.GuiCheckBox(r, text, ref value);

            drawY += (int)r.height + betweenRows;

            return clicked == 1;
        }

        /// <summary>
        /// Creates a textbox where user can write.
        /// The width of the textbox is determined by the TextBoxEntry's Length
        /// </summary>
        /// <param name="data">Contents of the text area</param>
        public void TextBox(TextBoxEntry data)
        {
            // Make an estimate of the width
            // Width of W: usually the widest letter
            Vector2 rayWH = Raylib.MeasureTextEx(font, "W", textSize, 0.0f);
            // Multiply with text box character count
            int allChars = (int)rayWH.X * data.GetLength();
            int padding = RayGui.GuiGetStyle((int)GuiControl.TEXTBOX, (int)GuiControlProperty.TEXT_PADDING);
            allChars += padding * 2;
            rayWH.Y += padding * 2;
            // Compare to menu width
            int w = Math.Max(allChars, menuWidth);
            int h = Math.Max((int)rayWH.Y, rowHeight);

            unsafe
            {
                fixed (sbyte* textPointer = data.bytes)
                {
                    if (RayGui.GuiTextBox(new Rectangle(drawX, drawY, w, h), textPointer, data.GetLength(), data.IsActive) == 1)
                    {
                        data.IsActive = !data.IsActive;
                    }
                }
            }

            drawY += h + betweenRows;
        }

        /// <summary>
        /// Creates a spinner that can be increased and decreased
        /// </summary>
        /// <param name="text">Name of the spinner, shown on the left side</param>
        /// <param name="currentValue">Current value. This is changed when value is decreased or increased.</param>
        /// <param name="minValue">Smallest possible value</param>
        /// <param name="maxValue">Largest possible value</param>
        /// <param name="isActive">Is this control active, is changed automatically</param>
        /// <returns>True if the value was changed, False otherwise</returns>
        public bool Spinner(string text, ref int currentValue, int minValue, int maxValue, ref bool isActive)
        {
            int oldValue = currentValue;
            Vector2 textWH = GetTextArea(GuiControl.SPINNER, text);
            Vector2 aWH = Raylib.MeasureTextEx(font, "A", textSize, 0.0f);
            int textW = (int)(Raylib.MeasureTextEx(font, text, textSize, 0.0f).X + aWH.X);
            unsafe
            {
                isActive = (RayGui.GuiSpinner(new Rectangle(drawX + textW, drawY, textWH.X - textW, textWH.Y), text, ref currentValue, minValue, maxValue, isActive) == 1);
            }

            drawY += (int)textWH.Y + betweenRows;
            return (oldValue != currentValue);
        }

        /// <summary>
        /// Creates a slider that can be moved horizontally with the mouse.
        /// </summary>
        /// <param name="minText">Text on the left side of slider: minimum value</param>
        /// <param name="maxText">Text on the right side of slider: max value</param>
        /// <param name="value">Current value of slider. Is changed when value changes.</param>
        /// <param name="min">Smallest possible value</param>
        /// <param name="max">Largest possible value</param>
        public void Slider(string minText, string maxText, ref float value, float min, float max)
        {
            Vector2 areaWH = GetTextArea(GuiControl.SLIDER, maxText);
            Vector2 minWH = Raylib.MeasureTextEx(font, minText, textSize, 0.0f);
            Vector2 maxWH = Raylib.MeasureTextEx(font, maxText, textSize, 0.0f);
            Vector2 aWH = Raylib.MeasureTextEx(font, "A", textSize, 0.0f);
            int textW = (int)(minWH.X + maxWH.X + aWH.X);
            int x = drawX + (int)(minWH.X + aWH.X / 2);
            RayGui.GuiSlider(new Rectangle(x, drawY, menuWidth - textW, areaWH.Y), minText, maxText, ref value, min, max);
            drawY += (int)areaWH.Y + betweenRows;
        }

        /// <summary>
        /// Creates a progress bar that can be be used to display progress.
        /// </summary>
        /// <param name="minText">Text on the left side of bar: minimum value</param>
        /// <param name="maxText">Text on the right side of bar: max value</param>
        /// <param name="value">Current value of bar.</param>
        /// <param name="min">Smallest possible value</param>
        /// <param name="max">Largest possible value</param>
        public void ProgressBar(string minText, string maxText, ref float value, float min, float max)
        {
            Vector2 areaWH = GetTextArea(GuiControl.SLIDER, maxText);
            Vector2 minWH = Raylib.MeasureTextEx(font, minText, textSize, 0.0f);
            Vector2 maxWH = Raylib.MeasureTextEx(font, maxText, textSize, 0.0f);
            Vector2 aWH = Raylib.MeasureTextEx(font, "A", textSize, 0.0f);
            int textW = (int)(minWH.X + maxWH.X + aWH.X);
            int x = drawX + (int)(minWH.X + aWH.X / 2);
            RayGui.GuiProgressBar(new Rectangle(x, drawY, menuWidth - textW, areaWH.Y), minText, maxText, ref value, min, max);
            drawY += (int)areaWH.Y + betweenRows;
        }

        public int ColorPicker(string text, float heightInRows, ref int color)
        {
            Color c = Raylib.GetColor((uint)color);
            float colorAreaSize = menuWidth * 0.8f;
            RayGui.GuiColorPicker(new Rectangle(drawX, drawY, colorAreaSize, colorAreaSize), text, ref c);
            drawY += (int)colorAreaSize;
            return Raylib.ColorToInt(c);
        }

        /// <summary>
        /// Creates a box that cycles through choices when pressed
        /// </summary>
        /// <param name="data">Available choices</param>
        /// <returns>True if clicked</returns>
        public bool ComboBox(string text, MultipleChoiceEntry entries)
        {
            int tempActive = entries.GetIndex();
            int oldValue = tempActive;
            Rectangle r = GetRectangleForText(entries.GetEntryAt(tempActive), GuiControl.COMBOBOX);
            int newValue = RayGui.GuiComboBox(r, entries.GetConcatOptions(), ref tempActive);
            entries.SetIndex(tempActive);

            drawY += (int)r.height + betweenRows;
            return (newValue != oldValue);

        }

        /// <summary>
        /// Displays a MultipleChoiceEntry as a group of buttons. Only one of the buttons
        /// can be selected at one time.
        /// </summary>
        /// <param name="data">Available choices</param>
        /// <returns>True if the selected entry changes</returns>
        public bool ToggleGroup(MultipleChoiceEntry data)
        {
            // Find the widest text?
            // What if menu is too narrow?
            Vector2 textWH = GetTextArea(GuiControl.TOGGLE, data.GetSelected());
            // padding between choices is about 1 px
            int oldIndex = data.GetIndex();
            RayGui.GuiToggleGroup(
                new Rectangle(drawX, drawY, menuWidth, textWH.Y),
                data.GetConcatOptions(), ref data.GetIndex());

            drawY += (int)(data.GetChoiceAmount() * (textWH.Y + betweenRows));
            return (data.GetIndex() != oldIndex);
        }

        /// <summary>
        /// Creates a dropdown menu where user can select one of the choices.
        /// The choices are drawn only when the dropdown is clicked first.
        /// You must call EndMenu() if you use a DropDown
        /// </summary>
        /// <param name="data">Available choices</param>
        /// <returns>True if selected entry changed</returns>
        public bool DropDown(MultipleChoiceEntry data)
        {
            int oldValue = data.GetIndex();
            data.DrawY = drawY;
            if (data.IsOpen)
            {
                RayGui.GuiDisable();
                dropDowns.Push(data);
            }
            else
            {
                // Draw here 
                DrawDropdown(data);
            }
            int newValue = data.GetIndex();
            drawY += (int)GetTextArea(GuiControl.DROPDOWNBOX, "A").Y + 1;
            return (oldValue != newValue);
        }

        /// <summary>
        /// Call this after drawing all items.
        /// This will draw the opened dropdowns correctly.
        /// </summary>
        /// <returns>Height of the menu in pixels</returns>
        public int EndMenu()
        {
            // Draw all open dropdowns in reverse order
            // so that the are drawn over other elements
            if (dropDowns.Count > 0)
            {
                RayGui.GuiEnable();
                for (int i = 0; i < dropDowns.Count; i++)
                {
                    DrawDropdown(dropDowns.Pop());
                }
            }
            return drawY - startY;
        }

        public static Color GetBackgroundColor()
        {
            return Raylib.GetColor((uint)RayGui.GuiGetStyle((int)GuiControl.DEFAULT, (int)GuiControlProperty.BASE_COLOR_NORMAL));
        }
        public static Color GetLineColor()
        {
            return Raylib.GetColor((uint)RayGui.GuiGetStyle((int)GuiControl.DEFAULT, (int)GuiControlProperty.TEXT_COLOR_NORMAL));
        }


        /// <summary>
        /// Sets the background color(s) used by menu elements. Use Raylib.ColorToInt to convert
        /// from enumerated color to html color format: 0xRRGGBBAA
        /// </summary>
        /// <param name="normal">Color for normal state</param>
        public static void SetBackgroundColors(int normal, int focused, int pressed)
        {
            GuiControl c = GuiControl.DEFAULT;
            {

                RayGui.GuiSetStyle((int)c, (int)GuiControlProperty.BASE_COLOR_NORMAL, normal);
                RayGui.GuiSetStyle((int)c, (int)GuiControlProperty.BASE_COLOR_FOCUSED, focused);
                RayGui.GuiSetStyle((int)c, (int)GuiControlProperty.BASE_COLOR_PRESSED, pressed);

            }
        }

        /// <summary>
        /// Sets the border color(s) used by menu elements. Use Raylib.ColorToInt to convert
        /// from enumerated color to html color format: 0xRRGGBBAA
        /// </summary>
        /// <param name="normal">Color for normal state</param>
        public static void SetBorderColors(int normal, int focused, int pressed)
        {
            GuiControl c = GuiControl.DEFAULT;
            {

                RayGui.GuiSetStyle((int)c, (int)GuiControlProperty.BORDER_COLOR_NORMAL, normal);
                RayGui.GuiSetStyle((int)c, (int)GuiControlProperty.BORDER_COLOR_PRESSED, pressed);
                RayGui.GuiSetStyle((int)c, (int)GuiControlProperty.BORDER_COLOR_FOCUSED, focused);
            }
        }


        /// <summary>
        /// Sets the text color(s) used by menu elements. Use Raylib.ColorToInt to convert
        /// from enumerated color to html color format: 0xRRGGBBAA
        /// </summary>
        /// <param name="normal">Color for normal state</param>
        /// <param name="focused">Color for focused state. If not provided, normal color is used</param>
        /// <param name="pressed">Color for pressed state. If not provided, normal color is used</param>
        public static void SetTextColors(int normal, int focused, int pressed)
        {
            GuiControl c = GuiControl.DEFAULT;
            {
                RayGui.GuiSetStyle((int)c, (int)GuiControlProperty.TEXT_COLOR_NORMAL, normal);
                RayGui.GuiSetStyle((int)c, (int)GuiControlProperty.TEXT_COLOR_FOCUSED, focused);
                RayGui.GuiSetStyle((int)c, (int)GuiControlProperty.TEXT_COLOR_PRESSED, pressed);
            }
        }

        /// <summary>
        /// Draws an open dropdown over other menu elements
        /// </summary>
        /// <param name="data">The choices of the dropdown</param>
        private void DrawDropdown(MultipleChoiceEntry data)
        {
            int tempActive = data.GetIndex();
            Vector2 textWH = GetTextArea(GuiControl.DROPDOWNBOX, data.GetEntryAt(tempActive));
            unsafe
            {
                if (RayGui.GuiDropdownBox(new Rectangle(drawX, data.DrawY, menuWidth, textWH.Y), data.GetConcatOptions(), ref tempActive, data.IsOpen) == 1)
                {
                    data.IsOpen = !data.IsOpen;
                }
            }
            data.SetIndex(tempActive);
        }

        /// <summary>
        /// Calculates the dimensions that a string of text will need to be visible.
        /// </summary>
        /// <param name="control">The type of menu element or control</param>
        /// <param name="text">Text to be measured</param>
        /// <param name="addPadding">Should the style's padding be added to the results. Default value is true</param>
        /// <returns>Width and height of the area in pixels.</returns>
        private Vector2 GetTextArea(GuiControl control, string text, bool addPadding = true)
        {
            int padding = RayGui.GuiGetStyle((int)control, (int)GuiControlProperty.TEXT_PADDING);
            Vector2 textWH = Raylib.MeasureTextEx(font, text, textSize, 0.0f);
            if (addPadding)
            {
                textWH.X += padding * 2;
                textWH.Y += padding * 2;
            }
            int w = Math.Max((int)textWH.X, menuWidth);
            int h = Math.Max((int)textWH.Y, rowHeight);
            return new Vector2(w, h);
        }
    }
}