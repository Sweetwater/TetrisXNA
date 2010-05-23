using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TetrisXNA
{
    class Block
    {
        protected Game1 game;
        protected Model model;
        public Model Model {
            get { return model; }
        }

        public Vector3 Position;

        // ブロックの隙間を抜けるため
        // 見たためより衝突ボックスを少し小さくする
        private static float s = 0.0996f;
        private static float hs = s / 2;
        private Vector3 sizeMin = new Vector3(-hs, -hs, -hs);
        private Vector3 sizeMax = new Vector3(hs, hs, hs);
        public BoundingSphere Bounding {
            get { 
                return new BoundingSphere(Position, hs);
            }
        }

        public Block(Game1 game, Model model) 
        {
            this.game = game;
            this.model = model;
        }

        public virtual void Draw() {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateTranslation(Position);
                    effect.View = game.View;
                    effect.Projection = game.Projection;
                }
                mesh.Draw();
            }
        }
    }
}
