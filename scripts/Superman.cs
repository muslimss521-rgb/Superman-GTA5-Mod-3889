using System;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;

namespace SupermanGTA5
{
    public class Superman : Script
    {
        private bool menuOpen = false;
        private int menuIndex = 0;

        private bool supermanEnabled = false;
        private bool flightEnabled = false;
        private bool speedEnabled = false;
        private bool heatVisionEnabled = false;

        private readonly string[] menuItems =
        {
            "SUPERMAN ON/OFF",
            "FLIGHT ON/OFF",
            "SUPER SPEED ON/OFF",
            "HEAT VISION ON/OFF",
            "SUPER PUNCH",
            "RESTORE PLAYER"
        };

        public Superman()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
            Interval = 0;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                menuOpen = !menuOpen;
                return;
            }

            if (!menuOpen)
            {
                return;
            }

            if (e.KeyCode == Keys.Up)
            {
                menuIndex--;

                if (menuIndex < 0)
                {
                    menuIndex = menuItems.Length - 1;
                }

                return;
            }

            if (e.KeyCode == Keys.Down)
            {
                menuIndex++;

                if (menuIndex >= menuItems.Length)
                {
                    menuIndex = 0;
                }

                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                ActivateMenu();
                return;
            }

            if (e.KeyCode == Keys.Back)
            {
                menuOpen = false;
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;

            if (player == null || !player.Exists())
            {
                return;
            }

            if (flightEnabled)
            {
                UpdateFlight(player);
            }

            if (speedEnabled)
            {
                UpdateSpeed(player);
            }

            if (heatVisionEnabled)
            {
                UpdateHeatVision(player);
            }

            if (menuOpen)
            {
                DrawMenu();
            }
        }

        private void ActivateMenu()
        {
            switch (menuIndex)
            {
                case 0:
                    ToggleSuperman();
                    break;

                case 1:
                    ToggleFlight();
                    break;

                case 2:
                    ToggleSpeed();
                    break;

                case 3:
                    ToggleHeatVision();
                    break;

                case 4:
                    SuperPunch();
                    break;

                case 5:
                    RestorePlayer();
                    break;
            }
        }

        private void ToggleSuperman()
        {
            supermanEnabled = !supermanEnabled;

            if (!supermanEnabled)
            {
                flightEnabled = false;
                speedEnabled = false;
                heatVisionEnabled = false;

                Ped player = Game.Player.Character;

                if (player != null && player.Exists())
                {
                    player.CanRagdoll = true;
                    player.IsInvincible = false;

                    Function.Call(
                        Hash.SET_ENTITY_HAS_GRAVITY,
                        player.Handle,
                        true
                    );

                    Function.Call(
                        Hash.SET_ENTITY_VELOCITY,
                        player.Handle,
                        0.0f,
                        0.0f,
                        0.0f
                    );

                    Function.Call(
                        Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                        Game.Player,
                        1.0f
                    );
                }

                Notify("SUPERMAN OFF");
            }
            else
            {
                Notify("SUPERMAN ON");
            }
        }

        private void ToggleFlight()
        {
            if (!supermanEnabled)
            {
                Notify("TURN SUPERMAN ON FIRST");
                return;
            }

            flightEnabled = !flightEnabled;

            Ped player = Game.Player.Character;

            if (flightEnabled)
            {
                Function.Call(
                    Hash.SET_ENTITY_HAS_GRAVITY,
                    player.Handle,
                    false
                );

                player.CanRagdoll = false;

                Notify("FLIGHT ON");
            }
            else
            {
                Function.Call(
                    Hash.SET_ENTITY_HAS_GRAVITY,
                    player.Handle,
                    true
                );

                Function.Call(
                    Hash.SET_ENTITY_VELOCITY,
                    player.Handle,
                    0.0f,
                    0.0f,
                    0.0f
                );

                player.CanRagdoll = true;

                Notify("FLIGHT OFF");
            }
        }

        private void UpdateFlight(Ped player)
        {
            if (player == null || !player.Exists())
            {
                return;
            }

            Vector3 forward = player.ForwardVector;
            Vector3 right = player.RightVector;

            float speed = 25.0f;

            bool boost =
                Game.IsKeyPressed(Keys.LShiftKey) ||
                Game.IsKeyPressed(Keys.RShiftKey);

            if (boost)
            {
                speed = 80.0f;
            }

            Vector3 velocity =
                new Vector3(0.0f, 0.0f, 0.0f);

            if (Game.IsKeyPressed(Keys.W))
            {
                velocity += forward * speed;
            }

            if (Game.IsKeyPressed(Keys.S))
            {
                velocity -= forward * speed;
            }

            if (Game.IsKeyPressed(Keys.A))
            {
                velocity -= right * speed;
            }

            if (Game.IsKeyPressed(Keys.D))
            {
                velocity += right * speed;
            }

            if (Game.IsKeyPressed(Keys.Space))
            {
                velocity.Z += speed;
            }

            if (Game.IsKeyPressed(Keys.ControlKey))
            {
                velocity.Z -= speed;
            }

            Function.Call(
                Hash.SET_ENTITY_VELOCITY,
                player.Handle,
                velocity.X,
                velocity.Y,
                velocity.Z
            );

            Function.Call(
                Hash.DISABLE_CONTROL_ACTION,
                0,
                30,
                true
            );

            Function.Call(
                Hash.DISABLE_CONTROL_ACTION,
                0,
                31,
                true
            );
        }

        private void ToggleSpeed()
        {
            if (!supermanEnabled)
            {
                Notify("TURN SUPERMAN ON FIRST");
                return;
            }

            speedEnabled = !speedEnabled;

            if (!speedEnabled)
            {
                Function.Call(
                    Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                    Game.Player,
                    1.0f
                );

                Notify("SUPER SPEED OFF");
            }
            else
            {
                Notify("SUPER SPEED ON");
            }
        }

        private void UpdateSpeed(Ped player)
        {
            Function.Call(
                Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                Game.Player,
                1.49f
            );

            if (Game.IsKeyPressed(Keys.W) &&
                Game.IsKeyPressed(Keys.LShiftKey))
            {
                Vector3 forward = player.ForwardVector;

                Function.Call(
                    Hash.SET_ENTITY_VELOCITY,
                    player.Handle,
                    forward.X * 25.0f,
                    forward.Y * 25.0f,
                    player.Velocity.Z
                );
            }
        }

        private void ToggleHeatVision()
        {
            if (!supermanEnabled)
            {
                Notify("TURN SUPERMAN ON FIRST");
                return;
            }

            heatVisionEnabled = !heatVisionEnabled;

            if (heatVisionEnabled)
            {
                Notify("HEAT VISION ON");
            }
            else
            {
                Notify("HEAT VISION OFF");
            }
        }

        private void UpdateHeatVision(Ped player)
        {
            Vector3 start =
                player.Position +
                player.ForwardVector * 0.6f;

            start.Z += 0.55f;

            Vector3 end =
                start +
                player.ForwardVector * 80.0f;

            Function.Call(
                Hash.DRAW_LINE,
                start.X,
                start.Y,
                start.Z,
                end.X,
                end.Y,
                end.Z,
                255,
                40,
                40,
                255
            );

            if (Game.IsKeyPressed(Keys.E))
            {
                Entity target =
                    GetTarget(player, 80.0f);

                if (target != null && target.Exists())
                {
                    Function.Call(
                        Hash.APPLY_DAMAGE_TO_ENTITY,
                        target.Handle,
                        10,
                        false
                    );
                }
            }
        }

        private void SuperPunch()
        {
            if (!supermanEnabled)
            {
                Notify("TURN SUPERMAN ON FIRST");
                return;
            }

            Ped player = Game.Player.Character;

            if (player == null || !player.Exists())
            {
                return;
            }

            Entity target =
                GetTarget(player, 5.0f);

            if (target == null || !target.Exists())
            {
                Notify("NO TARGET");
                return;
            }

            Vector3 direction =
                player.ForwardVector;

            Function.Call(
                Hash.APPLY_FORCE_TO_ENTITY,
                target.Handle,
                1,
                direction.X * 30.0f,
                direction.Y * 30.0f,
                5.0f,
                0.0f,
                0.0f,
                0.0f,
                0,
                false,
                true,
                true,
                false,
                true
            );

            Notify("SUPER PUNCH");
        }

        private Entity GetTarget(
            Ped player,
            float distance)
        {
            Vector3 start =
                player.Position;

            start.Z += 0.5f;

            Vector3 end =
                start +
                player.ForwardVector * distance;

            RaycastResult result =
                World.Raycast(
                    start,
                    end,
                    IntersectOptions.Everything,
                    player
                );

            if (result.DidHit)
            {
                return result.HitEntity;
            }

            return null;
        }

        private void RestorePlayer()
        {
            Ped player = Game.Player.Character;

            if (player == null || !player.Exists())
            {
                return;
            }

            supermanEnabled = false;
            flightEnabled = false;
            speedEnabled = false;
            heatVisionEnabled = false;

            player.IsInvincible = false;
            player.CanRagdoll = true;

            player.Health = player.MaxHealth;

            Function.Call(
                Hash.SET_ENTITY_HAS_GRAVITY,
                player.Handle,
                true
            );

            Function.Call(
                Hash.SET_ENTITY_VELOCITY,
                player.Handle,
                0.0f,
                0.0f,
                0.0f
            );

            Function.Call(
                Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                Game.Player,
                1.0f
            );

            Notify("PLAYER RESTORED");
        }

        private void DrawMenu()
        {
            string text =
                "SUPERMAN TRAINER\n\n";

            for (int i = 0; i < menuItems.Length; i++)
            {
                if (i == menuIndex)
                {
                    text += "> ";
                }
                else
                {
                    text += "  ";
                }

                text += menuItems[i];
                text += "\n";
            }

            text +=
                "\nUP/DOWN - SELECT" +
                "\nENTER - ACTIVATE" +
                "\nBACKSPACE - CLOSE";

            Screen.ShowSubtitle(
                text,
                1
            );
        }

        private void Notify(string message)
        {
            Screen.ShowSubtitle(
                message,
                1500
            );
        }
    }
}
