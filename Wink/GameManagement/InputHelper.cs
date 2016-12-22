using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

public class InputHelper
{
    protected MouseState currentMouseState, previousMouseState;
    protected KeyboardState currentKeyboardState, previousKeyboardState;
    protected Vector2 scale, offset;
    protected Camera camera;

    protected List<GameObject> leftMousePressedHandlers;

    public InputHelper()
    {
        scale = Vector2.One;
        offset = Vector2.Zero;
        leftMousePressedHandlers = new List<GameObject>();
    }

    public void Update()
    {
        leftMousePressedHandlers.Clear();

        previousMouseState = currentMouseState;
        previousKeyboardState = currentKeyboardState;
        currentMouseState = Mouse.GetState();
        currentKeyboardState = Keyboard.GetState();
    }

    public Vector2 Scale
    {
        get { return scale; }
        set { scale = value; }
    }

    public Vector2 Offset
    {
        get { return offset; }
        set { offset = value; }
    }

    public Camera Camera
    {
        get { return camera; }
        set { camera = value; }
    }

    public Vector2 MousePosition
    {
        get { return ( new Vector2(currentMouseState.X, currentMouseState.Y) - offset ) / scale; }
    }

    /// <summary>
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="action">The delegate method that should be executed if the left mouse button has been pressed.</param>
    /// <param name="doAswell">If this should be done even if other action has already been taken.</param>
    /// <returns></returns>
    public void IfMouseLeftButtonPressedOn(GameObject handler, Action action, Rectangle sensitiveArea, bool doAswell = false)
    {
        bool isPressed = IsMouseLeftButtonPressed();
        Vector2 globalMousePosition = MousePosition;
        if (handler is SpriteGameObject)
        {
            globalMousePosition += ((handler as SpriteGameObject).CameraSensitivity * (camera != null ? camera.Position : Vector2.One));
        }
        if (isPressed && (leftMousePressedHandlers.Count == 0 || doAswell) && sensitiveArea.Contains(globalMousePosition))
        {
            action.Invoke();
            leftMousePressedHandlers.Add(handler);
        }
    }

    public void IfMouseLeftButtonPressedOn(GameObject handler, Action action, bool doAswell = false)
    {
        IfMouseLeftButtonPressedOn(handler, action, handler.BoundingBox, doAswell);
    }

    public bool IsMouseLeftButtonPressed()
    {
        return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
    }

    public bool MouseLeftButtonReleased()
    {
        return currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed;
    }

    public bool MouseLeftButtonDown()
    {
        return currentMouseState.LeftButton == ButtonState.Pressed;
    }

    public bool KeyPressed(Keys k)
    {
        return currentKeyboardState.IsKeyDown(k) && previousKeyboardState.IsKeyUp(k);
    }

    public bool IsKeyDown(Keys k)
    {
        return currentKeyboardState.IsKeyDown(k);
    }

    public Keys[] GetPressedKeys()
    {
        return currentKeyboardState.GetPressedKeys().Where(key => !previousKeyboardState.GetPressedKeys().Contains(key)).ToArray();
    }

    public bool AnyKeyPressed
    {
        get { return currentKeyboardState.GetPressedKeys().Length > 0 && previousKeyboardState.GetPressedKeys().Length == 0; }
    }

    /// <summary>
    /// Returns amount scrolled since last tick
    /// </summary>
    public int ScrollWheelDelta
    {
        get { return currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue; }
    }
}