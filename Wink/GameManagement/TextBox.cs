using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

class TextBox : GameObjectList
{
    Random r = new Random();
    Vector2 objPosition;
    Vector2 center;
    Rectangle limitBB;

    public TextBox(string path, int textPlace, string overlay, string font, float objPositionX, float objPositionY, Vector2 textPosition, int cameraSensitivity) : base(100, "")
    {

        visible = true;
        List<string> textLines = new List<string>();
        StreamReader fileReader = new StreamReader(path);
        string line = fileReader.ReadLine();
        int width = line.Length;
        while (line != null)
        {
            textLines.Add(line);
            line = fileReader.ReadLine();
        }

        if (textPlace == 0)
            textPlace = r.Next(1, textLines.Count);


        SpriteGameObject textFrame = new SpriteGameObject(overlay, 1, "", 0, cameraSensitivity, 1);
        Add(textFrame);

        objPosition = new Vector2(objPositionX, objPositionY);
        center = new Vector2(textFrame.Width / 2, textFrame.Height);
        Position = objPosition - center;

        
        TextGameObject text = new TextGameObject(font, cameraSensitivity, 2);

        text.Text = textLines[textLines.Count - textPlace];
        text.Position = textPosition;
        text.Color = Color.Black;
        Add(text);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        //Check if position is out of bounds, if so correct.
        MovingCamera camera = GameWorld.Find("camera") as MovingCamera;

        if (camera != null)
        {
            limitBB = camera.CameraLimit;

            if (Position.X < limitBB.Left)
                position.X = limitBB.Left;
            else if (Position.X  + center.X*2 > limitBB.Right)
                position.X = limitBB.Right - center.X*2 ;

            if (Position.Y > limitBB.Bottom)
                position.Y = limitBB.Bottom;
            else if (Position.Y < limitBB.Top)
                position.Y = limitBB.Top;
        }
    }
}
