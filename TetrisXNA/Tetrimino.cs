using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TetrisXNA
{
    class Tetrimino
    {
        private Game1 game;
        private Block[] blocks;
        public Block[] Blocks {
            get { return blocks; }
            set { blocks = value; }
        }

        private float rotation;
        public float Rotation {
            get { return rotation; }
            set { rotation = value; }
        }
        public Vector3 Position;

        public static int typeNum = 7;
        private int type;
        private static float s = 0.1f;
        private Vector3[][] layouts = new Vector3[][] {
            // 棒
            new Vector3[]{
                new Vector3(-1 * s, 0 * s, 0),
                new Vector3( 0 * s, 0 * s, 0),
                new Vector3( 1 * s, 0 * s, 0),
                new Vector3( 2 * s, 0 * s, 0),
            },
            // 四角
            new Vector3[]{
                new Vector3(-1 * s, 1 * s, 0),
                new Vector3( 0 * s, 1 * s, 0),
                new Vector3(-1 * s, 0 * s, 0),
                new Vector3( 0 * s, 0 * s, 0),
            },
            // S字
            new Vector3[]{
                new Vector3( 0 * s, 0 * s, 0),
                new Vector3( 1 * s, 0 * s, 0),
                new Vector3(-1 * s,-1 * s, 0),
                new Vector3( 0 * s,-1 * s, 0),
            },
            // Z字
            new Vector3[]{
                new Vector3(-1 * s, 0 * s, 0),
                new Vector3( 0 * s, 0 * s, 0),
                new Vector3( 0 * s,-1 * s, 0),
                new Vector3( 1 * s,-1 * s, 0),
            },
            // J字
            new Vector3[]{
                new Vector3( 0 * s, 1 * s, 0),
                new Vector3( 0 * s, 0 * s, 0),
                new Vector3( 1 * s, 0 * s, 0),
                new Vector3( 2 * s, 0 * s, 0),
            },
            // L字
            new Vector3[]{
                new Vector3(-2 * s, 0 * s, 0),
                new Vector3(-1 * s, 0 * s, 0),
                new Vector3( 0 * s, 0 * s, 0),
                new Vector3( 0 * s, 1 * s, 0),
            },
            // T字
            new Vector3[]{
                new Vector3( 0 * s, 1 * s, 0),
                new Vector3(-1 * s, 0 * s, 0),
                new Vector3( 0 * s, 0 * s, 0),
                new Vector3( 1 * s, 0 * s, 0),
            },
        };

        //public BoundingBox[] BoundingBox
        //{
        //    get
        //    {
        //        BoundingBox[] boundings = new BoundingBox[blocks.Length];
        //        for (int i = 0; i < blocks.Length; i++)
        //        {
        //            boundings[i] = blocks[i].Bounding;
        //        }

        //        return boundings;
        //    }
        //}

        public Tetrimino(Game1 game, Model model, int type) {
            this.game = game;
            this.rotation = 0;
            this.type = type;
            this.Position.X = 0.05f;

            blocks = new Block[layouts[type].Length];

            for (int i = 0; i < layouts[type].Length; i++)
			{
			    Vector3 position = this.Position + layouts[type][i];
                blocks[i] = new Block(game, model);
                blocks[i].Position = position;
			}
        }

        public void Update() {
            if (blocks == null) return;

            Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);

            for (int i = 0; i < layouts[type].Length; i++)
            {
                blocks[i].Position = this.Position + Vector3.Transform(layouts[type][i], rotationMatrix);
            }
        }

        public Block[] GetMoveBlocks(Vector3 moveVector)
        {
            Block[] moveBlocks = new Block[layouts[type].Length];
            for (int i = 0; i < blocks.Length; i++)
            {
                moveBlocks[i] = new Block(game, null);
                moveBlocks[i].Position = blocks[i].Position + moveVector;
            }
            return moveBlocks;
        }
        
        public Block[] GetRotationBlocks(float rotation)
        {

            Matrix rotationMatrix = Matrix.CreateRotationZ(this.rotation + rotation);

            Block[] rotationBlocks = new Block[layouts[type].Length];
            for (int i = 0; i < layouts[type].Length; i++)
            {
                rotationBlocks[i] = new Block(game, null);
                rotationBlocks[i].Position = this.Position + Vector3.Transform(layouts[type][i], rotationMatrix);
            }
            return rotationBlocks;
        }

        public void Draw()
        {
            if (blocks == null) return;

            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].Draw();
            }
        }
    }
}
