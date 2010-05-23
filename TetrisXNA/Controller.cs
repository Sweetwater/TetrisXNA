using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TetrisXNA
{
    class Controller
    {
        private enum State {
            Normal,
            Fall,
        }
        private State state;

        private Tetrimino tetrimino;
        public Tetrimino Tetrimino {
            get { return tetrimino; }
            set { 
                tetrimino = value;
                state = State.Normal;
            }
        }
        private KeyboardState oldKeyboard;
        private KeyboardState newKeyboard;

        public bool IsLeftRotatable;
        public bool IsRightRotatable;

        public bool IsLeftMovale;
        public bool IsRightMovale;

        private float moveValue = 0.1f;


        private Keys[] UseKeys = {
            Keys.Z,
            Keys.X,
            Keys.Left,
            Keys.Right,
            Keys.Up,
            Keys.Down};

        private int keyWaitCount = 30;
        private int keyIntervalCount = 5;

        private Dictionary<Keys, bool> isEnableKeys = new Dictionary<Keys, bool>();
        private Dictionary<Keys, int> keyWaits = new Dictionary<Keys, int>();
        private Dictionary<Keys, int> keyIntervals = new Dictionary<Keys, int>();

        public Controller() {
            this.oldKeyboard = Keyboard.GetState();

            foreach (Keys key in UseKeys)
            {
                isEnableKeys[key] = false;
                keyWaits[key] = keyWaitCount;
                keyIntervals[key] = 0;
            }
        }

        public void Update() {
            if (tetrimino == null) return;

            this.newKeyboard = Keyboard.GetState();

            foreach (Keys key in UseKeys)
	        {
        		isEnableKeys[key] = IsKeyTrriger(key);
                if (this.newKeyboard.IsKeyDown(key))
                {
                    keyWaits[key]--;
                    if (keyWaits[key] < 0)
                    {
                        keyWaits[key] = 0;
                        keyIntervals[key]--;
                        if (keyIntervals[key] < 0)
                        {
                            isEnableKeys[key] = true;
                            keyIntervals[key] = keyIntervalCount;
                        }
                    }
                }
                else {
                    keyWaits[key] = keyWaitCount;
                    keyIntervals[key] = 0;
                }
	        }

            if (state == State.Normal)
            {
                ControlNormal();
            }
            else if (state == State.Fall)
            {
                this.tetrimino.Position.Y -= 0.09f;
            }

            this.oldKeyboard = newKeyboard;
        }

        private bool IsKeyTrriger(Keys key) {
            return !oldKeyboard.IsKeyDown(key) && newKeyboard.IsKeyDown(key);
        }

        private void ControlNormal(){
            if (isEnableKeys[Keys.Left] && IsLeftMovale)
            {
                tetrimino.Position.X -= moveValue;
            }
            if (isEnableKeys[Keys.Right] && IsRightMovale)
            {
                tetrimino.Position.X += moveValue;
            }
            if (isEnableKeys[Keys.Down])
            {
                tetrimino.Position.Y -= moveValue;
            }
            if (isEnableKeys[Keys.Up])
            {
                tetrimino.Position.Y += moveValue;
            }

            if (isEnableKeys[Keys.Z] && IsLeftRotatable)
            {
                tetrimino.Rotation += (float)(Math.PI / 2.0f);
            }
            if (isEnableKeys[Keys.X] && IsRightRotatable)
            {
                tetrimino.Rotation -= (float)(Math.PI / 2.0f);
            }

            if (IsKeyTrriger(Keys.Space))
            {
                state = State.Fall;
            }
        }
    }
}
