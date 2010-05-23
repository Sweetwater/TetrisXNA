
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace TetrisXNA
{
    class VanishBlock : Block
    {
        private int animationCounter;
        private Vector3 colorVariation;
        public bool IsAnimationEnd {
            get { return (animationCounter == 0); }
        }

        public VanishBlock(Game1 game, Model model) :
            base(game, model)
        {
            animationCounter = 30;
        }

        public void Update() {
            if (animationCounter > 0) {
                animationCounter--;
                colorVariation += new Vector3(-0.1f, -0.1f, -0.1f);
            }
        }

        public override void Draw()
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.AmbientLightColor += colorVariation;

                    effect.World = Matrix.CreateTranslation(Position);
                    effect.View = game.View;
                    effect.Projection = game.Projection;
                }
                mesh.Draw();
            }
        }
    }
}
