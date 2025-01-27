using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using GameFramework;

namespace FireAndSmoke_WP8
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FireAndSmokeGame : GameFramework.GameHost
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Variables required for the scene to be rendered
        private BasicEffect _effect;

        public FireAndSmokeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Calculate the screen aspect ratio
            float aspectRatio = (float)GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height;
            // Create a projection matrix
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspectRatio, 0.1f, 1000.0f);

            // Calculate a view matrix (where we are looking from and to)
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 5, 5), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            // Create and initialize the effect
            _effect = new BasicEffect(GraphicsDevice);
            _effect.VertexColorEnabled = false;
            _effect.TextureEnabled = true;
            _effect.Projection = projection;
            _effect.View = view;
            _effect.World = Matrix.Identity;
            _effect.LightingEnabled = true;
            _effect.EnableDefaultLighting();
            //_effect.DirectionalLight0.Enabled = true;
            //_effect.DirectionalLight0.Direction = new Vector3(0, 0, -1);
            //_effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            //_effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures
            Textures.Add("Grass", Content.Load<Texture2D>("Grass"));
            Textures.Add("Sky", Content.Load<Texture2D>("Sky"));
            Textures.Add("Smoke", Content.Load<Texture2D>("Smoke"));
            Textures.Add("Fire", Content.Load<Texture2D>("Fire"));

            // ** Debug, reset the light to undo Content.Load<Model> switching them off...
            _effect.DirectionalLight0.DiffuseColor = _effect.DirectionalLight0.DiffuseColor;
            _effect.DirectionalLight0.SpecularColor = _effect.DirectionalLight0.SpecularColor;
            // ** Debug, set the initial samplerstate to LinearWrap
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // Reset the game
            ResetGame();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();

            // Update all the game objects
            UpdateAll(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(80, 127, 171));

            // Draw the main scenery
            DrawObjects(gameTime, _effect);

            // Draw the particles
            DrawParticles(gameTime, _effect, Textures["Smoke"]);
            DrawParticles(gameTime, _effect, Textures["Fire"]);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Reset the game
        /// </summary>
        private void ResetGame()
        {
            // Clear any existing objects
            GameObjects.Clear();

            // Add the ground
            GameObjects.Add(new GroundObject(this, Textures["Grass"]));

            // Add some fire particles
            for (int i = 0; i < 75; i++)
            {
                GameObjects.Add(new FireParticleObject(this, Textures["Fire"], new Vector3(0, 0.1f, 0), new Vector3(0.1f)));
            }

            // Add the camera to the game
            Camera = new CameraObject(this);

            // Add the sky box
            Skybox = new MatrixSkyboxObject(this, Textures["Sky"], new Vector3(0, 0.2f, 0), new Vector3(2, 0.8f, 2));
        }

    }
}
