using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class Table : GameObject
    {
        private class Column
        {
            public static List<Type> columnTypes = new List<Type>();

            public Column()
            {
                columnTypes.Add(typeof(object));
            }

            public virtual string ToString(object obj)
            {
                return obj.ToString();
            }
        }

        private class Column<U> : Column
        {
            Func<U, string> func;

            public Column(Func<U, string> func)
            {
                columnTypes.Add(typeof(U));
                this.func = func;
            }

            public override string ToString(object obj)
            {
                if (obj is U)
                {
                    U Uobj = (U)obj;
                    return func.Invoke(Uobj);
                }
                else
                {
                    return base.ToString(obj);
                }
            }
        }

        class TableRowValuesIncompatibleException : Exception
        {
        }

        List<Column> columns;
        List<List<object>> values;

        string[,] stringTable;
        int[] columnWidths;

        SpriteFont sf;

        const int cellHeight = 40;
        const int cellMargin = 2;

        public int Width
        {
            get { return columnWidths.Sum(); }
        }

        public int Height
        {
            get { return Column.columnTypes.Count * cellHeight; }
        }

        public Table()
        {
            columns = new List<Column>();
            values = new List<List<object>>();
            sf = GameEnvironment.AssetManager.GetFont("TextFieldFont");
        }

        /// <summary>
        /// </summary>
        /// <param name="t">The Type of objects that will be in this column.</param>
        /// <param name="f">The Func that will describe how to extract the string to display.</param>
        public void AddColumn<T>(Func<T, string> f) where T : class
        {
            columns.Add(new Column<T>(f));
        }

        /// <summary>
        /// .ToString() will be used to extract the string that will be displayed.
        /// </summary>
        /// <param name="t">The Type of objects that will be in this column.</param>
        public void AddColumn()
        {
            columns.Add(new Column());
        }

        public void AddRow(List<object> values)
        {
            if (values.Count != Column.columnTypes.Count)
            {
                throw new TableRowValuesIncompatibleException();
            }
            
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].GetType() != Column.columnTypes[i] && !values[i].GetType().IsSubclassOf(Column.columnTypes[i]))
                {
                    throw new TableRowValuesIncompatibleException();
                }
            }

            this.values.Add(values);

            BuildStrings();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            Texture2D bg = GameEnvironment.AssetManager.GetSingleColorPixel(Color.DarkGray);
            Texture2D fg = GameEnvironment.AssetManager.GetSingleColorPixel(Color.White);

            for (int x = 0; x < stringTable.GetLength(0); x++)
            {
                int totalWidth = columnWidths.Take(x).Sum();
                for (int y = 0; y < stringTable.GetLength(1); y++)
                {
                    int totalHeight = y * cellHeight;
                    Rectangle dest = new Rectangle(totalWidth + (int)GlobalPosition.X, totalHeight + (int)GlobalPosition.Y, columnWidths[x], cellHeight);
                    spriteBatch.Draw(bg, dest, Color.White);
                    dest.X += cellMargin;
                    dest.Y += cellMargin;
                    dest.Width -= cellMargin;
                    dest.Height -= cellMargin;
                    spriteBatch.Draw(fg, dest, Color.White);
                    spriteBatch.DrawString(sf, stringTable[x, y], dest.Location.ToVector2(), Color.Black);
                }
            }
            spriteBatch.Draw(bg, new Rectangle(Width + (int)GlobalPosition.X, (int)GlobalPosition.Y, cellMargin, Height), Color.White);
            spriteBatch.Draw(bg, new Rectangle((int)GlobalPosition.X, Height + (int)GlobalPosition.Y, Width + cellMargin, cellMargin), Color.White);
        }

        public void BuildStrings()
        {
            stringTable = new string[Column.columnTypes.Count, values.Count];
            columnWidths = new int[Column.columnTypes.Count];
            for (int y = 0; y < values.Count; y++)
            {
                List<object> row = values[y];
                for (int x = 0; x < row.Count; x++)
                {
                    stringTable[x, y] = columns[x].ToString(row[x]);
                    int width = (int)Math.Ceiling(sf.MeasureString(stringTable[x, y]).X) + cellMargin;
                    if (width > columnWidths[x])
                    {
                        columnWidths[x] = width;
                    }
                }
            }
        }
    }
}
