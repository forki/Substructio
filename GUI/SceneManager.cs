﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using QuickFont;
using Substructio.Core;

namespace Substructio.GUI
{
    public class SceneManager : IDisposable
    {
        #region Member Variables

        #endregion

        #region Properties

        private readonly List<Scene> _scenesToAdd;
        private readonly List<Scene> _scenesToRemove;

        private bool InputSceneFound;
        public List<Scene> SceneList;

        public Camera ScreenCamera { get; private set; }
        public QFont Font { get; private set; }
        public GameWindow GameWindow { get; private set; }
        public string FontPath { get; private set; }

        private float _rotation;
        private int _direction = 1;

        public int Width {get { return GameWindow.Width; }}
        public int Height {get { return GameWindow.Height; }}

        #endregion

        #region Constructors

        /// <summary>
        /// The default Constructor.
        /// </summary>
        public SceneManager(GameWindow gameWindow, Camera camera, QFont font, string fontPath)
        {
            GameWindow = gameWindow;
            SceneList = new List<Scene>();
            _scenesToAdd = new List<Scene>();
            _scenesToRemove = new List<Scene>();
            FontPath = fontPath;
            Font = font;
            Font.ProjectionMatrix = camera.ScreenProjectionMatrix;
            //Font = new QFont(Directories.LibrariesDirectory + Directories.TextFile, 18);
            ScreenCamera = camera;
            ScreenCamera.Center = Vector2.Zero;
            ScreenCamera.MaximumScale = new Vector2(10000, 10000);
        }

        #endregion

        #region Public Methods

        public void Draw(double time)
        {
            Font.ResetVBOs();
            Scene excl = SceneList.Where(scene => scene.Visible).FirstOrDefault(scene => scene.Exclusive);
            if (excl == null)
            {
                foreach (Scene scene in SceneList.Where(screen => screen.Visible))
                {
                    scene.Draw(time);
                }
            }
            else
            {
                excl.Draw(time);
            }
            Font.Draw();
        }

        public void Update(double time)
        {
            AddRemoveScenes();

            ScreenCamera.Update(time);
            ScreenCamera.SnapToCenter();
            ScreenCamera.UpdateProjectionMatrix();
            ScreenCamera.UpdateModelViewMatrix();

            //if (!SceneList.Last().Loaded)
            //{
            //    SceneList.Last().Load();
            //}
            //else
            //{

            //    SceneList.Last().Update(time, true);
            //}
            for (int i = SceneList.Count - 1; i >= 0; i--)
            {
                if (!SceneList[i].Loaded)
                {
                    SceneList[i].Load();
                }
                else
                {
                    if (!InputSceneFound && SceneList[i].Visible)
                    {
                        SceneList[i].Update(time, true);
                        InputSceneFound = true;
                    }
                    else
                    {
                        SceneList[i].Update(time);
                    }
                }
            }
            InputSceneFound = false;
            InputSystem.Update(GameWindow.Focused);
        }

        public void DrawTextLine(string text, Vector3 position, Color4 colour)
        {
            Font.Print(text, position, QFontAlignment.Centre, (Color)colour);
        }

        public void DrawProcessedText(ProcessedText pText, Vector3 position, Color4 colour)
        {
            Font.Print(pText, position, (Color)colour);
        }

        private void AddRemoveScenes()
        {
            foreach (Scene scene in _scenesToRemove)
            {
                SceneList.Remove(scene);
            }

            foreach (Scene scene in _scenesToAdd)
            {
                SceneList.Add(scene);
            }

            _scenesToAdd.Clear();
            _scenesToRemove.Clear();
        }

        public void Resize(EventArgs e)
        {
            ScreenCamera.UpdateResize(GameWindow.Width, GameWindow.Height);
            Font.ProjectionMatrix = ScreenCamera.ScreenProjectionMatrix;
            foreach (Scene scene in SceneList)
            {
                scene.Resize(e);
            }
        }

        public void AddScene(Scene s)
        {
            s.SceneManager = this;
            _scenesToAdd.Add(s);
        }

        public void RemoveScene(Scene s)
        {
            s.Dispose();
            _scenesToRemove.Add(s);
        }
        #endregion

        #region Private Methods

        #endregion

        public void Dispose()
        {
            foreach (Scene scene in SceneList)
            {
                scene.Dispose();
            }
            SceneList.Clear();
        }
    }
}