using System;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using GTA.UI;

namespace SupermanGTA5
{
    public class Superman : Script
    {
        // ==========================================
        // STATE
        // ==========================================

        private bool menuOpen = false;
        private int menuIndex = 0;

        private bool supermanEnabled = false;
        private bool flightEnabled = false;
        private bool superSpeedEnabled = false;
        private bool godMode = false;

        private bool heatVision = false;
        private bool freezeBreath = false;

        private float flightSpeed = 35.0f;
        private float boostSpeed = 90.0f;

        private readonly string[] menuItems =
        {
            "SUPERMAN: ON/OFF",
            "FLIGHT: ON/OFF",
            "BOOST FLIGHT",
            "SUPER SPEED: ON/OFF",
            "GOD MODE: ON/OFF",
            "HEAT VISION",
            "FREEZE BREATH",
            "SUPER PUNCH",
            "GROUND POUND",
            "THUNDER CLAP",
            "RESTORE PLAYER"
        };

        // ==========================================
        // CONSTRUCTOR
        // ==========================================

        public Superman()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;

            Interval = 0;
        }

        // ==========================================
        // KEYBOARD
        // ==========================================

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                menuOpen = !menuOpen;
                return;
            }

            if (!menuOpen)
                return;

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
                ActivateMenuItem();
                return;
            }

            if (e.KeyCode == Keys.Back)
            {
                menuOpen = false;
            }
        }

        // ==========================================
        // MAIN LOOP
        // ==========================================

        private void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;

            if (player == null || !player.Exists())
            {
                return;
            }

            if (godMode)
            {
                player.IsInvincible = true;
            }

            if (supermanEnabled)
            {
                UpdateSuperman(player);
            }

            if (flightEnabled)
            {
                UpdateFlight(player);
            }

            if (superSpeedEnabled)
            {
                UpdateSuperSpeed(player);
            }

            if (heatVision)
            {
                UpdateHeatVision(player);
            }

            if (freezeBreath)
            {
                UpdateFreezeBreath(player);
            }

            if (menuOpen)
            {
                DrawMenu();
            }
        }

        // ==========================================
        // SUPERMAN
        // ==========================================

        private void ToggleSuperman()
        {
            supermanEnabled = !supermanEnabled;

            if (!supermanEnabled)
            {
                flightEnabled = false;
                superSpeedEnabled = false;
                heatVision = false;
                freezeBreath = false;

                Ped player = Game.Player.Character;

                if (player != null && player.Exists())
                {
                    player.CanRagdoll = true;

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
                }
            }

            Notification(
                supermanEnabled
                    ? "SUPERMAN: ON"
                    : "SUPERMAN: OFF"
            );
        }

        private void UpdateSuperman(Ped player)
        {
            if (flightEnabled)
            {
                player.CanRagdoll = false;
            }
            else
            {
                player.CanRagdoll = true;
            }
        }

        // ==========================================
        // FLIGHT
        // ==========================================

        private void ToggleFlight()
        {
            if (!supermanEnabled)
            {
                Notification("TURN SUPERMAN ON FIRST");
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

                Notification("FLIGHT: ON");
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

                Notification("FLIGHT: OFF");
            }
        }

        private void UpdateFlight(Ped player)
        {
            if (!player.Exists())
            {
                return;
            }

            Vector3 forward = player.ForwardVector;
            Vector3 right = player.RightVector;

            float speed = flightSpeed;

            bool boost =
                Game.IsKeyPressed(Keys.LShiftKey) ||
                Game.IsKeyPressed(Keys.RShiftKey);

            if (boost)
            {
                speed = boostSpeed;
            }

            Vector3 velocity =
                new Vector3(0.0f, 0.0f, 0.0f);

            if (Game.IsKeyPressed(Keys.W))
            {
                velocity += forward * speed;
            }

            if (Game.IsKeyPressed(Keys.S))
            {
                velocity -= forward * speed * 0.65f;
            }

            if (Game.IsKeyPressed(Keys.A))
            {
                velocity -= right * speed * 0.55f;
            }

            if (Game.IsKeyPressed(Keys.D))
            {
                velocity += right * speed * 0.55f;
            }

            if (Game.IsKeyPressed(Keys.Space))
            {
                velocity.Z += speed * 0.75f;
            }

            if (Game.IsKeyPressed(Keys.ControlKey))
            {
                velocity.Z -= speed * 0.75f;
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

            if (boost)
            {
                Function.Call(
                    Hash.SHAKE_GAMEPLAY_CAM,
                    "SMALL_EXPLOSION_SHAKE",
                    0.08f
                );
            }
        }

        // ==========================================
        // SUPER SPEED
        // ==========================================

        private void ToggleSuperSpeed()
        {
            if (!supermanEnabled)
            {
                Notification("TURN SUPERMAN ON FIRST");
                return;
            }

            superSpeedEnabled = !superSpeedEnabled;

            if (!superSpeedEnabled)
            {
                Function.Call(
                    Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                    Game.Player,
                    1.0f
                );

                Function.Call(
                    Hash.SET_SWIM_MULTIPLIER_FOR_PLAYER,
                    Game.Player,
                    1.0f
                );
            }

            Notification(
                superSpeedEnabled
                    ? "SUPER SPEED: ON"
                    : "SUPER SPEED: OFF"
            );
        }

        private void UpdateSuperSpeed(Ped player)
        {
            Function.Call(
                Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                Game.Player,
                1.49f
            );

            Function.Call(
                Hash.SET_SWIM_MULTIPLIER_FOR_PLAYER,
                Game.Player,
                1.49f
            );

            bool sprint =
                Game.IsKeyPressed(Keys.LShiftKey) ||
                Game.IsKeyPressed(Keys.RShiftKey);

            if (Game.IsKeyPressed(Keys.W) && sprint)
            {
                Vector3 forward =
                    player.ForwardVector;

                Function.Call(
                    Hash.SET_ENTITY_VELOCITY,
                    player.Handle,
                    forward.X * 30.0f,
                    forward.Y * 30.0f,
                    player.Velocity.Z
                );
            }
        }

        // ==========================================
        // HEAT VISION
        // ==========================================

        private void ToggleHeatVision()
        {
            if (!supermanEnabled)
            {
                Notification("TURN SUPERMAN ON FIRST");
                return;
            }

            heatVision = !heatVision;

            if (heatVision)
            {
                freezeBreath = false;
            }

            Notification(
                heatVision
                    ? "HEAT VISION: ON"
                    : "HEAT VISION: OFF"
            );
        }

        private void UpdateHeatVision(Ped player)
        {
            Vector3 start =
                player.Position +
                player.ForwardVector * 0.65f +
                new Vector3(0.0f, 0.0f, 0.55f);

            Vector3 end =
                start +
                player.ForwardVector * 80.0f;

            DrawLaser(start, end);

            if (Game.IsKeyPressed(Keys.E))
            {
                Entity target =
                    GetTargetEntity(player, 80.0f);

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

        // ==========================================
        // FREEZE BREATH
        // ==========================================

        private void ToggleFreezeBreath()
        {
            if (!supermanEnabled)
            {
                Notification("TURN SUPERMAN ON FIRST");
                return;
            }

            freezeBreath = !freezeBreath;

            if (freezeBreath)
            {
                heatVision = false;
            }

            Notification(
                freezeBreath
                    ? "FREEZE BREATH: ON"
                    : "FREEZE BREATH: OFF"
            );
        }

        private void UpdateFreezeBreath(Ped player)
        {
            Vector3 start =
                player.Position +
                player.ForwardVector * 0.7f +
                new Vector3(0.0f, 0.0f, 0.55f);

            Vector3 end =
                start +
                player.ForwardVector * 25.0f;

            DrawBreath(start, end);

            Entity target =
                GetTargetEntity(player, 25.0f);

            if (target != null && target.Exists())
            {
                Function.Call(
                    Hash.FREEZE_ENTITY_POSITION,
                    target.Handle,
                    true
                );
            }
        }

        // ==========================================
        // SUPER PUNCH
        // ==========================================

        private void SuperPunch()
        {
            if (!supermanEnabled)
            {
                Notification("TURN SUPERMAN ON FIRST");
                return;
            }

            Ped player = Game.Player.Character;

            Entity target =
                GetTargetEntity(player, 5.0f);

            if (target == null || !target.Exists())
            {
                Notification("NO TARGET");
                return;
            }

            Vector3 direction =
                player.ForwardVector;

            Function.Call(
                Hash.APPLY_FORCE_TO_ENTITY,
                target.Handle,
                1,
                direction.X * 35.0            }

            if (!menuOpen)
                return;

            if (e.KeyCode == Keys.Up)
            {
                menuIndex--;

                if (menuIndex < 0)
                    menuIndex = menuItems.Length - 1;

                return;
            }

            if (e.KeyCode == Keys.Down)
            {
                menuIndex++;

                if (menuIndex >= menuItems.Length)
                    menuIndex = 0;

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
            }
        }

        // =========================
        // MAIN LOOP
        // =========================

        private void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;

            if (player == null || !player.Exists())
                return;

            if (godMode)
            {
                player.IsInvincible = true;
            }

            if (supermanEnabled)
            {
                UpdateSuperman(player);
            }

            if (flightEnabled)
            {
                UpdateFlight(player);
            }

            if (superSpeedEnabled)
            {
                UpdateSuperSpeed(player);
            }

            if (heatVision)
            {
                UpdateHeatVision(player);
            }

            if (freezeBreath)
            {
                UpdateFreezeBreath(player);
            }

            if (menuOpen)
            {
                DrawMenu();
            }
        }

        // =========================
        // SUPERMAN
        // =========================

        private void ToggleSuperman()
        {
            supermanEnabled = !supermanEnabled;

            if (!supermanEnabled)
            {
                flightEnabled = false;
                superSpeedEnabled = false;
                heatVision = false;
                freezeBreath = false;

                Function.Call(
                    Hash.SET_ENTITY_HAS_GRAVITY,
                    Game.Player.Character.Handle,
                    true
                );

                Game.Player.Character.CanRagdoll = true;
            }

            Notification(
                supermanEnabled
                    ? "SUPERMAN: ON"
                    : "SUPERMAN: OFF"
            );
        }

        private void UpdateSuperman(Ped player)
        {
            if (flightEnabled)
            {
                player.CanRagdoll = false;
            }
            else
            {
                player.CanRagdoll = true;
            }
        }

        // =========================
        // FLIGHT
        // =========================

        private void ToggleFlight()
        {
            if (!supermanEnabled)
            {
                Notification("TURN SUPERMAN ON FIRST");
                return;
            }

            flightEnabled = !flightEnabled;

            if (flightEnabled)
            {
                Function.Call(
                    Hash.SET_ENTITY_HAS_GRAVITY,
                    Game.Player.Character.Handle,
                    false
                );

                Game.Player.Character.CanRagdoll = false;

                Notification("FLIGHT: ON");
            }
            else
            {
                Function.Call(
                    Hash.SET_ENTITY_HAS_GRAVITY,
                    Game.Player.Character.Handle,
                    true
                );

                Function.Call(
                    Hash.SET_ENTITY_VELOCITY,
                    Game.Player.Character.Handle,
                    0.0f,
                    0.0f,
                    0.0f
                );

                Game.Player.Character.CanRagdoll = true;

                Notification("FLIGHT: OFF");
            }
        }

        private void UpdateFlight(Ped player)
        {
            if (!player.Exists())
                return;

            Vector3 forward = player.ForwardVector;
            Vector3 right = player.RightVector;

            float speed = flightSpeed;

            bool boost =
                Game.IsKeyPressed(Keys.LShiftKey) ||
                Game.IsKeyPressed(Keys.RShiftKey);

            if (boost)
            {
                speed = boostSpeed;
            }

            Vector3 velocity = Vector3.Zero;

            if (Game.IsKeyPressed(Keys.W))
            {
                velocity += forward * speed;
            }

            if (Game.IsKeyPressed(Keys.S))
            {
                velocity -= forward * speed * 0.65f;
            }

            if (Game.IsKeyPressed(Keys.A))
            {
                velocity -= right * speed * 0.55f;
            }

            if (Game.IsKeyPressed(Keys.D))
            {
                velocity += right * speed * 0.55f;
            }

            if (Game.IsKeyPressed(Keys.Space))
            {
                velocity.Z += speed * 0.75f;
            }

            if (Game.IsKeyPressed(Keys.ControlKey))
            {
                velocity.Z -= speed * 0.75f;
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

            if (boost)
            {
                Function.Call(
                    Hash.SHAKE_GAMEPLAY_CAM,
                    "SMALL_EXPLOSION_SHAKE",
                    0.08f
                );
            }
        }

        // =========================
        // SUPER SPEED
        // =========================

        private void ToggleSuperSpeed()
        {
            if (!supermanEnabled)
            {
                Notification("TURN SUPERMAN ON FIRST");
                return;
            }

            superSpeedEnabled = !superSpeedEnabled;

            if (!superSpeedEnabled)
            {
                Function.Call(
                    Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                    Game.Player,
                    1.0f
                );

                Function.Call(
                    Hash.SET_SWIM_MULTIPLIER_FOR_PLAYER,
                    Game.Player,
                    1.0f
                );
            }

            Notification(
                superSpeedEnabled
                    ? "SUPER SPEED: ON"
                    : "SUPER SPEED: OFF"
            );
        }

        private void UpdateSuperSpeed(Ped player)
        {
            Function.Call(
                Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER,
                Game.Player,
                1.49f
            );

            Function.Call(
                Hash
