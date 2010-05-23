using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace TetrisXNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum State {
            Play,
            Vanish,
            Dead,
        }

        private State state;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        private Random random = new Random();

        private float aspectRatio;
        private Model backgroundModel;
        private Model frameModel;
        private Model[] blockModels;

        private Vector3 cameraPosition;

        private Tetrimino nextTetrimino;
        private Tetrimino tetrimino;

        private Controller controller;

        private Matrix projection;
        public Matrix Projection {
            get { return projection; }
        }

        private Matrix view;
        public Matrix View {
            get { return view; }
        }

        private List<Block> blocks;
        private List<VanishBlock> vanishBlocks;

        private float[] vanishDownY;

        private bool isOldDownEnter;

        private FPSCounter fpsCounter = new FPSCounter();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
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
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            font = Content.Load<SpriteFont>("DefaultFont");

            // プロジェクション行列の作成
            aspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                aspectRatio,
                0.005f,
                1000.0f);

            // モデルのロード
            backgroundModel = Content.Load<Model>("background");
            frameModel = Content.Load<Model>("frame");

            String[] modelNames = {
                                  "blockRed",
                                  "blockYellow",
                                  "blockPurple",
                                  "blockGreen",
                                  "blockBlue",
                                  "blockOrange",
                                  "blockCyan"
                                  };

            blockModels = new Model[7];
            for (int i = 0; i < blockModels.Length; i++)
            {
                blockModels[i] = Content.Load<Model>(modelNames[i]);
            }

            // カメラ位置の設定
            cameraPosition = new Vector3(0, 10, -10);
            UpdateViewMatrix();

            // コントロールの作成
            controller = new Controller();

            // ブロックの作成
            blocks = new List<Block>();
            vanishBlocks = new List<VanishBlock>();

            // テトリミノの設定
            SetNextTetrimino();

            state = State.Play;
        }

        private Vector3 nextTetriminoPosition = new Vector3(-0.9f, 1.0f, 0.0f);
        private Vector3 tetriminoStartPosition = new Vector3(-0.05f, 1.2f, 0.0f);

        private void SetNextTetrimino() {
            if (nextTetrimino == null) nextTetrimino = CreateTetrimino(-1);

            tetrimino = nextTetrimino;
            tetrimino.Position = tetriminoStartPosition;
            controller.Tetrimino = tetrimino;

            tetrimino.Update();
            if (CheckBlockCollision(tetrimino.Blocks)) {
                state = State.Dead;
                foreach (Block block in blocks)
                {
                    VanishBlock vanishBlock = new VanishBlock(this, block.Model);
                    vanishBlock.Position = block.Position;
                    vanishBlocks.Add(vanishBlock);
                }
            }

            nextTetrimino = CreateTetrimino(-1);
            nextTetrimino.Position = nextTetriminoPosition;
        }

        private Tetrimino CreateTetrimino(int type) {
            if (type < 0) {
                type = random.Next(Tetrimino.typeNum);
            }
            return new Tetrimino(this, blockModels[type], type);
        }

        private void UpdateViewMatrix() {
            view = Matrix.CreateLookAt(
                cameraPosition,
                Vector3.Zero,
                Vector3.Up);
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
            fpsCounter.Update(gameTime.ElapsedRealTime.Milliseconds);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState keystate = Keyboard.GetState();

            Vector3 cameraMove = new Vector3(0.1f, 0.1f, 0.1f);

            if (keystate.IsKeyDown(Keys.LeftShift) ||
                keystate.IsKeyDown(Keys.RightShift))
            {
                if (keystate.IsKeyDown(Keys.Left)) cameraPosition.X -= cameraMove.X;
                if (keystate.IsKeyDown(Keys.Right)) cameraPosition.X += cameraMove.X;
                if (keystate.IsKeyDown(Keys.Up)) cameraPosition.Y -= cameraMove.Y;
                if (keystate.IsKeyDown(Keys.Down)) cameraPosition.Y += cameraMove.Y;
                if (keystate.IsKeyDown(Keys.Z)) cameraPosition.Z -= cameraMove.Z;
                if (keystate.IsKeyDown(Keys.X)) cameraPosition.Z += cameraMove.Z;

                if (keystate.IsKeyDown(Keys.D1)) cameraPosition = new Vector3(0, 0, 3);
                if (keystate.IsKeyDown(Keys.D2)) cameraPosition = new Vector3(3, 0, 0);
                if (keystate.IsKeyDown(Keys.D3)) cameraPosition = new Vector3(0, 3, 0.01f);
            }

            //Tetrimino oldTetrimino = tetrimino;
            //if (keystate.IsKeyDown(Keys.D1)) tetrimino = CreateTetrimino(0);
            //if (keystate.IsKeyDown(Keys.D2)) tetrimino = CreateTetrimino(1);
            //if (keystate.IsKeyDown(Keys.D3)) tetrimino = CreateTetrimino(2);
            //if (keystate.IsKeyDown(Keys.D4)) tetrimino = CreateTetrimino(3);
            //if (keystate.IsKeyDown(Keys.D5)) tetrimino = CreateTetrimino(4);
            //if (keystate.IsKeyDown(Keys.D6)) tetrimino = CreateTetrimino(5);
            //if (keystate.IsKeyDown(Keys.D7)) tetrimino = CreateTetrimino(6);

            //if (oldTetrimino != tetrimino) {
            //    controller.Tetrimino = tetrimino;
            //}

            //if (keystate.IsKeyDown(Keys.NumPad4)) cameraPosition = new Vector3(10, 0, 0);
            //if (keystate.IsKeyDown(Keys.NumPad5)) cameraPosition = new Vector3(10, 0, 0);
            //if (keystate.IsKeyDown(Keys.NumPad6)) cameraPosition = new Vector3(10, 0, 0);
            //if (keystate.IsKeyDown(Keys.NumPad7)) cameraPosition = new Vector3(10, 0, 0);
            //if (keystate.IsKeyDown(Keys.NumPad8)) cameraPosition = new Vector3(10, 0, 0);


            //Vector3 lightValue = new Vector3(0.01f, 0.01f, 0.01f);
            //if (keystate.IsKeyDown(Keys.RightControl))
            //{
            //    lightValue = new Vector3(-0.01f, -0.01f, -0.01f);
            //}
            //if (keystate.IsKeyDown(Keys.D1)) Block.AmbientLightColor += lightValue;
            //if (keystate.IsKeyDown(Keys.D2)) Block.DiffuseColor += lightValue;
            //if (keystate.IsKeyDown(Keys.D3)) Block.SpecularColor += lightValue;
            //if (keystate.IsKeyDown(Keys.D4)) Block.EmissiveColor += lightValue;
            //if (keystate.IsKeyDown(Keys.D5)) Block.SpecularPower += lightValue.X;
            
            UpdateViewMatrix();

            switch (state)
	        {
		        case State.Play:
                    tetrimino.Position.Y -= 0.001f;

                    CheckMovale();
                    CheckRotatable();

                    controller.Update();
                    tetrimino.Update();

                    CheckCollision();
                    CheckLine();
                    break;
                case State.Vanish:
                    foreach (VanishBlock vanishBlock in vanishBlocks)
                    {
                        vanishBlock.Update();
                    }
                    if (vanishBlocks[0].IsAnimationEnd) {
                        fallBlock();
                        vanishBlocks.Clear();
                        state = State.Play;
                    }
                    break;
                case State.Dead:
                    foreach (VanishBlock vanishBlock in vanishBlocks)
                    {
                        vanishBlock.Update();
                    }
                    if (vanishBlocks[0].IsAnimationEnd)
                    {
                        bool isTriggerEnter = !isOldDownEnter && keystate.IsKeyDown(Keys.Enter);
                        if (isTriggerEnter)
                        {
                            Reset();
                        }
                    }
                    break;
                default:
                    break;
	        }

            nextTetrimino.Update();

            isOldDownEnter = keystate.IsKeyDown(Keys.Enter);

            Matrix backgroundMatrix = Matrix.CreateTranslation(0, 0, -0.05f);
            UpdateModel(backgroundModel, backgroundMatrix);
            UpdateModel(frameModel, Matrix.Identity);

            base.Update(gameTime);
        }

        private void Reset() {
            nextTetrimino = null;
            tetrimino = null;
            blocks.Clear();
            vanishBlocks.Clear();
            SetNextTetrimino();
            state = State.Play;
        }

        private static Vector3 leftFramePos = new Vector3(-0.55f, 0, 0);
        private static Vector3 rightFramePos = new Vector3(0.55f, 0, 0);
        private static BoundingBox leftFrameBounding = new BoundingBox(
            new Vector3(-0.05f + leftFramePos.X, -1.0f + leftFramePos.Y, -0.05f + leftFramePos.Z),
            new Vector3(0.05f + leftFramePos.X, 1.0f + leftFramePos.Y, 0.05f + leftFramePos.Z));
        private static BoundingBox rightFrameBounding = new BoundingBox(
            new Vector3(-0.05f + rightFramePos.X, -1.0f + rightFramePos.Y, -0.05f + rightFramePos.Z),
            new Vector3(0.05f + rightFramePos.X, 1.0f + rightFramePos.Y, 0.05f + rightFramePos.Z));

        private void CheckMovale() {
            if (tetrimino == null) return;

            Block[] leftMoveBlocks = tetrimino.GetMoveBlocks(new Vector3(-0.1f, 0, 0));
            Block[] rightMoveBlocks = tetrimino.GetMoveBlocks(new Vector3(0.1f, 0, 0));

            bool isLeftFrameMovale = (false == CheckFrameCollision(leftMoveBlocks));
            bool isRightFrameMovale = (false == CheckFrameCollision(rightMoveBlocks));
            bool isLeftBlockMovale = (false == CheckBlockCollision(leftMoveBlocks));
            bool isRightBlockMovale = (false == CheckBlockCollision(rightMoveBlocks));

            controller.IsLeftMovale = (isLeftFrameMovale && isLeftBlockMovale);
            controller.IsRightMovale = (isRightFrameMovale && isRightBlockMovale);
        }

        private void CheckRotatable() {
            if (tetrimino == null) return;

            Block[] leftRotationBlocks = tetrimino.GetRotationBlocks((float)(Math.PI / 2.0f));
            Block[] rightRotationBlocks = tetrimino.GetRotationBlocks(-(float)(Math.PI / 2.0f));

            bool isLeftFrameRotatable = (false == CheckFrameCollision(leftRotationBlocks));
            bool isRightFrameRotatable = (false == CheckFrameCollision(rightRotationBlocks));
            bool isLeftBlockRotatable = (false == CheckBlockCollision(leftRotationBlocks));
            bool isRightBlockRotatable = (false == CheckBlockCollision(rightRotationBlocks));

            controller.IsLeftRotatable = (isLeftFrameRotatable && isLeftBlockRotatable);
            controller.IsRightRotatable = (isRightFrameRotatable && isRightBlockRotatable);
        }

        private bool CheckBlockCollision(Block[] checkBlocks) {
            foreach (Block checkBlock in checkBlocks)
            {
                BoundingSphere checkBounding = checkBlock.Bounding;
                foreach (Block block in blocks)
                {
                    bool isIntersects = checkBounding.Intersects(block.Bounding);
                    if (isIntersects) return true;
                }
            }
            return false;
        }

        private bool CheckFrameCollision(Block[] blocks) {
            BoundingBox[] frameBoundings = { leftFrameBounding, rightFrameBounding };

            foreach (Block block in blocks)
            {
                BoundingSphere blockBounding = block.Bounding;
                foreach (BoundingBox frameBounding in frameBoundings)
                {
                    bool isIntersects = blockBounding.Intersects(frameBounding);
                    if (isIntersects) return true;
                }
            }
            return false;
        }

        private void CheckLine() {
            Vector3 linePosition = new Vector3(-0.6f, floorPosition.Y, 0);
            Vector3 lineDirection = new Vector3(1, 0, 0);

            Ray line = new Ray(linePosition, lineDirection);
            Vector3[] vanishPositions = new Vector3[4];
            int vanishPositionNum = 0;
            for (int i = 0; i < 20; i++)
            {
                line.Position.Y += blockSize;
                int intersectsNum = 0;
                Block[] lineBlocks = new Block[10];

                foreach (Block block in blocks)
                {
                    bool isIntersects = (null != block.Bounding.Intersects(line));
                    if (isIntersects)
                    {
                        lineBlocks[intersectsNum] = block;
                        intersectsNum++;
                    }

                    if (intersectsNum == 10) {
                        vanishPositions[vanishPositionNum] = line.Position;
                        vanishPositionNum++;
                        foreach (Block lineBlock in lineBlocks)
                        {
                            VanishBlock vanishBlock = new VanishBlock(this, lineBlock.Model);
                            vanishBlock.Position = lineBlock.Position;
                            blocks.Remove(lineBlock);
                            vanishBlocks.Add(vanishBlock);
                        }
                        state = State.Vanish;
                        break;
                    }
                }
            }

            int index;
            vanishDownY = new float[blocks.Count];
            for (int i = 0; i < vanishPositionNum; i++)
            {
                float y = vanishPositions[i].Y;
                index = 0;
                foreach (Block block in blocks)
                {
                    if (block.Position.Y > y)
                    {
                        vanishDownY[index] += blockSize;
                    }
                    index++;
                }
            }
        }

        private void fallBlock() {
            int index = 0;
            foreach (Block block in blocks)
            {
                block.Position.Y -= vanishDownY[index];
                index++;
            }
        }

        private float blockSize = 0.1f;
        private static Vector3 floorPosition = new Vector3(0, -1.05f, 0);
        private BoundingBox floorBox = new BoundingBox(
            new Vector3(-0.6f + floorPosition.X, -0.05f + floorPosition.Y, -0.05f + floorPosition.Z),
            new Vector3(0.6f + floorPosition.X, 0.05f + floorPosition.Y, 0.05f + floorPosition.Z));

        private void CheckCollision()
        {
            if (controller.Tetrimino == null) return;

            Block[] tetriminoBlocks = tetrimino.Blocks;
            for (int i = 0; i < tetriminoBlocks.Length; i++)
            {
                bool isIntersects = floorBox.Intersects(tetriminoBlocks[i].Bounding);
                if (isIntersects)
                {
                    CorrectionBlockPosition(tetrimino.Blocks);
                    SetNextTetrimino();
                    return;
                }

                if (CheckCollisionBlock(tetriminoBlocks[i].Bounding)) {
                    CorrectionBlockPosition(tetrimino.Blocks);
                    SetNextTetrimino();
                    return;
                }
            }
        }

        private bool CheckCollisionBlock(BoundingSphere bounding) {
            foreach (Block block in blocks)
            {
                bool isIntersects = block.Bounding.Intersects(bounding);
                if (isIntersects)
                {
                    return true;
                }
            }
            return false;
        }

        private void CorrectionBlockPosition(Block[] correctBlocks) {
            for (int i = 0; i < correctBlocks.Length; i++)
            {
                float blockY = correctBlocks[i].Position.Y;
                float distance = blockY - floorPosition.Y;
                double value = distance * 10.0f;
                value = Math.Ceiling(value);
                float offsetY = (float)value / 10.0f;

                correctBlocks[i].Position.Y = floorPosition.Y + offsetY;
                blocks.Add(correctBlocks[i]);
            }
        }

        private void UpdateModel(Model model, Matrix matrix) {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = matrix;
                    effect.View = this.View;
                    effect.Projection = this.Projection;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //spriteBatch.Begin();
            //spriteBatch.DrawString(font, "FPS:" + fpsCounter.FPS, new Vector2(100.0f, 100.0f), Color.White);
            //spriteBatch.End();

            // TODO: Add your drawing code here
            if (state != State.Dead)
            {
                nextTetrimino.Draw();
                tetrimino.Draw();
            }

            foreach (Block block in blocks)
            {
                block.Draw();
            }

            foreach (VanishBlock vanishBlock in vanishBlocks)
            {
                vanishBlock.Draw();
            }

            drawModel(backgroundModel);
            drawModel(frameModel);

            base.Draw(gameTime);
        }

        private void drawModel(Model model) {
            foreach (ModelMesh mesh in model.Meshes)
            {
                mesh.Draw();
            }
        }
    }
}
