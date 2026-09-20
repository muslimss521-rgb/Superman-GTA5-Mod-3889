using System;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using GTA.UI;

namespace SupermanGTA5
{
    public class Superman : Script
    {
        // =========================
        // STATE
        // =========================

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
        private float punchForce = 35.0f;
        private float throwForce = 70.0f;

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

        // =========================
        // CONSTRUCTOR
        // =========================

        public Superman()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
            Interval = 0;
        }

        // =========================
        // KEYBOARD
        // =========================

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // F5 - open/close trainer
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
                    menuIndex = menuItems.Length - 1;
            }

            if (e.KeyCode == Keys.Down)
            {
                menuIndex++;

                if (menuIndex >= menuItems.Length)
                    menuIndex = 0;
            }

            if (e.KeyCode == Keys.Enter)
            {
                ActivateMenuItem();
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
            }

            Notification(
                supermanEnabled
                    ? "SUPERMAN: ON"
                    : "SUPERMAN: OFF"
            );
        }

        private void UpdateSuperman(Ped player)
        {
            // Keep Superman protected from ragdoll while using powers.
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
                Notification("Turn Superman ON first.");
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

            // Shift = boost
            bool boost =
                Game.IsKeyPressed(Keys.LShiftKey) ||
                Game.IsKeyPressed(Keys.RShiftKey);

            if (boost)
                speed = boostSpeed;

            Vector3 velocity = new Vector3(0f, 0f, 0f);

            // W
            if (Game.IsKeyPressed(Keys.W))
            {
                velocity += forward * speed;
            }

            // S
            if (Game.IsKeyPressed(Keys.S))
            {
                velocity -= forward * speed * 0.65f;
            }

            // A
            if (Game.IsKeyPressed(Keys.A))
            {
                velocity -= right * speed * 0.55f;
            }

            // D
            if (Game.IsKeyPressed(Keys.D))
            {
                velocity += right * speed * 0.55f;
            }

            // Space = up
            if (Game.IsKeyPressed(Keys.Space))
            {
                velocity.Z += speed * 0.75f;
            }

            // Ctrl = down
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

            // Disable normal pedestrian movement while flying.
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

            // Slight camera/body movement at high speed.
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
                Notification("Turn Superman ON first.");
                return;
            }

            superSpeedEnabled = !superSpeedEnabled;

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

            // Very fast forward movement when sprinting.
            if (Game.IsKeyPressed(Keys.W) &&
                (Game.IsKeyPressed(Keys.LShiftKey) ||
                 Game.IsKeyPressed(Keys.RShiftKey)))
            {
                Vector3 forward = player.ForwardVector;

                Function.Call(
                    Hash.SET_ENTITY_VELOCITY,
                    player.Handle,
                    forward.X * 30.0f,
                    forward.Y * 30.0f,
                    player.Velocity.Z
                );
            }
        }

        // =========================
        // HEAT VISION
        // =========================

        private void ToggleHeatVision()
        {
            if (!supermanEnabled)
            {
                Notification("Turn Superman ON first.");
                return;
            }

            heatVision = !heatVision;
            freezeBreath = false;

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
                new Vector3(0f, 0f, 0.55f);

            Vector3 end =
                start +
                player.ForwardVector * 80.0f;

            DrawLaser(start, end);

            if (Game.IsKeyPressed(Keys.E))
            {
                Entity target = GetTargetEntity(player, 80.0f);

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

        // =========================
        // FREEZE / SUPER BREATH
        // =========================

        private void ToggleFreezeBreath()
        {
            if (!supermanEnabled)
            {
                Notification("Turn Superman ON first.");
                return;
            }

            freezeBreath = !freezeBreath;
            heatVision = false;

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
                new Vector3(0f, 0f, 0.55f);

            Vector3 end =
                start +
                player.ForwardVector * 25.0f;

            DrawBreath(start, end);

            Entity target = GetTargetEntity(player, 25.0f);

            if (target != null && target.Exists
