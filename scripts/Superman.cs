using System;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using Screen = GTA.UI.Screen;

namespace SupermanGTA5
{
    public class Superman : Script
    {
        private bool menuOpen = false;
        private bool supermanEnabled = false;
        private bool flightEnabled = false;
        private bool superSpeedEnabled = false;
        private bool heatVisionEnabled = false;

        private int selectedItem = 0;

        private readonly string[] menuItems =
        {
            "SUPERMAN ON / OFF",
            "FLIGHT ON / OFF",
            "SUPER SPEED ON / OFF",
            "HEAT VISION ON / OFF",
            "SUPER PUNCH",
            "RESTORE PLAYER"
        };

        private DateTime lastPunchTime = DateTime.MinValue;
        private DateTime lastHeatTime = DateTime.MinValue;

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
                selectedItem = 0;
                return;
            }

            if (!menuOpen)
                return;

            if (e.KeyCode == Keys.Up)
            {
                selectedItem--;

                if (selectedItem < 0)
                    selectedItem = menuItems.Length - 1;

                return;
            }

            if (e.KeyCode == Keys.Down)
            {
                selectedItem++;

                if (selectedItem >= menuItems.Length)
                    selectedItem = 0;

                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                ActivateMenuItem();
                return;
            }

            if (e.KeyCode == Keys.Back)
            {
                menuOpen = false;
                return;
            }
        }

        private void ActivateMenuItem()
        {
            Ped player = Game.Player.Character;

            if (player == null || !player.Exists())
                return;

            switch (selectedItem)
            {
                case 0:
                    supermanEnabled = !supermanEnabled;

                    if (supermanEnabled)
                    {
                        player.IsInvincible = true;
                        player.CanRagdoll = false;
                    }
                    else
                    {
                        RestorePlayer();
                    }

                    break;

                case 1:
                    flightEnabled = !flightEnabled;
                    break;

                case 2:
                    superSpeedEnabled = !superSpeedEnabled;
                    break;

                case 3:
                    heatVisionEnabled = !heatVisionEnabled;
                    break;

                case 4:
                    SuperPunch(player);
                    break;

                case 5:
                    RestorePlayer();
                    break;
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;

            if (player == null || !player.Exists())
                return;

            if (menuOpen)
            {
                DrawMenu();
            }

            if (!supermanEnabled)
                return;

            if (flightEnabled)
            {
                UpdateFlight(player);
            }

            if (superSpeedEnabled)
            {
                UpdateSuperSpeed(player);
            }
            else
            {
                Function.Call(
                    Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                    Game.Player.Handle,
                    1.0f
                );
            }

            if (heatVisionEnabled)
            {
                UpdateHeatVision(player);
            }

            if (Game.IsControlJustPressed(
                Control.Attack,
                0))
            {
                SuperPunch(player);
            }
        }

        private void DrawMenu()
        {
            string text = "SUPERman\n\n";

            for (int i = 0; i < menuItems.Length; i++)
            {
                string prefix = i == selectedItem ? "> " : "  ";

                string state = "";

                if (i == 0)
                    state = supermanEnabled ? " [ON]" : " [OFF]";

                if (i == 1)
                    state = flightEnabled ? " [ON]" : " [OFF]";

                if (i == 2)
                    state = superSpeedEnabled ? " [ON]" : " [OFF]";

                if (i == 3)
                    state = heatVisionEnabled ? " [ON]" : " [OFF]";

                text += prefix + menuItems[i] + state + "\n";
            }

            text += "\nF5 - Close";
            text += "\nUP/DOWN - Select";
            text += "\nENTER - Activate";

            Screen.ShowSubtitle(text, 1);
        }

        private void UpdateFlight(Ped player)
        {
            player.CanRagdoll = false;

            Vector3 velocity = Vector3.Zero;

            Vector3 forward = player.ForwardVector;
            Vector3 right = player.RightVector;

            if (Game.IsKeyPressed(Keys.W))
            {
                velocity += forward * 8.0f;
            }

            if (Game.IsKeyPressed(Keys.S))
            {
                velocity -= forward * 5.0f;
            }

            if (Game.IsKeyPressed(Keys.D))
            {
                velocity += right * 5.0f;
            }

            if (Game.IsKeyPressed(Keys.A))
            {
                velocity -= right * 5.0f;
            }

            if (Game.IsKeyPressed(Keys.Space))
            {
                velocity.Z += 7.0f;
            }

            if (Game.IsKeyPressed(Keys.ControlKey))
            {
                velocity.Z -= 7.0f;
            }

            if (Game.IsKeyPressed(Keys.ShiftKey))
            {
                velocity *= 2.5f;
            }

            if (velocity.Length() < 0.1f)
            {
                velocity = new Vector3(
                    0.0f,
                    0.0f,
                    0.15f
                );
            }

            player.Velocity = velocity;

            Function.Call(
                Hash.SET_ENTITY_HAS_GRAVITY,
                player.Handle,
                false
            );
        }

        private void UpdateSuperSpeed(Ped player)
        {
            Function.Call(
                Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                Game.Player.Handle,
                1.49f
            );

            Function.Call(
                Hash.SET_SWIM_MULTIPLIER_FOR_PLAYER,
                Game.Player.Handle,
                1.49f
            );
        }

        private void UpdateHeatVision(Ped player)
        {
            if ((DateTime.Now - lastHeatTime).TotalMilliseconds < 150)
                return;

            if (!Game.IsKeyPressed(Keys.E))
                return;

            Entity target = GetTarget(player, 60.0f);

            if (target == null || !target.Exists())
                return;

            lastHeatTime = DateTime.Now;

            if (target is Ped)
            {
                Ped ped = (Ped)target;

                if (!ped.IsDead)
                {
                    ped.Health -= 10;

                    if (ped.Health < 0)
                        ped.Health = 0;
                }
            }

            Screen.ShowSubtitle(
                "HEAT VISION",
                500
            );
        }

        private Entity GetTarget(Ped player, float distance)
        {
            Ped[] peds = World.GetNearbyPeds(
                player,
                distance
            );

            Ped best = null;
            float bestDistance = distance;

            foreach (Ped ped in peds)
            {
                if (ped == null ||
                    !ped.Exists() ||
                    ped.Handle == player.Handle)
                {
                    continue;
                }

                if (ped.IsDead)
                    continue;

                Vector3 direction =
                    ped.Position - player.Position;

                float currentDistance =
                    direction.Length();

                if (currentDistance > distance ||
                    currentDistance <= 0.1f)
                {
                    continue;
                }

                direction.Normalize();

                float dot =
                    Vector3.Dot(
                        player.ForwardVector,
                        direction
                    );

                if (dot < 0.65f)
                    continue;

                if (currentDistance < bestDistance)
                {
                    best = ped;
                    bestDistance = currentDistance;
                }
            }

            return best;
        }

        private void SuperPunch(Ped player)
        {
            if ((DateTime.Now - lastPunchTime).TotalMilliseconds < 500)
                return;

            lastPunchTime = DateTime.Now;

            Entity target = GetTarget(
                player,
                5.0f
            );

            if (target == null ||
                !target.Exists())
            {
                return;
            }

            Vector3 force =
                player.ForwardVector * 35.0f;

            Function.Call(
                Hash.APPLY_FORCE_TO_ENTITY,
                target.Handle,
                1,
                force.X,
                force.Y,
                12.0f,
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

            if (target is Ped)
            {
                Ped ped = (Ped)target;

                if (!ped.IsDead)
                {
                    ped.Health -= 25;

                    if (ped.Health < 0)
                        ped.Health = 0;
                }
            }
        }

        private void RestorePlayer()
        {
            Ped player = Game.Player.Character;

            if (player == null ||
                !player.Exists())
            {
                return;
            }

            supermanEnabled = false;
            flightEnabled = false;
            superSpeedEnabled = false;
            heatVisionEnabled = false;

            player.IsInvincible = false;
            player.CanRagdoll = true;

            Function.Call(
                Hash.SET_ENTITY_HAS_GRAVITY,
                player.Handle,
                true
            );

            Function.Call(
                Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                Game.Player.Handle,
                1.0f
            );

            Function.Call(
                Hash.SET_SWIM_MULTIPLIER_FOR_PLAYER,
                Game.Player.Handle,
                1.0f
            );

            player.Velocity = Vector3.Zero;

            Screen.ShowSubtitle(
                "SUPERMAN RESTORED",
                1000
            );
        }
    }
}
